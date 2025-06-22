using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    int noteNum;       //総ノーツ数
 
    List<int> LaneNum = new List<int>();                //何番のレーンにノーツが落ちてくるか。
    List<int> NoteType = new List<int>();               //ノーツの種類
    List<float> NotesTime = new List<float>();          //ノーツが判定線と重なる時間。
    List<GameObject> NotesObj = new List<GameObject>(); //ノーツオブジェクトを格納する変数。

    int memoraizeNoteNum = 0;  
    [SerializeField] private float NotesSpeed;                     //ノーツの速度。
    [SerializeField] GameObject[] notesPrefab = new GameObject[9];                     //ノーツPrefab。

    public IEnumerator Play()
    {
        //if (CountDown <= 0)
        //noteNum = 0;
        string songName = GameManager.BGMClip[GameManager.SelectedBGMIndex].name;
        if (GameManager.SelectedBGMIndex == 0)
        {
            songName = $"{songName} Tutorial";
        }
        Debug.Log($"songName{songName}");
        PlayBGM(songName);
        yield break;
    }
    void PlayBGM(string SongName)
    {
        GameManager.BGMSource.clip = GameManager.BGMClip[GameManager.SelectedBGMIndex]; //のちにインデックスは引数で決定する仕様に。
        GameManager.BGMSource.Play();

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

            float z = NotesTime[i] * NotesSpeed + 0.8f;

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