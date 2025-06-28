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
    [SerializeField] InGameManager inGameManager;
    [SerializeField] GameObject sparksEffect;

    Rigidbody rigidbody;

    float slidePower = 300f;

    public bool isInvalid = false;
    bool isRotation = false;
    bool isDamageReturn = false;
    float tutorialStartTime = 0;

    public bool isSuccessInvalid = false;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, 0.0265f, 0.20404f);
        pos.y = Mathf.Clamp(pos.y, 0.83f, 1.188f);
        transform.position = pos;

        if (transform.position == new Vector3(0.20404f, 1.188f, -0.11f))
        {
            isInvalid = false;
            isDamageReturn = false;
        }

        if ((transform.position.x == 0.2f && transform.position.y == 1.188f)
            || (transform.position.x == 0.0265f && transform.position.y == 0.83f))
        {
            rigidbody.velocity = Vector3.zero;
        }

        if (Input.GetButtonDown("RightBlade")
            &&
            !isInvalid)
        {
            isRotation = false;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce((transform.up * -1) * slidePower, ForceMode.Force);
            StartCoroutine(RotationZControllerDown());
        }
        else if (Input.GetButton("RightBlade")
            &&
            !isInvalid)
        {
            if (transform.position.y < 1.15f)//0.97838
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.AddForce((transform.up * -1) * (slidePower * 1.8f), ForceMode.Force);
            }
        }
        else if (Input.GetButtonUp("RightBlade")
            &&
            !isDamageReturn)
        {
            isRotation = true;
            isInvalid = true;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(transform.up * (slidePower * 2f), ForceMode.Force);
            StartCoroutine(RotationZControllerUp());
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
            BallController.isNotDamage = true;
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, 0.053f, 0.20404f);
            pos.y = Mathf.Clamp(pos.y, 0.86f, 0.853f);
            transform.position = pos;
            sparksEffect.SetActive(true);
            collision.gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(1, 1, 0.7843137254901961f));

            if (!collision.gameObject.GetComponent<NoteController>().IsCollision)
            {
                if (GameManager.IsTutorial)
                {
                    tutorialStartTime = 0;
                    tutorialStartTime = Time.time;
                }
                float judgTime = Time.time - JudgmentLineZ.standardTimes[1];
                noteType = scoreManager.JudgNoteType(collision.gameObject.tag);
                judgment = scoreManager.JudgJudgment(judgTime);
                if (judgment != Judgment.Miss)
                {
                    scoreManager.CalculateScore(noteType, judgment, collision.gameObject); //←Judgment型の変数
                }
            }
        }
        else if (!GameManager.IsTutorial
            &&
            collision.gameObject.CompareTag("Stage"))
        {
            isDamageReturn = true;
            inGameManager.Damage();
            StartCoroutine(posReset());
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("RightNote")
            ||
            collision.gameObject.CompareTag("RightNoteLong"))
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, 0.053f, 0.20404f);
            pos.y = Mathf.Clamp(pos.y, 0.86f, 0.85228f);
            transform.position = pos;
            if (collision.gameObject.CompareTag("RightNoteLong")
                &&
                !collision.gameObject.GetComponent<NoteController>().IsCollisionStay)
            {
                StartCoroutine(LongNoteManager(collision.gameObject));
                noteType = scoreManager.JudgNoteType(collision.gameObject.tag);
                scoreManager.CalculateScore(noteType, judgment, collision.gameObject);
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("RightNote")
            ||
            collision.gameObject.CompareTag("RightNoteLong"))
        {
            if (GameManager.IsTutorial
                &&
                Time.time - tutorialStartTime >= 1.3f
                &&
                !isSuccessInvalid)
            {
                GameManager.IsLoop = false;
            }

            BallController.isNotDamage = false;
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, 0.05375149f, 0.20404f);
            pos.y = Mathf.Clamp(pos.y, 0.86f, 0.85228f);
            transform.position = pos;
            sparksEffect.SetActive(false);
            collision.gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(1, 1, 0f));
        }
    }

    IEnumerator LongNoteManager(GameObject noteLong)
    {
        noteLong.GetComponent<NoteController>().IsCollisionStay = true;
        yield return new WaitForSeconds(0.0166f);                                //ほぼワンフレームにつき加点
        noteLong.GetComponent<NoteController>().IsCollisionStay = false;
    }
    IEnumerator RotationZControllerDown()
    {
        float timer = 0;
        float startTime = Time.time;
        float duration = 6f;   //
        float rotationT = 0;
        float rotationZ = 0;
        int roopCount = 0;
        float keisu = 0.8f;   //

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
    }
    IEnumerator RotationZControllerUp()
    {
        float timer = 0;
        float startTime = Time.time;
        float duration = 4f;
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
            transform.rotation = Quaternion.Euler(0, 0, rotationZ); //timerの更新から。
            timer = Time.time - startTime;
            roopCount++;
            yield return null;
        }
    }
    IEnumerator posReset()
    {
        isRotation = true;
        isInvalid = true;
        rigidbody.velocity = Vector3.zero;

        float timer = 0;
        float startTime = Time.time;
        float duration = 1.25f;
        float rotationT = 0;
        float rotationZ = 0;
        int roopCount = 0;
        float keisu = 0.9f;

        while (timer <= duration
            &&
            isRotation)
        {
            rigidbody.AddForce(transform.up * 1f, ForceMode.Force);
            rotationT = Mathf.Lerp(0, 1, timer / duration);
            rotationZ = Mathf.Lerp(-50, 0, rotationT);
            transform.rotation = Quaternion.Euler(0, 0, rotationZ); //timerの更新から。
            timer = Time.time - startTime;
            roopCount++;
            yield return null;
        }
    }
}