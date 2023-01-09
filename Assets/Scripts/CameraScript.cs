using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public GameObject hero;

    void Start()
    {
        transform.localPosition = new Vector3(0, 2.5f, -10);
    }

    void Update()
    {
        transform.eulerAngles = hero.transform.eulerAngles;
    }
}
