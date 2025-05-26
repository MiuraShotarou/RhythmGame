using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

public class BallController : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] HealthManager healthManager;
    Rigidbody rigidbody;
    float pushPower = 3f;
    //returnPower�����B

    bool ischecked = false;
    bool isInvalid = true;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, 0.82f, 100f);
        transform.position = pos;
    }

    private void FixedUpdate()
    {
        if (transform.position.y == 0.9f)
        {
            //rigidbody.velocity = Vector3.zero;
        }

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
            Debug.Log(scoreManager.totalScore);
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

        if (other.gameObject.CompareTag("Stage"))
        {

            healthManager.Damage();
        }

        if (other.gameObject.CompareTag("MainNote"))
        {
            Debug.Log("MainNote");
            //judgment����̏����������BScorManager�ɂ́u����v�̕ϐ��Ɓu�m�[�c�̎�ށv��n���B
            //judgment���� �� judgment ����judgmentType�������ɓ���ă��\�b�h���N���B
            //longNote�̏ꍇ�AStay �֐��ɐ؂�ւ���Ύ��R�ƃX�R�A�̃t���[�����ɂ�������Z���s����B
            //�Ăяo�����\�b�h�ɂ́A�m�[�c�̎�ށA�W���b�W�����g�̎�ނ������ɓn���Ηǂ��B
        }

        Debug.Log(rigidbody.velocity);
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
}


// �{�[���𗎂Ƃ��A���ɖ߂��B
// �@ �|�W�V�������w�肵�A�����Ɉړ�������B
// �A ���������Ďw��̈ʒu�Ŏ~�߂�B
// �B �w�肵���ʒu�ɂ����d�͂𔭐�������B
// �C �㉺�Ƀx�N�g����^�������A�{�^���������ƃI���E�I�t���o����B