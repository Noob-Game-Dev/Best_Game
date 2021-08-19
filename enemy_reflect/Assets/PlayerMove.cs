using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        
    }

    void Update()
    {
        Reflect();
        Jump();
        Walk();
        CheckingGround();

        //Debug.Log(rb.velocity.x);
        TransColor();

        healthNOW = health;
    }


    Color playerColor;
    float colorSpeed = 3;
    bool onBlink = false;
    //int value = 0;
    void TransColor()
    {
        if (Input.GetKeyDown(KeyCode.H) && !onBlink)
        {
            playerColor = sr.color;
            onBlink = true;
        }

        if (onBlink)
        {
            if (playerColor.a <= 0)
            {
                playerColor.a += colorSpeed * Time.deltaTime;
                sr.color = playerColor;
            }
            else if (playerColor.a >= 1)
            {
                playerColor.a -= colorSpeed * Time.deltaTime;
                sr.color = playerColor;
            }
        }
    }




    public Vector2 moveVector;
    public int speed = 3;
    void Walk()
    {
        moveVector.x = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveVector.x * speed, rb.velocity.y);
        anim.SetFloat("moveX", Mathf.Abs(moveVector.x));
    }


    // Отражение спрайтов
    public bool faceRight = true;
    void Reflect()
    {
        if ((moveVector.x > 0 && !faceRight) || (moveVector.x < 0 && faceRight))
        {
            Vector3 temp = transform.localScale;
            temp.x *= -1;
            transform.localScale = temp;
            faceRight = !faceRight;
        }
    }


    // Прыжок с земли
    public int jumpForce = 10;
    void Jump()
    {
        if (onGround)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.StopPlayback();
                anim.Play("jump");
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }
    }


    // Проверка земли
    public bool onGround;
    public LayerMask Ground;
    public Transform GroundCheck;
    public float GCRadius = 0.2f;
    void CheckingGround()
    {
        onGround = Physics2D.OverlapCircle(GroundCheck.position, GCRadius, Ground);
        anim.SetBool("onGround", onGround);
    }


    public float check_RADIUS = 0.04f;
    private void OnDrawGizmos()
    {
        // СЕРАЯ пустая сфера для проверки земли
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(GroundCheck.position, GCRadius);
    }




    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.layer.Equals("Platform"))
        if (collision.gameObject.layer == 9)
        {
            this.transform.parent = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //if (collision.gameObject.layer.Equals("Platform"))
        if (collision.gameObject.layer == 9)
        {
            this.transform.parent = null;
        }
    }


    public int health = 100;
    public int healthNOW;

    public void PlayerTakeDamage(int damageValue)
    {
        health -= damageValue;
        Debug.Log("Игрок получил " + damageValue + " ед. урона!");
        Debug.Log("Здоровье игрока: " + health);
    }








    //enum TypeFallDamage { PROCENT, FIXED, VEL_SECTOR, DIST_SECTOR }// направление луча (вниз-вверх)

    //[SerializeField] TypeFallDamage TypeDamage; // выпадающий список выбора направления для отображения в инспекторе



    //public int fallDamage = 10;

    //public int maxSafeVelocity = -20;
    ////public int maxDamageVelocity = -30;
    //public int CritVelocity = -40;
    //public void FallDamage()
    //{
    //    //if (rb.velocity.y < maxSafeVelocity)
    //    //{
    //    //    //var damageNow = (int)Mathf.Abs(rb.velocity.y * (int)(fallDamage / maxSafeVelocity));
    //    //    //var damageNow = (int)Mathf.Abs(rb.velocity.y * (int)(fallDamage / maxSafeVelocity )) - fallDamage;
    //    //    var damageNow = Mathf.Abs(((int)rb.velocity.y / maxSafeVelocity)) * fallDamage - fallDamage;
    //    //    //var damageNow = (int)(Mathf.Abs(rb.velocity.y / maxSafeVelocity) * fallDamage);
    //    //    health -= damageNow;
    //    //    Debug.Log("Ускорение при падении: " + rb.velocity.y);
    //    //    Debug.Log("Игрок получил " + damageNow + " ед. урона!");
    //    //    Debug.Log("Здоровье игрока: " + health);
    //    //}



    //    if (rb.velocity.y >= maxSafeVelocity)
    //    {
    //        Debug.Log("Ускорение при падении: " + rb.velocity.y);
    //        Debug.Log("Без урона!");
    //    }
    //    else if (rb.velocity.y < maxSafeVelocity && rb.velocity.y > CritVelocity)
    //    {
    //        Debug.Log("Ускорение при падении: " + rb.velocity.y);

    //        int damageNow = 0;


    //        if (TypeDamage == TypeFallDamage.PROCENT)
    //        {
    //            // урон = % жёлтой зоны падения (жизнь - 100%, конец жёлтой полосы - 99% урона, начало - 1% урона... значение из fallDamage ни на что не влияет)
    //            damageNow = (int)((maxSafeVelocity - rb.velocity.y) / (maxSafeVelocity - CritVelocity) * 100);
    //        }
    //        else if (TypeDamage == TypeFallDamage.FIXED)
    //        {
    //            // урон = значению из fallDamage на протяжении всей дистанции от безопасной, до критичной скорости
    //            damageNow = fallDamage;
    //        }
    //        else if (TypeDamage == TypeFallDamage.VEL_SECTOR)
    //        {
    //            // урон = значению, кратному fallDamage (если fallDamage = 10, то будет 10 участков, каждый из которых отнимет 10, 20, 30 и т.д.)
    //            damageNow = (int)Mathf.Abs((((maxSafeVelocity - rb.velocity.y) / (maxSafeVelocity - CritVelocity) * 100) / fallDamage)) * fallDamage;
    //        }
    //        else if (TypeDamage == TypeFallDamage.DIST_SECTOR)
    //        {
    //            // урон = значению, кратному fallDamage (шкала равномерная по дистанции)
    //            damageNow = (int)((((maxSafeVelocity - rb.velocity.y) / (maxSafeVelocity - CritVelocity) * 100)) / fallDamage) * fallDamage;
    //        }







    //        // урон = значению из fallDamage на протяжении всей дистанции от безопасной, до критичной скорости
    //        //var damageNow = fallDamage;

    //        // урон = % жёлтой зоны падения (жизнь - 100%, конец жёлтой полосы - 99% урона, начало - 1% урона... значение из fallDamage ни на что не влияет)
    //        //var damageNow = (int)((maxSafeVelocity - rb.velocity.y) / (maxSafeVelocity - CritVelocity) * 100);

    //        // урон = значению, кратному fallDamage (если fallDamage = 10, то будет 10 участков, каждый из которых отнимет 10, 20, 30 и т.д.)
    //        //var damageNow = (int)Mathf.Abs((((maxSafeVelocity - rb.velocity.y) / (maxSafeVelocity - CritVelocity) * 100) / fallDamage)) * fallDamage;

    //        // урон = значению, кратному fallDamage (шкала равномерная по дистанции)
    //        //var damageNow = (int)((((maxSafeVelocity - rb.velocity.y) / (maxSafeVelocity - CritVelocity) * 100)) / fallDamage) * fallDamage;





    //        // какая скорость будет на конце сектора
    //        // растояние
    //        // (maxSafeVelocity - CritVelocity) * 100) / fallDamage - процент урона по дистанции
    //        // (int)(100 / fallDamage) - кол-во секторов
    //        // скорость для участка (конца)
    //        // (maxSafeVelocity - CritVelocity) / (int)(100 / fallDamage) - скорость для участка (конца)




    //        // (int)(100 / fallDamage) - целое кол-во делений
    //        // ((maxSafeVelocity - rb.velocity.y) / (maxSafeVelocity - CritVelocity) * 100) - процент жёлтой шкалы
    //        // (int)(( ((maxSafeVelocity - rb.velocity.y) / (maxSafeVelocity - CritVelocity) * 100) ) / fallDamage) - номер сектора
    //        // номер сектора * урон
    //        // (int)(( ((maxSafeVelocity - rb.velocity.y) / (maxSafeVelocity - CritVelocity) * 100) ) / fallDamage) * fallDamage

    //        Debug.Log("Игрок получил " + damageNow + " ед. урона!");
    //        Debug.Log("Здоровье игрока: " + health);
    //    }
    //    else if (rb.velocity.y <= CritVelocity)
    //    {
    //        Debug.Log("Ускорение при падении: " + rb.velocity.y);
    //        Debug.Log("!!!Получен критический урон при падении!!!");
    //    }

    //}


    //// ------------- Блок для удобства настройки (отрисовка границ урона) ------------- 

    //enum DirLine { DOWN, UP }// направление луча (вниз-вверх)

    //[SerializeField] DirLine DirectionLine; // выпадающий список выбора направления для отображения в инспекторе
    //int k;
    //public float GravityScaleStart = 3f;

    //private void OnDrawGizmosSelected()
    //{
    //    if (DirectionLine == DirLine.UP) { k = 1; }
    //    else if (DirectionLine == DirLine.DOWN) { k = -1; }

    //    Gizmos.color = Color.green; // высота падения без урона
    //    Gizmos.DrawLine(
    //        transform.position,
    //        new Vector3(transform.position.x, transform.position.y + k * (Mathf.Pow(maxSafeVelocity, 2f) / (2f * 9.81f * GravityScaleStart)))
    //        );

    //    Gizmos.color = Color.yellow; // высота падения со средним уроном
    //    Gizmos.DrawLine(
    //        new Vector3(transform.position.x, transform.position.y + k * (Mathf.Pow(maxSafeVelocity, 2f) / (2f * 9.81f * GravityScaleStart))),
    //        new Vector3(transform.position.x, transform.position.y + k * (Mathf.Pow(CritVelocity, 2f) / (2f * 9.81f * GravityScaleStart)))
    //        );

    //    Gizmos.color = Color.red; // высота падения с высоким уроном
    //    Gizmos.DrawRay(
    //        new Vector3(transform.position.x, transform.position.y + k * (Mathf.Pow(CritVelocity, 2f) / (2f * 9.81f * GravityScaleStart))),
    //        new Vector3(0f, k, 0f)
    //        );

    //    if (TypeDamage == TypeFallDamage.PROCENT && TypeDamage == TypeFallDamage.FIXED)
    //    {

    //    }
    //    else if (TypeDamage == TypeFallDamage.VEL_SECTOR)
    //    {
    //        Gizmos.color = Color.magenta;

    //        var SpeedOneSector = ((maxSafeVelocity - CritVelocity) / (int)(100 / fallDamage));

    //        for (int i = 0; i < (int)(100 / fallDamage); i++)
    //        {
    //            Gizmos.DrawLine(
    //                new Vector2(transform.position.x - 0.5f, transform.position.y + k * (Mathf.Pow(maxSafeVelocity - i * SpeedOneSector, 2f) / (2f * 9.81f * GravityScaleStart))),
    //                new Vector2(transform.position.x + 0.5f, transform.position.y + k * (Mathf.Pow(maxSafeVelocity - i * SpeedOneSector, 2f) / (2f * 9.81f * GravityScaleStart)))
    //                );

    //            // (int)(100 / fallDamage) - общее кол-во отрезков
    //            // i - номер отрезка
    //            // получить конечную скорость на конце участка: maxSafeVelocity - i * скорость одноного участка
    //            // скорость одноного участка: скорость жёлтого отрезка / на кол-во отрезков
    //            // изменение скорости на одном жёлтом отрезке: maxSafeVelocity - CritVelocity (положительная) / (int)(100 / fallDamage) - положительный результат
    //            // maxSafeVelocity - i * ((maxSafeVelocity - CritVelocity) / (int)(100 / fallDamage))
    //            // maxSafeVelocity - i * SpeedOneSector
    //        }
    //    }
    //    else if (TypeDamage == TypeFallDamage.DIST_SECTOR)
    //    {
    //        Gizmos.color = Color.magenta;

    //        var yellowZoneDistance = Mathf.Abs(Mathf.Pow(maxSafeVelocity, 2f) / (2f * 9.81f * GravityScaleStart) - Mathf.Pow(CritVelocity, 2f) / (2f * 9.81f * GravityScaleStart));
    //        var sectorDistance = yellowZoneDistance / (int)(100 / fallDamage);

    //        for (int i = 0; i < (int)(100 / fallDamage); i++)
    //        {
    //            Gizmos.DrawLine(
    //                new Vector2(transform.position.x - 0.5f, transform.position.y + k * (Mathf.Pow(maxSafeVelocity, 2f) / (2f * 9.81f * GravityScaleStart) + sectorDistance * i)),
    //                new Vector2(transform.position.x + 0.5f, transform.position.y + k * (Mathf.Pow(maxSafeVelocity, 2f) / (2f * 9.81f * GravityScaleStart) + sectorDistance * i))
    //                );


    //            // Mathf.Pow(maxSafeVelocity, 2f) / (2f * 9.81f * GravityScaleStart))
    //            // Mathf.Pow(maxSafeVelocity, 2f) / (2f * 9.81f * GravityScaleStart) - положение конца безопасной зоны (начало жёлтой)
    //            // Mathf.Pow(CritVelocity, 2f) / (2f * 9.81f * GravityScaleStart) - положение окончания жёлтой зоны (начало красной)

    //            // Mathf.Pow(maxSafeVelocity, 2f) / (2f * 9.81f * GravityScaleStart) - Mathf.Pow(CritVelocity, 2f) / (2f * 9.81f * GravityScaleStart) - длина жёлтой зоны
    //            // var yellowZoneDistance = Mathf.Pow(maxSafeVelocity, 2f) / (2f * 9.81f * GravityScaleStart) - Mathf.Pow(CritVelocity, 2f) / (2f * 9.81f * GravityScaleStart)
    //            // var sectorDistance = yellowZoneDistance / (int)(100 / fallDamage)

    //            // Mathf.Pow(maxSafeVelocity, 2f) / (2f * 9.81f * GravityScaleStart) + sectorDistance * i

    //            // (int)(100 / fallDamage) - кол-во отрезков
    //            // узнать скорость в конце каждого отрезка

    //        }
    //    }


    //}

}
