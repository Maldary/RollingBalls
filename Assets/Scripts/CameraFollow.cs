using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5.0f;
    public float rotationSpeed = 5.0f;
    public float distance = 0.0f;
    public float height = 2.0f;

    public bool CanFollow
    {
        get;
        set;
    }
    private void FixedUpdate()
    {
        
        if (!CanFollow)
        {
            return;
        }
        
        Vector3 targetVelocity = target.GetComponent<Rigidbody>().velocity;
        Vector3 targetPosition = target.position + targetVelocity * Time.deltaTime;

        Vector3 offset = new Vector3(0, height, -distance);
        offset = Quaternion.LookRotation(targetVelocity) * offset;

        transform.position = Vector3.Lerp(transform.position, targetPosition + offset, followSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetVelocity), rotationSpeed * Time.deltaTime);
    }
    
   
}