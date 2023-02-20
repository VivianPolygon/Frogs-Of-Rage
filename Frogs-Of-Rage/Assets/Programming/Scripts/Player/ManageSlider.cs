using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageSlider : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public GameObject needle;

    private float needleMinMax = 180f;
    private float curNeedleRot;


    public static event Action SetStaminaMax;
    public static event Action SetStaminaValue;
    public static event Action SetHealthMax;
    public static event Action SetHealthValue;

    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
        slider.value = value;

        fill.color = gradient.Evaluate(1f);
    }
    public void SetValue(float value)
    {
        slider.value = value;

        fill.color = gradient.Evaluate(slider.normalizedValue);

    }

    private void Update()
    {
        SetStaminaMax?.Invoke();
        SetStaminaValue?.Invoke();
        SetHealthMax?.Invoke();
        SetHealthValue?.Invoke();
        needle.transform.eulerAngles = new Vector3(0, 0, GetNeedleRot());
    }


    private float GetNeedleRot()
    {
        float totalAngleSize = needleMinMax - -needleMinMax;

        float valueNormalized = slider.value / slider.maxValue;

        return needleMinMax - valueNormalized * totalAngleSize;
    }

}
