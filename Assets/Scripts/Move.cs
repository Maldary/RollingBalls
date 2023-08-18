using System.Collections;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.VisualScripting;

public class Move : MonoBehaviour
{
	[SerializeField] private Text text;
	[SerializeField] public float speed;
	[SerializeField] public float maxSpeed;
	public Camera Playercamera;
	private Rigidbody _rb;
	private int _count;
	private Vector2 _inputValues;
	private bool _isReadyToMove, _isReadyToPlay = true;
	private Vector3 _initialPostion, _checkpointPosition;
	public AudioClip WinSound;
	public AudioSource _audioSource;
	public const float MaxSpeed = 20f;
	public float currentSpeed;
	public float frictionCoefficient = 1.5f;
	public Vector3 finishLinePosition;
	public float moveToFinishLineDuration = 1.0f;
	public GameObject panel;
	public GameObject CurrentPanel;
	public CameraFollow cameraFollow;
	public GameObject TutorText;
	private void Awake()
	{
		GetReferences();
	}

	private void GetReferences()
	{
		_rb = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		Application.targetFrameRate = 60;
		SetInitialPosition();
	}

	private void SetInitialPosition()
	{
		_initialPostion = transform.localPosition;
	}

	public void ReEnableMovement()
	{
		_isReadyToPlay = true;
	}

	private Vector2 GetInputData()
	{
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Moved)
			{
				if (!cameraFollow.CanFollow)
				{
					cameraFollow.CanFollow = true;
					TutorText.gameObject.SetActive(false);
				}
				Vector2 deltaPosition = touch.deltaPosition;
				deltaPosition.Normalize();
				float maxSwipeLength = 200f;
				float swipeLength = 0f;
				swipeLength += deltaPosition.magnitude;
				if (swipeLength >= maxSwipeLength)
				{
					return Vector2.zero;
				}
				return deltaPosition / 2;
			}
		}

		return Vector2.zero;
	}

	private void Moving()
	{
		_rb.constraints = RigidbodyConstraints.FreezeRotationZ;
		Vector3 movement = new Vector3(_inputValues.x, 0, _inputValues.y);
		movement = Playercamera.transform.TransformDirection(movement);
		movement.y = 0;
		movement.Normalize();
		float currentSpeed2 = Vector3.Dot(_rb.velocity, movement.normalized);
		if (currentSpeed2 < maxSpeed)
		{
			_rb.AddForce(movement * speed * Time.fixedDeltaTime, ForceMode.Acceleration);
		}
		_rb.velocity = Vector3.ClampMagnitude(_rb.velocity, maxSpeed);
	}

void FixedUpdate()
	{
		if (!_isReadyToPlay) return;
		if(_isReadyToMove) Moving();
	}
	
	private void CameraMoveToTarget(Vector3 position, float duration)
	{
		Playercamera.transform.DOMove(position, duration);
	}

    private void Update()
    {
	    if (!_isReadyToPlay)
	    {
		    return;
	    }
		_inputValues = GetInputData();
		_isReadyToMove = Input.anyKey;
		currentSpeed = _rb.velocity.magnitude;
		
		
    }
    private float GetPitch(float speedy) {
	    float pitch = (speedy / 10);
	    return Mathf.Clamp(pitch, 0f, 1.5f);
    }
    private void OnCollisionEnter(Collision collision) {
	    if (collision.gameObject.CompareTag("MovablePlace") && currentSpeed > 0f) {
		    _audioSource.loop = true;
		    _audioSource.pitch = GetPitch(_rb.velocity.magnitude);
		    _audioSource.Play();
		    
	    }
	    
    }
    private void OnCollisionExit(Collision collision) {
	    if (collision.gameObject.CompareTag("MovablePlace")) {
		    _audioSource.loop = false;
		    _audioSource.Stop();
		    _audioSource.pitch = 1f;
	    }
    }

    private void OnCollisionStay(Collision collision) {
	    if (collision.gameObject.CompareTag("MovablePlace")) {
		    _audioSource.pitch = GetPitch(currentSpeed);
	    }
	    if (collision.gameObject.CompareTag("MetallStick")) {
		    _audioSource.pitch = GetPitch(currentSpeed);
	    }
    }
    
    
    
	private void Stop()
    {
		//_rb.AddForce(Vector3.zero);
		//_rb.AddTorque(Vector3.zero);
		_rb.velocity = Vector3.zero;
		_rb.angularVelocity = Vector3.zero;
	}

	private void MovePlayerToCheckpointPosition()
    {
		transform.localPosition = _checkpointPosition;
		ReEnableMovement();
    }
	
	
	private void UpdateCheckPoint(Vector3 transformLocalPosition)
    {
		_checkpointPosition = transformLocalPosition;
    }
	
	void SetPlayerStatic()
	{
		_rb.velocity = Vector3.zero;
		_rb.isKinematic = true;
	}
	IEnumerator SecCoroutine()
	{
		yield return new WaitForSeconds(0.6f);
		SetPlayerStatic();
	}
	private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("DeathTrigger"))
        {
			_isReadyToMove = false;
			_isReadyToPlay = false;
			Stop();
			MovePlayerToCheckpointPosition();
			return;
        }
        if (other.CompareTag("CheckPoint"))
        {
			UpdateCheckPoint(transform.localPosition);
			Debug.Log("Checkpoint!");
		}
        if (other.CompareTag("Finish"))
        {
	        AudioSource.PlayClipAtPoint(WinSound, transform.position);
	        Collider collider = other.gameObject.GetComponent<Collider>();
			DOTween.To(() => _rb.velocity, x => _rb.velocity = x, Vector3.zero, 0.6f)
				.SetEase(Ease.InOutQuad)
				.OnComplete(() =>
				{
					DOTween.To(() => frictionCoefficient, x => frictionCoefficient = x, 10f, 0.6f)
						.SetEase(Ease.InOutQuad)
						.OnUpdate(() =>
						{
							PhysicMaterial physicsMat = collider.sharedMaterial;
							physicsMat.staticFriction = frictionCoefficient;
							physicsMat.dynamicFriction = frictionCoefficient;
							collider.sharedMaterial = physicsMat;
						});
				});
			cameraFollow.enabled = false;
			StartCoroutine(SecCoroutine());
			CameraMoveToTarget(finishLinePosition, moveToFinishLineDuration);
			CurrentPanel.SetActive(false);
			panel.SetActive(true);
			// SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    


}

