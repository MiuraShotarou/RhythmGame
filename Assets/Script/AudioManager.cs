using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    int noteNum;       //総ノーツ数

    List<int> countNoteType = new List<int>();                //何番のレーンにノーツが落ちてくるか。
    List<int> NoteType = new List<int>();               //ノーツの種類
    List<float> NotesTime = new List<float>();          //ノーツが判定線と重なる時間。
    List<GameObject> NotesObj = new List<GameObject>(); //総ノーツ数

    int memoraizeNoteNumR = 0;
    int memoraizeNoteNumL = 0;
    [SerializeField] private float NotesSpeed;                     //ノーツの速度。
    [SerializeField] GameObject[] notesPrefab = new GameObject[9];                     //ノーツPrefab。
    [SerializeField] GameObject rightNoteEffect;
    [SerializeField] GameObject leftNoteEffect;

    [SerializeField] InGameManager inGameManager;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] Rigidbody rightBladeRigidbody;
    [SerializeField] Rigidbody leftBladeRigidbody;
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] GameObject greatGenerator;
    [SerializeField] GameObject spaceKeyUI;
    [SerializeField] GameObject spaceKeyUI2;
    [SerializeField] GameObject rKeyUI;
    [SerializeField] GameObject fSpaceKeyUI;
    [SerializeField] GameObject fRKeyUI;
    [SerializeField] GameObject countDownPrefab;
    [SerializeField] GameObject startPrefab;
    [SerializeField] GameObject completedPrefab;
    [SerializeField] GameObject resultPanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject[] rulePages;
    [SerializeField] GameObject scoreTextObject;
    [SerializeField] GameObject canvas;

    [SerializeField] GameObject[] resultTextObjects = new GameObject[11];
    [SerializeField] GameObject[] rankImageObjects = new GameObject[5];

    Coroutine activeSpaceKeyUI;
    Coroutine activeRKeyUI;

    Coroutine[] tutorialJudgs = new Coroutine[2];
    Coroutine[] returnOrStay = new Coroutine[2];

    static int _redCount;
    static int _buleCount;
    static int _yellowCount;
    static bool _isTutorialPause;

    public static int RedCount { get { return _redCount; } set { _redCount = value; } }
    public static int BlueCount { get { return _buleCount; } set { _buleCount = value; } }
    public static int YellowCount { get { return _yellowCount; } set { _yellowCount = value; } }
    public static bool IsTutorialPause { get { return _isTutorialPause ; } set {  _isTutorialPause = value; } }

    public void Play()
    {
        string songName = GameManager.BGMClip[GameManager.SelectedBGMIndex].name;
        if (!GameManager.IsTutorial)
        {
            StartCoroutine(PlayBGM(songName));
            StartCoroutine(ShowResult());
        }
        else if (GameManager.IsTutorial)
        {
            StartCoroutine(PlayTutorial());
        }
    }
    IEnumerator PlayBGM(string songName)
    {
        yield return StartCoroutine(CountDown());
        GameManager.BGMSource.clip = GameManager.BGMClip[GameManager.SelectedBGMIndex];
        GameManager.BGMSource.Play();

        string inputString = Resources.Load<TextAsset>(songName).ToString();
        Data inputJson = JsonUtility.FromJson<Data>(inputString);            //JsonUtility.FromJson<Data>(inputString);

        noteNum = inputJson.notes.Length;
        for (int i = 0; i < inputJson.notes.Length; i++)
        {
            GameObject notePrefab = notesPrefab[inputJson.notes[i].block];
            float kankaku = 60 / (inputJson.BPM * (float)inputJson.notes[i].LPB); //間隔　とは　一分間で何回ビートがあるのか、と 1拍につきいくつのラインがるのか　で掛けた値を60で割ったもの。
            float beatSec = kankaku * (float)inputJson.notes[i].LPB;
            float time = (beatSec * inputJson.notes[i].num / (float)inputJson.notes[i].LPB) + inputJson.offset + 0.01f;
            NotesTime.Add(time);
            countNoteType.Add(inputJson.notes[i].block);
            NoteType.Add(inputJson.notes[i].type);                                //Listに登録しておいて、終点のLongNoteが見つかった場合にそちらをインスタンシエートする。
            float z = NotesTime[i] * NotesSpeed + 0.5f;

            if (inputJson.notes[i].type == 2
                &&
                inputJson.notes[i].block == 4
                &&
                memoraizeNoteNumR == 0)
            {
                memoraizeNoteNumR = inputJson.notes[i].num;
                continue;
            }
            else if (inputJson.notes[i].type == 2
                &&
                inputJson.notes[i].block == 6
                &&
                memoraizeNoteNumL == 0)
            {
                memoraizeNoteNumL = inputJson.notes[i].num;
                continue;
            }
            else if (inputJson.notes[i].type == 2
                &&
                inputJson.notes[i].block == 4
                &&
                memoraizeNoteNumR != 0)
            {
                GameObject effectObject;

                int longMultiplier = inputJson.notes[i].num - memoraizeNoteNumR;
                memoraizeNoteNumR = 0;
                z = z - (0.015688f * longMultiplier);
                GameObject longNote = Instantiate(notePrefab, new Vector3(notePrefab.transform.position.x, notePrefab.transform.position.y, z), Quaternion.identity);
                longNote.transform.localScale = new Vector3(notePrefab.transform.localScale.x, notePrefab.transform.localScale.y, notePrefab.transform.localScale.z * longMultiplier);
                effectObject = Instantiate(rightNoteEffect);
                effectObject.transform.SetParent(longNote.transform);
                effectObject.transform.localPosition = new Vector3(0, 0, -0.5f);
                NotesObj.Add(longNote);
                continue;
            }
            else if (inputJson.notes[i].type == 2
                &&
                inputJson.notes[i].block == 6
                &&
                memoraizeNoteNumL != 0)
            {
                GameObject effectObject;

                int longMultiplier = inputJson.notes[i].num - memoraizeNoteNumL;
                memoraizeNoteNumL = 0;
                z = z - (0.015688f * longMultiplier);
                GameObject longNote = Instantiate(notePrefab, new Vector3(notePrefab.transform.position.x, notePrefab.transform.position.y, z), Quaternion.identity);
                longNote.transform.localScale = new Vector3(notePrefab.transform.localScale.x, notePrefab.transform.localScale.y, notePrefab.transform.localScale.z * longMultiplier);
                effectObject = Instantiate(leftNoteEffect);
                effectObject.transform.SetParent(longNote.transform);
                effectObject.transform.localPosition = new Vector3(0, 0, -0.5f);
                NotesObj.Add(longNote);
                continue;
            }
                NotesObj.Add(Instantiate(notePrefab, new Vector3(notePrefab.transform.position.x, notePrefab.transform.position.y, z), notePrefab.transform.rotation));
        }
    }

    IEnumerator CountDown()
    {
        scoreManager.totalScore = 0;

        if (!GameManager.IsGameOver)
        {
            tutorialPanel.SetActive(true);
            GameObject rulePage = rulePages[GameManager.CaluclatePlayModeIndex];
            rulePage.transform.localScale = new Vector3(1, 0, 1);
            rulePage.SetActive(true);
            GameManager.SESource.clip = GameManager.SEClip[4];
            GameManager.SESource.Play();
            yield return StartCoroutine(ActiveTutorialPanel(rulePage, 0.5f));
            StartCoroutine(ExpandTutorialPanel(rulePage, 0.5f));
            yield return new WaitForSeconds(9f);
            yield return StartCoroutine(AnExpandTutorialPanel(rulePage, 0.5f));
            yield return StartCoroutine(AnActiveTutorialPanel(rulePage, 0.5f));
            rulePage.SetActive(false);
        }
        GameObject countDown = Instantiate(countDownPrefab, canvas.transform);
        Destroy(countDown, 1.6f);
        yield return new WaitForSeconds(1.8f);
        GameObject startObject = Instantiate(startPrefab, canvas.transform);
        Destroy(startObject, 1.2f);
        StartCoroutine(ActiveGameObjectUI(scoreTextObject, 2.0f, "TMP"));
        yield return new WaitForSeconds(2f);
        GameManager.IsInvalid = false;
        rightBladeRigidbody.isKinematic = false;
        leftBladeRigidbody.isKinematic = false;
    }

    IEnumerator ShowResult()
    {
        scoreTextObject.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
        foreach (GameObject gameObject in rankImageObjects)
        {
            gameObject.GetComponent<Image>().color = new Color(gameObject.GetComponent<Image>().color.r, gameObject.GetComponent<Image>().color.g, gameObject.GetComponent<Image>().color.b, 0);
        }
        foreach (GameObject gameObject in resultTextObjects)
        {
            gameObject.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
        }
        yield return new WaitUntil(() => GameManager.BGMSource.isPlaying == true);

        yield return new WaitForSeconds(20f);
        yield return new WaitUntil(() => GameManager.BGMSource.time < 2f);
        GameManager.BGMSource.volume = 0;

        GameManager.IsInvalid = true;
        rightBladeRigidbody.isKinematic = true;
        leftBladeRigidbody.isKinematic = true;
        int totalScore = Mathf.FloorToInt(scoreManager.totalScore);
        float modeMultiplier = 0;
        float rankMultiplier = 0;
        int allRedCount = 0;
        int allBlueCount = 0;
        int allYellowCount = 0;
        int exellentCount = ScoreManager.JudgmentsCounter[3];
        int veryGoodCount = ScoreManager.JudgmentsCounter[2];
        int goodCount = ScoreManager.JudgmentsCounter[1];
        int missCount = ScoreManager.JudgmentsCounter[0];
        int successCount = exellentCount + veryGoodCount + goodCount + missCount;
        GameObject rankImageObject = null;
        Rank rank = Rank.None;

        if (GameManager.CaluclatePlayModeIndex == 0)
        {
            modeMultiplier = 1;
        }
        else if (GameManager.CaluclatePlayModeIndex == 1)
        {
            modeMultiplier = 1.2f;
        }
        float successPercent = (float)successCount / NotesObj.Count * 100;
        float exellentPercent = (float)exellentCount / NotesObj.Count * 100;
        if (successPercent == 100
            &&
            exellentCount == 100)
        {
            rank = Rank.SSS;
            rankImageObject = rankImageObjects[0];
            rankMultiplier = 2f;
        }
        else if (successPercent >= 95)
        {
            rank = Rank.S;
            rankImageObject = rankImageObjects[1];
            rankMultiplier = 1.8f;
        }
        else if (successPercent >= 80)
        {
            rank = Rank.A;
            rankImageObject = rankImageObjects[2];
            rankMultiplier = 1.5f;
        }
        else if (successPercent >= 50)
        {
            rank = Rank.B;
            rankImageObject = rankImageObjects[3];
            rankMultiplier = 1.3f;
        }
        else
        {
            rank = Rank.C;
            rankImageObject = rankImageObjects[4];
            rankMultiplier = 1.0f;
        }
        foreach (int noteType in countNoteType) //母数でしかない
        {
            switch (noteType)
            {
                case 0:
                case 7:
                case 8:
                    allRedCount++;
                    break;
                case 1:
                    allBlueCount++;
                    break;
                case 4:
                case 6:
                    allYellowCount++;
                    break;
            }
        }
        int finalScore = Mathf.FloorToInt(totalScore * rankMultiplier);
        finalScore = Mathf.FloorToInt(finalScore * modeMultiplier);
        missCount += NotesObj.Count - successCount;
        allYellowCount = allYellowCount / 2;

        string[] resultTexts = {$"{GameManager.PlayModes[GameManager.CaluclatePlayModeIndex]}",
                               $"× {rankMultiplier.ToString("F1")}",
                               $"<#ff0000>MainNote</color> {RedCount}",
                               $"<#0000ff>ImpactNote</color> {BlueCount}",
                               $"<#ffff00>BladeNote</color> {YellowCount}",
                               $"<#FFFFFF>Excellent</color> {exellentCount}",
                               $"<#C8C8C8>VeryGood</color> {veryGoodCount}",
                               $"<#969696>Good</color> {goodCount}",
                               $"<#323232>Miss</color> {missCount}",
                               $"{totalScore}",
                               $"{finalScore}" };

        GameObject completedObject = Instantiate(completedPrefab, canvas.transform);
        Destroy(completedObject, 1.5f);
        yield return new WaitForSeconds(5f);
        resultPanel.transform.localScale = new Vector3(1, 0, 1);
        resultPanel.SetActive(true);
        GameManager.SESource.clip = GameManager.SEClip[4];
        GameManager.SESource.Play();
        yield return StartCoroutine(ActiveTutorialPanel(resultPanel, 0.5f));
        yield return StartCoroutine(ExpandTutorialPanel(resultPanel, 0.5f));
        StartCoroutine(inGameManager.NonActivePointLight());
        StartCoroutine(inGameManager.NonActiveSpotLight());
        StartCoroutine(inGameManager.NonActiveSpotLight2());
        StartCoroutine(ActiveGameObjectUI(rankImageObject, 0.5f, "Image"));
        TextMeshProUGUI[] tmpChildren = resultPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI child in tmpChildren)
        {
            GameObject go = child.gameObject;
            StartCoroutine(ActiveGameObjectUI(go, 0.5f, "TMP"));
        }
        for (int i = 0; i < resultTextObjects.Length; i++)
        {
            resultTextObjects[i].GetComponent<TextMeshProUGUI>().text = resultTexts[i];
        }
        StartCoroutine(ActiveKeyUI(spaceKeyUI2, 1f));

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        GameManager.BGMScore[GameManager.SelectedBGMIndex] = Mathf.Max(GameManager.BGMScore[GameManager.SelectedBGMIndex], finalScore);
        GameManager.AllRedCount[GameManager.SelectedBGMIndex] = Mathf.Max(GameManager.AllRedCount[GameManager.SelectedBGMIndex], allRedCount);
        GameManager.AllBlueCount[GameManager.SelectedBGMIndex] = Mathf.Max(GameManager.AllBlueCount[GameManager.SelectedBGMIndex], allBlueCount);
        GameManager.AllYellowCount[GameManager.SelectedBGMIndex] = Mathf.Max(GameManager.AllYellowCount[GameManager.SelectedBGMIndex], allYellowCount);
        GameManager.RedCount[GameManager.SelectedBGMIndex] = Mathf.Max(GameManager.RedCount[GameManager.SelectedBGMIndex], RedCount);
        GameManager.BlueCount[GameManager.SelectedBGMIndex] = Mathf.Max(GameManager.BlueCount[GameManager.SelectedBGMIndex], BlueCount);
        GameManager.YellowCount[GameManager.SelectedBGMIndex] = Mathf.Max(GameManager.YellowCount[GameManager.SelectedBGMIndex], YellowCount);
        GameManager.SESource.clip = GameManager.SEClip[4];
        GameManager.SESource.Play();
        yield return StartCoroutine(AnExpandTutorialPanel(resultPanel, 0.5f));
        yield return StartCoroutine(AnActiveTutorialPanel(resultPanel, 0.5f));
        resultPanel.SetActive(false);
        StartCoroutine(inGameManager.BlackOut("ReturnTitle"));
        SceneManager.LoadScene("TitleScene");
    }

    IEnumerator ActiveGameObjectUI(GameObject gameObjectUI, float duration, string T)
    {
        gameObjectUI.SetActive(true);
        float a = 0;
        float timer = 0;
        float startTime = Time.time; 
        Color roopColor = Color.white;

        if (T == "TMP")
        {
            roopColor = gameObjectUI.GetComponent<TextMeshProUGUI>().color;
        }
        else if (T == "Image")
        {
            roopColor = gameObjectUI.GetComponent<Image>().color;
        }
        while (timer <= duration) 
        {
            roopColor.a = a;

            if (T == "TMP")
            {
                gameObjectUI.GetComponent<TextMeshProUGUI>().color = roopColor;
            }
            else if (T == "Image")
            {
                gameObjectUI.GetComponent<Image>().color = roopColor;
            }

            timer = Time.time - startTime;
            a = Mathf.Lerp(0, 1, timer / duration);
            yield return null;
        }
    }

    IEnumerator PlayTutorial()
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
        IsTutorialPause = true;
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
        yield return new WaitForSeconds(0.5f);
        activeSpaceKeyUI = StartCoroutine(ActiveKeyUI(spaceKeyUI, 2f));
        activeRKeyUI = StartCoroutine(ActiveKeyUI(rKeyUI, 2f));

        tutorialJudgs[0] = StartCoroutine(BGMChoice());
        tutorialJudgs[1] = StartCoroutine(ReturnChoice());
        GameManager.IsTutorial = false;
    }

    IEnumerator ActiveTutorialPanel(GameObject gameObject, float duration)
    {
        {
            float a = 0;
            float timer = 0;
            float startTime = Time.time; 
            Color roopColor = gameObject.GetComponent<Image>().color;

            while (timer <= duration) 
            {
                roopColor.a = a;
                gameObject.GetComponent<Image>().color = roopColor;

                timer = Time.time - startTime;
                a = Mathf.Lerp(0, 1, timer / duration);
                yield return null;
            }
            roopColor.a = 1;
            gameObject.GetComponent<Image>().color = roopColor;
        }
    }
    IEnumerator ExpandTutorialPanel(GameObject gameObject, float duration)
    {
        {
            float timer = 0;
            float startTime = Time.time; 
            float roopScaleY = 0;

            while (timer <= duration) 
            {
                gameObject.transform.localScale = new Vector3(1, roopScaleY, 1);

                timer = Time.time - startTime;
                roopScaleY = Mathf.Lerp(0, 1, timer / duration);
                yield return null;
            }
             gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
    }
    IEnumerator AnActiveTutorialPanel(GameObject gameObject, float duration)
    {
        {
            float a = 1;
            float timer = 0;
            float startTime = Time.time; 
            Color roopColor = gameObject.GetComponent<Image>().color;

            while (timer <= duration) 
            {
                roopColor.a = a;
                gameObject.GetComponent<Image>().color = roopColor;

                timer = Time.time - startTime;
                a = Mathf.Lerp(1, 0, timer / duration);
                yield return null;
            }
            roopColor.a = 0;
            gameObject.GetComponent<Image>().color = roopColor;
        }
    }
    IEnumerator AnExpandTutorialPanel(GameObject gameObject, float duration)
    {
        {
            float timer = 0;
            float startTime = Time.time; 
            float roopScaleY = 0;

            while (timer <= duration) 
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
                    GameObject noteObject = Instantiate(notesPrefab[notePrefabInedex], new Vector3(notesPrefab[notePrefabInedex].transform.position.x, notesPrefab[notePrefabInedex].transform.position.y, 10), Quaternion.identity);
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

        GameManager.SESource.clip = GameManager.SEClip[4];                         
        GameManager.SESource.Play();
        StartCoroutine(inGameManager.BlackOut("Play"));
        foreach (var coroutine in tutorialJudgs)
        {
            StopCoroutine(coroutine);
        }
    }

    IEnumerator ReturnChoice()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.R));

        GameObject tutorialPage10 = tutorialPanel.transform.GetChild(9).gameObject; //確認画面を開く
        tutorialPage10.transform.localScale = new Vector3(1, 0, 1);
        tutorialPage10.SetActive(true);
        GameManager.SESource.clip = GameManager.SEClip[4];                         
        GameManager.SESource.Play();
        yield return StartCoroutine(ActiveTutorialPanel(tutorialPage10, 0.5f));
        yield return StartCoroutine(ExpandTutorialPanel(tutorialPage10, 0.5f));
        activeSpaceKeyUI =  StartCoroutine(ActiveKeyUI(fSpaceKeyUI, 2f));
        activeRKeyUI = StartCoroutine(ActiveKeyUI(fRKeyUI, 2f));

        returnOrStay[0] = StartCoroutine(ChoiceYes());
        returnOrStay[1] = StartCoroutine(ChoiceNo());
        foreach (var coroutine in tutorialJudgs)
        {
            StopCoroutine(coroutine);
        }
    }

    IEnumerator ChoiceYes()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        StopCoroutine(returnOrStay[1]);
        GameManager.SESource.clip = GameManager.SEClip[4];
        GameManager.SESource.Play();
        StopCoroutine(activeSpaceKeyUI);
        StopCoroutine(activeRKeyUI);
        fSpaceKeyUI.SetActive(false);
        fRKeyUI.SetActive(false);
        GameManager.BGMSource.Stop();
        StartCoroutine(inGameManager.BlackOut("ReturnTitle"));
        GameManager.IsPause = false;
    }
    IEnumerator ChoiceNo()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.R));
        StopCoroutine(returnOrStay[0]);
        if (IsTutorialPause
            &&
            !pausePanel.activeSelf)
        {
            GameObject tutorialPage10 = tutorialPanel.transform.GetChild(9).gameObject; //確認画面を閉じる
            GameManager.SESource.clip = GameManager.SEClip[4];
            GameManager.SESource.Play();
            StopCoroutine(activeSpaceKeyUI);
            StopCoroutine(activeRKeyUI);
            fSpaceKeyUI.SetActive(false);
            fRKeyUI.SetActive(false);
            yield return StartCoroutine(AnExpandTutorialPanel(tutorialPage10, 0.5f));
            StartCoroutine(AnActiveTutorialPanel(tutorialPage10, 0.5f));
            tutorialPage10.SetActive(false);
            tutorialJudgs[0] = StartCoroutine(BGMChoice());
            tutorialJudgs[1] = StartCoroutine(ReturnChoice());
            foreach (var coroutine in returnOrStay)
            {
                StopCoroutine(coroutine);
            }
            GameManager.IsPause = false;
        }
        else if (!IsTutorialPause
            &&
            pausePanel.activeSelf)
        {
            GameManager.SESource.clip = GameManager.SEClip[4];
            GameManager.SESource.Play();
            StopCoroutine(activeSpaceKeyUI);
            StopCoroutine(activeRKeyUI);
            fSpaceKeyUI.SetActive(false);
            fRKeyUI.SetActive(false);
            yield return StartCoroutine(AnExpandTutorialPanel(pausePanel, 0.5f));
            StartCoroutine(AnActiveTutorialPanel(pausePanel, 0.5f));
            pausePanel.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            FlowingController[] flowingControllerObjs = FindObjectsOfType<FlowingController>();
            for (int i = 0; i < flowingControllerObjs.Length; i++)
            {
                flowingControllerObjs[i].enabled = true;
            }
            if (!GameManager.IsTutorial)
            {
                GameManager.BGMSource.Play();
            }
            GameManager.IsPause = false;
        }
    }

    public IEnumerator PauseController()
    {
        if (IsTutorialPause
            ||
            GameManager.IsPause)
        {
            yield break;
        }

        if (!GameManager.IsPause)
        {
            GameManager.IsPause = true;
            GameManager.BGMSource.Pause();
            FlowingController[] flowingControllerObjs = FindObjectsOfType<FlowingController>();
            for (int i = 0; i < flowingControllerObjs.Length; i++)
            {
                flowingControllerObjs[i].enabled = false;
            }
            pausePanel.transform.localScale = new Vector3(1, 0, 1); //ポーズ画面を開く
            pausePanel.SetActive(true);
            GameManager.SESource.clip = GameManager.SEClip[4];
            GameManager.SESource.Play();
            yield return StartCoroutine(ActiveTutorialPanel(pausePanel, 0.5f));
            yield return StartCoroutine(ExpandTutorialPanel(pausePanel, 0.5f));
            activeSpaceKeyUI = StartCoroutine(ActiveKeyUI(fSpaceKeyUI, 2f));
            activeRKeyUI = StartCoroutine(ActiveKeyUI(fRKeyUI, 2f));
            returnOrStay[0] = StartCoroutine(ChoiceYes()); //
            returnOrStay[1] = StartCoroutine(ChoiceNo());  //
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