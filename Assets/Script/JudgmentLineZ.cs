using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgmentLineZ : MonoBehaviour
{
    [SerializeField] Transform judgmentTransform;

    public static float[] standardTimes = new float[8];

    float toleranceZ;
    bool isChecked = false;

    void Start()
    {
        toleranceZ = judgmentTransform.transform.position.z;
    }
    void Update()
    {
        if (transform.position.z <= toleranceZ
            &&
            !isChecked)
        {
            isChecked = true;
            //Debug.Log("JudgmentZにあたってるよー");
            switch (gameObject.tag) //noteのタグを取得。
            {
                case ("MainNote"):
                    standardTimes[0] = Time.time;
                    break;
                case ("RightNote"):
            Debug.Log("RightNoteが起動している");
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
