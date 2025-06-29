using UnityEngine;

public class FlowingController : MonoBehaviour
{
    float flowingSpeed = 2f;

    void LateUpdate()
    {
        transform.position -= transform.forward * Time.deltaTime * flowingSpeed;
    }
}
