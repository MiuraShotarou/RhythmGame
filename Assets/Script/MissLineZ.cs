using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissLineZ : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    private void OnTriggerEnter(Collider other)
    {
        scoreManager.CalculateScore(NoteType.Normal, Judgment.Miss);
        //other.gameObject.SetActive(false);
    }
}
