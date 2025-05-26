using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgmentZ : MonoBehaviour
{
    public static float[] standardTimes = new float[8];

    [SerializeField] Transform judgmentTransform;
    float toleranceZ;

    void Start()
    {
        toleranceZ = judgmentTransform.transform.position.z;
    }
    void Update()
    {
        if (transform.position.z <= (toleranceZ + 0.005f)
        &&
        transform.position.z >= (toleranceZ - 0.005f))
        {
            //Debug.Log("JudgmentZ�ɂ������Ă��[");
            switch (gameObject.tag) //note�̃^�O���擾�B
            {
                case ("MainNote"):
                    standardTimes[0] = Time.time;
                    break;
                case ("RightNote"):
            Debug.Log("RightNote���N�����Ă���");
                    standardTimes[1] = Time.time;
                break;
                case ("Leftnote"):
                    standardTimes[2] = Time.time;
                    break;
                case ("RightRightNote"):
                    standardTimes[3] = Time.time;
                    break;
                case ("LeftLeftNote"):
                    standardTimes[4] = Time.time;
                    break;
                case ("MainNoteLong"):
                    standardTimes[5] = Time.time;
                    break;
                case ("RightNoteLong"):
                    standardTimes[6] = Time.time;
                    break;
                case ("LeftNotelong"):
                    standardTimes[7] = Time.time;
                    break;
            }
        }

    }
}
