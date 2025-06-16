using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum NoteCategory //未使用
{
    MainNote,
    MainNoteLong,
    RightNote,
    RightNoteLong,
    LeftNote,
    LeftNoteLong,
    RightRightNote,
    LeftLeftNote,
    BlueNote
}

public enum NoteType
{
    Normal,
    Long
}
public enum Judgment
{
    Excellent,
    VeryGood,
    Good,
    Miss
}
public enum Rank
{
    SSS,
    S,
    A,
    B,
    C
}

public class ScoreManager : MonoBehaviour
{
    float ownershipScore; //リザルト画面・曲のセレクト画面に使用。

    //フィールド変数の定義
    float _totalScore;
    public float TotalScore //リザルト画面に使用。
    {
        get {return _totalScore;}
        set
        {
            if (_totalScore != _totalScore + value)
            {
                _totalScore += value;
                //Debug.Log($"スコアに加点。value{value}, totalScore{_totalScore}"); //スコアがちゃんと加算されているかの確認
            }
        }
    }

    float[] noteScore = { 10f, 1f }; //長押し系ノーツは10/10 → 1score / 1second
    float[] judgmentMultiplier = { 0f, 0.8f, 1.0f, 1.2f };
    float[] rankMultiplier = { 0.8f, 1.0f, 1.3f, 1.8f, 2.0f };

    float[] judgmentCounter = new float[4];

    public NoteType JudgNoteType(string tag)
    {
        if (!tag.Contains("Long"))
        {
            return NoteType.Normal;
        }
        else if (tag.Contains("Long"))
        {
            return NoteType.Long;
        }
        else
        {
            Debug.LogError("タグで識別できないので、Nomalを返しました。");
            return NoteType.Normal;
        }
    }
    public Judgment JudgJudgment(float judgTime)
    {
        if (judgTime < 0.055f) //0.02fの間に
        {
            return Judgment.Excellent;
        }
        else if (judgTime < 0.085f) //0.05fの間に
        {
            return Judgment.VeryGood;
        }
        else if (judgTime < 0.115f) //0.08fの間に
        {
            return Judgment.Good;
        }
        else
        {
            return Judgment.Miss;
        }
    }
    public void CalculateScore(NoteType noteType, Judgment judgment)
    {
        int noteIndex = -1;
        int judgmentIndex = -1;

        switch (noteType)
        {
            case NoteType.Normal:
                noteIndex = 0;
                break;
            case NoteType.Long:
                noteIndex = 1;
                break;
        }
        switch (judgment)
        {
            case Judgment.Miss:
                judgmentIndex = 0;
                break;
            case Judgment.Good:
                judgmentIndex = 1;
                break;
            case Judgment.VeryGood:
                judgmentIndex = 2;
                break;
            case Judgment.Excellent:
                judgmentIndex = 3;
                break;
        }

        if (noteIndex != -1
            && judgmentIndex != -1)
        {
            TotalScore = noteScore[noteIndex] * judgmentMultiplier[judgmentIndex]; //スコアの加算
            judgmentCounter[judgmentIndex]++; 　　　　　　　　　　　　　　　　　　　//ノーツ評価をカテゴリ別にカウントする。
        }
        else
        {
            Debug.Log("Indexが上手く割り当てられていない");
        }
    }
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    float deltaTime = 0.0f;
    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }
    void OnGUI() //知らない関数
    {
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.} FPS", fps);
        GUI.Label(new Rect(10, 10, 100, 25), text);
    }
}