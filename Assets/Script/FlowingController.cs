using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowingController : MonoBehaviour
{
    public float flowingSpeed = 1f;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position -= transform.forward * Time.deltaTime * flowingSpeed;
    }
}
