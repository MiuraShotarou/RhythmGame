using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    public int health = 2 + 100;

    public void Damage()
    {
        health--;
        StartCoroutine(DamageShake());
        if (health <= 0)
        {
            //GameOver();
        }
    }

    public void GameOver()
    {
        //画面暗転
        //復活
        //好きなタイミングで再開　→　ボールを上から支えているガラス or 糸が破壊される。
    }

    //float s = 0.001f;
    //float l = 0.01f; 
    float t = 0; //0.5 が真ん中 0 1
    float halfRange = 0.01f;
    float duration = 2f;

    Vector3 originalPosition;

    public IEnumerator DamageShake()
    {
        //originalPosition = cameraTransform.position;
        float timer = 0;
        float startTime = Time.time; //ゲーム開始からの時間　仮：1min →　新time - imin
        char invert = 'Y';
        float invertY = 0;
        float invertX = -1;
        float keisu = 0.0005f;
        float roopCount = 0;

        while (timer <= duration) //時間で振動の切り上げ
        {
            float halfX = Mathf.Lerp(0 + halfRange, 0, t);
            float halfY = Mathf.Lerp(1 + halfRange, 1, t); //半径の取得
            
            if (invert == 'Y')
            {
                if (invertY != 2)
                {
                    invertY = 2;
                }
                else if (invertY == 2)
                {
                    invertY = halfY * 2;
                }
                invert = 'X';
            }
            else if (invert == 'X')
            {
                if (invertX == 1)
                {
                    invertX = -1;
                }
                else if (invertX != 1)
                {
                    invertX = 1;
                }
                invert = 'Y';
            }

            Vector3 cameraPosition = cameraTransform.position;
            cameraPosition.x = invertX * halfX; // - halfRange/ + のほうが良いかも？？
            cameraPosition.y = invertY - halfY; // - halfRange/
            cameraTransform.position = cameraPosition;

            //range = Mathf.Lerp(range, 0, timer/duration); //均一なrange変数の減少(多分)。
            t = Mathf.Lerp(0, 1, timer + (keisu * roopCount) / duration); //均一なt変数の減少(多分)。　　　　　　　→　一定の座標間隔でカメラのポジションが螺旋を描いて原点に到達する。
            roopCount++;
            timer = Time.time - startTime;
            //Debug.Log(cameraTransform.position);
            Debug.Log(timer);
            yield return null;
        }

        cameraTransform.position = new Vector3(0f, 1f, -0.5f);//originalPosition;
    }
    // 螺旋の書き方
}
