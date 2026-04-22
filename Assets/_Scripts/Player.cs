using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed;
    
    private PlayerInputActions _playerControls;
    private InputAction _moveAction;
    
    private Rigidbody2D _rigidbody;
    private Vector2 _movement;

    private void Awake()
    {
        _rigidbody =  GetComponent<Rigidbody2D>();
        _playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _moveAction = _playerControls.Player.Move;
        _moveAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
    }

    private void Update()
    {
        _movement = _moveAction.ReadValue<Vector2>();
        
        MusicGenerator.Instance.SetTempo((transform.position.y + 5f) * 12 , 0f);
    }

    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = _movement * _speed;
    }
}