using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgmentLineZ : MonoBehaviour
{
    [SerializeField] Transform judgmentTransform;

    public static float[] standardTimes = new float[8];

    float toleranceZ;
    bool isChecked = false;

    //void Start()
    //{
    //toleranceZ = judgmentTransform.transform.position.z;
    //}
    //void Update()
    //{
    //if (transform.position.z <= toleranceZ
    //&&
    //!isChecked)
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("JudgmentLineZ")
            &&
            !isChecked)
        {
            isChecked = true; //����Ȃ�����
            Debug.Log("JudgmentZ�ɂ������Ă��[");
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
                    standardTimes[0] = Time.time;
                    break;
                case ("RightNoteLong"):
                    standardTimes[1] = Time.time;
                    break;
                case ("LeftNotelong"):
                    standardTimes[2] = Time.time;
                    break;
            }
        }

        if (other.gameObject.CompareTag("MissLine"))
        {
            Debug.Log($"MissTime{Time.time - standardTimes[1]}");
        }
    }
}
