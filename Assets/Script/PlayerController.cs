
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float shiftRun = 8f;

    private float Hori;
    private float Vert;

    [Header("Jump")]
    public float jumpForce = 7f;
    public bool isGround;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundRadius = 0.25f;

    [Header("Levitation")]
    public float maxLevitationTime = 4f;
    public float levitationLift = 3f;
    public float levitationRecharge = 1f;

    private float levitationTime;
    private Rigidbody rb;

    public float maxLevitationHeight = 3f;
    private float groundHeight;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        levitationTime = maxLevitationTime;
    }

    private void Update()
    {
        // Movement input
        Hori = Input.GetAxis("Horizontal");
        Vert = Input.GetAxis("Vertical");

        // Check if player is grounded
        isGround = Physics.CheckSphere(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        // Recharge levitation on the ground
        if (isGround)
        {
            levitationTime = Mathf.Min(
                maxLevitationTime,
                levitationTime + levitationRecharge * Time.deltaTime
            );
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (!isGround && Input.GetKey(KeyCode.Space) && levitationTime > 0f && transform.position.y < groundHeight + maxLevitationHeight)
        {
            levitationTime -= Time.deltaTime;

            rb.AddForce(
                Vector3.up * levitationLift,
                ForceMode.Acceleration
            );
        }
        if (isGround)
        {
            groundHeight = transform.position.y;

            levitationTime = Mathf.Min(
                maxLevitationTime,
                levitationTime + levitationRecharge * Time.deltaTime
            );
        }
    }

    private void FixedUpdate()
    {
        Vector3 move = transform.right * Hori +
                       transform.forward * Vert;

        move.y = 0f;
        move = Vector3.ClampMagnitude(move, 1f);

        // Hold Shift to sprint
        float currentSpeed = Input.GetKey(KeyCode.LeftShift)
            ? shiftRun
            : speed;

        // Move using Rigidbody physics
        Vector3 velocity = rb.linearVelocity;

        rb.linearVelocity = new Vector3(
            move.x * currentSpeed,
            velocity.y,
            move.z * currentSpeed
        );
        if (transform.position.y >= groundHeight + maxLevitationHeight && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x,0f, rb.linearVelocity.z );
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundRadius
            );
        }
    }
}
