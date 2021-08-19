using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    public GameObject GUN;
    public GameObject AXE;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GUN.SetActive(!GUN.activeInHierarchy);
            AXE.SetActive(!AXE.activeInHierarchy);
        }
        
    }
}
