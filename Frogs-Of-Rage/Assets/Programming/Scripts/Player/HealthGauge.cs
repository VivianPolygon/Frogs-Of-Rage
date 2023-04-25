using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthGauge : MonoBehaviour
{
    [SerializeField] private GameObject sec1, sec2, sec3, sec4;
    [SerializeField] private Dictionary<GameObject, float> secList = new Dictionary<GameObject, float>();

    private float _maxValue, _value;

    public static event Action SetStaminaMax;
    public static event Action SetStaminaValue;
    public static event Action SetHealthMax;
    public static event Action SetHealthValue;


    private void Awake()
    {
        secList.Add(sec1, 0.25f);
        secList.Add(sec2, 0.5f);
        secList.Add(sec3, 0.75f);
        secList.Add(sec4, 1f);

        foreach (KeyValuePair<GameObject, float> keyValuePair in secList)
        {
            keyValuePair.Key.SetActive(true);
        }
    }

    public void SetMaxValue(float value)
    {
        _maxValue = value;
        _value = value;

        foreach (KeyValuePair<GameObject, float> keyValuePair in secList)
        {
            keyValuePair.Key.SetActive(true);
        }
    }
    public void SetValue(float value)
    {
        _value = value;

        //if(_value < _maxValue / 100)
        //{

        //}
        //foreach (KeyValuePair<GameObject, float> keyValuePair in secList)
        //{
        //    if(keyValuePair.Value < (_value / 100))
        //    {
        //        keyValuePair.Key.SetActive(false);
        //    }
        //}
    }

    private void Update()
    {
        SetStaminaMax?.Invoke();
        SetStaminaValue?.Invoke();
        SetHealthMax?.Invoke();
        SetHealthValue?.Invoke();
    }


}
