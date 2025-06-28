using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
//using static TreeEditor.TreeEditorHelper;

//
public class BallController : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] InGameManager inGameManager;

    GameObject arrowPlane;
    Rigidbody rigidbody;
    float pushPower = 5f;
    float miniY = 0.79f;
    bool isBlue = false;
    public static bool isNotDamage = false;

    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        arrowPlane = transform.GetChild(0).gameObject;
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, miniY, 100f);
        transform.position = pos;

        if (Input.GetButtonDown("ChangeBlue")) //試験的
        {
            isBlue = true;
            arrowPlane.SetActive(true);
        }
        else if (Input.GetButtonDown("ChangeRed"))
        {
            isBlue = false;
            arrowPlane.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (Input.GetButtonDown("PushBall")
            && !GameManager.IsInvalid)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(Vector2.down * pushPower * 1.2f, ForceMode.Impulse);
        }
        else if (Input.GetButtonUp("PushBall")
            && !GameManager.IsInvalid)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(Vector2.up * (pushPower * 0.25f), ForceMode.Impulse);
        }
        //else if (Input.GetButton("PushBall")
        //    && !GameManager.IsInvalid)
        //{
        //    rigidbody.AddForce(Vector2.down * 150, ForceMode.Force);
        //}

        if (transform.position.x == 0
            && !GameManager.IsInvalid)
        {
            AntiGravityDeviceControler.isAntiGravity = true;
            GravityDeviceControler.isGravity = true;
        }
    }

    NoteType noteType;
    Judgment judgment;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RightBlade"))
        {
            StartCoroutine(OnBounceBallRightBlade());
        }
        else if (other.gameObject.CompareTag("LeftBlade"))
        {
            StartCoroutine(OnBounceBallLeftBlade());
        }
        else if (other.gameObject.CompareTag("MainNote")
            ||
            other.gameObject.CompareTag("MainNoteLong")) //連続でNoteを叩く場合、前NoteのJudgmentLineZが当たった時間 + 0.115fの間に次NoteがJudgmentLineZに当たってはいけない。
        {
            if (GameManager.IsTutorial
                &&
                GameManager.IsLoop)
            {
                GameManager.IsLoop = false;
                audioSource.Play();
            }

            if (!isBlue)
            {
                isNotDamage = true;
            }
            else if (isBlue)
            {
                inGameManager.Damage();
                PosReset("Other"); //追加
                isBlue = false;
                arrowPlane.SetActive(false);
                return;
            }

            miniY = 0.85f;

            if (!other.gameObject.GetComponent<NoteController>().IsCollision)
            {
                float judgTime = Time.time - JudgmentLineZ.standardTimes[0];

                noteType = scoreManager.JudgNoteType(other.gameObject.tag);
                judgment = scoreManager.JudgJudgment(judgTime);
                scoreManager.CalculateScore(noteType, judgment, other.gameObject);
            }
        }
        else if (other.gameObject.CompareTag("BlueNote"))
        {
            isNotDamage = true;

            if (!isBlue
                &&
                !other.gameObject.GetComponent<NoteController>().IsCollision)
            {
                Transform crackTrs = other.transform.GetChild(2);
                crackTrs.gameObject.SetActive(true);
                return;
                //見た目を変える。
            }
            else if (GameManager.IsTutorial
            &&
            GameManager.IsLoop)
            {
                GameManager.IsLoop = false;
                audioSource.Play();
            }

            miniY = 0.85f;
            isBlue = false;
            arrowPlane.SetActive(false);

            if (!other.gameObject.GetComponent<NoteController>().IsCollision)
            {
                float judgTime = Time.time - JudgmentLineZ.standardTimes[5];

                noteType = scoreManager.JudgNoteType(other.gameObject.tag);
                judgment = scoreManager.JudgJudgment(judgTime);
                scoreManager.CalculateScore(noteType, judgment, other.gameObject);
            }
        }
        else if (other.gameObject.CompareTag("RightRightNote"))
        {
            isNotDamage = true;
            rigidbody.velocity = Vector3.zero;
            Vector3 forceDirection = new Vector3(-1f, 0.4f, 0f);
            rigidbody.AddForce(forceDirection * (pushPower * 0.2f), ForceMode.Impulse);
            StartCoroutine(ActiveGravityAndAntiGravity(0.1f)); //移植

            if (isBlue) //未確認
            {
                inGameManager.Damage();
                isBlue = false;
                arrowPlane.SetActive(false);
                return;
            }
            if (GameManager.IsTutorial
                &&
                GameManager.IsLoop)
            {
                GameManager.IsLoop = false;
                audioSource.Play();
            }

            if (!other.gameObject.GetComponent<NoteController>().IsCollision) //スコアの計算式。
            {
                float judgTime = Time.time - JudgmentLineZ.standardTimes[3];

                noteType = scoreManager.JudgNoteType(other.gameObject.tag);
                judgment = scoreManager.JudgJudgment(judgTime);
                scoreManager.CalculateScore(noteType, judgment, other.gameObject);
            }
        }
        else if (other.gameObject.CompareTag("LeftLeftNote"))
        {
            if (GameManager.IsTutorial
                &&
                GameManager.IsLoop)
            {
                GameManager.IsLoop = false;
                audioSource.Play();
            }

            isNotDamage = true;
            rigidbody.velocity = Vector3.zero;
            Vector3 forceDirection = new Vector3(1f, 0.4f, 0f);
            rigidbody.AddForce(forceDirection * (pushPower * 0.2f), ForceMode.Impulse);
            StartCoroutine(ActiveGravityAndAntiGravity(0.1f)); //移植

            if (isBlue) //未確認
            {
                inGameManager.Damage();
                isBlue = false;
                arrowPlane.SetActive(false);
                return;
            }
            if (GameManager.IsTutorial
                &&
                GameManager.IsLoop)
            {
                GameManager.IsLoop = false;
                audioSource.Play();
            }

            if (!other.gameObject.GetComponent<NoteController>().IsCollision) //スコアの計算式。
            {
                float judgTime = Time.time - JudgmentLineZ.standardTimes[4];

                //Debug.Log($"JudgmentZ.standardTimes{JudgmentLineZ.standardTimes[4]}; judgTime{judgTime}");
                noteType = scoreManager.JudgNoteType(other.gameObject.tag);
                judgment = scoreManager.JudgJudgment(judgTime);
                scoreManager.CalculateScore(noteType, judgment, other.gameObject);
            }
        }
        else if (other.gameObject.CompareTag("RightDamageBlock"))
        {
            rigidbody.velocity = Vector3.zero;
            Vector3 forceDirection = new Vector3(-1f, 0.1f, 0f);
            rigidbody.AddForce(forceDirection * (pushPower * 0.2f), ForceMode.Impulse);
            StartCoroutine(ActiveGravityAndAntiGravity(0.2f)); //移植

            isBlue = false;
            arrowPlane.SetActive(false);
            if (!GameManager.IsTutorial
                &&
                !isNotDamage)
            {
                StartCoroutine(IsNotDamageController());
                inGameManager.Damage();
            }
        }
        else if (other.gameObject.CompareTag("LeftDamageBlock"))
        {
            rigidbody.velocity = Vector3.zero;
            Vector3 forceDirection = new Vector3(1f, 0.1f, 0f);
            rigidbody.AddForce(forceDirection * (pushPower * 0.2f), ForceMode.Impulse);
            StartCoroutine(ActiveGravityAndAntiGravity(0.2f)); //いじり

            isBlue = false;
            arrowPlane.SetActive(false);
            if (!GameManager.IsTutorial
                &&
                !isNotDamage)
            {
                StartCoroutine(IsNotDamageController());
                inGameManager.Damage();
            }
        }
        else if (other.gameObject.CompareTag("RightNoteLong")
             ||
             other.gameObject.CompareTag("LeftNoteLong"))
        {
            return;
        }
        else if (other.gameObject.GetComponent<MeshRenderer>().enabled != false
            &&
            !isNotDamage
            &&
            !GameManager.IsTutorial)
        {
            StartCoroutine(IsNotDamageController());
            //Debug.Log($"{other.gameObject}などに当たっている");
            inGameManager.Damage();
            PosReset("Other"); //追加
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("MainNote")
            ||
            other.gameObject.CompareTag("MainNoteLong")
            ||
            other.gameObject.CompareTag("BlueNote")
            ||
            other.gameObject.CompareTag("RightRightNote")
            ||
            other.gameObject.CompareTag("LeftLeftNote"))
        {
            miniY = 0.79f;

            StartCoroutine(IsNotDamageController());
        }
    }

    IEnumerator OnBounceBallRightBlade()
    {
        GameManager.IsInvalid = true;
        GravityDeviceControler.isGravity = false;
        AntiGravityDeviceControler.isAntiGravity = false;

        rigidbody.velocity = Vector3.zero;
        Vector3 forceDirection = new Vector3(-1f, 0.8f, 0f).normalized;
        rigidbody.AddForce(forceDirection * pushPower, ForceMode.Impulse);
        yield return null;
    }
    IEnumerator OnBounceBallLeftBlade()
    {
        GameManager.IsInvalid = true;
        GravityDeviceControler.isGravity = false;
        AntiGravityDeviceControler.isAntiGravity = false;

        rigidbody.velocity = Vector3.zero;
        Vector3 forceDirection = new Vector3(1f, 0.8f, 0f).normalized;
        rigidbody.AddForce(forceDirection * pushPower, ForceMode.Impulse);
        yield return null;
    }

    IEnumerator ActiveGravityAndAntiGravity(float i)
    {
        yield return new WaitForSeconds(i);

        AntiGravityDeviceControler.isAntiGravity = true;
        GravityDeviceControler.isGravity = true;

        rigidbody.velocity = Vector3.zero;
        Vector3 vector = transform.position;
        vector.x = 0f;
        transform.position = vector;

        if (transform.position.x == 0f)
        {
            GameManager.IsInvalid = false;
        }
    }

    IEnumerator LongNoteManager(GameObject noteLong)
    {
        noteLong.GetComponent<NoteController>().IsCollisionStay = true;
        yield return new WaitForSeconds(0.0166f);                                //ほぼワンフレームにつき加点
        noteLong.GetComponent<NoteController>().IsCollisionStay = false;
    }
    public IEnumerator PosReset(string collisionName)
    {
        Debug.Log("posReset起動"); //赤色の点滅とかがあると良いかもしれない。
        float timer = 0;
        //float startTime = Time.time;
        float duration = 1f;
        Vector3 startPos = new Vector3();
        Vector3 endPos = new Vector3(0f, 0.93f, -0.1f);
        //int roopCount = 0;
        //float keisu = 0.9f;
        switch (collisionName)
        {
            case "RightDamageBlock":
                startPos = new Vector3(0.3162518f, 1.126752f, -0.1f);
                break;
            case "LeftDamageBlock":
                startPos = new Vector3(-0.3162518f, 1.126752f, -0.1f);
                break;
            case "Other":
                startPos = new Vector3(0f, 0.79f, -0.1f);
                break;
        }

        while (timer <= duration)
        {
            float t = Mathf.Lerp(0, 1, timer / duration);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            timer = timer + Time.deltaTime;
            //roopCount++;
            yield return null;
        }
    }

    IEnumerator IsNotDamageController()
    {
        isNotDamage = true;
        yield return new WaitForSeconds(0.5f);
        isNotDamage = false;
    }
}


// ボールを落とし、元に戻す。
// ① ポジションを指定し、そこに移動させる。
// ② 減速させて指定の位置で止める。
// ③ 指定した位置にだけ重力を発生させる。
// ④ 上下にベクトルを与え続け、ボタンを押すとオン・オフが出来る。