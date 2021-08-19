using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float lifeTime;

    public float distance;
    public LayerMask DestroyObjectMask;

    public int damageValue = 10;

    private void Start()
    {
        Invoke("DestroyBullet", lifeTime);
    }

    void Update()
    {
        //RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.right, distance, DestroyObjectMask);

        //if (hitInfo.collider != null)
        //{
        //    if (hitInfo.collider.tag == "Enemy")
        //    {
        //        Debug.Log("Урон врагу!");
        //        hitInfo.collider.GetComponent<EnemyDamage>().TakeDamage(damageValue);
        //    }
        //    DestroyBullet();
        //}



        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }


    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Enemy")
    //    {
    //        Debug.Log("Урон врагу!");
    //        //collision.gameObject.GetComponent<EnemyDamage>().TakeDamage(damageValue);
    //        collision.GetComponent<EnemyDamage>().TakeDamage(damageValue);
    //    }
    //    DestroyBullet();
    //}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyDamage enemy = collision.GetComponent<EnemyDamage>();
        Debug.Log(enemy);
        if (enemy != null)
        {
            //Debug.Log("Урон врагу!");
            enemy.TakeDamage(damageValue);
        }
        DestroyBullet();
    }


    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Enemy")
    //    {
    //        //EnemyDamage enemy = collision.GetComponent<EnemyDamage>();
    //        //enemy.TakeDamage(damageValue);
    //        collision.GetComponent<EnemyDamage>().TakeDamage(damageValue);
    //    }
    //    DestroyBullet();
    //}

}
