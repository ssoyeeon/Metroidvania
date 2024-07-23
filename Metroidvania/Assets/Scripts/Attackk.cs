using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attackk : MonoBehaviour
{
    public float dmgVaule = 4;                      //������ �� 
    public GameObject throwbleObject;               //���� �� �ִ� ������Ʈ  
    public Transform attackCheck;                   

    private Rigidbody2D rigidbody2D;
    public Animator animator;                       //�ִϸ����� �Ҥ�����
    public bool canAttack = true;                   //���� ���� üũ
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
