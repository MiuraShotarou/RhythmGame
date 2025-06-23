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

    [SerializeField] GameObject tutorialPanel;
    [SerializeField] GameObject greatGenerator;

    public void Play()
    {
        //if (CountDown <= 0)
        //noteNum = 0;
        Debug.Log(GameManager.SelectedBGMIndex);
        string songName = GameManager.BGMClip[GameManager.SelectedBGMIndex].name;
        GameManager.IsInvalid = true;
        if (GameManager.SelectedBGMIndex != 0)
        {
            PlayBGM(songName);
        }
        else if (GameManager.SelectedBGMIndex == 0)
        {
            GameManager.IsTutorial = true; //戻すの忘れない
            songName = $"{songName} Tutorial";
            StartCoroutine(PlayTutorial(songName));
        }
    }
    void PlayBGM(string songName)
    {
        GameManager.BGMSource.clip = GameManager.BGMClip[GameManager.SelectedBGMIndex]; //のちにインデックスは引数で決定する仕様に。
        GameManager.BGMSource.Play();

        string inputString = Resources.Load<TextAsset>(songName).ToString(); //SongName ← string "テスト";
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

    IEnumerator PlayTutorial(string songName)
    {
        GameObject tutorialPage1 = tutorialPanel.transform.GetChild(0).gameObject;
        tutorialPage1.transform.localScale = new Vector3(1, 0, 1);
        tutorialPanel.SetActive(true);
        tutorialPage1.SetActive(true);
        GameManager.SESource.clip = GameManager.SEClip[3];
        GameManager.SESource.Play();
        yield return StartCoroutine(ActiveTutorialPanel(tutorialPage1, 0.5f));
        StartCoroutine(ExpandTutorialPanel(tutorialPage1, 0.5f));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        yield return StartCoroutine(AnExpandTutorialPanel(tutorialPage1, 0.5f));
        yield return StartCoroutine(AnActiveTutorialPanel(tutorialPage1, 0.5f));
        GameManager.IsInvalid = false;
        GameObject tutorialPage2 = tutorialPanel.transform.GetChild(1).gameObject;
        tutorialPage2.transform.localScale = new Vector3(1, 0, 1);
        tutorialPage2.SetActive(true);
        GameManager.SESource.clip = GameManager.SEClip[4];
        GameManager.SESource.Play();
        yield return StartCoroutine(ActiveTutorialPanel(tutorialPage2, 0.5f));
        StartCoroutine(ExpandTutorialPanel(tutorialPage2, 0.5f));

        yield return StartCoroutine(NoteGenerator(0, 5f));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));//

    }

    //    yield return new WaitForSeconds(10f);

    //    GameManager.BGMSource.clip = GameManager.BGMClip[GameManager.SelectedBGMIndex]; //のちにインデックスは引数で決定する仕様に。
    //    GameManager.BGMSource.Play();

    //    string inputString = Resources.Load<TextAsset>(songName).ToString(); //SongName ← string "テスト";
    //    Data inputJson = JsonUtility.FromJson<Data>(inputString);            //JsonUtility.FromJson<Data>(inputString);

    //    noteNum = inputJson.notes.Length;
    //    for (int i = 0; i < inputJson.notes.Length; i++)
    //    {
    //        GameObject notePrefab = notesPrefab[inputJson.notes[i].block];
    //        float kankaku = 60 / (inputJson.BPM * (float)inputJson.notes[i].LPB); //間隔　とは　一分間で何回ビートがあるのか、と 1拍につきいくつのラインがるのか　で掛けた値を60で割ったもの。
    //        float beatSec = kankaku * (float)inputJson.notes[i].LPB;
    //        float time = (beatSec * inputJson.notes[i].num / (float)inputJson.notes[i].LPB) + inputJson.offset + 0.01f;
    //        NotesTime.Add(time);
    //        LaneNum.Add(inputJson.notes[i].block);
    //        NoteType.Add(inputJson.notes[i].type); //Listに登録しておいて、終点のLongNoteが見つかった場合にそちらをインスタンシエートする。

    //        float z = NotesTime[i] * NotesSpeed + 0.8f;

    //        if (inputJson.notes[i].type == 2
    //            &&
    //            memoraizeNoteNum == 0)
    //        {
    //            memoraizeNoteNum = inputJson.notes[i].num;
    //            continue;
    //        }
    //        else if (inputJson.notes[i].type == 2
    //            &&
    //            memoraizeNoteNum != 0)
    //        {
    //            Debug.Log($"i == {i} のbeforeZは{z}です。");
    //            int longMultiplier = inputJson.notes[i].num - memoraizeNoteNum;
    //            notePrefab.transform.localScale = new Vector3(notePrefab.transform.localScale.x, notePrefab.transform.localScale.y, notePrefab.transform.localScale.z * longMultiplier);
    //            Debug.Log($"notePrefab.transform.localScale == {notePrefab.transform.localScale}");
    //            z = z - (0.015688f * longMultiplier);
    //            memoraizeNoteNum = 0;
    //            NotesObj.Add(Instantiate(notePrefab, new Vector3(0, 0.8f, z), Quaternion.identity));
    //            Debug.Log($"afterZ == {z}です。");
    //            notePrefab.transform.localScale = new Vector3(notePrefab.transform.localScale.x, notePrefab.transform.localScale.y, notePrefab.transform.localScale.z / longMultiplier);
    //            continue;
    //        }
    //        NotesObj.Add(Instantiate(notePrefab, new Vector3(notePrefab.transform.position.x, notePrefab.transform.position.y, z), notePrefab.transform.rotation));
    //        Debug.Log($"i == {i} のblockは {inputJson.notes[i].block}, numは{inputJson.notes[i].num}, typは{inputJson.notes[i].type}");
    //    }
    //}
    IEnumerator ActiveTutorialPanel(GameObject gameObject, float duration)
    {
        {
            float a = 0;
            float timer = 0;
            float startTime = Time.time; //ゲーム開始からの時間　仮：1min →　新time - imin
            Color roopColor = gameObject.GetComponent<Image>().color;

            while (timer <= duration) //時間で振動の切り上げ
            {
                roopColor.a = a;
                gameObject.GetComponent<Image>().color = roopColor;

                timer = Time.time - startTime;
                a = Mathf.Lerp(0, 1, timer / duration);
                yield return null;
            }
        }
    }
    IEnumerator ExpandTutorialPanel(GameObject gameObject, float duration)
    {
        {
            float scaleY = 0;
            float timer = 0;
            float startTime = Time.time; //ゲーム開始からの時間　仮：1min →　新time - imin
            float roopScaleY = 0;

            while (timer <= duration) //時間で振動の切り上げ
            {
                gameObject.transform.localScale = new Vector3(1, roopScaleY, 1);

                timer = Time.time - startTime;
                roopScaleY = Mathf.Lerp(0, 1, timer / duration);
                yield return null;
            }
        }
    }
    IEnumerator AnActiveTutorialPanel(GameObject gameObject, float duration)
    {
        {
            float a = 1;
            float timer = 0;
            float startTime = Time.time; //ゲーム開始からの時間　仮：1min →　新time - imin
            Color roopColor = gameObject.GetComponent<Image>().color;

            while (timer <= duration) //時間で振動の切り上げ
            {
                roopColor.a = a;
                gameObject.GetComponent<Image>().color = roopColor;

                timer = Time.time - startTime;
                a = Mathf.Lerp(1, 0, timer / duration);
                yield return null;
            }
        }
    }
    IEnumerator AnExpandTutorialPanel(GameObject gameObject, float duration)
    {
        {
            float scaleY = 1;
            float timer = 0;
            float startTime = Time.time; //ゲーム開始からの時間　仮：1min →　新time - imin
            float roopScaleY = 0;

            while (timer <= duration) //時間で振動の切り上げ
            {
                gameObject.transform.localScale = new Vector3(1, roopScaleY, 1);

                timer = Time.time - startTime;
                roopScaleY = Mathf.Lerp(1, 0, timer / duration);
                yield return null;
            }
        }
    }

    IEnumerator NoteGenerator(int notePrefabInedex, float interval)
    {
        GameManager.IsLoop = true;

        while (GameManager.IsLoop)
        {
            GameObject noteObject = Instantiate(notesPrefab[notePrefabInedex], new Vector3(0, 0.8f, 10), Quaternion.identity);
            Destroy(noteObject, 5.5f);
            yield return new WaitForSeconds(interval);
        }
        Instantiate(greatGenerator, tutorialPanel.transform);
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