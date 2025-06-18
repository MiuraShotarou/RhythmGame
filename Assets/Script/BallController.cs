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
    [SerializeField] HealthManager healthManager;
    Rigidbody rigidbody;
    float pushPower = 3f;
    float miniY = 0.82f;

    bool ischecked = false;
    bool isInvalid = true;
    bool isBlue = false;
    public static bool isNotDamage = false;
    bool isNotBlockDamage = false;

    GameObject bluePlane;
    // Start is called before the first frame update
    void Start()
    {
        bluePlane = transform.GetChild(0).gameObject;
        Debug.Log(bluePlane);
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, miniY, 100f);
        transform.position = pos;
    }

    void FixedUpdate()
    {
        if (Input.GetButtonDown("PushBall")
            && isInvalid)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(Vector2.down * pushPower, ForceMode.Impulse);
        }
        else if (Input.GetButtonUp("PushBall")
            && isInvalid)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(Vector2.up * pushPower, ForceMode.Impulse);
        }
        else if (Input.GetButton("PushBall")
            && isInvalid)
        {
            //rigidbody.velocity = Vector3.zero;
            //Debug.Log("Hold呼ばれた");
            rigidbody.AddForce(Vector2.down * 150, ForceMode.Force);
        }
        else if (Input.GetButtonDown("ChangeBlue")) //試験的
        {
            isBlue = true;
            bluePlane.SetActive(true);
        }

        if (transform.position.x == 0
            && isInvalid)
        {
            //Debug.Log("Gravityが戻った");
            AntiGravityDeviceControler.isAntiGravity = true;
            GravityDeviceControler.isGravity = true;
            isNotBlockDamage = false; //追加
        }

        if (ischecked)
        {
            Vector3 memorize = rigidbody.velocity;
            if (rigidbody.velocity != memorize)
            {
                //Debug.Log(rigidbody.velocity);
            }
        }
    }

    NoteType noteType;
    Judgment judgment;
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("RightBlade"))
        {
            //Debug.Log("接触・RightBlade");
            StartCoroutine(OnBounceBallRightBlade());

            bluePlane.SetActive(false);
        }
        else if (other.gameObject.CompareTag("LeftBlade"))
        {
            //Debug.Log("接触・LeftBlade");
            StartCoroutine(OnBounceBallLeftBlade());

            bluePlane.SetActive(false);
        }
        else if (other.gameObject.CompareTag("MainNote")
            ||
            other.gameObject.CompareTag("MainNoteLong")) //連続でNoteを叩く場合、前NoteのJudgmentLineZが当たった時間 + 0.115fの間に次NoteがJudgmentLineZに当たってはいけない。
        {
            if (!isBlue)
            {
                isNotDamage = true;
            }
            else if (isBlue)
            {
                healthManager.Damage();
                PosReset("Other"); //追加
                isBlue = false;
                bluePlane.SetActive(false);
                return;
            }

            miniY = 0.85f;

            if (!other.gameObject.GetComponent<NoteController>().IsCollision)
            {
                float judgTime = Time.time - JudgmentLineZ.standardTimes[0];

                //Debug.Log($"JudgmentZ.standardTimes{JudgmentLineZ.standardTimes[0]}; judgTime{judgTime}");
                other.gameObject.GetComponent<NoteController>().IsCollision = true;
                noteType = scoreManager.JudgNoteType(other.gameObject.tag);
                judgment = scoreManager.JudgJudgment(judgTime);
                scoreManager.CalculateScore(noteType, judgment);
            }
        }
        else if (other.gameObject.CompareTag("BlueNote")) //怪しい
        {
            if (!isBlue)
            {
                Transform crackTrs = other.transform.GetChild(2);
                crackTrs.gameObject.SetActive(true);
                return;
                //見た目を変える。
            }
            isBlue = false;
            bluePlane.SetActive(false);

            isNotDamage = true;
            miniY = 0.85f;

            if (!other.gameObject.GetComponent<NoteController>().IsCollision)
            {
                float judgTime = Time.time - JudgmentLineZ.standardTimes[5];

                //Debug.Log($"JudgmentZ.standardTimes{JudgmentLineZ.standardTimes[0]}; judgTime{judgTime}");
                other.gameObject.GetComponent<NoteController>().IsCollision = true;
                noteType = scoreManager.JudgNoteType(other.gameObject.tag);
                judgment = scoreManager.JudgJudgment(judgTime);
                scoreManager.CalculateScore(noteType, judgment);
            }
        }
        else if (other.gameObject.CompareTag("RightRightNote"))
        {
            isNotBlockDamage = true;
            rigidbody.velocity = Vector3.zero;
            Vector3 forceDirection = new Vector3(-1f, 0.1f, 0f);
            rigidbody.AddForce(forceDirection * (pushPower * 0.5f), ForceMode.Impulse);
            StartCoroutine(ActiveGravityAndAntiGravity(0.1f)); //移植

            if (isBlue) //未確認
            {
                healthManager.Damage();
                isBlue = false;
                bluePlane.SetActive(false);
            }

            if (!other.gameObject.GetComponent<NoteController>().IsCollision) //スコアの計算式。
            {
                float judgTime = Time.time - JudgmentLineZ.standardTimes[3];

                //Debug.Log($"JudgmentZ.standardTimes{JudgmentLineZ.standardTimes[3]}; judgTime{judgTime}");
                other.gameObject.GetComponent<NoteController>().IsCollision = true;
                noteType = scoreManager.JudgNoteType(other.gameObject.tag);
                judgment = scoreManager.JudgJudgment(judgTime);
                scoreManager.CalculateScore(noteType, judgment);
            }
        }
        else if (other.gameObject.CompareTag("LeftLeftNote")) //スコアの計算式がない。
        {
            isNotBlockDamage = true;
            rigidbody.velocity = Vector3.zero;
            Vector3 forceDirection = new Vector3(1f, 0.1f, 0f);
            rigidbody.AddForce(forceDirection * (pushPower * 0.5f), ForceMode.Impulse);
            StartCoroutine(ActiveGravityAndAntiGravity(0.1f)); //移植

            if (isBlue) //未確認
            {
                healthManager.Damage();
                isBlue = false;
                bluePlane.SetActive(false);
            }

            if (!other.gameObject.GetComponent<NoteController>().IsCollision) //スコアの計算式。
            {
                float judgTime = Time.time - JudgmentLineZ.standardTimes[4];

                //Debug.Log($"JudgmentZ.standardTimes{JudgmentLineZ.standardTimes[4]}; judgTime{judgTime}");
                other.gameObject.GetComponent<NoteController>().IsCollision = true;
                noteType = scoreManager.JudgNoteType(other.gameObject.tag);
                judgment = scoreManager.JudgJudgment(judgTime);
                scoreManager.CalculateScore(noteType, judgment);
            }
        }
        else if (other.gameObject.CompareTag("RightDamageBlock")
            &&
            !isNotBlockDamage)
        {
            rigidbody.velocity = Vector3.zero;
            Vector3 forceDirection = new Vector3(-1f, 0.1f, 0f);
            rigidbody.AddForce(forceDirection * (pushPower * 0.5f), ForceMode.Impulse);
            StartCoroutine(ActiveGravityAndAntiGravity(1f)); //移植

            isBlue = false;
            bluePlane.SetActive(false);
            healthManager.Damage();
            //StartCoroutine(PosReset("RightDamageBlock"));
        }
        else if (other.gameObject.CompareTag("LeftDamageBlock")
            &&
            !isNotBlockDamage)
        {
            rigidbody.velocity = Vector3.zero;
            Vector3 forceDirection = new Vector3(1f, 0.1f, 0f);
            rigidbody.AddForce(forceDirection * (pushPower * 0.5f), ForceMode.Impulse);
            StartCoroutine(ActiveGravityAndAntiGravity(1f)); //いじり

            isBlue = false;
            bluePlane.SetActive(false);
            healthManager.Damage();
            //StartCoroutine(PosReset("LeftDamageBlock"));
        }
        else if (other.gameObject.GetComponent<MeshRenderer>().enabled != false
            &&
            !isNotDamage)
        {
            StartCoroutine(IsNotDamageController());
            //Debug.Log($"{other.gameObject}などに当たっている");
            healthManager.Damage();
            PosReset("Other"); //追加
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("MainNoteLong")
           &&
           !other.gameObject.GetComponent<NoteController>().isCollisionStay)
        {
            //Vector3 pos = transform.position;
            //pos.y = Mathf.Clamp(pos.y, 0.85f, 100f);
            //transform.position = pos;
            StartCoroutine(LongNoteManager(other.gameObject));
            noteType = scoreManager.JudgNoteType(other.gameObject.tag);
            scoreManager.CalculateScore(noteType, judgment);
            //Debug.Log($"Notetype{noteType}, Judgment{judgment}");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("MainNote")
            ||
            other.gameObject.CompareTag("MainNoteLong")
            ||
            other.gameObject.CompareTag("BlueNote"))
        {
            miniY = 0.82f;

            isNotDamage = false;
            //Vector3 pos = transform.position;
            //pos.y = Mathf.Clamp(pos.y, 0.85f, 100f);
            //transform.position = pos;
        }
    }


    IEnumerator OnBounceBallRightBlade()
    {
        isInvalid = false;
        GravityDeviceControler.isGravity = false;
        AntiGravityDeviceControler.isAntiGravity = false;

        rigidbody.velocity = Vector3.zero;
        Vector3 forceDirection = new Vector3(-1f, 0.8f, 0f).normalized;
        rigidbody.AddForce(forceDirection * pushPower, ForceMode.Impulse);
        yield return null;
    }
    IEnumerator OnBounceBallLeftBlade()
    {
        isInvalid = false;
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
            isInvalid = true;
        }
    }

    IEnumerator LongNoteManager(GameObject noteLong)
    {
        noteLong.GetComponent<NoteController>().isCollisionStay = true;
        yield return new WaitForSeconds(0.0166f);                                //ほぼワンフレームにつき加点
        noteLong.GetComponent<NoteController>().isCollisionStay = false;
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
                startPos = new Vector3(0f, 0.82f, -0.1f);
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