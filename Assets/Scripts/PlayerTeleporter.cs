using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{

    [SerializeField] private float _speed;
    [SerializeField] private Transform _moveTo;
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _cameraLookPoint;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private CameraController _cameraController;
    public Transform MoveTo { get { return _moveTo; } set { _moveTo = value; } }
    private Transform _transform;
    private void Start()
    {
        _transform = GetComponent<Transform>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if(Vector3.Distance(transform.position, _moveTo.position) <= 0.2f)
        {
            _player.transform.parent = null;
            _player.SetActive(true);
            _cameraController.SwitchLookPoint(_cameraLookPoint);
            _transform.parent = _player.transform;
            gameObject.SetActive(false);
        }
        else
        {
            if(_transform.position.y < _moveTo.transform.position.y)
                _transform.position = Vector3.MoveTowards(transform.position, new Vector3(_transform.position.x, _moveTo.position.y, _transform.position.z), _speed * Time.deltaTime);
            else
                _transform.position = Vector3.MoveTowards(transform.position, _moveTo.position, _speed * Time.deltaTime);
        }
    }
}
