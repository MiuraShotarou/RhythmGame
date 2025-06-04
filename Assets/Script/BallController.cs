using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using static TreeEditor.TreeEditorHelper;

public class BallController : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] HealthManager healthManager;
    Rigidbody rigidbody;
    float pushPower = 3f;
    float miniY = 0.82f;

    bool ischecked = false;
    bool isInvalid = true;

    bool isNotDamage = false;
    // Start is called before the first frame update
    void Start()
    {
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
        else if (Input.GetButtonDown("Test")) //�����I
        {
            Debug.Log(scoreManager.TotalScore);
        }

        if (transform.position.x == 0
            && isInvalid)
        {
            //Debug.Log("Gravity���߂���");
            AntiGravityDeviceControler.isAntiGravity = true;
            GravityDeviceControler.isGravity = true;
        }

        if (ischecked)
        {
            Vector3 memorize = rigidbody.velocity;
            if (rigidbody.velocity != memorize)
            {
                Debug.Log(rigidbody.velocity);
            }
        }
    }

    NoteType noteType;
    Judgment judgment;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RightBlade"))
        {
            Debug.Log("�ڐG�ERightBlade");
            StartCoroutine(OnBounceBallRightBlade());

            ischecked = true;
        }

        if (other.gameObject.CompareTag("LeftBlock"))
        {
            //ischecked = true;
            rigidbody.velocity = Vector3.zero;
            Vector3 forceDirection = new Vector3(1f, 0.1f, 0f);
            rigidbody.AddForce(forceDirection * (pushPower * 0.5f), ForceMode.Impulse);
            StartCoroutine(ActiveGravityAndAntiGravity(0.1f)); //�ڐA
        }
        
        if (other.gameObject.CompareTag("LeftDamageBlock"))
        {
            rigidbody.velocity = Vector3.zero;
            Vector3 forceDirection = new Vector3(1f, 0.1f, 0f);
            rigidbody.AddForce(forceDirection * (pushPower * 0.5f), ForceMode.Impulse); //�������x���Ƃ܂�����������Ȃ��B
            StartCoroutine(ActiveGravityAndAntiGravity(0.2f)); //�ڐA
            healthManager.Damage();
        }

        if (other.gameObject.CompareTag("RightDamageBlock"))
        {
            rigidbody.velocity = Vector3.zero;
            Vector3 forceDirection = new Vector3(1f, 0.1f, 0f);
            rigidbody.AddForce(forceDirection * (pushPower * 0.5f), ForceMode.Impulse); //�������x���Ƃ܂�����������Ȃ��B
            StartCoroutine(ActiveGravityAndAntiGravity(0.2f)); //�ڐA
            healthManager.Damage();
        }

        if (other.gameObject.CompareTag("Stage")
            ||
            other.gameObject.CompareTag("RightDamageBlock")
            ||
            other.gameObject.CompareTag("LeftDamageBlock")
            &&
            !isNotDamage)
        {
            Debug.Log("Stage�Ȃǂɓ������Ă���");
            healthManager.Damage();
        }

        if (other.gameObject.CompareTag("MainNote")
            ||
            other.gameObject.CompareTag("MainNoteLong"))
        {
            isNotDamage = true;
            miniY = 0.85f;

            if (!other.gameObject.GetComponent<NoteController>().isCollision)
            {
                float judgTime = Time.time - JudgmentLineZ.standardTimes[0];

                Debug.Log($"JudgmentZ.standardTimes{JudgmentLineZ.standardTimes[0]}; judgTime{judgTime}");
                other.gameObject.GetComponent<NoteController>().isCollision = true;
                noteType = scoreManager.JudgNoteType(other.gameObject.tag);
                judgment = scoreManager.JudgJudgment(judgTime);
                scoreManager.CalculateScore(noteType, judgment);
            }
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
            other.gameObject.CompareTag("MainNoteLong"))
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
}


// �{�[���𗎂Ƃ��A���ɖ߂��B
// �@ �|�W�V�������w�肵�A�����Ɉړ�������B
// �A ���������Ďw��̈ʒu�Ŏ~�߂�B
// �B �w�肵���ʒu�ɂ����d�͂𔭐�������B
// �C �㉺�Ƀx�N�g����^�������A�{�^���������ƃI���E�I�t���o����B