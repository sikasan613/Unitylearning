using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                Type t = typeof(T);

                instance = (T)FindObjectOfType(t);
                if(instance == null)
                {
                    Debug.LogError(t + "���A�^�b�`���Ă���GameObject�͂���܂���");
                }
            }
            return instance;
        }
    }

    virtual protected void Awake()
    {
        if(this != Instance)
        {
            Destroy(this);
            //Debug.LogError(typeof(T) +
            //    " �͊��ɑ���GameObject�ɃA�^�b�`����Ă��邽�߁A�R���|�[�l���g��j�����܂���." +
            //    " �A�^�b�`����Ă���GameObject�� " + Instance.gameObject.name + " �ł�.");
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

}
