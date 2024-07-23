using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float life = 10;
    private bool isPlat;
    private bool isObstacle;

    private Transform fallCheck;                //체크 위치는 Awake 가져온다.
    private Transform wallCheck;

    public LayerMask turnLayerMask;             //땅인 레이어 마스크를 가져온다.
    private Rigidbody2D rigidbody2D;
    private bool facingRight = true;
    public float speed = 5f;
    public bool isInvincible = false;           //무적 체크
    private bool isHitted = false;              //타격 체크

    private Timer hitTimer;
    private Timer destoryTimer;

    public float knockBackDruation = 0.2f;
    private float knockbackCounter;

    private Animator animator;


    private void Awake()
    {
        fallCheck = transform.Find("FallCheck");        //하위 하이러키에서 FallCheck(게임 오브젝트 이름) 찾아서 할당
        wallCheck = transform.Find("WallCheck");
        rigidbody2D = GetComponent<Rigidbody2D>();
        hitTimer = new Timer(0.1f);                     //무적 시간
        destoryTimer = new Timer(3.25f);                //죽는 모션 보고 캐릭터 삭제
        animator = GetComponent<Animator>();
    }
    void Flip()                                         //캐릭터 방향 전환
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && life > 0 && !isInvincible)
        {
            collision.gameObject.GetComponent<CharacterStatus>().ApplyDamage(2, transform.position);
        }
    }

    void EndInvincible()            //무적이 끝났을 때 상태 값을 초기화 시킨다.
    {
        isHitted = false;
        isInvincible = false;
        animator.SetBool("Hit", false);
    }

    void StartHitTimer()                 //피격 받았을 때 타이머 설정
    {
        isHitted = true;
        isInvincible = true;
        hitTimer.Start();
    }

    void StartDestorySequence()    //적군 피격 후 설정 값 함수
    {
        animator.SetBool("IsDead", true);
        CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
        capsule.size = new Vector2(1f, 0.25f);
        capsule.offset = new Vector2(0f, -0.8f);
        capsule.direction = CapsuleDirection2D.Horizontal;
        rigidbody2D.velocity = Vector2.zero;
        destoryTimer.Start();
    }

    public void ApplyDamage(float damage)
    {
        if (!isInvincible)
        {
            float direction = damage / Mathf.Abs(damage);
            damage = Mathf.Abs(damage);
            animator.SetTrigger("Hit");
            life -= damage;
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.AddForce(new Vector2(direction * 500f, 100f));
            StartHitTimer();
            knockbackCounter = knockBackDruation;
        }
    }

    private void FixedUpdate()
    {
        if(life < 0)
        {
            if(!destoryTimer.IsRunning())
            {
                StartDestorySequence();
            }
            return;
        }

        if(knockbackCounter > 0)
        {
            return;
        }

        isPlat = Physics2D.OverlapCircle(fallCheck.position, 0.2f, 1 << LayerMask.NameToLayer("Default"));
        isObstacle = Physics2D.OverlapCircle(wallCheck.position, 0.2f, turnLayerMask);

        if(!isHitted && Mathf.Abs(rigidbody2D.velocity.y) < 0.5f)
        {
            if(isPlat && !isObstacle)
            {
                if (facingRight)
                {
                    rigidbody2D.velocity = new Vector2(-speed , rigidbody2D.velocity.y);
                }
                else
                {
                    rigidbody2D.velocity = new Vector2(speed, rigidbody2D.velocity.y);
                }
            }
            else
            {
                Flip();
            }
        }
    }

    void Update()
    {
        hitTimer.Update(Time.deltaTime);
        destoryTimer.Update(Time.deltaTime);

        if (isInvincible)                            //무적이고
        {
            if (hitTimer.GetRemainingTime() <= 0)    //타이머 체크해서 무적 시간이 끝났을 때
            {
                EndInvincible();
            }
        }

        if (knockbackCounter > 0)
        {
            knockbackCounter -= Time.deltaTime;
            if (knockbackCounter <= 0)
            {
                rigidbody2D.velocity = Vector2.zero;
            }
        }

        if (destoryTimer.IsRunning() && destoryTimer.GetRemainingTime() <= 0)
        {
            Destroy(gameObject);
        }
    }
}
