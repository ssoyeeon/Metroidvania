using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableWeapon : MonoBehaviour
{
    public Vector2 dirextion;
    public bool hasHit = false;
    public float speed = 10.0f;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!hasHit)
            GetComponent<Rigidbody2D>().velocity = dirextion * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.SendMessage("ApplyDamage", Mathf.Sign(dirextion.x) * 2f);
            Destroy(gameObject);
        }
        else if(collision.gameObject.tag != "Player")
        {
            Destroy(gameObject);
        }
    }
}
