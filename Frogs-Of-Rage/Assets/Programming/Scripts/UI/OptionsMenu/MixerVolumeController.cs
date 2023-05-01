using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MixerVolumeController: MonoBehaviour
{
    private SettingsManager _settingsManager;

    private MixerVolume _sfxMixerVolume;
    private MixerVolume _musicMixerVolume;

    /// <summary>
    /// Initilizes the Mixer Volume Controller, Stand in for a constructor
    /// </summary>
    /// <param name="settings"> The options settings, used for slider/applied value ranges </param>
    /// <param name="mixer"> The main audio mixer </param>
    /// <param name="settingsManager"> The settings manager this is being created from </param>
    public void InitilizeMixerVolumeController(OptionsMenuSettings settings, AudioMixer mixer, SettingsManager settingsManager)
    {
        //iitilizes the settings manager
        _settingsManager = settingsManager; 

        //initilizes SFX
        if (_sfxMixerVolume == null)
        {
            _sfxMixerVolume = new MixerVolume();
        }
        _sfxMixerVolume.InitilizeMixerController(settings, _settingsManager.SFXSlider, mixer, MixerVolume.MixerType.SFX);

        //initilizes Music
        if(_musicMixerVolume == null)
        {
            _musicMixerVolume = new MixerVolume();
        }
        _musicMixerVolume.InitilizeMixerController(settings, _settingsManager.MusicSlider, mixer, MixerVolume.MixerType.Music);
    }

    //Manualy caculates the volumes. Ussualy is done when sliders values change via callbacks.
    public void ManualCaculateVolumes(float sfxSliderValue, float musicSliderValue)
    {
        //SFX Slider
        if (_sfxMixerVolume != null)
            _sfxMixerVolume.CalculateVolume(sfxSliderValue);

        //Music Slider
        if (_musicMixerVolume != null)
            _musicMixerVolume.CalculateVolume(musicSliderValue);

    }
}

public class MixerVolume
{
    private string _volumeParameterName;
    public enum MixerType
    {
        SFX,
        Music
    }

    private AudioMixer _mixer;
    private Slider _inputSlider;

    private SettingsValueRange _sliderValueRange;
    private SettingsValueRange _appliedValueRange;

    /// <summary>
    /// Initilizes the Mixer Controller
    /// </summary>
    public void InitilizeMixerController( OptionsMenuSettings settings, Slider inputSlider, AudioMixer mixer, MixerType type)
    {
        _mixer = mixer;
        _inputSlider = inputSlider;

        switch (type)
        {
            case MixerType.SFX:
                _sliderValueRange = settings.sfxSliderRange;
                _appliedValueRange = settings.appliedSFXSliderRange;
                _volumeParameterName = "SFXVolume";
                break;
            case MixerType.Music:
                _sliderValueRange = settings.musicSliderRange;
                _appliedValueRange = settings.appliedMusicSliderRange;
                _volumeParameterName = "MusicVolume";
                break;
            default:
                break;
        }

        if(_inputSlider && _mixer)
        {
            _inputSlider.onValueChanged.AddListener(CalculateVolume);
            CalculateVolume(_inputSlider.value);
        }
    }

    /// <summary>
    /// Calculates the volume based on the slider. Subscribed to the sliders on changed and passed through its value
    /// </summary>
    /// <param name="value"></param>
    public void CalculateVolume(float value)
    {
        //converts the slider scale to the audio scale
        UtilityFunctions.RemapValue
            (_sliderValueRange.minValue, _sliderValueRange.maxValue,
            _appliedValueRange.minValue, _appliedValueRange.maxValue,
            _inputSlider.value, out value);

        if (_mixer)
        {

            float maxValue = _appliedValueRange.maxValue;

            //prevents dividing by 0
            if (_appliedValueRange.maxValue == 0)
                _appliedValueRange.maxValue = 0.01f; 

            //sets the float value using the scale, divides max to always work.
            _mixer.SetFloat(_volumeParameterName, Mathf.Max(Mathf.Log10(value / maxValue) * 20, -80));
        }
    }
}
