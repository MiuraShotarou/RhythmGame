using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> bgmClip;
    public List<AudioClip> seClip;
    public AudioSource bgmSource;
    public AudioSource seSource;

    public int noteNum;       //総ノーツ数
    private string songName;  //曲名

    public List<int> LaneNum = new List<int>();                //何番のレーンにノーツが落ちてくるか。
    public List<int> NoteType = new List<int>();               //ノーツの種類
    public List<float> NotesTime = new List<float>();          //ノーツが判定線と重なる時間。
    public List<GameObject> NotesObj = new List<GameObject>(); //ノーツオブジェクトを格納する変数。

    [SerializeField] private float NotesSpeed;                     //ノーツの速度。
    [SerializeField] GameObject notePrefab;                           //ノーツPrefab。

    //public void AudioSelect() //引数を用意
    //{
    //if (i == 0);
    //}
    private void Start()
    {
        StartCoroutine(PlayAudio());
    }
    IEnumerator PlayAudio()
    {
        //if (CountDown <= 0)
        yield return new WaitForSeconds(100f); //3f
        bgmSource.clip = bgmClip[0]; //のちにインデックスは引数で決定する仕様に。
        noteNum = 0;
        songName = "SAIL AWAY";
        Load(songName);
        bgmSource.Play();
    }
    void Load(string SongName)
    {
        string inputString = Resources.Load<TextAsset>(SongName).ToString(); //SongName ← string "テスト";
        Data inputJson = JsonUtility.FromJson<Data>(inputString);            //JsonUtility.FromJson<Data>(inputString);

        noteNum = inputJson.notes.Length;
        for (int i = 0; i < inputJson.notes.Length; i++)
        {
            float kankaku = 60 / (inputJson.BPM * (float)inputJson.notes[i].LPB); //間隔　とは　一分間で何回ビートがあるのか、と 1拍につきいくつのラインがるのか　で掛けた値を60で割ったもの。
            float beatSec = kankaku * (float)inputJson.notes[i].LPB;
            float time = (beatSec * inputJson.notes[i].num / (float)inputJson.notes[i].LPB) + inputJson.offset + 0.01f;
            NotesTime.Add(time);
            LaneNum.Add(inputJson.notes[i].block);
            NoteType.Add(inputJson.notes[i].type);

            float z = NotesTime[i] * NotesSpeed + 1f;
            //Debug.Log(z);
            NotesObj.Add(Instantiate(notePrefab, new Vector3(inputJson.notes[i].block, 0.8f, z), Quaternion.identity));
        }
    }
}
        //それと同時に、ノーツの生成も行うという仕組みに。
[Serializable]
public class Data
{
    public string name;
    public int maxBlock;
    public int BPM;
    public int offset;
    public Note[] notes;

}
[Serializable]
public class Note
{
    public int type; //Type毎の種類分けは絶対に必要である。
    public int num;
    public int block;
    public int LPB;
}
//public class NoteGenerator : AudioManager
//{
//    Dictionary<NoteCategory, float, >;
//    //List<float> noteList
//}