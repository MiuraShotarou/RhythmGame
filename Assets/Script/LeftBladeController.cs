using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class LeftBladeController : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] HealthManager healthManager;
    [SerializeField] GameObject sparksEffect;
    //[SerializeField] ParticleSystem particleSystem;
    Rigidbody rigidbody;

    float slidePower = 400f;

    bool isInvalid = false;
    bool isRotation = false;
    bool isDamageReturn = false;
    //bool testBool = false;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -0.20404f, -0.0265f);
        pos.y = Mathf.Clamp(pos.y, 0.83f, 1.188f);
        transform.position = pos;

        if (transform.position == new Vector3(-0.20404f, 1.188f, -0.11f))
        {
            isInvalid = false;
            isDamageReturn = false;
        }

        if ((transform.position.x == -0.2f && transform.position.y == 1.188f)
            || (transform.position.x == -0.0265f && transform.position.y == 0.83f))
        {
            rigidbody.velocity = Vector3.zero;
        }


        if (Input.GetButtonDown("LeftBlade")
            &&
            !isInvalid)
        {
            isRotation = false;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce((transform.up * -1) * slidePower, ForceMode.Force);
            StartCoroutine(RotationZControllerDown());
        }
        else if (Input.GetButton("LeftBlade")
            &&
            !isInvalid)
        {
            if (transform.position.y < 1.15f)//0.97838
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.AddForce((transform.up * -1) * (slidePower * 2.1f), ForceMode.Force);
            }
        }
        else if (Input.GetButtonUp("LeftBlade")
            &&
            !isDamageReturn)
        {
            isRotation = true;
            isInvalid = true;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(transform.up * slidePower, ForceMode.Force);
            StartCoroutine(RotationZControllerUp());
        }
    }

    NoteType noteType;
    Judgment judgment;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("LeftNote")
            ||
            collision.gameObject.CompareTag("LeftNoteLong"))
        {
            BallController.isNotDamage = true;
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -0.20404f, -0.053f);
            pos.y = Mathf.Clamp(pos.y, 0.86f, 0.853f);
            transform.position = pos;
            sparksEffect.SetActive(true);
            //particleSystem.Play();
            if (!collision.gameObject.GetComponent<NoteController>().IsCollision)
            {
                float judgTime = Time.time - JudgmentLineZ.standardTimes[2];

                Debug.Log($"LeftBladeのJudgmentZ.standardTimes{JudgmentLineZ.standardTimes[2]}; judgTime{judgTime}");
                collision.gameObject.GetComponent<NoteController>().IsCollision = true;
                noteType = scoreManager.JudgNoteType(collision.gameObject.tag);
                judgment = scoreManager.JudgJudgment(judgTime);
                scoreManager.CalculateScore(noteType, judgment); //←Judgment型の変数
            }
        }
        else
        {
            isDamageReturn = true;
            healthManager.Damage();
            StartCoroutine(posReset());
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("LeftNote")
            ||
            collision.gameObject.CompareTag("LeftNoteLong"))
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -0.20404f, -0.053f);
            pos.y = Mathf.Clamp(pos.y, 0.86f, 0.85228f);
            transform.position = pos;
            if (collision.gameObject.CompareTag("LeftNoteLong")
                &&
                !collision.gameObject.GetComponent<NoteController>().isCollisionStay)
            {
                StartCoroutine(LongNoteManager(collision.gameObject));
                noteType = scoreManager.JudgNoteType(collision.gameObject.tag);
                scoreManager.CalculateScore(noteType, judgment);
                Debug.Log($"Notetype{noteType}, Judgment{judgment}");
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("LeftNote")
            ||
            collision.gameObject.CompareTag("LeftNoteLong"))
        {
            BallController.isNotDamage = false;
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -0.20404f , -0.05375149f);
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
    IEnumerator RotationZControllerDown()
    {
        float timer = 0;
        float startTime = Time.time;
        float duration = 3.5f;
        float rotationT = 0;
        float rotationZ = 0;
        int roopCount = 0;
        float keisu = 0.9f;

        while (timer <= duration
            &&
            !isRotation)
        {
            rotationT = Mathf.Lerp(0, 1, (timer + (roopCount * keisu)) / duration);
            rotationZ = Mathf.Lerp(0, 50, rotationT);
            transform.rotation = Quaternion.Euler(0, 0, rotationZ); //timerの更新から。
            timer = Time.time - startTime;
            roopCount++;
            yield return null;
        }
    }
    IEnumerator RotationZControllerUp()
    {
        Debug.Log("Upのほうが呼ばれている");
        float timer = 0;
        float startTime = Time.time;
        float duration = 3.5f;
        float rotationT = 0;
        float rotationZ = 0;
        int roopCount = 0;
        float keisu = 0.9f;

        while (timer <= duration
            &&
            isRotation)
        {
            rotationT = Mathf.Lerp(0, 1, (timer + (roopCount * keisu)) / duration);
            rotationZ = Mathf.Lerp(50, 0, rotationT);
            //Debug.Log($"rotationZ;{rotationZ}");
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
            rotationZ = Mathf.Lerp(50, 0, rotationT);
            transform.rotation = Quaternion.Euler(0, 0, rotationZ); //timerの更新から。
            timer = Time.time - startTime;
            roopCount++;
            yield return null;
        }
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
}