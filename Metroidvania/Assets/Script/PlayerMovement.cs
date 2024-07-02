using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //public CharacterMovement controller;

    public float runSpeed = 40.0f;
    float horizontalMove = 0f;
    bool jump = false;
    bool dash = false;

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        //animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if(Input.GetKeyDown(KeyCode.Z))
        {
            jump = true;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            dash = true;
        }
    }

    public void OnFall()                //������ ó�� Event
    {
        //animator.SetBool("isJumping", ture));
    }

    public void OnLanding()             //�ٴڿ� ���� ó�� Event
    {
        //animator.SetBool("isJumping", false));
    }

    void FixedUpdate()
    {
        //ĳ���� ������ ������ �Լ�
        //controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash)

        jump = false;
        dash = false;
        
    }
}
