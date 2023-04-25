using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaGauge : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public float sliderMaxPercent = 0.625f;



    public static event Action SetStaminaMax;
    public static event Action SetStaminaValue;
    public static event Action SetHealthMax;
    public static event Action SetHealthValue;

    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
        slider.value = value;
        slider.value = Mathf.Clamp(slider.value, 0, slider.maxValue * sliderMaxPercent);
    }
    public void SetValue(float value)
    {
        slider.value = value;
        slider.value = Mathf.Clamp(slider.value, 0, slider.maxValue * sliderMaxPercent);

    }

    private void Update()
    {
        SetStaminaMax?.Invoke();
        SetStaminaValue?.Invoke();
        SetHealthMax?.Invoke();
        SetHealthValue?.Invoke();
    }

}
