using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour

{
    float speed;
    public float normalSpeed = 2;
    public float fastSpeed = 3;
    public int positionOfPatrool;
    public Transform point;
    bool moveingRight;
    Transform player;
    public float stoppingDistance;

    bool chill = false;
    bool angry = false;
    bool goBack = false;
    public Animator anim;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    Vector3 playerPositionX;
    void Update()
    {
        ReflectForEnemy();

        if (Vector2.Distance(transform.position, point.position) < positionOfPatrool && angry == false)
        {
            chill = true;
        }

        if (Vector2.Distance(transform.position, player.position) < stoppingDistance)
        {
            angry = true;
            chill = false;
            goBack = false;
        }

        if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
        {
            goBack = true;
            angry = false;
        }


        playerPositionX = new Vector3(player.position.x, transform.position.y, transform.position.z);


        if (chill == true) { Chill(); }
        else if (angry == true) { Angry(); }
        else if (goBack == true) { GoBack(); }

    }

    void Chill()
    {
        if (transform.position.x > point.position.x + positionOfPatrool) { moveingRight = false; }
        else if (transform.position.x < point.position.x - positionOfPatrool) { moveingRight = true; }

        if (moveingRight) { transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y); }
        else { transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y); }

        speed = normalSpeed;
    }

    void Angry()
    {
        transform.position = Vector2.MoveTowards(transform.position, /*player.position*/ playerPositionX, speed * Time.deltaTime);
        speed = fastSpeed;
    }

    void GoBack()
    {
        transform.position = Vector2.MoveTowards(transform.position, point.position, speed * Time.deltaTime);
        speed = normalSpeed;
    }


    float prevPositionX = 0;
    void ReflectForEnemy()
    {

        Vector3 temp = transform.localScale;

        if (angry) // без изменений в состоянии агра при столкновении с персом будет дрожать отражение
        {
            if (player.position.x < transform.position.x) { temp.x = -1; /*moveingRight = false;*/ }
            else if (player.position.x > transform.position.x) { temp.x = 1; /*moveingRight = true;*/ }
        }
        else
        {
            if (transform.position.x < prevPositionX) { temp.x = -1; moveingRight = false; } // без moveingRight при входе в зону патрулирования происходит промаргивание отражения спрайтов
            else if (transform.position.x > prevPositionX) { temp.x = 1; moveingRight = true; }
        }

        transform.localScale = temp;

        prevPositionX = transform.position.x;







        


        ////transform.LookAt(player, Vector2.right);

        //Vector3 temp = transform.localScale;
        //if (angry) // без изменений в состоянии агра при столкновении с персом будет дрожать отражение
        //{
        //    //diff = player.transform.position - transform.position;//позиция
        //    //diff.Normalize();

        //    if (player.position.x < transform.position.x) { temp.x = -1; /*moveingRight = false;*/ }
        //    else if (player.position.x > transform.position.x) { temp.x = 1; /*moveingRight = true;*/ }

        //    //if (diff.x < 0) { temp.x = -1; /*moveingRight = false;*/ }
        //    //else if (diff.x > 0) { temp.x = 1; /*moveingRight = true;*/ }

        //}
        //else
        //{
        //    if (transform.position.x < prevPositionX) { temp.x = -1; moveingRight = false; } // без moveingRight при входе в зону патрулирования происходит промаргивание отражения спрайтов
        //    else if (transform.position.x > prevPositionX) { temp.x = 1; moveingRight = true; }
        //}

        //transform.localScale = temp;

        //prevPositionX = transform.position.x;

        ////Debug.Log(Vector2.Dot(Vector2.left, player.position));
        ////Debug.Log(Vector2.Angle(transform.position, player.position));


        ////Debug.Log(diff);

        ////float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        ////transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);//тут прибавляй вращение, можешь поставить Lerp или Slerp


        


    }


    Color yellowAlfa = new Color(1, 0.92f, 0.016f, 0.5f);
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawLine
        (
            new Vector2(point.position.x - positionOfPatrool, point.position.y),
            new Vector2(point.position.x + positionOfPatrool, point.position.y)
        );

        Gizmos.color = yellowAlfa;
        Gizmos.DrawLine
        (
            new Vector2(transform.position.x - stoppingDistance, transform.position.y + 0.6f),
            new Vector2(transform.position.x + stoppingDistance, transform.position.y + 0.6f)
        );
    }


}