using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            //If there isnt an instance of this then create it
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    _instance = obj.AddComponent<T>();
                }
                DontDestroyOnLoad(_instance.gameObject);
            }
            //If there is already one then return it
            return _instance;
        }
    }

    public virtual void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this as T;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //private void OnDestroy()
    //{
    //    _instance= null;
    //}
}

