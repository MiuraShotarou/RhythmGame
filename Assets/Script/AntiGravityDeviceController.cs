using System.Collections.Generic;
using UnityEngine;

public class AntiGravityDeviceControler : MonoBehaviour
{
    Vector3 forceDirectionY = new Vector3(0f, 1f, 0f);
    Vector3 forceDirectionX = new Vector3(1f, 0f, 0f);
    float forcePower = 150f;

    Rigidbody rigidbody;

    public static bool isAntiGravity = true;
    void OnTriggerStay(Collider other)
    {
        if (!isAntiGravity)
        {
            return;
        }

        if (other.gameObject.CompareTag("Ball")
            && other.transform.position.y < 0.935f //0.915f
            //&& isAntiGravity
            )
        {
            //Debug.Log("AntiGravity‚©‚©‚Á‚Ä‚¢‚é");
            rigidbody = other.GetComponent<Rigidbody>();
            rigidbody.AddForce(forceDirectionY * forcePower, ForceMode.Force);
            //rigidbody.AddForce(forceDirectionX * forcePower, ForceMode.Force);
            //rigidbody.AddForce((forceDirectionX * -1) * forcePower, ForceMode.Force);
        }
        else if (other.gameObject.CompareTag("Ball")
        && !(other.transform.position.y < 0.935f))
        {
            rigidbody.velocity = Vector3.zero;
        }
    }
}
