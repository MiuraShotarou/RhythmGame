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
    //returnPower検討。

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
            //Debug.Log("Hold呼ばれた");
            rigidbody.AddForce(Vector2.down * 150, ForceMode.Force);
        }
        else if (Input.GetButtonDown("Test")) //試験的
        {
            Debug.Log(scoreManager.totalScore);
        }

        if (transform.position.x == 0
            && isInvalid)
        {
            //Debug.Log("Gravityが戻った");
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
            Debug.Log("接触・RightBlade");
            StartCoroutine(OnBounceBallRightBlade());

            ischecked = true;
        }

        if (other.gameObject.CompareTag("LeftBlock"))
        {
            //ischecked = true;
            rigidbody.velocity = Vector3.zero;
            Vector3 forceDirection = new Vector3(1f, 0.1f, 0f);
            rigidbody.AddForce(forceDirection * (pushPower * 0.5f), ForceMode.Impulse);
            StartCoroutine(ActiveGravityAndAntiGravity(0.1f)); //移植
        }
        
        if (other.gameObject.CompareTag("LeftDamageBlock"))
        {
            rigidbody.velocity = Vector3.zero;
            Vector3 forceDirection = new Vector3(1f, 0.1f, 0f);
            rigidbody.AddForce(forceDirection * (pushPower * 0.5f), ForceMode.Impulse); //同じ速度だとまずいかもしれない。
            StartCoroutine(ActiveGravityAndAntiGravity(0.2f)); //移植
            healthManager.Damage();
        }

        if (other.gameObject.CompareTag("Stage"))
        {

            healthManager.Damage();
        }

        if (other.gameObject.CompareTag("MainNote"))
        {
            Debug.Log("MainNote");
            //judgment判定の処理を書く。ScorManagerには「判定」の変数と「ノーツの種類」を渡す。
            //judgment判定 → judgment 毎にjudgmentTypeを引数に入れてメソッドを起動。
            //longNoteの場合、Stay 関数に切り替えれば自然とスコアのフレーム数における加算が行える。
            //呼び出すメソッドには、ノーツの種類、ジャッジメントの種類を引数に渡せば良い。
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


// ボールを落とし、元に戻す。
// ① ポジションを指定し、そこに移動させる。
// ② 減速させて指定の位置で止める。
// ③ 指定した位置にだけ重力を発生させる。
// ④ 上下にベクトルを与え続け、ボタンを押すとオン・オフが出来る。