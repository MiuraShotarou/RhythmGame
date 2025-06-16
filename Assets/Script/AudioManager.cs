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

    public int noteNum;       //���m�[�c��
    private string songName;  //�Ȗ�

    public List<int> LaneNum = new List<int>();                //���Ԃ̃��[���Ƀm�[�c�������Ă��邩�B
    public List<int> NoteType = new List<int>();               //�m�[�c�̎��
    public List<float> NotesTime = new List<float>();          //�m�[�c��������Əd�Ȃ鎞�ԁB
    public List<GameObject> NotesObj = new List<GameObject>(); //�m�[�c�I�u�W�F�N�g���i�[����ϐ��B

    [SerializeField] private float NotesSpeed;                     //�m�[�c�̑��x�B
    [SerializeField] GameObject notePrefab;                           //�m�[�cPrefab�B

    //public void AudioSelect() //������p��
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
        bgmSource.clip = bgmClip[0]; //�̂��ɃC���f�b�N�X�͈����Ō��肷��d�l�ɁB
        noteNum = 0;
        songName = "SAIL AWAY";
        Load(songName);
        bgmSource.Play();
    }
    void Load(string SongName)
    {
        string inputString = Resources.Load<TextAsset>(SongName).ToString(); //SongName �� string "�e�X�g";
        Data inputJson = JsonUtility.FromJson<Data>(inputString);            //JsonUtility.FromJson<Data>(inputString);

        noteNum = inputJson.notes.Length;
        for (int i = 0; i < inputJson.notes.Length; i++)
        {
            float kankaku = 60 / (inputJson.BPM * (float)inputJson.notes[i].LPB); //�Ԋu�@�Ƃ́@�ꕪ�Ԃŉ���r�[�g������̂��A�� 1���ɂ������̃��C������̂��@�Ŋ|�����l��60�Ŋ��������́B
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
        //����Ɠ����ɁA�m�[�c�̐������s���Ƃ����d�g�݂ɁB
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
    public int type; //Type���̎�ޕ����͐�΂ɕK�v�ł���B
    public int num;
    public int block;
    public int LPB;
}
//public class NoteGenerator : AudioManager
//{
//    Dictionary<NoteCategory, float, >;
//    //List<float> noteList
//}