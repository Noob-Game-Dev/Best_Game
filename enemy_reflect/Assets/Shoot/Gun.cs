using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float offset = 0f;
    public Transform shotPoint;
    public GameObject bulletPref;

    float reloadTimer;
    public float reloadTime; // время перезарядки

    void Update()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);

        if (reloadTimer <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            //if (Input.GetKeyDown(KeyCode.T))
            {
                Instantiate(bulletPref, shotPoint.position, transform.rotation);
                reloadTimer = reloadTime;
            }
        }
        else
        {
            reloadTimer -= Time.deltaTime;
        }
    }
}
