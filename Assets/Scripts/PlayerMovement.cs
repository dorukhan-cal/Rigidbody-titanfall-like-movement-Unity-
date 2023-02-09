using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float groundDrag;


    [Header("References")]
    [SerializeField] private Transform playerReference;
    [SerializeField] private Transform groundReference;

    [Header("Layers")]
    [SerializeField] private LayerMask groundMask;

    private float horizontalInput, verticalInput;
    private float groundDistance = 0.2f;
    private bool isGrounded;
    private bool canJump;

    private Vector3 movementDirection;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        checkGround();

        getInput();

        handleDrag();
    }

    void FixedUpdate()
    {
        handlePlayerMovement();
    }

    private void getInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void handlePlayerMovement()
    {
        
        //Speed limiter here
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(flatVelocity.magnitude > moveSpeed)
        {
            Vector3 constrainedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(constrainedVelocity.x, rb.velocity.y, constrainedVelocity.y);
        }

       /*Alternative speed limiter using Mathf.Clamp
        visually works fine, 
        however makes the diagonal speed of the rigidbody exceed moveSpeed
        could be good optimization if worked on a little bit.
        rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -moveSpeed , moveSpeed), rb.velocity.y, Mathf.Clamp(rb.velocity.z, -moveSpeed , moveSpeed)); */

        //The actual movement 
        movementDirection = playerReference.forward * verticalInput + playerReference.right * horizontalInput;
        rb.AddForce(movementDirection.normalized * moveSpeed * 5f, ForceMode.Force);
    }

    private void checkGround()
    {
        isGrounded = Physics.CheckSphere(groundReference.position, groundDistance, groundMask);
    }

    private void handleDrag()
    {
        if(isGrounded) rb.drag = groundDrag;
        else rb.drag = 0f;
    }

    private void handleJump()
    {
        //reset y velocity to keep consistent jump height
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
    }
}
