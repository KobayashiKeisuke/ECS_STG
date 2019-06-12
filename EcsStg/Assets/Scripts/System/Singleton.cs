using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance = null;
    public static T I { get { return instance; } }
    private void Awake()
    {
        if(instance == null )
        {
            instance = this as T;
            instance.Init();
        }
    }
    protected virtual void Init()
    {
    }
    public static bool IsExist()
    {
        return instance != null;
    }
}

