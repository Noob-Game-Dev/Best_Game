using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFall : MonoBehaviour
{
    // Для работы скрипта следует сделать персонажу пустой дочерний объект, добавить ему триггерный коллайдер (например EdgeCollider2D, Box..., Circle...)
    // и разместить его под персонажем, чтобы при нахождении на земле срабатывал данный триггер,
    // иначе OnTrigger-методы будут срабатывать при пересечении любых триггеров объектов персонажа со слоями земли

    Rigidbody2D rb;
    [SerializeField] LayerMask GroundMask; // маска слоёв от которых перс может получить урон при столкновении

    void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>(); // ссылка на компонент Rigidbody2D родительского объекта
        velocityPrev = rb.velocity.y;
    }


    [SerializeField] float damageHeigt = 5f; // максимальная высота падения без урона
    [SerializeField] int damageValue = 10; // урон за падение с высоты большей чем damageHeigt и меньшей чем (2 * damageHeigt)

    float velocityPrev = 0f; // для записи последнего положительного значения ускорения
    float fallPoint; // координаты по Y точки начала падения
    bool velocityChange = false; // состояния записи последнего значения ускорения


    //private void FixedUpdate()
    //{
    //    if (velocityChange) // если персонаж не стоит на земле
    //    {
    //        if (rb.velocity.y >= 0) { velocityPrev = rb.velocity.y; } // запись значения ускорения, если оно положительное
    //        else { fallPoint = transform.position.y; velocityChange = false; } // запись координат по Y если ускорение изменилось на отрицательное и отключение записи величины ускорения
    //    }
    //}




    [SerializeField] int safeVelocity = -20; // урон за падение с высоты большей чем damageHeigt и меньшей чем (2 * damageHeigt)
    bool reWriteFallPoint;

    private void FixedUpdate()
    {
        if (velocityChange) // если персонаж не стоит на земле
        {
            Debug.Log("Текущее ускорение: " + rb.velocity.y);

            if (rb.velocity.y < 0)
            {
                if (velocityPrev > safeVelocity) // если ускорение не достигло заданного предела
                {
                    velocityPrev = rb.velocity.y;
                    reWriteFallPoint = true;
                    Debug.Log("velocityPrev: " + velocityPrev);
                }
            }

            if ((velocityPrev <= safeVelocity) && reWriteFallPoint) // если ускорение больше, либо рано максимальному пределу
            {
                fallPoint = transform.position.y; // записываем точку начала падения
                Debug.Log("Ускорение -20 или меньше. Точка падения: " + fallPoint);
                reWriteFallPoint = false; // отключаем перезапись
            }

            velocityPrev = rb.velocity.y;

        }
        //else { velocityChange = false; }
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        // для проверки пересечения с одним слоем можно использовать (collision.gameObject.layer == 8), где 8 - номер слоя (например земли)
        if ((GroundMask.value & 1 << collision.gameObject.layer) != 0) // проверка на пересечение со всеми слоями маски Ground (Ground, Platform, MovePlatform...)
        {
            velocityChange = false;
            if (fallPoint - transform.position.y > 0 && rb.velocity.y <= safeVelocity) // если разница по Y между точкой начала падения и текущим положением, значит произошло падение
            {
                Debug.Log("Высота падения: " + (fallPoint - transform.position.y));
                // значение урона умножается на округлённое до целого значения растояние падения делённое на максимальную величину без урона
                // например при уроне в 10 ед. за юнит и при падении с 0.9 юнита урона не будет, с 1.1 юнита урон будет равен 10 ед.,
                // при падении с 2.5 юнитов - урон 20 ед, с 6 метров - 60 ед.
                // величина урона - damageValue, высота падения без урона - damageHeigt
                Debug.Log("Урон: " + damageValue * (int)((fallPoint - transform.position.y) / damageHeigt));
            }
        }
    }


    // Для стабильных срабатываний включать запись положительных ускорений стоит после "отрыва от земли"
    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((GroundMask.value & 1 << collision.gameObject.layer) != 0)
        {
            velocityChange = true;
        }
    }


    // ------------- Блок для удобства настройки (отрисовка границ урона) ------------- 

    enum DirLine { DOWN, UP }// направление луча (вниз-вверх)

    [SerializeField] DirLine DirectionLine = DirLine.DOWN; // выпадающий список выбора направления для отображения в инспекторе
    int k;

    private void OnDrawGizmos()
    {
        if (DirectionLine == DirLine.UP) { k = 1; }
        else if (DirectionLine == DirLine.DOWN) { k = -1; }

        Gizmos.color = Color.green; // высота падения без урона
        Gizmos.DrawLine(
            transform.position,
            new Vector3(transform.position.x, transform.position.y + k * (Mathf.Sqrt(safeVelocity) / (2f * 9.81f)) * 1)); 

        Gizmos.color = Color.yellow; // высота падения со средним уроном
        Gizmos.DrawLine(
            new Vector3(transform.position.x, transform.position.y + k * (Mathf.Sqrt(safeVelocity) / (2f * 9.81f)) * 1),
            new Vector3(transform.position.x, transform.position.y + k * (Mathf.Sqrt(safeVelocity) / (2f * 9.81f)) * 2)
            );

        Gizmos.color = Color.red; // высота падения с высоким уроном
        Gizmos.DrawLine(
            new Vector3(transform.position.x, transform.position.y + k * (Mathf.Sqrt(safeVelocity) / (2f * 9.81f)) * 2),
            new Vector3(transform.position.x, transform.position.y + k * (Mathf.Sqrt(safeVelocity) / (2f * 9.81f)) * 3)
            );
        
    }


    // ДОПОЛНИТЕЛЬНО:
    // Для доступа с к скрипту на родительском объекте, например управления персом (в моём случае он называется PlayerMove)
    // можно сделать ссылку такого же типа, как имя скрипта: PlayerMove PM;
    // В ф-и Start() нужно указать ему ссылку на компонент: PM = GetComponentInParent<PlayerMove>();
    // После этого можно обращаться к публичным переменным скрипта, например к onGround: PM.onGround;
    // То же самое можно сделать и для изменеия здоровья
}
