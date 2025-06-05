using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class RightBladeController : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] HealthManager healthManager;
    [SerializeField] GameObject sparksEffect;
    //[SerializeField] ParticleSystem particleSystem;
    Rigidbody rigidbody;

    float slidePower = 400f; //700

    bool isRotation = false;
    //bool testBool = false; //出撃
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, 0.0265f, 0.20404f);
        pos.y = Mathf.Clamp(pos.y, 0.83f, 1.188f); //0.97839f
        transform.position = pos;

        if ((transform.position.x == 0.2f && transform.position.y == 1.188f) //0.975f
            || (transform.position.x == 0.0265f && transform.position.y == 0.83f))
        {
            rigidbody.velocity = Vector3.zero;
        }

        if (Input.GetButtonDown("RightBlade"))
        {
            isRotation = false;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce((transform.up * -1) * slidePower, ForceMode.Force);
            StartCoroutine(RotationZControllerDown());
        }
        else if (Input.GetButtonUp("RightBlade"))
        {
            isRotation = true;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(transform.up * slidePower, ForceMode.Force);
            StartCoroutine(RotationZControllerUp());
        }
        else if (Input.GetButton("RightBlade"))
        {
            if (transform.position.y < 1.15f)//0.97838
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.AddForce((transform.up * -1) * (slidePower * 2.1f), ForceMode.Force);
            }
        }
    }

    NoteType noteType;
    Judgment judgment;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("RightNote")
            ||
            collision.gameObject.CompareTag("RightNoteLong"))
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, 0.053f, 0.20404f);
            pos.y = Mathf.Clamp(pos.y, 0.86f, 0.853f);
            transform.position = pos;
            sparksEffect.SetActive(true);
            //particleSystem.Play();
            if (!collision.gameObject.GetComponent<NoteController>().isCollision)
            {
                float judgTime = Time.time - JudgmentLineZ.standardTimes[1];

                collision.gameObject.GetComponent<NoteController>().isCollision = true;
                noteType = scoreManager.JudgNoteType(collision.gameObject.tag);
                judgment = scoreManager.JudgJudgment(judgTime);
                scoreManager.CalculateScore(noteType, judgment); //←Judgment型の変数
            }
        }
        else
        {
            healthManager.Damage();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("RightNote")
            ||
            collision.gameObject.CompareTag("RightNoteLong"))
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, 0.05375149f, 0.20404f);
            pos.y = Mathf.Clamp(pos.y, 0.86f, 0.85228f);
            transform.position = pos;
            if (collision.gameObject.CompareTag("RightNoteLong")
                &&
                !collision.gameObject.GetComponent<NoteController>().isCollisionStay)
            {
                StartCoroutine(LongNoteManager(collision.gameObject));
                noteType = scoreManager.JudgNoteType(collision.gameObject.tag);
                scoreManager.CalculateScore(noteType, judgment);
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("RightNote")
            ||
            collision.gameObject.CompareTag("RightNoteLong"))
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, 0.05375149f, 0.20404f);
            pos.y = Mathf.Clamp(pos.y, 0.86f, 0.85228f);
            transform.position = pos;
            sparksEffect.SetActive(false);
            //particleSystem.Stop();
            //testBool = false;
            //rigidbody.velocity = Vector3.zero;
            //rigidbody.AddForce((transform.up * -1) * slidePower, ForceMode.Force); //ここでAddforceしているのが良くないかも。
            //StartCoroutine(PositionLog());
        }
    }

    IEnumerator LongNoteManager(GameObject noteLong)
    {
        noteLong.GetComponent<NoteController>().isCollisionStay = true;
        yield return new WaitForSeconds(0.0166f);                                //ほぼワンフレームにつき加点
        noteLong.GetComponent<NoteController>().isCollisionStay = false;
    }

    IEnumerator PositionLog()
    {
        float counter = 0f;
        float countMax = 50f;

        Debug.Log($"離れる{rigidbody.velocity}");

        while (counter < countMax)
        {
            //Debug.Log($"Position{transform.position}"); //
            //counter++;
        }

        yield return null;
    }

    IEnumerator RotationZControllerDown()
    {
        float timer = 0;
        float startTime = Time.time;
        float duration = 4.5f;
        float rotationT = 0;
        float rotationZ = 0;
        int roopCount = 0;
        float keisu = 0.9f;

        while (timer <= duration
            &&
            !isRotation)
        {
            rotationT = Mathf.Lerp(0, 1, (timer + (roopCount * keisu)) / duration);
            rotationZ = Mathf.Lerp(0, -50, rotationT);
            transform.rotation = Quaternion.Euler(0, 0, rotationZ); //timerの更新から。
            timer = Time.time - startTime;
            roopCount++;
            yield return null;
        }
        Debug.Log("RotationZControllerDownが切れた");
    }
    IEnumerator RotationZControllerUp()
    {
        Debug.Log("Upのほうが呼ばれている");
        float timer = 0;
        float startTime = Time.time;
        float duration = 4.5f;
        float rotationT = 0;
        float rotationZ = 0;
        int roopCount = 0;
        float keisu = 0.9f;

        while (timer <= duration
            &&  
            isRotation)
        {
            rotationT = Mathf.Lerp(0, 1, (timer + (roopCount * keisu)) / duration);
            rotationZ = Mathf.Lerp(-50, 0, rotationT);
            Debug.Log($"rotationZ;{rotationZ}");
            transform.rotation = Quaternion.Euler(0, 0, rotationZ); //timerの更新から。
            timer = Time.time - startTime;
            roopCount++;
            yield return null;
        }
    }
}