using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("RightNote")
        //|| other.gameObject.CompareTag("RightNoteLong")
        )
        {
            float judgTime = Time.time - JudgmentZ.standardTimes[1];
            Debug.Log($"JudgmentZ.standardTimes{JudgmentZ.standardTimes[1]}; judgTime{judgTime}");
            //judgment判定
            //RightNoteが基準点に触れた時間を、別のクラスから取得する。
            //RgihtNoteと接触した時間を記録。
            //基準となる時間 - 触れた時の時間 を絶対値に変換。
            //
            scoreManager.CalculateScore(NoteType.Normal, Judgment.Excellent);
        }
    }
}