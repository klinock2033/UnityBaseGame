using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    private Rigidbody _rb;
    private InputSystem_Actions _input;
    private Vector3 _moveInput;


    [Header("Movement settings")] 
    public float moveSpeed = 5f;
    
    
    [Header("Jump settings")]
    public float jumpForce = 8;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField]private float _coyoteTimeCounter = 0;
    [SerializeField]private float _blockJumpTimer = 0f;
    [SerializeField] private float _jumpCooldown = 0.2f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckZone;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _groundDistance = 0.2f;
    
    

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogError("Rigidbody missing!");
            enabled = false;
            return;
        }
        _input = new InputSystem_Actions();
        if (_groundCheckZone == null)
        {
            _groundCheckZone =  transform.Find("GroundCheckZone");
            if (_groundCheckZone == null)
            {
                Debug.LogError("GroundCheckZone not assigned or found!");
            }
        }

        if (_groundMask == 0)
        {
            _groundMask = LayerMask.GetMask("Ground");
            if (_groundMask == 0)
            {
                Debug.LogError("GroundMask not assigned or found!");
            }
        }
        
        
    }

    void OnEnable()
    {
        _input.Enable();
        _input.Player.Move.performed += OnMovePerformed;
        _input.Player.Move.canceled += OnMoveCanceled;
        _input.Player.Jump.performed += OnJumpPerformed;
    }
    void OnDisable()
    {
        _input.Player.Move.performed -= OnMovePerformed;
        _input.Player.Move.canceled -= OnMoveCanceled;
        _input.Player.Jump.performed -= OnJumpPerformed;
        _input.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 v = ctx.ReadValue<Vector2>();
        
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        
        _moveInput = (forward * v.y + right * v.x).normalized;
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _moveInput = Vector3.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        Jump();
    }
    private bool IsGrounded()
    {
        return Physics.CheckSphere(_groundCheckZone.position, _groundDistance, _groundMask);
    }
    void Jump()
    {
        if (_coyoteTimeCounter > 0f && _blockJumpTimer <= 0)
        {
            _coyoteTimeCounter = 0f;
            _blockJumpTimer = _jumpCooldown;
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, jumpForce, _rb.linearVelocity.z);
        }
        
    }

    void OnDrawGizmos()
    {
        if (_groundCheckZone ==  null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheckZone.position, _groundDistance);
    }

    void Update()
    {
        if (IsGrounded())
        {
            _coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            if (_coyoteTimeCounter > 0f)
            {
                _coyoteTimeCounter -= Time.deltaTime;
            }
            
        }

        if (_blockJumpTimer > 0f)
        {
            _blockJumpTimer -= Time.deltaTime;
        }
    }
    void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}
