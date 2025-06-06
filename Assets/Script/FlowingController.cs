using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowingController : MonoBehaviour
{
    float flowingSpeed = 2f;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position -= transform.forward * Time.deltaTime * flowingSpeed;
        //Debug.Log(Time.deltaTime);
    }
}
