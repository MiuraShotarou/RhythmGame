using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    bool _isCollision = false;

    public bool IsCollision
    {
        get { return _isCollision; }
        set { _isCollision = value; if (_isCollision) { CrashNote(); } }
    }

    bool _isCollisionStay = false;

    public bool IsCollisionStay
    {
        get { return _isCollisionStay; }
        set { _isCollisionStay = value;}
    }
    void CrashNote()
    {
        ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Emit(1);                                                 //プレファブ毎にパーティクルシステムを変えれば良い
        }


        switch (gameObject.tag)
        {
            case "MainNote":
            case "RightRightNote":
            case "LeftLeftNote":
                AudioManager.RedCount++;
                break;
            case "BlueNote":
                AudioManager.BlueCount++;
                break;
            case "RightNoteLong":
            case "LeftNoteLong":
                AudioManager.YellowCount++;
                break;
        }
    }
}