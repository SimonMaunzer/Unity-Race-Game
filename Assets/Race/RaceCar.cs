using UnityEngine;
using UnityEngine.InputSystem;

public class RaceCar : MonoBehaviour
{
    public float maxSpeed = 100f;
    public float acceleration = 100f;
    public float turningSpeed = 45f;
    [Range(0f, 1f)] public float driftingFactor = 0f; 
    private Rigidbody rb;
    InputAction steer;
    InputAction reset1;
    InputAction reset2;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        steer = InputSystem.actions.FindAction("Move");
        reset1 = InputSystem.actions.FindAction("Previous");
        reset2 = InputSystem.actions.FindAction("Next");
    }

    // Update is called once per frame
    void Update()
    {
        if (reset1.WasReleasedThisFrame())
        {
            transform.Rotate(180,0,0);
            transform.position += Vector3.up;           
        }

        if (reset2.WasReleasedThisFrame())
        {
            transform.Rotate(0,45,0);
            transform.position += Vector3.up;           
        }

    }

    void FixedUpdate()
    {
        if (!touchingGround)
        {
            rb.linearVelocity += Vector3.down*100*Time.fixedDeltaTime;
            return;
        }       
        
        Vector2 direction = steer.ReadValue<Vector2>();

        // Rotate
        if (rb.linearVelocity.magnitude > 0.1f) transform.Rotate(Vector3.up * direction.x * turningSpeed * Time.deltaTime);

        // Add Movement
        rb.AddForce(transform.forward * direction.y * acceleration);
        if (rb.linearVelocity.magnitude > maxSpeed) rb.AddForce(transform.forward * direction.y * -1 * acceleration);

        // Adjust drifting        
        float dotProduct = Vector3.Dot(rb.linearVelocity.normalized, transform.forward);
        float speed = rb.linearVelocity.magnitude;
        Vector3 forward = transform.forward * (dotProduct > 0 ? 1 : -1) * (1-driftingFactor);
        Vector3 momentum = rb.linearVelocity.normalized  * driftingFactor;
        rb.linearVelocity = (forward + momentum) *speed;   

    }

    private bool touchingGround = false;

    void OnCollisionEnter(Collision collision)
    {
        touchingGround = true;
    }

    void OnCollisionExit(Collision collision)
    {
        touchingGround = false;
    }

    void OnCollisionStay(Collision collision)
    {
        touchingGround = true;
    }

}