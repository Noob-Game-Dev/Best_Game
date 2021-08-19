using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int health = 100;

    private void Update()
    {
        if (health <= 0) { Destroy(gameObject); }    
    }

    public void TakeDamage(int damageValue)
    {
        health -= damageValue;
        Debug.Log("Враг получил " + damageValue + " ед. урона!");
        Debug.Log("Здоровье врага: " + health);


       
    }




    public float reLoadTime = 1f;
    float reLoadTimer;
    public int damage;

    public float stopTime;
    float startStopTime;
    float normalSpeed;

    PlayerMove PM;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        //PM = FindObjectOfType<PlayerMove>();
        PM = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>();
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Debug.Log("В зоне поражения");
            if (reLoadTimer <= 0) { anim.SetTrigger("attack");/*anim.Play("enemyAtack"); Debug.Log("Запуск анимации");*/ reLoadTimer = reLoadTime; }
            else { reLoadTimer -= Time.deltaTime; }
        }
    }

    public void OnEnemyAttack()
    {
        //PM.PlayerTakeDamage(damage);
        PM.health -= damage;
        reLoadTimer = reLoadTime;
    }
}
