using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        textMeshProUGUI.text = $"Score {scoreManager.TotalScore}";
    }
}
