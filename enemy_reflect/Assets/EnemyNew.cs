using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNew : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Switch_State_enemy();
        Enemy_Switch_Anim();
    }


    public StateAnim State_enemy;
    public enum StateAnim
    {
        //enemy_idle,
        //enemy_walk,
        //enemy_stay,
        //enemy_angry

        stay, // составная анимация
        idle,
        patrol,
        angry
    }


    void Enemy_Switch_Anim()
    {
        switch (State_enemy)
        {
            case StateAnim.idle: anim.Play("enemy_idle"); break;

            case StateAnim.patrol: anim.Play("enemy_walk"); break;

            case StateAnim.stay: anim.Play("enemy_stay"); break;

            case StateAnim.angry: anim.Play("enemy_angry"); break;
        }
    }











    Color yellowAlfa = new Color(1, 0.92f, 0.016f, 0.5f);
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawLine
        (
            new Vector2(CheckPoint.position.x - PatrolDistance, CheckPoint.position.y),
            new Vector2(CheckPoint.position.x + PatrolDistance, CheckPoint.position.y)
        );

        Gizmos.color = yellowAlfa;
        Gizmos.DrawLine
        (
            new Vector2(transform.position.x - AngryDistance, transform.position.y + 0.6f),
            new Vector2(transform.position.x + AngryDistance, transform.position.y + 0.6f)
        );
    }



    public Transform CheckPoint;
    public float PatrolDistance;
    public float AngryDistance;
    Transform Player;

    void Switch_State_enemy()
    {
        //if (Input.GetKeyDown(KeyCode.Q)) { State_enemy = StateAnim.enemy_idle; }
        //if (Input.GetKeyDown(KeyCode.W)) { State_enemy = StateAnim.enemy_walk; }
        //if (Input.GetKeyDown(KeyCode.E)) { State_enemy = StateAnim.enemy_stay; }
        //if (Input.GetKeyDown(KeyCode.R)) { State_enemy = StateAnim.enemy_angry; }
    }
}
