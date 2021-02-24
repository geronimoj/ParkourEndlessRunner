using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public float speed = 10;
    public float jumpForce = 10;

    protected CharacterController cc = null;
    protected Vector2 moveInput = new Vector2();
    protected Vector3 velocity = new Vector2();

    protected bool jumpInput = false;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        jumpInput = jumpInput == false ? Input.GetButton("Jump") : true;
    }

    void FixedUpdate()
    {
        Vector3 delta = (moveInput.x * transform.right + moveInput.y * Vector3.Cross(transform.right, Vector3.up)) * speed;

        if (cc.isGrounded || moveInput.x !=0 || moveInput.y != 0)
        {
            velocity.x = delta.x;
            velocity.z = delta.z;
        }

        if (cc.isGrounded && velocity.y < 0)
            velocity.y = 0;

        if (jumpInput && cc.isGrounded)
            velocity.y = jumpForce;
        jumpInput = false;

        velocity += Physics.gravity * Time.fixedDeltaTime;

        cc.Move(velocity * Time.fixedDeltaTime);
    }
}
