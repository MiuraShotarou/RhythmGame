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
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, 0.0265f, 0.20404f);
        pos.y = Mathf.Clamp(pos.y, 0.83f, 0.97839f);
        transform.position = pos;

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
        Debug.Log($"ê⁄êGÇ∑ÇÈ{rigidbody.velocity}");

        if (collision.gameObject.CompareTag("RightNote")
            ||
            collision.gameObject.CompareTag("RightNoteLong"))
        {
            if (!collision.gameObject.GetComponent<NoteController>().isCollision)
            {
                float judgTime = Time.time - JudgmentLineZ.standardTimes[1];
                Debug.Log($"JudgmentZ.standardTimes{JudgmentLineZ.standardTimes[1]}; judgTime{judgTime}");
                collision.gameObject.GetComponent<NoteController>().isCollision = true;
                noteType = scoreManager.JudgNoteType(collision.gameObject.tag);
                judgment = scoreManager.JudgJudgment(judgTime);
                scoreManager.CalculateScore(noteType, judgment); //Å©Judgmentå^ÇÃïœêî
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("RightNote")
            ||
            collision.gameObject.CompareTag("RightNoteLong"))
        { 
            //rigidbody.velocity = Vector3.zero;
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
            //rigidbody.velocity = Vector3.zero;
            //rigidbody.AddForce((transform.up * -1) * slidePower, ForceMode.Force); //Ç±Ç±Ç≈AddforceÇµÇƒÇ¢ÇÈÇÃÇ™ó«Ç≠Ç»Ç¢Ç©Ç‡ÅB
            //StartCoroutine(PositionLog());
        }
    }

    IEnumerator LongNoteManager(GameObject noteLong)
    {
        noteLong.GetComponent<NoteController>().isCollisionStay = true;
        yield return new WaitForSeconds(0.1f);
        noteLong.GetComponent<NoteController>().isCollisionStay = false;
    }

    IEnumerator PositionLog()
    {
        float counter = 0f;
        float countMax = 50f;

        Debug.Log($"ó£ÇÍÇÈ{rigidbody.velocity}");

        while (counter < countMax)
        {
            //Debug.Log($"Position{transform.position}"); //
            //counter++;
        }

        yield return null;
    }
}