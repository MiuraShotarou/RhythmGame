using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum NoteCategory //���g�p
{
    MainNote,         //0
    BlueNote,         //1
    MainNoteLong,     //2
    RightNote,        //3
    RightNoteLong,    //4
    LeftNote,         //5
    LeftNoteLong,     //6
    RightRightNote,   //7
    LeftLeftNote,     //8
}

public enum NoteType
{
    Normal,
    Long,
    Blue
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
    static int _score;
    public static int Score { get { return _score; } set { _score = value; } }

    float ownershipScore; //���U���g��ʁE�Ȃ̃Z���N�g��ʂɎg�p�B

    //�t�B�[���h�ϐ��̒�`
    float _totalScore;
    public float TotalScore //���U���g��ʂɎg�p�B
    {
        get {return _totalScore;}
        set
        {
            if (_totalScore != _totalScore + value)
            {
                _totalScore += value;
                //Debug.Log($"�X�R�A�ɉ��_�Bvalue{value}, totalScore{_totalScore}"); //�X�R�A�������Ɖ��Z����Ă��邩�̊m�F
            }
        }
    }

    float[] noteScore = { 10f, 1f, 12f }; //�������n�m�[�c��10/10 �� 1score / 1second
    float[] judgmentMultiplier = { 0f, 0.8f, 1.0f, 1.2f };
    float[] rankMultiplier = { 0.8f, 1.0f, 1.3f, 1.8f, 2.0f };

    float[] judgmentCounter = new float[4];

    public NoteType JudgNoteType(string tag)
    {
        if (tag.Contains("Blue"))
        {
            return NoteType.Blue;
        }
        else if (!tag.Contains("Long"))
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
        if (judgTime < 0.055f) //0.02f�̊Ԃ�
        {
            return Judgment.Excellent;
        }
        else if (judgTime < 0.085f) //0.05f�̊Ԃ�
        {
            return Judgment.VeryGood;
        }
        else if (judgTime < 0.115f) //0.08f�̊Ԃ�
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
            case NoteType.Blue:
                noteIndex = 2;
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
            TotalScore = noteScore[noteIndex] * judgmentMultiplier[judgmentIndex]; //�X�R�A�̉��Z
            judgmentCounter[judgmentIndex]++; �@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@//�m�[�c�]�����J�e�S���ʂɃJ�E���g����B
            //�m�[�c�]������ʏ�ɕ\������B
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