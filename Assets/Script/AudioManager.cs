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
    List<GameObject> NotesObj = new List<GameObject>(); //ノーツオブジェクトを格納する変数。
    public int memoraizeNoteNum = 0;  

    [SerializeField] private float NotesSpeed;                     //ノーツの速度。
    [SerializeField] GameObject[] notesPrefab = new GameObject[9];                     //ノーツPrefab。

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
        yield return new WaitForSeconds(3f); //3f
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
            GameObject notePrefab = notesPrefab[inputJson.notes[i].block];
            float kankaku = 60 / (inputJson.BPM * (float)inputJson.notes[i].LPB); //間隔　とは　一分間で何回ビートがあるのか、と 1拍につきいくつのラインがるのか　で掛けた値を60で割ったもの。
            float beatSec = kankaku * (float)inputJson.notes[i].LPB;
            float time = (beatSec * inputJson.notes[i].num / (float)inputJson.notes[i].LPB) + inputJson.offset + 0.01f;
            NotesTime.Add(time);
            LaneNum.Add(inputJson.notes[i].block);
            NoteType.Add(inputJson.notes[i].type); //Listに登録しておいて、終点のLongNoteが見つかった場合にそちらをインスタンシエートする。

            float z = NotesTime[i] * NotesSpeed + 1f;

            if (inputJson.notes[i].type == 2
                &&
                memoraizeNoteNum == 0)
            {
                memoraizeNoteNum = inputJson.notes[i].num;
                continue;
            }
            else if (inputJson.notes[i].type == 2
                &&
                memoraizeNoteNum != 0)
            {
                Debug.Log($"i == {i} のbeforeZは{z}です。");
                int longMultiplier = inputJson.notes[i].num - memoraizeNoteNum;
                notePrefab.transform.localScale = new Vector3(notePrefab.transform.localScale.x, notePrefab.transform.localScale.y, notePrefab.transform.localScale.z * longMultiplier);
                Debug.Log($"notePrefab.transform.localScale == {notePrefab.transform.localScale}");
                z = z - (0.015688f * longMultiplier);
                memoraizeNoteNum = 0;
                NotesObj.Add(Instantiate(notePrefab, new Vector3(0, 0.8f, z), Quaternion.identity));
                Debug.Log($"afterZ == {z}です。");
                notePrefab.transform.localScale = new Vector3(notePrefab.transform.localScale.x, notePrefab.transform.localScale.y, notePrefab.transform.localScale.z / longMultiplier);
                continue;
            }
            NotesObj.Add(Instantiate(notePrefab, new Vector3(notePrefab.transform.position.x, notePrefab.transform.position.y, z), notePrefab.transform.rotation));
            Debug.Log($"i == {i} のblockは {inputJson.notes[i].block}, numは{inputJson.notes[i].num}, typは{inputJson.notes[i].type}");
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
    //public int noteLength;
    public Note[] notes;
    // + LPB があるはず。
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