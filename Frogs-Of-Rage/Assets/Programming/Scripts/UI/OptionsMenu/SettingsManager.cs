using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    #region "Inspector Variables and Related Properties
    [Tooltip("The 3rd person player camera")] private CinemachineVirtualCamera _playerCamera;
    [SerializeField] [Tooltip("The settings for the slider ranges")] private OptionsMenuSettings _settings;
    [SerializeField] [Tooltip("The Main Mixer")] private AudioMixer _mainMixer;

    [Header("Slider Components")]
    [Space(15)]
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _mouseSensitivitySlider;

    //getter only properties for sliders
    public Slider SFXSlider { get { return _sfxSlider; } }
    public Slider MusicSlider { get { return _musicSlider; } }
    public Slider MouseSensitivitySlider { get { return _mouseSensitivitySlider; } }
    #endregion

    private CinemachinePOV _cameraPOV; //retreived POV component that controls camera move speed
    private MixerVolumeController _mixerController; //The mixer controller component that controls the audio sliders

    #region "Slider Value Variables and Properties
    //curent values on the setting sliders.
    private float _inputtedSFXValue;
    private float _inputtedMusicValue;
    private float _inputtedMouseSensitvityValue;

    //Get only properties of values
    public float InputtedSFXValue { get { return _inputtedSFXValue; } }
    public float InputtedMusicValue { get { return _inputtedMusicValue; } }
    public float InputtedMouseSensitvityValue { get { return _inputtedMouseSensitvityValue; } }
    #endregion

    #region "Unity Runtime Functions"
    //initilizes the menu
    private void Awake()
    {
        
        
        //Loads prefs
        LoadSettings();

        //initilizes sliders
        InitilizeSlider(ref _sfxSlider, _settings.sfxSliderRange, _inputtedSFXValue);
        InitilizeSlider(ref _musicSlider, _settings.musicSliderRange, _inputtedMusicValue);
        InitilizeSlider(ref _mouseSensitivitySlider, _settings.mouseSensitivitySliderRange, _inputtedMouseSensitvityValue);

        //subscribes functions to sliders
        InitilizeMouseSensitivitySlider();
        InitilizeAudioSliders();


    }

    private void Start()
    {
        _playerCamera = GameManager.Instance.playerCam;
        //gets the POV component on the camera
        if (_playerCamera != null)
            _cameraPOV = _playerCamera.GetCinemachineComponent<CinemachinePOV>();
        else
            Debug.LogWarning("Please Make sure you assign the 3rd person camera from the player to the settings manager for it to work properly!");

        //initial refresh of applied settings after load
        ManualRefresh();
    }

    //saves settings when disabled
    private void OnDisable()
    {
        SaveSettings();
    }
    #endregion

    #region "Setting Application Functions
    private void InitilizeMouseSensitivitySlider()
    {
        if(_mouseSensitivitySlider != null)
        {
            _mouseSensitivitySlider.onValueChanged.AddListener(UpdateMouseSensitivity);
        }
    }

    private void InitilizeAudioSliders()
    {
        if(_mixerController != null)
        {
            Destroy(_mixerController);
        }

        _mixerController = gameObject.AddComponent<MixerVolumeController>();
        _mixerController.InitilizeMixerVolumeController(_settings, _mainMixer, this);
    }

    #endregion

    //saves and loads slider values, not applied values
    #region "Saving/Loading (PlayerPrefs)"
    private void SaveSettings()
    {
        //saves settings as input ranges
        PlayerPrefs.SetFloat("SFX", _inputtedSFXValue);
        PlayerPrefs.SetFloat("Music", _inputtedMusicValue);
        PlayerPrefs.SetFloat("MouseSensitvity", _inputtedMouseSensitvityValue);
    }
    private void LoadSettings()
    {
        //loads settings as input ranges. defaults to middle of the input range. Mouse sensitivity defaults to 25% up the range
        _inputtedSFXValue = PlayerPrefs.GetFloat("SFX", _settings.sfxSliderRange.InterpolateValues(0.5f));
        _inputtedMusicValue = PlayerPrefs.GetFloat("Music", _settings.musicSliderRange.InterpolateValues(0.5f));
        _inputtedMouseSensitvityValue = PlayerPrefs.GetFloat("MouseSensitvity", _settings.mouseSensitivitySliderRange.InterpolateValues(0.25f));
    }
    #endregion

    #region "Helper Functions"
    /// <summary>
    /// Initilizes a settings slider
    /// </summary>
    /// <param name="slider"> Slider to initilize </param>
    /// <param name="range"> settiings value range for the slider </param>
    /// <param name="value"> value to set the slider to </param>
    private void InitilizeSlider(ref Slider slider, SettingsValueRange range, float value)
    {
        if(slider != null)
        {
            slider.minValue = range.minValue;
            slider.maxValue = range.maxValue;
            slider.value = value;
        }
    }

    /// <summary>
    /// Updates the mouse sensitivity from current values on slider
    /// </summary>
    /// <param name="value"></param>
    private void UpdateMouseSensitivity(float value)
    {
        if (_cameraPOV != null)
        {
            float _appliedSensitivityValue;

            //transfers the value from the slider's range into a range usable by the camera
            UtilityFunctions.RemapValue
                (_settings.mouseSensitivitySliderRange.minValue, _settings.mouseSensitivitySliderRange.maxValue,
                _settings.appliedMouseSensitivitySliderRange.minValue, _settings.appliedMouseSensitivitySliderRange.maxValue,
                value, out _appliedSensitivityValue);

            _cameraPOV.m_VerticalAxis.m_MaxSpeed = _appliedSensitivityValue;
            _cameraPOV.m_HorizontalAxis.m_MaxSpeed = _appliedSensitivityValue;
        }
        else
        {
            Debug.LogError("No CinemachinePOV on the camera found");
        }
    }

    /// <summary>
    /// Refreshes applied settings manualy.
    /// </summary>
    private void ManualRefresh()
    {
        if(_mouseSensitivitySlider)
            UpdateMouseSensitivity(_mouseSensitivitySlider.value);

        if (_mixerController && _sfxSlider && _musicSlider)
            _mixerController.ManualCaculateVolumes(_sfxSlider.value, _musicSlider.value);
    }
    #endregion
} 

//Struct for value ranges
[System.Serializable]
public struct SettingsValueRange
{
    public float minValue;
    public float maxValue;

    /// <summary>
    /// Flips values if max is less than min
    /// </summary>
    public void TryFlipValues()
    {
        if (minValue > maxValue)
        {
            float holder = maxValue;
            maxValue = minValue;
            minValue = holder;
        }
    }

    /// <summary>
    /// Gets a value in the range for the given interpolation value. 0 is min, 1 is max. 
    /// </summary>
    /// <param name="IValue"></param>
    public float InterpolateValues(float IValue)
    {
        IValue = Mathf.Clamp01(IValue);

        return (maxValue - minValue) * IValue + minValue;
    }
}
