using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBlink : MonoBehaviour
{
    SpriteRenderer sr; // для ссылки на SpriteRenderer
    Color spriteColor; // для хранения цвета спрайта

    void Start()
    {
        sr = GetComponent<SpriteRenderer>(); // получение ссылки на SpriteRenderer
    }


    void Update()
    {
        // если нажата S и отключено плавное моргание, то включаем его и получаем цвет спрайта
        if (Input.GetKeyDown(KeyCode.S) && !slowOnBlink) { slowOnBlink = true; spriteColor = sr.color; }

        // если нажата F и отключено резкое моргание, то включаем его, получаем цвет спрайта и включаем режим прозрачности
        if (Input.GetKeyDown(KeyCode.F) && !fastOnBlink) { fastOnBlink = true; spriteColor = sr.color; fastDark = true; }

        if (slowOnBlink) { BlinkSlow(); } // если режим slowOnBlink включён, то запускаем метод для плавного моргания
        if (fastOnBlink) { BlinkFast(); } // если режим fastOnBlink включён, то запускаем метод для резкого моргания
    }


    public bool slowOnBlink = false; // для хранения состояния режима моргания

    public float slowBlinkTime = 2f; // время, в течении которого необходимо плавно моргать
    public int slowBlinkRate = 6; // кол-во морганий за указанное в slowBlinkTime время

    float slowBlinkValue = 0f; // время, прошедшее с начала затемнения или осветления
    float slowBlinkValueTime = 0f; // общее время работы метода плавного моргания
    bool directionAlfa = true; // для направления изменений (true - повышаем альфу (прозрачность меньше), false - уменьшаем альфу (прозрачность больше) )

    void BlinkSlow() // логика плавного моргания
    {
        if (slowBlinkValueTime < slowBlinkTime) // если время работы метода ещё не превысило заданного времени (в slowBlinkTime)
        {
            slowBlinkValue += Time.deltaTime; // считаем время с момента последнего обнуления slowBlinkValue, время полуцикла (угасание или появление)

            if (slowBlinkValue <= slowBlinkTime / (slowBlinkRate * 2) ) // если прошедшее время не превысило заданного времени для одного цикла (погас-появился)
            {
                // время одного полуцикла (цикл - это погас-появился, полуцикл - это либо погас, либо появился) - это 100%, либо 1
                // а прошедшее время (slowBlinkValue) - какой-то процент от времени полуцикла ( (slowBlinkTime / (slowBlinkRate * 2)) )
                // если альфа должна уменьшаться, то изменяем её: из 100% времени (1) вычитаем прошедшее время с начала цикла в процентном соотношении
                if (!directionAlfa) { spriteColor.a = 1 - slowBlinkValue / (slowBlinkTime / (slowBlinkRate * 2)); }
                // если альфа должна увеличиваться, то приравниваем её к процентному соотношению времени, прошедшего с начала цикла
                else if (directionAlfa) { spriteColor.a = slowBlinkValue / (slowBlinkTime / (slowBlinkRate * 2)); }
            }
            // если время полуцикла закончилось, то меняем направление изменения альфы (если уменьшалась, то теперь будет увеличиваться)
            // в общее время записываем время, которое затрачено на полуцикл и обнуляем счётчик времени затраченного на полуцикл
            else { directionAlfa = !directionAlfa; slowBlinkValueTime += slowBlinkValue; slowBlinkValue = 0f; }

            sr.color = spriteColor; // изменяем цвет спрайта на цвет с изменённой прозрачностью
        }
        else // если время работы метода превысило установленное в slowBlinkTime или равно ему
        {
            slowOnBlink = false; // отключаем режим плавного моргания
            spriteColor.a = 1; // чтобы избежать погрешностей, вручную выставляем альфу на 100%
            sr.color = spriteColor; // и изменяем цвет спрайта
            slowBlinkValueTime = 0f; // обнуляем общее время работы метода
            slowBlinkValue = 0f; // и обнуляем время работы полуцикла
        }
    }


    public bool fastOnBlink = false; // состояние режима резкого моргания

    public float fastBlinkTime = 2f; // установленное время резкого моргания
    public int fastBlinkRate = 6; // кол-во затемнений за время резкого моргания
    public float fastDelayBlink = 0.2f; // задержка (время на которое спрайт должен исчезнуть)

    float fastBlinkValue = 0f; // время, прошедшее с начала изменения алфа-канала цвета спрайта
    float fastBlinkValueTime = 0f; // время, прошедшее с начала работы метода
    bool fastDark = false; // состояние спрайта (false - спрайт видно, true - спрайт не видно)

    void BlinkFast() // логика резкого моргания
    {
        if (fastBlinkValueTime < fastBlinkTime) // если время работы метода ещё не превысило заданного времени (в slowBlinkTime)
        {
            fastBlinkValue += Time.deltaTime; // считаем время, с момента последнего обнуления fastBlinkValue, время в состоянии альфа = 0 или альфа = 1

            if (fastDark) // если спрайт должно быть НЕ ВИДНО
            {
                // если время, прошедшее с момента, как спрайт "погас" меньше, чем время выделенное на это состояние,
                // т.е. если спрайт ещё должен быть прозрачным, то приравниваем альфу к нулю
                if (fastBlinkValue < fastDelayBlink) { spriteColor.a = 0f; }
                // если же пора включить видимость спрайта, то отключаем прозрачное состояние спрайта,
                // записываем время в состоянии прозрачности в общее время метода и обнуляем время состояния
                else { fastDark = false; fastBlinkValueTime += fastBlinkValue; fastBlinkValue = 0f; }
            }
            else if (!fastDark) // если спрайт должно быть ВИДНО
            {
                // если время состояния (спрайт видно) не превышает времени для одного такого состояния (исключая время на полную прозрачность), то альфу приравниваем к 1
                if (fastBlinkValue < (fastBlinkTime - fastDelayBlink * fastBlinkRate) / fastBlinkRate) { spriteColor.a = 1f; }
                // если же время состояния превысило выделенное, то включаем прозрачноть, добавляем ко времени метода время затраченное на состояние (спрайт видно)
                // и обнуляем затраченное на состояние видимости спрайта время
                else { fastDark = true; fastBlinkValueTime += fastBlinkValue; fastBlinkValue = 0f; }
            }

            sr.color = spriteColor; // изменяем цвет спрайта на цвет с изменённой прозрачностью
        }
        else
        {
            fastOnBlink = false; // отключаем режим резкого моргания
            spriteColor.a = 1; // чтобы избежать погрешностей, вручную выставляем альфу на 100%
            sr.color = spriteColor; // и изменяем цвет спрайта
            fastBlinkValueTime = 0f; // обнуляем общее время работы метода
            fastBlinkValue = 0f; // и обнуляем время работы состояния
        }
    }
}
