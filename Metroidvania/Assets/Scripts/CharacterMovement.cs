using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400.0f;                                //���� �� �߰��Ǵ� ���� ��
    [Range(0, 0.3f)][SerializeField] private float m_MovementSmoothing = 0.05f;         //�̵��� �ε巴�� �ϱ� ���� ��źȭ 
    [SerializeField] private bool m_AirControl = false;                                 //���� �߿��� ���� �� �� �ִ��� ����
    [SerializeField] private LayerMask m_WhatIsGround;                                  //ĳ���Ϳ��� ���� �������� �����ϴ� ����ũ
    [SerializeField] private Transform m_GroundCheck;                                   //�÷��̾ ���� ��Ҵ��� Ȯ���ϴ� ��ġ
    [SerializeField] private Transform m_WallCheck;                                     //�÷��̾ ���� ��Ҵ��� Ȯ���ϴ� ��ġ

    const float k_GroundedRadius = 0.2f;                            //���� ��Ҵ��� Ȯ���ϴ� ��ħ ���� �ݰ� 
    private bool m_IsGrounded;                                      //�÷��̾ ���� ��Ҵ��� ����
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;                              //�÷��̾ ���� ��� ������ �ٶ󺸰� �ִ� �� ����
    private Vector3 velocity = Vector3.zero;                        //�ӵ�
    private float limitFallSpeed = 25f;                              //���� �ӵ� ����

    public bool canDoubleJump = true;                               //�÷��̾ ���� ������ �� �� �ִ��� ����
    private bool m_IsWall = false;                                  //�÷��̾� �տ� ���� �ִ��� ����
    private bool m_IsWallSlidding = false;                          //�÷��̾ ���� Ÿ�� �ִ��� ����
    private bool oldWallSlidding = false;                           //���� �����ӿ��� �÷��̾ ���� Ÿ�� �־����� ����
    private float prevVelocityX = 0f;
    private bool canCheck = false;                                  //�÷��̾ ���� Ÿ�� �ִ��� Ȯ���ϱ� ���� ����

    private Animator animator;

    private float jumpWallStartX = 0;
    private float jumpWallDistX = 0;                    //�÷��̾�� �� ������ �Ÿ�
    private bool limitVelOnWallJump = false;            //���� FPS���� �� ���� �Ÿ��� �����ϱ� ���� ����

    [Header("Events")]
    [Space]
    public UnityEvent OnFallEvent;
    public UnityEvent OnLandEvent;
    private CharacterStatus status;
    private void Awake()                //������Ʈ���� �����ͼ� �����Ѵ�.
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        status = GetComponent<CharacterStatus>();

        if (OnFallEvent == null)                //�̺�Ʈ�� �߰� �ȵǾ� ���� �� �̺�Ʈ�� �Ҵ��Ѵ�. 
            OnFallEvent = new UnityEvent();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    private void FixedUpdate()                      //FiexedUpdate���� �̵� ������ �Ѵ�. 
    {
        bool wasGrouned = m_IsGrounded;             //�� üũ ���� ���� ������ �־ �� �����Ӹ��� ���� 
        m_IsGrounded = false;
        //GroundCheck ��ġ���� ������ ������ �Ͱ� ��ġ���� Ȯ�� 
        Collider2D[] groundColiiders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for(int i = 0; i < groundColiiders.Length; i++)
        {
            if (groundColiiders[i].gameObject != gameObject)
            {
                m_IsGrounded = true;
                if(!wasGrouned)
                {
                    OnLandEvent.Invoke();
                    animator.SetBool("IsJumping", false);
                    animator.SetBool("IsDoubleJumping", false);
                    animator.SetBool("JumpUp", false);
                    if (!m_IsWall && !status.isDashing) canDoubleJump = true;
                    if (m_Rigidbody2D.velocity.y < 0f) limitVelOnWallJump = false;                   
                }
            }
        }
        if (!m_IsGrounded) OnFallEvent.Invoke();                    //���� �ƴҷ� �������� �ִٴ� �̺�Ʈ�� ȣ��

        bool wasWallSlidding = m_IsWallSlidding;
        m_IsWallSlidding = false;
        m_IsWall = false;

        //WallCheck ��ġ���� ������ ������ �Ͱ� ��ġ���� Ȯ��
        Collider2D[] WallColiiders = Physics2D.OverlapCircleAll(m_WallCheck.position, k_GroundedRadius, m_WhatIsGround);
        for(int i = 0; i < WallColiiders.Length ; i++)
        {
            if (WallColiiders[i].gameObject != gameObject)
            {
                m_IsWall = true;
                if (!m_IsGrounded && !status.isDashing && m_Rigidbody2D.velocity.y < 0)
                {
                    m_IsWallSlidding = true;
                    if(!wasWallSlidding)                //���� �������� ���� �ƴϿ��ٸ� üũ�Ͽ� ������ ���� �����̵� �ϰ� ���� �ϰ� �Ѵ�. 
                    {
                        m_WallCheck.localPosition = new Vector3(-m_WallCheck.localPosition.x, m_WallCheck.localPosition.y, 0);
                        Flip();
                    }
                }
            }
        }

        prevVelocityX = m_Rigidbody2D.velocity.x;           //���� �ӵ��� �Ҵ��Ѵ�.

        if(limitVelOnWallJump)
        {
            if (m_Rigidbody2D.velocity.y < -0.5f) limitVelOnWallJump = false;
            jumpWallDistX = (jumpWallStartX - transform.position.x) * transform.localScale.x;       //������ �Ÿ��� ����Ѵ�.
            if(jumpWallDistX < -0.5f && jumpWallDistX > -1f)
            {
                status.canMove = true;
            }
            else if(jumpWallDistX < -1f && jumpWallDistX >= -2f)
            {
                status.canMove = true;
                m_Rigidbody2D.velocity = new Vector2(10.0f * transform.localScale.x, m_Rigidbody2D.velocity.y);
            }
            else if(jumpWallDistX < -2)
            {
                limitVelOnWallJump = false;
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
            }
            else if(jumpWallDistX > 0)
            {
                limitVelOnWallJump = false;
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
            }
        }

        if(!m_IsWallSlidding && wasWallSlidding)
        {
            m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
            animator.SetBool("IsWallSliding", false);
        }

        animator.SetFloat("Speed", Mathf.Abs(m_Rigidbody2D.velocity.x));        //�ִϸ����Ϳ� �ӵ� ���� 
    }

    public void Move(float move, bool jump , bool dash)
    {
        if(status.canMove)          //Ŭ�������� �̵��� �����ϴٰ� �ϸ�
        {
            if(dash && status.canDash && !m_IsWallSlidding) //��ð� ������ ������ �˻��Ͽ� ��� ����
            {
                status.StartDash(0.1f);
            }
            if(status.isDashing)        //��� ���� �� ó��
            {
                m_Rigidbody2D.velocity = new Vector2(transform.localScale.x * status.mDashForce, 0);
            }
            else if(m_IsGrounded || m_AirControl)   //�÷��̾ ���� ��Ұų� ���� ��� ���������� 
            {
                if (m_Rigidbody2D.velocity.y < -limitFallSpeed) m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, -limitFallSpeed); //�ӵ� ����

                Vector3 targetVelocity = new Vector2(move * 10.0f, m_Rigidbody2D.velocity.y);   //��ǥ �ӵ��� ���
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing); //�ӵ��� �ε巴�� ����

                if(move > 0 && !m_FacingRight && !m_IsWallSlidding) //�Է��� ������, �÷��̾ ���� ���� �ִٸ�
                {
                    Flip();                                         //�÷��̾ �����´�.
                }
                else if(move < 0 && m_FacingRight && !m_IsWallSlidding)
                {
                    Flip();
                }               
            }

            if(m_IsGrounded && jump)        //�÷��̾� ����
            {
                animator.SetBool("IsJumping", true);                    //�ִϸ����Ϳ� ���� ���� ����
                animator.SetBool("JumpUp", true);
                m_IsGrounded = false;                                   //�����ϸ鼭 ���� ���� �����Ƿ� BOOL �� ����
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));   //�÷��̾�� ���� ���� �߰� 
                canDoubleJump = true;
            }
            else if(!m_IsGrounded && jump && canDoubleJump && !m_IsWallSlidding)
            {
                canDoubleJump = false;
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce / 1.2f));   //�÷��̾�� ���� ���� �߰� 
                animator.SetBool("IsDoubleJumping", true);              //�ִϸ����Ϳ� ���� ���� ���� ����
            }
            else if(m_IsWall && !m_IsGrounded)
            {
                if(!oldWallSlidding && m_Rigidbody2D.velocity.y < 0 || status.isDashing)    //���� �پ �����̵� �Ǵ� ����
                {
                    m_IsWallSlidding = true;
                    m_WallCheck.localPosition = new Vector3(-m_WallCheck.localPosition.x, m_WallCheck.localPosition.y, 0);
                    Flip();
                    status.checkTimer.Start();
                    canDoubleJump = true;
                    animator.SetBool("IsWallSliding", true);
                }
                status.isDashing = false;

                if(m_IsWallSlidding)                                            //�� �����̵� ��
                {
                    if(move * transform.localScale.x > 0.1f)
                    {
                        status.endSlidingTimer.Start();
                    }
                    else
                    {
                        oldWallSlidding = true;
                        m_Rigidbody2D.velocity = new Vector2(-transform.localScale.x * 2, -5);
                    }
                }

                if(jump && m_IsWallSlidding)                                    //�� �����̵� �� ����
                {
                    animator.SetBool("IsJumping", true);                        //�ִϸ����Ϳ� ���� ���� ����
                    animator.SetBool("JumpUp", true);
                    m_Rigidbody2D.velocity = new Vector2(0.0f, 0.0f);           //�ӵ��� ����
                    m_Rigidbody2D.AddForce(new Vector2(transform.localScale.x * m_JumpForce * 1.2f, m_JumpForce));  //���� ���� �������ش�. 
                    jumpWallStartX = transform.position.x;
                    limitVelOnWallJump = true;
                    canDoubleJump = true;
                    m_IsWallSlidding = false;
                    animator.SetBool("IsWallSliding", false);
                    oldWallSlidding = false;
                    m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
                    status.moveTimer.Start();
                }
                else if(dash && status.canDash)                                     //�� ��� ���� 
                {
                    m_IsWallSlidding = false;
                    animator.SetBool("IsWallSliding", false);
                    oldWallSlidding = false;
                    m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
                    canDoubleJump = true;
                    status.StartDash(0.1f);
                }
            }
            else if(m_IsWallSlidding && !m_IsWall && status.checkTimer.GetRemainingTime() <= 0) //�� �����̵� ���̰� ���� ������ ��
            {
                m_IsWallSlidding = false;
                animator.SetBool("IsWallSliding", false);
                oldWallSlidding = false;
                m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
                canDoubleJump = true;
            }
        }
    }

    private void Flip()                             //�ø� �Լ��� ȣ�� �Ͽ� ���� ��ȯ
    {
        m_FacingRight = !m_FacingRight;             //�÷��̾ �ٶ󺸴� ������ ��ȯ
        Vector3 theScale = transform.localScale;    //�÷��̾��� x ���� �������� -1�� ���ؼ� ������ �ش�.
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
