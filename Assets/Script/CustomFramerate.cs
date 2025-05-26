using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFramerate : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = -1;
    }
}
