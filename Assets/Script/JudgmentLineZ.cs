using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgmentLineZ : MonoBehaviour
{
    public static float[] standardTimes = new float[8];

    bool isChecked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("JudgmentLineZ")
            &&
            !isChecked)
        {
            isChecked = true; //いらないかも
            Debug.Log("JudgmentZにあたってるよー");
            switch (gameObject.tag) //noteのタグを取得。
            {
                case ("MainNote"):
                    standardTimes[0] = Time.time;
                    break;
                case ("RightNote"):
                    standardTimes[1] = Time.time;
                    break;
                case ("LeftNote"):
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
                case ("LeftNoteLong"):
                    standardTimes[2] = Time.time;
                    break;
            }
        }

        //if (other.gameObject.CompareTag("MissLine"))
        //{
            //Debug.Log($"MissTime{Time.time - standardTimes[1]}");
        //}
    }
}