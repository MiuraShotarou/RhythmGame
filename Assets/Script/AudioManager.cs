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

    [SerializeField] InGameManager inGameManager;
    [SerializeField] Rigidbody rightBladeRigidbody;
    [SerializeField] Rigidbody leftBladeRigidbody;
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] GameObject greatGenerator;
    [SerializeField] GameObject spaceKeyUI;
    [SerializeField] GameObject rKeyUI;
    Coroutine activeSpaceKeyUI;
    Coroutine activeRKeyUI;

    Func<IEnumerator> tutorialJudg;
    Func<string, IEnumerator> playOrReturn;
    public void Play()
    {
        //if (CountDown <= 0)
        //noteNum = 0;
        Debug.Log(GameManager.SelectedBGMIndex);
        string songName = GameManager.BGMClip[GameManager.SelectedBGMIndex].name;
        GameManager.IsInvalid = true;
        rightBladeRigidbody.isKinematic = true;
        leftBladeRigidbody.isKinematic = true;
        if (!GameManager.IsTutorial)
        {
            PlayBGM(songName);
        }
        else if (GameManager.IsTutorial)
        {
            StartCoroutine(PlayTutorial());
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

    IEnumerator PlayTutorial()
    {
        tutorialJudg = BGMChoice;
        tutorialJudg += ReturnChoice;
        playOrReturn = ChoiceYes;
        playOrReturn += ChoiceNo;

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
        tutorialPage1.SetActive(false);
        GameManager.IsInvalid = false;
        GameObject tutorialPage2 = tutorialPanel.transform.GetChild(1).gameObject;
        tutorialPage2.transform.localScale = new Vector3(1, 0, 1);
        tutorialPage2.SetActive(true);
        GameManager.SESource.clip = GameManager.SEClip[4];
        GameManager.SESource.Play();
        yield return StartCoroutine(ActiveTutorialPanel(tutorialPage2, 0.5f));
        StartCoroutine(ExpandTutorialPanel(tutorialPage2, 0.5f));

        yield return StartCoroutine(NoteGenerator(0, 1f, 1));
        GameManager.IsInvalid = true;
        yield return new WaitForSeconds(1.33f);
        NoteController[] noteControllers = GameObject.FindObjectsOfType<NoteController>();
        foreach (NoteController noteController in noteControllers)
        {
            Destroy(noteController.gameObject);
        }
        yield return StartCoroutine(AnExpandTutorialPanel(tutorialPage2, 0.5f));
        yield return StartCoroutine(AnActiveTutorialPanel(tutorialPage2, 0.5f));
        tutorialPage2.SetActive(false);
        GameObject tutorialPage3 = tutorialPanel.transform.GetChild(2).gameObject;
        tutorialPage3.transform.localScale = new Vector3(1, 0, 1);
        tutorialPage3.SetActive(true);
        GameManager.SESource.clip = GameManager.SEClip[4];
        GameManager.SESource.Play();
        yield return StartCoroutine(ActiveTutorialPanel(tutorialPage3, 0.5f));
        StartCoroutine(ExpandTutorialPanel(tutorialPage3, 0.5f));
        yield return new WaitForSeconds(2f);
        activeSpaceKeyUI = StartCoroutine(ActiveKeyUI(spaceKeyUI, 2f));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));//
        StopCoroutine(activeSpaceKeyUI);
        spaceKeyUI.SetActive(false);
        yield return StartCoroutine(AnExpandTutorialPanel(tutorialPage3, 0.5f));
        yield return StartCoroutine(AnActiveTutorialPanel(tutorialPage3, 0.5f));
        tutorialPage3.SetActive(false);
        GameObject tutorialPage4 = tutorialPanel.transform.GetChild(3).gameObject;
        tutorialPage4.transform.localScale = new Vector3(1, 0, 1);
        tutorialPage4.SetActive(true);
        GameManager.SESource.clip = GameManager.SEClip[4];
        GameManager.SESource.Play();
        yield return StartCoroutine(ActiveTutorialPanel(tutorialPage4, 0.5f));
        StartCoroutine(ExpandTutorialPanel(tutorialPage4, 0.5f));
        yield return new WaitForSeconds(2f);
        activeSpaceKeyUI = StartCoroutine(ActiveKeyUI(spaceKeyUI, 2f));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));//
        StopCoroutine(activeSpaceKeyUI);
        spaceKeyUI.SetActive(false);
        yield return StartCoroutine(AnExpandTutorialPanel(tutorialPage4, 0.5f));
        yield return StartCoroutine(AnActiveTutorialPanel(tutorialPage4, 0.5f));
        tutorialPage4.SetActive(false);
        GameObject tutorialPage5 = tutorialPanel.transform.GetChild(4).gameObject;
        tutorialPage5.transform.localScale = new Vector3(1, 0, 1);
        tutorialPage5.SetActive(true);
        GameManager.SESource.clip = GameManager.SEClip[4];
        GameManager.SESource.Play();
        yield return StartCoroutine(ActiveTutorialPanel(tutorialPage5, 0.5f));
        StartCoroutine(ExpandTutorialPanel(tutorialPage5, 0.5f));
        GameManager.IsInvalid = false;
        yield return StartCoroutine(NoteGenerator(1, 1f, 1));
        GameManager.IsInvalid = true;
        yield return new WaitForSeconds(1.33f);
        NoteController[] noteControllers2 = GameObject.FindObjectsOfType<NoteController>();
        foreach (NoteController noteController in noteControllers2)
        {
            Destroy(noteController.gameObject);
        }
        yield return StartCoroutine(AnExpandTutorialPanel(tutorialPage5, 0.5f));
        yield return StartCoroutine(AnActiveTutorialPanel(tutorialPage5, 0.5f));
        tutorialPage5.SetActive(false);
        GameObject tutorialPage6 = tutorialPanel.transform.GetChild(5).gameObject;
        tutorialPage6.transform.localScale = new Vector3(1, 0, 1);
        tutorialPage6.SetActive(true);
        GameManager.SESource.clip = GameManager.SEClip[4];
        GameManager.SESource.Play();
        yield return StartCoroutine(ActiveTutorialPanel(tutorialPage6, 0.5f));
        StartCoroutine(ExpandTutorialPanel(tutorialPage6, 0.5f));
        GameManager.IsInvalid = false;
        rightBladeRigidbody.isKinematic = false;
        yield return StartCoroutine(NoteGenerator(4, 2f, 100));
        rightBladeRigidbody.isKinematic = true;
        GameManager.IsInvalid = true;
        yield return new WaitForSeconds(1.33f);
        NoteController[] noteControllers3 = GameObject.FindObjectsOfType<NoteController>();
        foreach (NoteController noteController in noteControllers3)
        {
            Destroy(noteController.gameObject);
        }
        yield return StartCoroutine(AnExpandTutorialPanel(tutorialPage6, 0.5f));
        yield return StartCoroutine(AnActiveTutorialPanel(tutorialPage6, 0.5f));
        tutorialPage6.SetActive(false);
        GameObject tutorialPage7 = tutorialPanel.transform.GetChild(6).gameObject;
        tutorialPage7.transform.localScale = new Vector3(1, 0, 1);
        tutorialPage7.SetActive(true);
        GameManager.SESource.clip = GameManager.SEClip[4];
        GameManager.SESource.Play();
        yield return StartCoroutine(ActiveTutorialPanel(tutorialPage7, 0.5f));
        StartCoroutine(ExpandTutorialPanel(tutorialPage7, 0.5f));
        RightBladeController rightBladeController = rightBladeRigidbody.gameObject.GetComponent<RightBladeController>();
        LeftBladeController leftBladeController = leftBladeRigidbody.gameObject.GetComponent<LeftBladeController>();
        rightBladeController.isSuccessInvalid = true;
        leftBladeController.isSuccessInvalid = true;
        GameManager.IsInvalid = false;
        rightBladeRigidbody.isKinematic = false;
        StartCoroutine(NoteGenerator(4, 2f, 200));
        yield return StartCoroutine(NoteGenerator(8, 2f, 1));
        rightBladeController.isSuccessInvalid = false;
        leftBladeController.isSuccessInvalid = false;
        rightBladeRigidbody.isKinematic = true;
        GameManager.IsInvalid = true;
        yield return new WaitForSeconds(1.33f);
        NoteController[] noteControllers4 = GameObject.FindObjectsOfType<NoteController>();
        foreach (NoteController noteController in noteControllers4)
        {
            Destroy(noteController.gameObject);
        }
        yield return StartCoroutine(AnExpandTutorialPanel(tutorialPage7, 0.5f));
        yield return StartCoroutine(AnActiveTutorialPanel(tutorialPage7, 0.5f));
        tutorialPage7.SetActive(false);
        GameObject tutorialPage8 = tutorialPanel.transform.GetChild(7).gameObject;
        tutorialPage8.transform.localScale = new Vector3(1, 0, 1);
        tutorialPage8.SetActive(true);
        GameManager.SESource.clip = GameManager.SEClip[4];
        GameManager.SESource.Play();
        yield return StartCoroutine(ActiveTutorialPanel(tutorialPage8, 0.5f));
        StartCoroutine(ExpandTutorialPanel(tutorialPage8, 0.5f));
        yield return new WaitForSeconds(2f);
        activeSpaceKeyUI = StartCoroutine(ActiveKeyUI(spaceKeyUI, 2f));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));//
        StopCoroutine(activeSpaceKeyUI);
        spaceKeyUI.SetActive(false);
        yield return StartCoroutine(AnExpandTutorialPanel(tutorialPage8, 0.5f));
        yield return StartCoroutine(AnActiveTutorialPanel(tutorialPage8, 0.5f));
        tutorialPage8.SetActive(false);
        GameObject tutorialPage9 = tutorialPanel.transform.GetChild(8).gameObject;
        tutorialPage9.transform.localScale = new Vector3(1, 0, 1);
        tutorialPage9.SetActive(true);
        GameManager.SESource.clip = GameManager.SEClip[4];
        GameManager.SESource.Play();
        yield return StartCoroutine(ActiveTutorialPanel(tutorialPage9, 0.5f));
        StartCoroutine(ExpandTutorialPanel(tutorialPage9, 0.5f));
        yield return new WaitForSeconds(2f);
        activeSpaceKeyUI = StartCoroutine(ActiveKeyUI(spaceKeyUI, 2f));
        activeRKeyUI = StartCoroutine(ActiveKeyUI(rKeyUI, 2f));

        StartCoroutine(tutorialJudg());
        GameManager.IsTutorial = false;
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

    IEnumerator NoteGenerator(int notePrefabInedex, float interval, float scaleMultiplier) //4 8 
    {
        GameManager.IsLoop = true;
        int i = 0;

        while (GameManager.IsLoop)
        {
            if (i == 0)
            {
                if (notePrefabInedex != 4
                    &&
                    notePrefabInedex != 6)
                {
                    GameObject noteObject = Instantiate(notesPrefab[notePrefabInedex]);
                    noteObject.transform.position = new Vector3(notesPrefab[notePrefabInedex].transform.position.x, notesPrefab[notePrefabInedex].transform.position.y, 10);
                    Destroy(noteObject, 5.5f);
                }
                else if (notePrefabInedex == 4
                        ||
                        notePrefabInedex == 6)
                {
                    Debug.Log(notePrefabInedex);
                    GameObject noteObject = Instantiate(notesPrefab[notePrefabInedex], new Vector3(0, 0.8f, 10), Quaternion.identity);
                    noteObject.transform.localScale = new Vector3(noteObject.transform.localScale.x, noteObject.transform.localScale.y, noteObject.transform.localScale.z * scaleMultiplier);
                    Destroy(noteObject, 7f);
                }
            }

            i++;

            if (i > 4)
            {
                i = 0;
            }
            yield return new WaitForSeconds(interval);
        }
        yield return new WaitForSeconds(2f);
        Instantiate(greatGenerator, tutorialPanel.transform);
    }
    IEnumerator ActiveKeyUI(GameObject keyUI, float interval)
    {
        Color loopColor = keyUI.GetComponent<Image>().color;
        bool isChange = false;
        keyUI.SetActive(true);
        while (true)
        {
            if (isChange)
            {
                loopColor.a = 0.5882352941176471f;
                keyUI.GetComponent<Image>().color = loopColor;
                isChange = false;
            }
            else
            {
                loopColor.a = 1;
                keyUI.GetComponent<Image>().color = loopColor;
                isChange = true;
            }
            yield return new WaitForSeconds(interval);
        }
    }
    IEnumerator BGMChoice()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        GameObject tutorialPage9 = tutorialPanel.transform.GetChild(8).gameObject; //曲プレイ用の確認画面を開く(Textの書き換え処理)
        tutorialPage9.transform.localScale = new Vector3(1, 0, 1);
        tutorialPage9.SetActive(true);
        GameManager.SESource.clip = GameManager.SEClip[4];                         //SE変える
        GameManager.SESource.Play();
        yield return StartCoroutine(ActiveTutorialPanel(tutorialPage9, 0.5f));
        StartCoroutine(ExpandTutorialPanel(tutorialPage9, 0.5f));

        StartCoroutine(playOrReturn("Play"));
        StopCoroutine(tutorialJudg()); //BGMChoice() , ReturnChoice　が停止する。
    }

    IEnumerator ReturnChoice()//
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.R));
        GameObject tutorialPage9 = tutorialPanel.transform.GetChild(8).gameObject; //確認画面を開く
        tutorialPage9.transform.localScale = new Vector3(1, 0, 1);
        tutorialPage9.SetActive(true);
        GameManager.SESource.clip = GameManager.SEClip[4];                         //SE変える
        GameManager.SESource.Play();
        yield return StartCoroutine(ActiveTutorialPanel(tutorialPage9, 0.5f));
        StartCoroutine(ExpandTutorialPanel(tutorialPage9, 0.5f));

        StartCoroutine(playOrReturn("Return"));
        StopCoroutine(tutorialJudg()); //BGMChoice() , ReturnChoice　が停止する。
    }

    IEnumerator ChoiceYes(string panelType)//
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        if (panelType == "Play")
        {
            GameManager.SESource.clip = GameManager.SEClip[4];                         //SE変える
            GameManager.SESource.Play();
            inGameManager.BlackOut("Play");
            StopCoroutine(playOrReturn("Play"));
        }
        else if (panelType == "Return")
        {
            GameManager.SESource.clip = GameManager.SEClip[4];                         //SE変える(もにゅ)
            GameManager.SESource.Play();
            inGameManager.BlackOut("ReturnTitle");
            StopCoroutine(playOrReturn("Return"));
        }
    }
    IEnumerator ChoiceNo(string panelType)//
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.R));

        if (panelType == "Play")
        {
            GameObject tutorialPage9 = tutorialPanel.transform.GetChild(8).gameObject; //曲プレイ用の認画面を閉じる(Textの書き換え)
            GameManager.SESource.clip = GameManager.SEClip[4];                         //SE変える
            GameManager.SESource.Play();
            yield return StartCoroutine(AnActiveTutorialPanel(tutorialPage9, 0.5f));
            StartCoroutine(AnExpandTutorialPanel(tutorialPage9, 0.5f));
            StopCoroutine(playOrReturn("Play"));
        }
        else if (panelType == "Return")
        {
            GameObject tutorialPage9 = tutorialPanel.transform.GetChild(8).gameObject; //確認画面を閉じる
            GameManager.SESource.clip = GameManager.SEClip[4];                         //SE変える
            GameManager.SESource.Play();
            yield return StartCoroutine(AnActiveTutorialPanel(tutorialPage9, 0.5f));
            StartCoroutine(AnExpandTutorialPanel(tutorialPage9, 0.5f));
            StopCoroutine(playOrReturn("Retrun"));
        }

        StartCoroutine(tutorialJudg());
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