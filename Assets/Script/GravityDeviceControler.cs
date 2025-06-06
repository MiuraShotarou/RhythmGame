using System.Collections.Generic;
using UnityEngine;

public class GravityDeviceControler : MonoBehaviour
{
    Vector3 forceDirectionY = new Vector3(0f, -1f, 0f);
    float forcePower = 150f;

    Rigidbody rigidbody;

    public static bool isGravity = true; // PushBall‹N“®—p
    void OnTriggerStay(Collider other)
    {
        if (!isGravity)
        {
            return;
        }

        if (other.gameObject.CompareTag("Ball")
            && other.transform.position.y > 0.93f //0.91f
            //&& isGravity
            )
        {
            //Debug.Log("Gravity‚©‚©‚Á‚Ä‚¢‚é");
            rigidbody = other.GetComponent<Rigidbody>();
            rigidbody.AddForce(forceDirectionY * forcePower, ForceMode.Force);
        }
    }
}
