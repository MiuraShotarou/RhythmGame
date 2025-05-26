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
        //��ʈÓ]
        //����
        //�D���ȃ^�C�~���O�ōĊJ�@���@�{�[�����ォ��x���Ă���K���X or �����j�󂳂��B
    }

    //float s = 0.001f;
    //float l = 0.01f; 
    float t = 0; //0.5 ���^�� 0 1
    float halfRange = 0.01f;
    float duration = 2f;

    Vector3 originalPosition;

    public IEnumerator DamageShake()
    {
        //originalPosition = cameraTransform.position;
        float timer = 0;
        float startTime = Time.time; //�Q�[���J�n����̎��ԁ@���F1min ���@�Vtime - imin
        char invert = 'Y';
        float invertY = 0;
        float invertX = -1;
        float keisu = 0.0005f;
        float roopCount = 0;

        while (timer <= duration) //���ԂŐU���̐؂�グ
        {
            float halfX = Mathf.Lerp(0 + halfRange, 0, t);
            float halfY = Mathf.Lerp(1 + halfRange, 1, t); //���a�̎擾
            
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
            cameraPosition.x = invertX * halfX; // - halfRange/ + �̂ق����ǂ������H�H
            cameraPosition.y = invertY - halfY; // - halfRange/
            cameraTransform.position = cameraPosition;

            //range = Mathf.Lerp(range, 0, timer/duration); //�ψ��range�ϐ��̌���(����)�B
            t = Mathf.Lerp(0, 1, timer + (keisu * roopCount) / duration); //�ψ��t�ϐ��̌���(����)�B�@�@�@�@�@�@�@���@���̍��W�Ԋu�ŃJ�����̃|�W�V������������`���Č��_�ɓ��B����B
            roopCount++;
            timer = Time.time - startTime;
            //Debug.Log(cameraTransform.position);
            Debug.Log(timer);
            yield return null;
        }

        cameraTransform.position = new Vector3(0f, 1f, -0.5f);//originalPosition;
    }
    // �����̏�����
}
