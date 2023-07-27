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
	public Camera Playercamera;
	private Rigidbody _rb;
	private int _count;
	private bool _isReadyToMove, _isReadyToPlay = true;
	private Vector3 _initialPostion, _checkpointPosition;
	public AudioClip WinSound;
	public AudioSource _audioSource;
	public const float MaxSpeed = 20f;
	public float currentSpeed;
	public float frictionCoefficient = 1.5f;
	public Vector3 finishLinePosition;
	private CameraFollow _cameraFollow;
	public float moveToFinishLineDuration = 1.0f;
	public GameObject panel;
	public GameObject CurrentPanel;
	public CameraFollow cameraFollow;
	public AudioClip metallSound;
	public float smoothness;
	public VariableJoystick Joystick;
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
		return new Vector2(Joystick.Horizontal, Joystick.Vertical);
	}
	private void Moving()
	{
		Vector3 movementDirection = new Vector3(GetInputData().x, 0, GetInputData().y);

		movementDirection = Playercamera.transform.TransformDirection(movementDirection);
		movementDirection.y = 0;
		movementDirection.Normalize();

		Vector3 currentVelocity = _rb.velocity;
		currentVelocity.y = 0f;

		_rb.AddForce(movementDirection * speed - currentVelocity , ForceMode.Force);
	}
	void FixedUpdate()
	{
		if (!_isReadyToPlay)
		{
			return;
		}

		if (_isReadyToMove)
		{
			if (GetInputData() != Vector2.zero)
			{
				Moving();
			}
		}
	}
	
	private void CameraMoveToTarget(Vector3 position, float duration)
	{
		Playercamera.transform.DOMove(position, duration);
	}

    private void Update()
    {
		if (!_isReadyToPlay) return;
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


