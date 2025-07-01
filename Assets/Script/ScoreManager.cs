using UnityEngine;

public enum NoteCategory //未使用
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
    C,
    None
}

public class ScoreManager : MonoBehaviour
{
    static int[] _judgmentsCounter = new int[4];

    public static int[] JudgmentsCounter { get { return _judgmentsCounter; } set { _judgmentsCounter = value; } }
    
    public float totalScore;
    float[] noteScore = { 10f, 1f, 12f }; //長押し系ノーツは10/10 → 1score / 1second
    float[] judgmentMultiplier = { 0f, 1.0f, 1.2f, 1.4f };

    public GameObject[] judgmentPrefabs;

    GameObject _judgPrefab;

    GameObject JudgPrefab { get { return _judgPrefab; } set { _judgPrefab = value; } }
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
            Debug.LogError("タグで識別できないので、Nomalを返しました。");
            return NoteType.Normal;
        }
    }
    public Judgment JudgJudgment(float judgTime) //基準 0.053 ～ 0.115
    {
        if (judgTime < 0.023f) //0.02fの間に → 0.01
        {
            return Judgment.Excellent;
        }
        else if (judgTime < 0.055f) //0.05fの間に
        {
            return Judgment.VeryGood;
        }
        else if (judgTime < 1f) //0.08fの間に
        {
            return Judgment.Good;
        }
        else
        {
            return Judgment.Miss;
        }
    }
    public void CalculateScore(NoteType noteType, Judgment judgment, GameObject other)
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
            totalScore += noteScore[noteIndex] * judgmentMultiplier[judgmentIndex]; //スコアの加算
            if (!other.GetComponent<NoteController>().IsCollision)
            {
                JudgmentsCounter[judgmentIndex]++;                  //ノーツ評価をカテゴリ別にカウントする。
                other.GetComponent<NoteController>().IsCollision = true;
            }

            if (!JudgPrefab) //null
            {
                JudgPrefab = Instantiate(judgmentPrefabs[judgmentIndex], GameObject.Find("Canvas").transform);
                Destroy(JudgPrefab, 1.35f);
            }
            else if (JudgPrefab)
            {
                JudgPrefab.GetComponent<Animator>().Play("2"); //ここやる
                Destroy(JudgPrefab, 1.35f);
                JudgPrefab = Instantiate(judgmentPrefabs[judgmentIndex], GameObject.Find("Canvas").transform);
                Destroy(JudgPrefab, 1.35f);
            }
            //ノーツ評価を画面上に表示する。
        }
        else
        {
            Debug.Log("Indexが上手く割り当てられていない");
        }

    }
}