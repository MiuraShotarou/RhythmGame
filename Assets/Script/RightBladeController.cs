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
    Rigidbody rigidbody;

    float slidePower = 700f;

    //bool testBool = false;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
  
        //if (!testBool)
        //{
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, 0.0265f, 0.20404f);
            pos.y = Mathf.Clamp(pos.y, 0.83f, 0.97839f);
            transform.position = pos;
        //}

        if ((transform.position.x == 0.2f && transform.position.y == 0.975f)
            || (transform.position.x == 0.0265f && transform.position.y == 0.83f))
        {
            rigidbody.velocity = Vector3.zero;
        }

        if (Input.GetButtonDown("RightBlade"))
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce((transform.up * -1) * slidePower, ForceMode.Force);
        }
        else if (Input.GetButtonUp("RightBlade"))
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(transform.up * slidePower, ForceMode.Force);
        }
    }

    NoteType noteType;
    Judgment judgment;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"接触する{rigidbody.velocity}");

        if (collision.gameObject.CompareTag("RightNote")
            ||
            collision.gameObject.CompareTag("RightNoteLong"))
        {
            //testBool = true;

            //if (testBool)
            //{
                Vector3 pos = transform.position;
                pos.x = Mathf.Clamp(pos.x, 0.053f, 0.20404f);
                pos.y = Mathf.Clamp(pos.y, 0.86f, 0.853f);
                transform.position = pos;
            //}
            if (!collision.gameObject.GetComponent<NoteController>().isCollision)
            {
                float judgTime = Time.time - JudgmentLineZ.standardTimes[1];
                Debug.Log($"JudgmentZ.standardTimes{JudgmentLineZ.standardTimes[1]}; judgTime{judgTime}");
                collision.gameObject.GetComponent<NoteController>().isCollision = true;
                noteType = scoreManager.JudgNoteType(collision.gameObject.tag);
                judgment = scoreManager.JudgJudgment(judgTime);
                scoreManager.CalculateScore(noteType, judgment); //←Judgment型の変数
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("RightNote")
            ||
            collision.gameObject.CompareTag("RightNoteLong"))
        {
            //if (testBool)
            //{
                Vector3 pos = transform.position;
                pos.x = Mathf.Clamp(pos.x, 0.05375149f, 0.20404f);
                pos.y = Mathf.Clamp(pos.y, 0.86f, 0.85228f);
                transform.position = pos;
            //}
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
            //testBool = false;
            //rigidbody.velocity = Vector3.zero;
            //rigidbody.AddForce((transform.up * -1) * slidePower, ForceMode.Force); //ここでAddforceしているのが良くないかも。
            //StartCoroutine(PositionLog());
        }
    }

    IEnumerator LongNoteManager(GameObject noteLong)
    {
        noteLong.GetComponent<NoteController>().isCollisionStay = true;
        Debug.Log("isCollisionStay = true");
        yield return new WaitForSeconds(0.28f); //判定時間より短くする必要あり。→　構造の思考し直し。
        noteLong.GetComponent<NoteController>().isCollisionStay = false;
        Debug.Log("isCollisionStay = false");
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