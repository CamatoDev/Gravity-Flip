using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Animator playerAnimator;
    Rigidbody rb;
    const float timeOut = 5f;
    float axisH, axisV;
    [SerializeField]
    float walkSpeed = 2f, runSpeed = 5f, rotationSpeed = 100f, jumpForce = 150f, countDown = timeOut;

    void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        axisH = Input.GetAxis("Horizontal");
        axisV = Input.GetAxis("Vertical");

        if (axisV > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
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
        if (axisV < 0)
        {
            transform.Translate(Vector3.forward * axisV * walkSpeed * Time.deltaTime);
            playerAnimator.SetBool("walkBack", true);
            playerAnimator.SetFloat("run", 0);
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce);
            playerAnimator.SetTrigger("jump");
        }
    }
}