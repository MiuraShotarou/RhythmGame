using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class HealthManager : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] GameObject crackedGlass;
    [SerializeField] GameObject ball;
    public int health = 2;

    bool isGameOver;

    public void Damage()
    {
        health--;
        Debug.Log("health" + health);
        if (health == 1)
        {
            Debug.Log("StageBreak");
            StartCoroutine(StageBreak(1f));
        }
        else if (health <= 0)
        {
            GameOver();
        }
    }

    IEnumerator StageBreak(float level)
    {
        FlowingController[] flowingControllers = FindObjectsOfType<FlowingController>();
        foreach (FlowingController controller in flowingControllers)
        {
            controller.enabled = false;
        }
        ball.GetComponent<Rigidbody>().isKinematic = true;

        GetComponent<AudioManager>().bgmSource.Pause();
        GetComponent<AudioManager>().seSource.clip = GetComponent<AudioManager>().seClip[0];
        GetComponent<AudioManager>().seSource.Play();

        yield return new WaitForSeconds(1f);
        foreach (FlowingController controller in flowingControllers)
        {
            if (controller != null)
            {
                controller.enabled = true;
            }
            else if (controller == null)
            {
                Debug.Log("FlowingController��null");
            }
        }
        ball.GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<AudioManager>().seSource.clip = GetComponent<AudioManager>().seClip[1];
        GetComponent<AudioManager>().seSource.Play();
        GetComponent<AudioManager>().bgmSource.UnPause();
        StartCoroutine(DamageShake());
        //StartCoroutine(ball.GetComponent<BallController>().PosReset("Other"));

        switch (level)
        {
            case 1:
                for (int i = 0; i < 12; i++)
                {
                    //Time.timeScale = 0f;
                    //yield return new WaitForSeconds(1f);
                    //Time.timeScale = 1f;

                    Instantiate(crackedGlass, new Vector3(0f, 0.25f, i), Quaternion.identity);
                    Instantiate(crackedGlass, new Vector3(-0.6f, 1, i), Quaternion.identity);
                    Instantiate(crackedGlass, new Vector3(0.6f, 1, i), Quaternion.identity);
                    yield return new WaitForSeconds(0f);
                }
                break;
        }
        
        while (!isGameOver)
        {
            var objF = Instantiate(crackedGlass, new Vector3(0f, 0.25f, 11), Quaternion.identity);
            var objR = Instantiate(crackedGlass, new Vector3(-0.6f, 1, 11), Quaternion.identity);
            var objL = Instantiate(crackedGlass, new Vector3(0.6f, 1, 11), Quaternion.identity);
            Destroy(objF, 10f);
            Destroy(objR, 10f);
            Destroy(objL, 10f);

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void GameOver()
    {
        Debug.Log("GameOver���Ă΂ꂽ");
        isGameOver = true;
        GetComponent<AudioManager>().bgmSource.Stop();
        GetComponent<AudioManager>().seSource.clip = GetComponent<AudioManager>().seClip[2];
        GetComponent<AudioManager>().seSource.Play();
        GameObject[] flowingControllerObjs = FindObjectsOfType<FlowingController>()
        .Select(fc => fc.gameObject)
        .ToArray();                                                                 //��͂��邱�ƁB
        foreach (GameObject obj in flowingControllerObjs)
        {
            Destroy(obj);
        }
        
        //��ʈÓ]
        //����
        //�D���ȃ^�C�~���O�ōĊJ�@���@�{�[�����ォ��x���Ă���K���X or �����j�󂳂��B
    }

    //float s = 0.001f;
    //float l = 0.01f;
    float t = 0; //0.5 ���^�� 0 1
    float halfRange = 0.01f;
    float duration = 0.5f;

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
            //Debug.Log(timer);
            yield return null;
        }

        cameraTransform.position = new Vector3(0f, 1f, -0.5f);//originalPosition;
    }
    // �����̏�����
}
