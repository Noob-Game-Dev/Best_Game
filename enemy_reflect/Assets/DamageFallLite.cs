using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFallLite : MonoBehaviour
{
    PlayerMove HealthScript; // <-- "PlayerMove" ЗАМЕНИТЬ НА НЕОБХОДИМЫЙ СКРИПТ (содержащий переменную здороья персонажа). ИМЯ ССЫЛКИ (HealthScript) не менять___________!!!
    Rigidbody2D rb;
    

    void Start()
    {
        HealthScript = GetComponentInParent<PlayerMove>(); // <-- PlayerMove ЗАМЕНИТЬ НА НЕОБХОДИМЫЙ СКРИПТ (содержащий переменную здороья персонажа)___________!!!
        rb = GetComponentInParent<Rigidbody2D>();
        allPlayerHealth = HealthScript.health; // <-- ссылка на переменную для хранения здоровья! "health" заменить на своё имя (если отличается)___________!!!

        fallDamage = Mathf.Abs(fallDamage);
        maxSafeVelocity = Mathf.Abs(maxSafeVelocity) * -1;
        CritVelocity = Mathf.Abs(CritVelocity) * -1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((DamageLayer.value & 1 << collision.gameObject.layer) != 0) // перебор слоёв в маске на соответствие
        {
            FallDamage();
        }
    }


    enum TypeFallDamage { PROCENT, FIXED, VEL_SECTOR, DIST_SECTOR } // режимы отрисовки вспомогательных лучей

    [SerializeField] TypeFallDamage TypeDamage = TypeFallDamage.PROCENT; // выпадающий список режимов отрисовки в Инспекторе

    public LayerMask DamageLayer;
    public int allPlayerHealth = 100;
    public int fallDamage = 10;

    public int maxSafeVelocity = 20;
    public int CritVelocity = 40;
    public void FallDamage()
    {
        if (rb.velocity.y >= maxSafeVelocity)
        {
            Debug.Log("Ускорение при падении: " + rb.velocity.y);
            Debug.Log("Без урона!");
        }
        else if (rb.velocity.y < maxSafeVelocity && rb.velocity.y > CritVelocity)
        {
            Debug.Log("Ускорение при падении: " + rb.velocity.y);

            int damageNow = 0;


            if (TypeDamage == TypeFallDamage.PROCENT)
            {
                // урон = % жёлтой зоны падения (жизнь - 100%, конец жёлтой полосы - 99% урона, начало - 1% урона... значение из fallDamage ни на что не влияет)
                damageNow = (int)((maxSafeVelocity - rb.velocity.y) / (maxSafeVelocity - CritVelocity) * allPlayerHealth);
            }
            else if (TypeDamage == TypeFallDamage.FIXED)
            {
                // урон = значению из fallDamage на протяжении всей дистанции от безопасной, до критичной скорости
                damageNow = fallDamage;
            }
            else if (TypeDamage == TypeFallDamage.VEL_SECTOR)
            {
                // урон = значению, кратному fallDamage (если fallDamage = 10, то будет 10 участков, каждый из которых отнимет 10, 20, 30 и т.д.)
                damageNow = (int)Mathf.Abs((((maxSafeVelocity - rb.velocity.y) / (maxSafeVelocity - CritVelocity) * allPlayerHealth) / fallDamage)) * fallDamage;
            }
            else if (TypeDamage == TypeFallDamage.DIST_SECTOR)
            {
                // урон = значению, кратному fallDamage (шкала равномерная по дистанции)
                var safeDistance = Mathf.Pow(maxSafeVelocity, 2f) / (2f * 9.81f * GravityScale);
                var yellowZoneDistance = Mathf.Abs(safeDistance - Mathf.Pow(CritVelocity, 2f) / (2f * 9.81f * GravityScale));
                var sectorDistance = yellowZoneDistance / (int)(allPlayerHealth / fallDamage);
                var distanceNow = (Mathf.Pow(rb.velocity.y, 2f) / (2f * 9.81f * GravityScale));
                var damageK = (int)((distanceNow - safeDistance) / sectorDistance);
                damageNow = damageK * fallDamage;
            }

            Debug.Log("Игрок получил " + damageNow + " ед. урона!");
            HealthScript.health -= damageNow;
            Debug.Log("Здоровье игрока: " + HealthScript.health);
        }
        else if (rb.velocity.y <= CritVelocity)
        {
            Debug.Log("Ускорение при падении: " + rb.velocity.y);
            HealthScript.health = 0;
            Debug.Log("!!!Получен критический урон при падении!!!");
        }

    }




    // ------------- Блок для удобства настройки (отрисовка границ урона) ------------- 

    enum DirLine { DOWN, UP }// направление луча (вниз-вверх)

    [SerializeField] DirLine DirectionLine = DirLine.DOWN; // выпадающий список выбора направления для отображения в Инспекторе
    int k;
    public float GravityScale = 3f;

    private void OnDrawGizmosSelected()
    {
        if (DirectionLine == DirLine.UP) { k = 1; }
        else if (DirectionLine == DirLine.DOWN) { k = -1; }

        var GRAVITY = (2f * 9.81f * GravityScale);
        var safeVelocityPoint = Mathf.Pow(maxSafeVelocity, 2f) / GRAVITY;
        var critVelocityPoint = Mathf.Pow(CritVelocity, 2f) / GRAVITY;


        Gizmos.color = Color.green; // высота падения без урона
        Gizmos.DrawLine(
            transform.position,
            new Vector3(transform.position.x, transform.position.y + k * (safeVelocityPoint))
            );

        Gizmos.color = Color.yellow; // высота падения со средним уроном
        Gizmos.DrawLine(
            new Vector3(transform.position.x, transform.position.y + k * (safeVelocityPoint)),
            new Vector3(transform.position.x, transform.position.y + k * (critVelocityPoint))
            );

        Gizmos.color = Color.red; // высота падения с высоким уроном
        Gizmos.DrawRay(
            new Vector3(transform.position.x, transform.position.y + k * (critVelocityPoint)),
            new Vector3(0f, k, 0f)
            );

        

        if (TypeDamage == TypeFallDamage.PROCENT && TypeDamage == TypeFallDamage.FIXED)
        {

        }
        else if (TypeDamage == TypeFallDamage.VEL_SECTOR)
        {
            var SpeedOneSector = ((maxSafeVelocity - CritVelocity) / (int)(allPlayerHealth / fallDamage));

            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                    new Vector2(transform.position.x, transform.position.y + k * (safeVelocityPoint)),
                    new Vector2(transform.position.x, transform.position.y + k * (Mathf.Pow(maxSafeVelocity - SpeedOneSector, 2f) / GRAVITY))
                    );

            Gizmos.color = Color.magenta;

            for (int i = 1; i <= (int)(allPlayerHealth / fallDamage); i++)
            {
                Gizmos.DrawLine(
                    new Vector2(transform.position.x - 0.5f, transform.position.y + k * (Mathf.Pow(maxSafeVelocity - i * SpeedOneSector, 2f) / GRAVITY)),
                    new Vector2(transform.position.x + 0.5f, transform.position.y + k * (Mathf.Pow(maxSafeVelocity - i * SpeedOneSector, 2f) / GRAVITY))
                    );
            }
        }
        else if (TypeDamage == TypeFallDamage.DIST_SECTOR)
        {
            var yellowZoneDistance = Mathf.Abs(safeVelocityPoint - critVelocityPoint);
            var sectorDistance = yellowZoneDistance / (int)(allPlayerHealth / fallDamage);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                    new Vector2(transform.position.x, transform.position.y + k * (safeVelocityPoint)),
                    new Vector2(transform.position.x, transform.position.y + k * (safeVelocityPoint + sectorDistance))
                    );

            Gizmos.color = Color.magenta;

            for (int i = 1; i <= (int)(allPlayerHealth / fallDamage); i++)
            {
                Gizmos.DrawLine(
                    new Vector2(transform.position.x - 0.5f, transform.position.y + k * (safeVelocityPoint + sectorDistance * i)),
                    new Vector2(transform.position.x + 0.5f, transform.position.y + k * (safeVelocityPoint + sectorDistance * i))
                    );
            }
        }
    }



}
