using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour{
    private Camera mainCamera;
    private Rigidbody2D rigbody;
    private Collider2D capsuleCollider;
    private Vector2 inputAxis = Vector2.zero;

    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    public float moveSpeed = 5f;
    public float maxJumpHeight = 2f;
    public float maxJumpTime = 1f;
    public float jumpForce => 2f * maxJumpHeight / (maxJumpTime / 1f);
    public float gravity => -2f * maxJumpHeight / Mathf.Pow(maxJumpTime / 2f, 2f);

    public bool isGrounded {get; private set;}
    public bool isRunning => Mathf.Abs(rigbody.linearVelocity.x) > 0.25f;
    public bool isJumping {get; private set;}
    public bool isFalling => rigbody.linearVelocity.y < 0f && !isGrounded;

    private void Awake()
    {
        mainCamera = Camera.main;
        rigbody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<Collider2D>();
        rigbody.gravityScale = 0f;
    }

    private void OnEnable()
    {
        rigbody.bodyType = RigidbodyType2D.Dynamic;
        capsuleCollider.enabled = true;
        moveAction.action.Enable();
        jumpAction.action.Enable();
        isJumping = false;
    }

    private void OnDisable()
    {
        rigbody.bodyType = RigidbodyType2D.Kinematic;
        capsuleCollider.enabled = false;
        moveAction.action.Disable();
        jumpAction.action.Disable();
        inputAxis = Vector2.zero;
        isJumping = false;
    }

    // Update is called once per frame
    private void Update()
    {
        inputAxis = moveAction.action.ReadValue<Vector2>();

        isGrounded = rigbody.Raycast(Vector2.down);
        if (jumpAction.action.WasPressedThisFrame() && isGrounded)
        {
            GroundedMovement();
        }
        ApplyGravity();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        HorizontalMovement();
        if (isJumping)
        {
            rigbody.linearVelocity = new Vector2(rigbody.linearVelocity.x, jumpForce);
            isJumping = false;
        }
        ClampPosition();
    }

    private void HorizontalMovement()
    {
        rigbody.linearVelocity = new Vector2(inputAxis.x * moveSpeed, rigbody.linearVelocity.y);

        //Wall collision
        if (inputAxis.x != 0 && rigbody.Raycast(new Vector2(inputAxis.x, 0))) {
            rigbody.linearVelocity = new Vector2(0f, rigbody.linearVelocity.y);
        }

        //Flip mario to face the direction he's moving
        if (rigbody.linearVelocity.x > 0f) {
            transform.eulerAngles = Vector3.zero;
        } else if (rigbody.linearVelocity.x < 0f) {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void GroundedMovement()
    {
        //Prevent gravity from building up
        rigbody.linearVelocity = new Vector2(rigbody.linearVelocity.x, 0f);
        isJumping = rigbody.linearVelocity.y > 0f;


        if (jumpAction.action.WasPressedThisFrame())
        {
            rigbody.linearVelocity = new Vector2(rigbody.linearVelocity.x, jumpForce);
            isJumping = true;
        }
    }

    private void ApplyGravity()
    {
        bool falling = rigbody.linearVelocity.y < 0f || !jumpAction.action.IsPressed();
        float multiplier = falling ? 2f : 1f;
        rigbody.linearVelocity += Physics2D.gravity.y * Time.fixedDeltaTime * multiplier * Vector2.up;
        rigbody.linearVelocity = new Vector2(rigbody.linearVelocity.x, Mathf.Max(rigbody.linearVelocity.y, gravity));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            //Bounce off enemy head
            if (transform.DotTest(collision.transform, Vector2.down))
            {
                rigbody.linearVelocity = new Vector2(rigbody.linearVelocity.x, jumpForce / 3f);
                isJumping = true;
            }
        }
        else if (collision.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            //Stop vertical movement if mario bonks his head
            if (transform.DotTest(collision.transform, Vector2.up)) {
                rigbody.linearVelocity = new Vector2(rigbody.linearVelocity.x, 0f);
            }
        }
    }

    private void ClampPosition()
    {
        Vector2 pos = rigbody.position;
        Vector2 leftEdge = mainCamera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        
        pos.x = Mathf.Clamp(pos.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);
        rigbody.position = pos;
    }
}
