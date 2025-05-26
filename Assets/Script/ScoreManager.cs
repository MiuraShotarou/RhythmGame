using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    float ownershipScore; //���U���g��ʁE�Ȃ̃Z���N�g��ʂɎg�p�B

    public float totalScore; //���U���g��ʂɎg�p�B

    float[] noteScore = { 10f, 1f }; //�������n�m�[�c��10/10 �� 1score / 1second
    float[] judgmentMultiplier = { 0f, 0.5f, 1.0f, 1.5f };
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
            Debug.LogError("�^�O�Ŏ��ʂł��Ȃ��̂ŁANomal��Ԃ��܂����B");
            return NoteType.Normal;
        }
    }
    public Judgment JudgJudgment(float judgTime)
    {
        if (judgTime < 0.1f)
        {
            return Judgment.Excellent;
        }
        else if (judgTime < 0.2f)
        {
            return Judgment.VeryGood;
        }
        else if (judgTime < 0.3f)
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
        Debug.Log("CalculateScore���Ă΂�Ă���B");
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
            totalScore += noteScore[noteIndex] * judgmentMultiplier[judgmentIndex]; //�X�R�A�̉��Z
            judgmentCounter[judgmentIndex]++; �@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@//�m�[�c�]�����J�e�S���ʂɃJ�E���g����B
        }
        else
        {
            Debug.Log("Index����肭���蓖�Ă��Ă��Ȃ�");
        }
    }
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI() //�m��Ȃ��֐�
    {
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.} FPS", fps);
        GUI.Label(new Rect(10, 10, 100, 25), text);
    }
}