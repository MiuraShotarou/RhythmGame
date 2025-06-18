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
            //Debug.Log("Hold�Ă΂ꂽ");
            rigidbody.AddForce(Vector2.down * 150, ForceMode.Force);
        }
        else if (Input.GetButtonDown("ChangeBlue")) //�����I
        {
            isBlue = true;
            bluePlane.SetActive(true);
        }

        if (transform.position.x == 0
            && isInvalid)
        {
            //Debug.Log("Gravity���߂���");
            AntiGravityDeviceControler.isAntiGravity = true;
            GravityDeviceControler.isGravity = true;
            isNotBlockDamage = false; //�ǉ�
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
            //Debug.Log("�ڐG�ERightBlade");
            StartCoroutine(OnBounceBallRightBlade());

            bluePlane.SetActive(false);
        }
        else if (other.gameObject.CompareTag("LeftBlade"))
        {
            //Debug.Log("�ڐG�ELeftBlade");
            StartCoroutine(OnBounceBallLeftBlade());

            bluePlane.SetActive(false);
        }
        else if (other.gameObject.CompareTag("MainNote")
            ||
            other.gameObject.CompareTag("MainNoteLong")) //�A����Note��@���ꍇ�A�ONote��JudgmentLineZ�������������� + 0.115f�̊ԂɎ�Note��JudgmentLineZ�ɓ������Ă͂����Ȃ��B
        {
            if (!isBlue)
            {
                isNotDamage = true;
            }
            else if (isBlue)
            {
                healthManager.Damage();
                PosReset("Other"); //�ǉ�
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
        else if (other.gameObject.CompareTag("BlueNote")) //������
        {
            if (!isBlue)
            {
                Transform crackTrs = other.transform.GetChild(2);
                crackTrs.gameObject.SetActive(true);
                return;
                //�����ڂ�ς���B
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
            StartCoroutine(ActiveGravityAndAntiGravity(0.1f)); //�ڐA

            if (isBlue) //���m�F
            {
                healthManager.Damage();
                isBlue = false;
                bluePlane.SetActive(false);
            }

            if (!other.gameObject.GetComponent<NoteController>().IsCollision) //�X�R�A�̌v�Z���B
            {
                float judgTime = Time.time - JudgmentLineZ.standardTimes[3];

                //Debug.Log($"JudgmentZ.standardTimes{JudgmentLineZ.standardTimes[3]}; judgTime{judgTime}");
                other.gameObject.GetComponent<NoteController>().IsCollision = true;
                noteType = scoreManager.JudgNoteType(other.gameObject.tag);
                judgment = scoreManager.JudgJudgment(judgTime);
                scoreManager.CalculateScore(noteType, judgment);
            }
        }
        else if (other.gameObject.CompareTag("LeftLeftNote")) //�X�R�A�̌v�Z�����Ȃ��B
        {
            isNotBlockDamage = true;
            rigidbody.velocity = Vector3.zero;
            Vector3 forceDirection = new Vector3(1f, 0.1f, 0f);
            rigidbody.AddForce(forceDirection * (pushPower * 0.5f), ForceMode.Impulse);
            StartCoroutine(ActiveGravityAndAntiGravity(0.1f)); //�ڐA

            if (isBlue) //���m�F
            {
                healthManager.Damage();
                isBlue = false;
                bluePlane.SetActive(false);
            }

            if (!other.gameObject.GetComponent<NoteController>().IsCollision) //�X�R�A�̌v�Z���B
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
            StartCoroutine(ActiveGravityAndAntiGravity(1f)); //�ڐA

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
            StartCoroutine(ActiveGravityAndAntiGravity(1f)); //������

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
            //Debug.Log($"{other.gameObject}�Ȃǂɓ������Ă���");
            healthManager.Damage();
            PosReset("Other"); //�ǉ�
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
        yield return new WaitForSeconds(0.0166f);                                //�قڃ����t���[���ɂ����_
        noteLong.GetComponent<NoteController>().isCollisionStay = false;
    }
    public IEnumerator PosReset(string collisionName)
    {
        Debug.Log("posReset�N��"); //�ԐF�̓_�łƂ�������Ɨǂ���������Ȃ��B
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


// �{�[���𗎂Ƃ��A���ɖ߂��B
// �@ �|�W�V�������w�肵�A�����Ɉړ�������B
// �A ���������Ďw��̈ʒu�Ŏ~�߂�B
// �B �w�肵���ʒu�ɂ����d�͂𔭐�������B
// �C �㉺�Ƀx�N�g����^�������A�{�^���������ƃI���E�I�t���o����B