using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Animator playerAnimator;
    float axisH, axisV;
    [SerializeField]
    float walkSpeed = 2f, runSpeed = 5f, rotationSpeed = 100f, jumpForce = 2f;

    void Awake()
    {
        playerAnimator = GetComponent<Animator>();
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
                playerAnimator.SetFloat("run", 0);
            }

        }
        else
        {
            playerAnimator.SetBool("walk", false);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            playerAnimator.SetTrigger("jump");
        }

        if (axisH != 0)
        {
            Debug.Log("AxisH " + axisH);
            Debug.Log("Vector3.up " + Vector3.up);
            Debug.Log("rotationSpeed " + rotationSpeed);
            Debug.Log("Time.deltaTime " + Time.deltaTime);
        }
        transform.Rotate(Vector3.up * axisH * rotationSpeed * Time.deltaTime);
    }
}
