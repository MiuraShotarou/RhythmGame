using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    void FixedUpdate()
    {
        string showScore = scoreManager.totalScore.ToString("F1");
        textMeshProUGUI.text = $"Score {showScore}";
    }
}
