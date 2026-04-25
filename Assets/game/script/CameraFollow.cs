using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 5f, -8f);
    [SerializeField] private float _smoothSpeed = 5f;
    [Range(0f, 2f)]
    [SerializeField] private float _mouseSensitivity = 10f;
    
    [Range(-90, 0)]
    [SerializeField] private float _minPitch = -30f;
    [Range(0, 90)]
    [SerializeField] private float _maxPitch = 30f;
    
    [SerializeField] private float _collisionRadius = 0.3f;
    [SerializeField] private LayerMask _collisionMask;

    private InputSystem_Actions _input;
    private Vector2 _lookInput;
    
    private float _yaw = 0f;
    private float _pitch = 0f;

    void Awake()
    {
        _input = new InputSystem_Actions();
    }

    void OnEnable()
    {
        _input.Enable();
        _input.Player.Look.performed += OnLook;
        _input.Player.Look.canceled += OnLook;
    }

    void OnDisable()
    {
        _input.Player.Look.performed -= OnLook;
        _input.Player.Look.canceled -= OnLook;
        _input.Disable();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        _lookInput = ctx.ReadValue<Vector2>();
    }
    
    private void LateUpdate()
    {
        if (_target == null) return;
        

        _yaw += _lookInput.x * _mouseSensitivity;
        _pitch -= _lookInput.y * _mouseSensitivity;
        
        //limit pitch
        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
        
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        
        
        Vector3 desiredPosition = _target.position + rotation * _offset;
        
        Vector3 direction = (desiredPosition - _target.position).normalized;
        float distance = Vector3.Distance(_target.position, desiredPosition);
        Debug.DrawRay(_target.position, direction * distance, Color.red);
        
        if (Physics.SphereCast(_target.position, _collisionRadius, direction, out RaycastHit hit, distance,
                _collisionMask))
        {
            desiredPosition = _target.position + direction * (hit.distance - 0.1f);
        }
        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
        transform.LookAt(_target);
        
    }
}
