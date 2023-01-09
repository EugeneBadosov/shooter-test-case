using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
    public Camera cam;
    void Start()
    {
        
    }

    void Update()
    {
        transform.rotation = cam.transform.rotation;
    }
}
