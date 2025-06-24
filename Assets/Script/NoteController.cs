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
        ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Emit(1);                                                 //プレファブ毎にパーティクルシステムを変えれば良い
        }
        else if (particleSystem == null)
        {
            Debug.Log("particleSystemが設定されていない。");
        }
    }
}