using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Animator playerAnimator;
    Rigidbody rb;
    const float timeOut = 15f;
    float axisH, axisV;
    [SerializeField]
    float walkSpeed = 2f, runSpeed = 5f, rotationSpeed = 100f, jumpForce = 150f, countDown = timeOut, jumpCoolDown = 0.5f, turnCoolDown = 1.5f;

    public float jumpCooling = 0f, turnCooling = 0f;
    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;
    bool isGrounded;
    void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Debug.Log(Input.GetJoystickNames().ToString());
        axisH = Input.GetAxis("Horizontal");
        axisV = Input.GetAxis("Vertical");

        //Debug.Log("axisH: " + Input.GetAxis("Vertical"));
        //Debug.Log("axisV: " + Input.GetAxis("Horizontal"));

        if (axisV > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint"))
            {
                transform.Translate(Vector3.forward * axisV * runSpeed * Time.deltaTime);
                playerAnimator.SetFloat("run", axisV);
            }
            else
            {
                transform.Translate(Vector3.forward * axisV * walkSpeed * Time.deltaTime);
                playerAnimator.SetBool("walk", true);
                playerAnimator.SetBool("walkBack", false);
                playerAnimator.SetFloat("run", 0);
            }

        }
        else
        {
            playerAnimator.SetBool("walk", false);
            playerAnimator.SetBool("walkBack", false);
        }

        if (axisV != 0)
        {
            transform.Rotate(Vector3.up * axisH * rotationSpeed * Time.deltaTime);
        }

        turnCooling += Time.deltaTime;
        if (axisV < 0 && turnCooling > turnCoolDown)
        {
            Debug.Log("Left Control Pressed && jumpCooling > turnCoolDown");
            transform.Rotate(Vector3.up * 180f);
            turnCooling = 0f;
        }
        else
        {
            playerAnimator.SetBool("walkBack", false);
        }

        //Idle Dance Rumba
        if (axisH == 0 && axisV == 0)
        {
            countDown -= Time.deltaTime;
            if (countDown <= 0)
            {
                playerAnimator.SetBool("dance", true);
                countDown = timeOut;
            }
        }
        else
        {
            playerAnimator.SetBool("dance", false);
            countDown = timeOut;
        }
    }

    void FixedUpdate()
    {
        GroundeCheck();
        Debug.Log("isGround = " + isGrounded);

        jumpCooling += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButton("JoystickJump"))
        {
            if (jumpCooling > jumpCoolDown)
            {
                rb.AddForce(Vector3.up * jumpForce);
                playerAnimator.SetTrigger("jump");
                jumpCooling = 0f;
            }
            playerAnimator.SetBool("dance", false);
            countDown = timeOut;

        }

        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Debug.Log("Left Control Pressed");
        }
    }

    void GroundeCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }
}