using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float reloadTime;
    float reloadTimer;

    public Transform attackPos;
    public LayerMask enemyMask;
    public float attackRadius;
    public int axeDamage;
    Animator anim;

    void Start()
    {
        anim = GetComponent/*InParent*/<Animator>();
    }

    void Update()
    {
        if (reloadTimer <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                anim.Play("attack");
                reloadTimer = reloadTime;
            }
        }
        else { reloadTimer -= Time.deltaTime; }
    }

    public void OnAttack()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPos.position, attackRadius, enemyMask);
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<EnemyDamage>().TakeDamage(axeDamage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRadius);
    }
}
