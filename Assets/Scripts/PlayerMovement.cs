using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float mouseSensitivity;
    public float selfRightingForce;
    public float speed;
    public float jumpForce;
    public Transform cam;
    Rigidbody rb;

    FloorDetector floorDetector;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        floorDetector = GetComponentInChildren<FloorDetector>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CameraMovement();
        //SelfRightingForce();
        bool onGround = OnGround();
        Movement(onGround ? 1f : .1f, onGround);
    }

    void CameraMovement()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        transform.rotation *= Quaternion.AngleAxis(mouseSensitivity * mouseX * Time.deltaTime, transform.up);
        cam.localRotation *= Quaternion.AngleAxis(mouseSensitivity * mouseY * Time.deltaTime, Vector3.right);
    }

    void Movement(float multiplier, bool canJump)
    {
        float xMove = Input.GetAxis("Horizontal");
        float yMove = Input.GetAxis("Vertical");
        Vector2 moveVector = new(xMove, yMove);
        moveVector = moveVector.sqrMagnitude > 1f ? moveVector.normalized : moveVector;
        if (canJump)
        {
            rb.linearVelocity = (moveVector.y * transform.forward + moveVector.x * transform.right) * speed * multiplier + rb.linearVelocity.y * Vector3.up;

        }
        else
        {
            Vector3 groundV = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);

            groundV += (moveVector.y * transform.forward + moveVector.x * transform.right) * speed * multiplier * Time.deltaTime;
            if (groundV.magnitude > speed)
            {
                groundV = groundV.normalized;
            }
            rb.linearVelocity = groundV.x * Vector3.right + groundV.y * Vector3.forward + rb.linearVelocity.y * Vector3.up;
        }

        if (canJump)
        {
            if (Input.GetButtonDown("Jump"))
                rb.linearVelocity += jumpForce * transform.up;
        }

    }

    bool OnGround()
    {
        return floorDetector.detected;
        //return Physics.Raycast(transform.position, Vector3.down, 1.15f, LayerMask.GetMask("Ground"));

    }

    void SelfRightingForce()
    {
        Quaternion target = Quaternion.LookRotation(new Vector3(transform.forward.x, 0f, transform.forward.z), transform.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, selfRightingForce * Time.deltaTime);
    }
}
