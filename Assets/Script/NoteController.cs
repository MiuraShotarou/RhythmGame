using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    public bool isCollisionStay = false;

    bool _isCollision = false;

    public bool IsCollision
    {
        get { return _isCollision; }
        set { _isCollision = value; if (_isCollision) { CrashNote(); } }
    }

    void CrashNote()
    {
        //GameObject effectObj = GetComponentInChildren<GameObject>();
        //ParticleSystem particleSystem = effectObj.GetComponent<ParticleSystem>();

        ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
        //Animator animator = GetComponent<Animator>();

        //Debug.Log(animator.name);
        if (particleSystem != null
            //&&
            //animator != null
            )
        {
            particleSystem.Emit(1);                                                 //�v���t�@�u���Ƀp�[�e�B�N���V�X�e����ς���Ηǂ�
            //animator.Play("CrushImage");
        }
        else if (particleSystem == null)
        {
            Debug.Log("particleSystem���ݒ肳��Ă��Ȃ��B");
        }
    }
}