using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Main Components")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _playerMass = 5;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private CameraController _cameraController;
    private Transform _transform;
    [Header("Move")]
    [SerializeField] private bool _canMove = true;
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _rotationSpeed = 15f;
    private Vector3 movementDirection;

    [Header("Jump")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private float _jumpForce = 15f;
    private float gravityForce = -9.8f;
    [SerializeField] private float tempFallingSpeed = -1f;

    [Header("Teleport")]
    [SerializeField] private float _teleportRadiusCheck = 10f;
    [SerializeField] private PlayerTeleporter _playerTeleporter;
    [SerializeField] private GameObject _teleportToGameObj;
    [SerializeField] private GameObject _teleportObj;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _cameraTransform = Camera.main.GetComponent<Transform>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();

        Teleport();
    }
    private void Move()
    {
        _isGrounded = _characterController.isGrounded;
        movementDirection.x = 0f;
        movementDirection.z = 0f;
        if ((Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f) && _canMove)
        {
            MovePlayer();
            RotatePlayer();
        }
        CalculateFallingSpeed();
        Jump();
        movementDirection.y = tempFallingSpeed;
        _characterController.Move(movementDirection * Time.deltaTime);
    }

    private void MovePlayer()
    {
        movementDirection.x = Input.GetAxisRaw("Horizontal") * _speed;
        movementDirection.z = Input.GetAxisRaw("Vertical") * _speed;
        movementDirection = Vector3.ClampMagnitude(movementDirection, _speed);
    }

    private void RotatePlayer()
    {
        Quaternion tempCameraRotation = _cameraTransform.rotation;
        _cameraTransform.rotation = Quaternion.Euler(0f, _cameraTransform.eulerAngles.y, 0f);
        movementDirection = _cameraTransform.TransformDirection(movementDirection);
        _cameraTransform.rotation = tempCameraRotation;

        _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.LookRotation(new Vector3(movementDirection.x, 0, movementDirection.z)), _rotationSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
            tempFallingSpeed = _jumpForce;
    }

    private void CalculateFallingSpeed()
    {
        if (!_isGrounded)
            tempFallingSpeed += gravityForce * _playerMass * Time.deltaTime;
        else
            tempFallingSpeed = -1f;
    }

    private void Teleport()
    {
        CheckTeleport();
        TeleportTo();
    }

    private void TeleportTo()
    {
        if (Input.GetKeyDown(KeyCode.T) && _teleportToGameObj != null)
        {
            tempFallingSpeed = _jumpForce;
            movementDirection.y = tempFallingSpeed;
            _characterController.Move(movementDirection * Time.deltaTime);

            _teleportObj.transform.parent = null;
            _playerTeleporter.MoveTo = _teleportToGameObj.transform;
            _teleportObj.SetActive(true);
            _teleportToGameObj.GetComponent<Outline>().enabled = false;
            _cameraController.SwitchLookPoint(_teleportObj.transform);
            _transform.parent = _teleportObj.transform;
            gameObject.SetActive(false);
        }
    }
    private void CheckTeleport()
    {
        if(_teleportToGameObj != null) _teleportToGameObj.GetComponent<Outline>().enabled = false;
        Collider[] colliders = Physics.OverlapSphere(_transform.position, _teleportRadiusCheck);
        GameObject withLowerAngle = null;
        float lowestAngle = 170f;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Teleport"))
            {
                float angle = Vector3.Angle(_cameraTransform.forward, _cameraTransform.position + collider.transform.position);
                if (angle < lowestAngle && angle >= 0f && angle < 180f)
                {
                    lowestAngle = angle;
                    withLowerAngle = collider.gameObject;
                }
            }
        }
        _teleportToGameObj = withLowerAngle;
        if(_teleportToGameObj != null)
        {
            _teleportToGameObj.GetComponent<Outline>().enabled = true;
        }
        
    }

    public void AddVertSpeed(float speed)
    {
        tempFallingSpeed = speed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _teleportRadiusCheck);
    }
}