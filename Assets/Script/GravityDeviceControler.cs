using UnityEngine;

public class GravityDeviceControler : MonoBehaviour
{
    Vector3 forceDirectionY = new Vector3(0f, -1f, 0f);
    float forcePower = 150f;

    Rigidbody rigidbody;

    public static bool isGravity = true; // PushBall‹N“®—p

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Ball")
    //       && other.transform.position.x == 0) //0.91f
    //    {
    //        rigidbody = other.GetComponent<Rigidbody>();
    //        rigidbody.AddForce(forceDirectionY * 2, ForceMode.Impulse);
    //    }
    //}
    void OnTriggerStay(Collider other)
    {
        if (!isGravity)
        {
            return;
        }

        if (other.gameObject.CompareTag("Ball")
            && other.transform.position.y > 0.93f //0.91f
            )
        {
            rigidbody = other.GetComponent<Rigidbody>();
            rigidbody.AddForce(forceDirectionY * forcePower, ForceMode.Force);
        }
    }
}
