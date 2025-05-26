using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowingController : MonoBehaviour
{
    public static float flowingSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= transform.forward * Time.deltaTime * flowingSpeed;
    }
}
