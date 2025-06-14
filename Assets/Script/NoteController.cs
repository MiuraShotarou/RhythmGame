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

        if (particleSystem != null)
        {
            particleSystem.Emit(1);
        }
        else
        {
            Debug.Log("particleSystemÇ™ê›íËÇ≥ÇÍÇƒÇ¢Ç»Ç¢ÅB");
        }
    }
}