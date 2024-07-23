using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attackk : MonoBehaviour
{
    public float dmgVaule = 4;                      //데미지 값 
    public GameObject throwbleObject;               //던질 수 있는 오브젝트  
    public Transform attackCheck;                   

    private Rigidbody2D rigidbody2D;
    public Animator animator;                       //애니메이터 할ㄷㅏㅇ
    public bool canAttack = true;                   //공격 여부 체크
    public bool isTimeToCheck = false;

    public GameObject cam;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X) && canAttack)
        {
            canAttack = true;
            animator.SetBool("IsAttacking", true);
            StartCoroutine(AttackCooldown());
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            GameObject throwableWeapon = Instantiate(throwbleObject, transform.position + new Vector3(transform.localPosition.x * 0.5f, -0.2f),
                Quaternion.identity);

            Vector2 direction = new Vector2(transform.localScale.x, 0);
            throwableWeapon.GetComponent<ThrowableWeapon>().dirextion = direction;
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(0.25f);
        canAttack = true;
    }
}
