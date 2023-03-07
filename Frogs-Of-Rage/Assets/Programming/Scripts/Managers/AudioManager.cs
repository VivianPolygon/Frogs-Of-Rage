using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    //Variables
    #region "Proximity Prioritization Variables and Properties"

    public enum ProximityMode
    {
        MainCamera,
        CustomObject
    }
    public enum AudioCullingQuality
    {
        High,
        Low
    }

    [SerializeField] private bool _proximityPrioritization;
    public bool ProximityPrioritization
    {
        get { return _proximityPrioritization; }
        private set { _proximityPrioritization = value; }
    }


    [SerializeField] [HideInInspector] private ProximityMode _proximityMode;
    [SerializeField] [HideInInspector] private AudioCullingQuality _cullingquality;

    public ProximityMode ProxMode
    {
        get { return _proximityMode; }
        set { _proximityMode = value; }
    }
    public AudioCullingQuality CullingQuality
    {
        get { return _cullingquality; }
        set { _cullingquality = value; }
    }

    [SerializeField] [HideInInspector] private Transform _customObjectTransform;

    public Transform CustomObjectTransform
    {
        get { return _customObjectTransform; }
        set { _customObjectTransform = value; }
    }

    private delegate void AudioCulling(Vector3 position, SoundType type);
    private AudioCulling _culling;

    #endregion

    #region "Audio Key Variables"
    //used to generate audiokeys
    private static int _audioSourceIndexer;
    //dictionary that is acsessed via keys
    private static Dictionary<int, TrackedAudioSource> _trackedAudioDictionary;
    #endregion

    #region "Sound Pool Variables"
    [SerializeField] [Min(0)] private int _lowPrioritySoundEffectLimit = 10;
    [SerializeField] [Min(0)] private int _playerSoundEffectLimit = 4;

    private Transform _lowPrioritySFXPool; // capped by _low priority sound limit
    public Transform LowPrioritySFXPool 
    {
        get
        {
            if (_lowPrioritySFXPool == null)
            {
                _lowPrioritySFXPool = CreateAudioPool(_lowPrioritySoundEffectLimit, "Low Priority Sound Sources");
            }
            return _lowPrioritySFXPool;
        }
        private set
        {
            _lowPrioritySFXPool = value;
        }
    }

    private Transform _highPrioritySFXPool; //creates a few by default, if pool is all in use creates a new one, isin't capped, use with care. 
    public Transform HighPrioritySFXPool // capped by _low priority sound limit
    {
        get
        {
            if (_highPrioritySFXPool == null)
            {
                _highPrioritySFXPool = CreateAudioPool(3, "High Priority Sound Sources");

            }
            return _highPrioritySFXPool;
        }
        private set
        {
            _highPrioritySFXPool = value;
        }
    }

    private Transform _playerSFXPool; // capped by player sound effect limit
    public Transform PlayerSFXPool // capped by _low priority sound limit
    {
        get
        {
            if (_playerSFXPool == null)
            {
                _playerSFXPool = CreateAudioPool(_playerSoundEffectLimit, "Player Sound Sources");

            }
            return _playerSFXPool;
        }
        private set
        {
            _playerSFXPool = value;
        }
    }

    private Transform _musicSFXTransform; //theres only ever one
    public Transform MusicSFXTransform // capped by _low priority sound limit
    {
        get
        {
            if (_musicSFXTransform == null)
            {
                _musicSFXTransform = new GameObject().transform;
                _musicSFXTransform.gameObject.AddComponent<AudioSource>().Stop();
                _musicSFXTransform.name = "Music Sound Source";
                _musicSFXTransform.parent = transform;
                _musicSFXTransform.gameObject.SetActive(false);
            }
            return _musicSFXTransform;
        }
        private set
        {
            _musicSFXTransform = value;
        }
    }
    #endregion

    #region "Sound Setting Variables"

    [Space(10)]
    [SerializeField] private SoundSettings[] _soundSettings;
    [SerializeField] private SoundSettings _defaultSoundSettings;
    #endregion

    // Functions
    public override void Awake()
    {
        base.Awake();
        _audioSourceIndexer = 0;
        _trackedAudioDictionary = new Dictionary<int, TrackedAudioSource>();

        SetCullingQuality(_cullingquality);
    }

    #region "Pooling Related Functions"
    private Transform CreateAudioPool(int poolSize, string poolName)
    {
        Transform newPool = new GameObject().transform;
        newPool.name = poolName;

        GameObject newPoolObject;
        for (int i = 0; i < poolSize; i++)
        {
            newPoolObject = new GameObject();
            newPoolObject.AddComponent<AudioSource>().Stop();
            newPoolObject.name = (poolName + " Object " + (i + 1).ToString());
            newPoolObject.SetActive(false);
            newPoolObject.transform.parent = newPool;
        }

        newPool.transform.parent = transform;
        return newPool;
    } // creates an individual pool

    //gets the first available an object from the inputed pool, creates a new one if allowed and one isint found
    private GameObject GetSoundObject(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.LowPrioritySoundEffect:
                return SearchPoolForInactiveObject(LowPrioritySFXPool, false);
            case SoundType.HighPrioritySoundEffect:
                return SearchPoolForInactiveObject(HighPrioritySFXPool, true);
            case SoundType.PlayerSoundEffect:
                return SearchPoolForInactiveObject(PlayerSFXPool, false);
            case SoundType.Music:
                return MusicSFXTransform.gameObject;
            default:
                break;
        }
        return null;
    }
    private GameObject SearchPoolForInactiveObject(Transform poolParent, bool createNewWhenFull)
    {
        for (int i = 0; i < poolParent.childCount; i++)
        {
            if (!poolParent.GetChild(i).gameObject.activeSelf)
            {
                return poolParent.GetChild(i).gameObject;
            }
        }
        if (createNewWhenFull)
        {
            GameObject newSoundObject = new GameObject();
            newSoundObject.AddComponent<AudioSource>().Stop();
            newSoundObject.name = poolParent.name + "Extra Object";
            newSoundObject.transform.parent = poolParent;

            return newSoundObject;
        }
        return null;
    }
    private void SetGameObjectParentToPool(GameObject obj, SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.LowPrioritySoundEffect:
                obj.transform.parent = LowPrioritySFXPool;
                break;
            case SoundType.HighPrioritySoundEffect:
                obj.transform.parent = HighPrioritySFXPool;
                break;
            case SoundType.PlayerSoundEffect:
                obj.transform.parent = PlayerSFXPool;
                break;
            case SoundType.Music:
                obj.transform.parent = MusicSFXTransform;
                break;
            default:
                break;
        }
    }
    #endregion

    #region "Sound Setting Related Functions"

    private SoundSettings GetSoundSettingsByString(string settingsName)
    {
        if (_soundSettings == null)
        {
            return DefaultSoundSettings();
        }

        for (int i = 0; i < _soundSettings.Length; i++)
        {
            if (settingsName == _soundSettings[i].name)
            {
                return _soundSettings[i];
            }
        }

        Debug.LogWarning("No Sound Setting by the name: <b>" + settingsName + "</b>");
        return DefaultSoundSettings();
    }

    private SoundSettings DefaultSoundSettings()
    {
        if (_defaultSoundSettings != null)
        {
            return _defaultSoundSettings;
        }
        else
        {
            //creates and sets sound settings if there is no default set
            SoundSettings newSoundSettings = (SoundSettings)ScriptableObject.CreateInstance("SoundSettings");
            return newSoundSettings;
        }
    }
    private void SetAudioSourceFromSoundSettings(AudioSource audioSource, SoundSettings soundSettings)
    {
        audioSource.priority = soundSettings.priority;
        audioSource.volume = soundSettings.volume;
        audioSource.pitch = soundSettings.pitch;
        audioSource.spatialBlend = soundSettings.spacialBlend;

        audioSource.minDistance = soundSettings.minDistance;
        audioSource.maxDistance = soundSettings.maxDistance;
        audioSource.spread = soundSettings.spread;
        audioSource.dopplerLevel = soundSettings.dopplerLevel;
    }

    #endregion

    #region "Change from Editor Functions"

    public void SetQuality(AudioCullingQuality newQuality)
    {
        CullingQuality = newQuality;
        SetCullingQuality(newQuality);
    }
    public void SetProximityMode(ProximityMode newProximityMode)
    {
        ProxMode = newProximityMode;
        _proximityMode = newProximityMode;
    }

    #endregion

    #region "Proximity Prioritization Functions"

    private void SetCullingQuality(AudioCullingQuality quality)
    {
        switch (quality)
        {
            case AudioCullingQuality.High:
                _culling = new AudioCulling(CullDistantAudioHighQuality);
                break;
            case AudioCullingQuality.Low:
                _culling = new AudioCulling(CullDistantAudioLowQuality);
                break;
            default:
                break;
        }
    }

    //Culls audio thats far away when a close source wants to fire for low priority sfx and player sfx
    private void CullDistantAudioHighQuality(Vector3 newAudio, SoundType type)
    {
        if (_proximityPrioritization)
        {
            float farthestDistance = 0;
            Vector3 comparisonObjectPosition = Vector3.zero;
            switch (_proximityMode)
            {
                case ProximityMode.MainCamera:
                    comparisonObjectPosition = Camera.main.transform.position;
                    farthestDistance = Vector3.Distance(newAudio, comparisonObjectPosition);
                    break;
                case ProximityMode.CustomObject:

                    if (!_customObjectTransform)
                    {
                        Debug.LogWarning("The Audio Manager is set to proximity prioritization for a custom object, but no refrence to that object is set, defaulting to no proximity prioritization until this is set");
                        return;
                    }

                    comparisonObjectPosition = _customObjectTransform.position;
                    farthestDistance = Vector3.Distance(newAudio, comparisonObjectPosition);
                    break;
                default:
                    break;
            }

            float currentDistance = 0;
            int farthestIndex = -1;

            switch (type)
            {
                case SoundType.LowPrioritySoundEffect:
                    for (int i = 0; i < LowPrioritySFXPool.childCount; i++)
                    {
                        if(LowPrioritySFXPool.GetChild(i).gameObject.activeSelf)
                            currentDistance = Vector3.Distance(LowPrioritySFXPool.GetChild(i).transform.position, comparisonObjectPosition);


                        if (currentDistance > farthestDistance)
                        {
                            farthestDistance = currentDistance;
                            farthestIndex = i;
                        }
                    }
                    if(farthestIndex >= 0)
                    {
                        if (LowPrioritySFXPool.GetChild(farthestIndex).gameObject.TryGetComponent(out IAudioSource audioSourceScript))
                            audioSourceScript.CullSource();
                    }
                    break;
                case SoundType.PlayerSoundEffect:
                    for (int i = 0; i < PlayerSFXPool.childCount; i++)
                    {
                        if (PlayerSFXPool.GetChild(i).gameObject.activeSelf)
                            currentDistance = Vector3.Distance(PlayerSFXPool.GetChild(i).transform.position, comparisonObjectPosition);


                        if (currentDistance > farthestDistance)
                        {
                            farthestDistance = currentDistance;
                            farthestIndex = i;
                        }
                    }
                    if (farthestIndex >= 0)
                    {
                        if (PlayerSFXPool.GetChild(farthestIndex).gameObject.TryGetComponent(out IAudioSource audioSourceScript))
                            audioSourceScript.CullSource();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void CullDistantAudioLowQuality(Vector3 newAudio, SoundType type)
    {
        if (_proximityPrioritization)
        {
            float farthestDistance = 0;
            Vector3 comparisonObjectPosition = Vector3.zero;
            switch (_proximityMode)
            {
                case ProximityMode.MainCamera:
                    comparisonObjectPosition = Camera.main.transform.position;
                    farthestDistance = Vector3.Distance(newAudio, comparisonObjectPosition);
                    break;
                case ProximityMode.CustomObject:

                    if (!_customObjectTransform)
                    {
                        Debug.LogWarning("The Audio Manager is set to proximity prioritization for a custom object, but no refrence to that object is set, defaulting to no proximity prioritization until this is set");
                        return;
                    }

                    comparisonObjectPosition = _customObjectTransform.position;
                    farthestDistance = Vector3.Distance(newAudio, comparisonObjectPosition);
                    break;
                default:
                    break;
            }

            float currentDistance = 0;

            switch (type)
            {
                case SoundType.LowPrioritySoundEffect:
                    for (int i = 0; i < LowPrioritySFXPool.childCount; i++)
                    {
                        if (LowPrioritySFXPool.GetChild(i).gameObject.activeSelf)
                        {
                            currentDistance = Vector3.Distance(LowPrioritySFXPool.GetChild(i).transform.position, comparisonObjectPosition);

                            if (currentDistance > farthestDistance)
                            {
                                if (LowPrioritySFXPool.GetChild(i).gameObject.TryGetComponent(out IAudioSource audioSourceScript))
                                    audioSourceScript.CullSource();
                            }
                        }
                    }
                    break;
                case SoundType.PlayerSoundEffect:
                    for (int i = 0; i < PlayerSFXPool.childCount; i++)
                    {
                        if (PlayerSFXPool.GetChild(i).gameObject.activeSelf)
                        {
                            currentDistance = Vector3.Distance(PlayerSFXPool.GetChild(i).transform.position, comparisonObjectPosition);

                            if (currentDistance > farthestDistance)
                            {

                                if (PlayerSFXPool.GetChild(i).gameObject.TryGetComponent(out IAudioSource audioSourceScript))
                                    audioSourceScript.CullSource();
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    #endregion

    #region "Playing Sounds Functions"


    public void PlaySoundEffect(Vector3 position, SoundType soundType, string soundSettings, AudioClip audioClip)
    {
        //does the culling check for low priority and player audio
        _culling(position, soundType);

        //gets the current sound object, returns if there are none available
        GameObject currentSFXObject = GetSoundObject(soundType);
        if (currentSFXObject == null) return;

        //sets the position
        currentSFXObject.transform.position = position;

        //gets the audio source and sets it's settings
        AudioSource audioSource;
        if (currentSFXObject.TryGetComponent(out audioSource))
        {
            if(audioSource == null)
            {
                audioSource = currentSFXObject.AddComponent<AudioSource>();
            }
        }
        SetAudioSourceFromSoundSettings(audioSource, GetSoundSettingsByString(soundSettings));


        //adds the basic sound source script and sets it
        BasicAudioSource source = currentSFXObject.AddComponent<BasicAudioSource>();
        source.SetBasicAudioSourcePosition(audioClip, audioSource);


        //sets object to true to run
        currentSFXObject.SetActive(true);
    }
    public void PlaySoundEffect(Transform soundParent, SoundType soundType, string soundSettings, AudioClip audioClip)
    {
        //does the culling check for low priority and player audio
        _culling(soundParent.position, soundType);

        //gets the current sound object, returns if there are none available
        GameObject currentSFXObject = GetSoundObject(soundType);
        if (currentSFXObject == null) return;

        //sets the position
        currentSFXObject.transform.position = soundParent.transform.position;

        //gets the audio source and sets it's settings
        AudioSource audioSource;
        if (currentSFXObject.TryGetComponent(out audioSource))
        {
            if (audioSource == null)
            {
                audioSource = currentSFXObject.AddComponent<AudioSource>();
            }
        }
        SetAudioSourceFromSoundSettings(audioSource, GetSoundSettingsByString(soundSettings));


        //adds the basic sound source script and sets it
        BasicAudioSource source = currentSFXObject.AddComponent<BasicAudioSource>();
        source.SetBasicAudioSourceTransform(audioClip, audioSource, soundParent);


        //sets object to true to run
        currentSFXObject.SetActive(true);
    }


    public int PlayTrackedAudio(Vector3 position, SoundType soundType, string soundSettings, AudioClip audioClip, bool loopsInitialy)
    {
        //does the culling check for low priority and player audio
        _culling(position, soundType);

        //gets the current sound object, returns if there are none available
        GameObject currentSFXObject = GetSoundObject(soundType);
        if (currentSFXObject == null) return 0;

        //sets the position
        currentSFXObject.transform.position = position;

        //gets the audio source and sets it's settings
        AudioSource audioSource;
        if (currentSFXObject.TryGetComponent(out audioSource))
        {
            if (audioSource == null)
            {
                audioSource = currentSFXObject.AddComponent<AudioSource>();
            }
        }
        SetAudioSourceFromSoundSettings(audioSource, GetSoundSettingsByString(soundSettings));

        //increments key value
        _audioSourceIndexer++;

        //adds the basic sound source script and sets it
        TrackedAudioSource source = currentSFXObject.AddComponent<TrackedAudioSource>();
        source.SetTrackedAudioSourcePosition(audioClip, audioSource, _audioSourceIndexer);


        //sets object to true to run
        currentSFXObject.SetActive(true);

        //sets to loop initialy
        source.SetToLoop(loopsInitialy);

        //adds to dictionary returns key. 
        _trackedAudioDictionary.Add(_audioSourceIndexer, source);
        return _audioSourceIndexer;
    }
    public int PlayTrackedAudio(Transform soundParent, SoundType soundType, string soundSettings, AudioClip audioClip, bool loopsInitialy)
    {
        //does the culling check for low priority and player audio
        _culling(soundParent.position, soundType);

        //gets the current sound object, returns if there are none available
        GameObject currentSFXObject = GetSoundObject(soundType);
        if (currentSFXObject == null) return 0;

        //sets the position
        currentSFXObject.transform.position = soundParent.transform.position;

        //gets the audio source and sets it's settings
        AudioSource audioSource;
        if (currentSFXObject.TryGetComponent(out audioSource))
        {
            if (audioSource == null)
            {
                audioSource = currentSFXObject.AddComponent<AudioSource>();
            }
        }
        SetAudioSourceFromSoundSettings(audioSource, GetSoundSettingsByString(soundSettings));

        //increments key value
        _audioSourceIndexer++;

        //adds the basic sound source script and sets it
        TrackedAudioSource source = currentSFXObject.AddComponent<TrackedAudioSource>();
        source.SetTrackedAudioSourceTransform(audioClip, audioSource, soundParent, _audioSourceIndexer);


        //sets object to true to run
        currentSFXObject.SetActive(true);

        //sets to loop initialy
        source.SetToLoop(loopsInitialy);

        //adds to dictionary returns key. 
        _trackedAudioDictionary.Add(_audioSourceIndexer, source);
        return _audioSourceIndexer;
    }

    #endregion

    #region "Tracked Audio Adjustment Functions"
    /// <summary>
    /// Restarts the audio from the beggining
    /// </summary>
    /// <param name="trackedAudioIntKey"> Integer key that corespondes to a tracked audio, generated when played, don't lose it. </param>
    public static void RestartTrackedAudio(int trackedAudioIntKey)
    {
        if (_trackedAudioDictionary.TryGetValue(trackedAudioIntKey, out TrackedAudioSource audioSource))
        {
            audioSource.MoveAudioPlayHead(0);
        }
    }
    /// <summary>
    /// Changes the playhead of the tracked audio to the float inputed, clamped to clip length.
    /// </summary>
    /// <param name="trackedAudioIntKey"></param>
    /// <param name="timeStamp"> Integer key that corespondes to a tracked audio, generated when played, don't lose it. </param>
    public static void ChangeTrackedAudioPlayhead(int trackedAudioIntKey, float timeStamp)
    {
        if (_trackedAudioDictionary.TryGetValue(trackedAudioIntKey, out TrackedAudioSource audioSource))
        {
            audioSource.MoveAudioPlayHead(timeStamp);
        }
    }
    /// <summary>
    /// Set the tracked audio to loop or stop looping. Looping tracked audio will never stop playing unless told to stop looping and then it comes to an end naturaly, or manualy canceled through AudioManager.CancelTrackedAudio()
    /// </summary>
    /// <param name="trackedAudioIntKey"> Integer key that corespondes to a tracked audio, generated when played, don't lose it. </param>
    /// <param name="loop"> true to set to loop, false to stop looping </param>
    public static void SetTrackedAudioLooping(int trackedAudioIntKey, bool loop)
    {
        if (_trackedAudioDictionary.TryGetValue(trackedAudioIntKey, out TrackedAudioSource audioSource))
        {
            audioSource.SetToLoop(loop);
        }
    }
    /// <summary>
    /// Immediatly stopes the tracked audio, removes the soundkey and audio object from the dictionary until it is called again through InputManager.PlayTrackedAudio()
    /// </summary>
    /// <param name="audioKey"> Integer key that corespondes to a tracked audio, generated when played, don't lose it. </param>
    public static void CancelTrackedAudio(int audioKey)
    {
        if (_trackedAudioDictionary.TryGetValue(audioKey, out TrackedAudioSource audioSource))
        {
            _trackedAudioDictionary.Remove(audioKey);
            audioSource.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Checks if the tracked audio is playing. Returns true if it is, returns false if it isin't, or the inputed soundkey dosen't link up to any audio
    /// </summary>
    /// <param name="audioKey"></param>
    /// <returns></returns>
    public static bool CheckTrackedAudioIsPlaying(int audioKey)
    {
        if (_trackedAudioDictionary.TryGetValue(audioKey, out TrackedAudioSource audioSource))
        {
            return audioSource.GetIsPlaying();
        }

        return false;
    }

    /// <summary>
    /// Gives you direct acsess to an audiosource on a tracked audio object. Only use if you need to overide a setting like volume or priority directly and specificaly and don't want to make a new sound setting scriptable object. do not use this to track or change if the audio is playing, instead use functions from the Audio Manager. 
    /// </summary>
    /// <param name="audioKey"> Integer key that corespondes to a tracked audio, generated when played, don't lose it. </param>
    /// <returns></returns>
    public static AudioSource GetAudioSource(int audioKey)
    {
        if (_trackedAudioDictionary.TryGetValue(audioKey, out TrackedAudioSource audioSource))
        {
            return audioSource.GetAudiosourceForOverride();
        }
        return null;
    }

    #endregion
}

//Interface for both types of audio source
public interface IAudioSource
{
    public void CullSource();
}

//classes for both types of audio source, both use IAudioSource
public class BasicAudioSource : MonoBehaviour, IAudioSource
{
    private AudioClip _audioclip;
    private AudioSource _audiosource;

    private Transform _transform;

    private bool _trackingTransform;
    
    public void SetBasicAudioSourcePosition(AudioClip clip, AudioSource source)
    {
        _trackingTransform = false;
        _transform = null;

        _audioclip = clip;
        _audiosource = source;
    }
    public void SetBasicAudioSourceTransform(AudioClip clip, AudioSource source, Transform transform)
    {
        _trackingTransform = true;
        _transform = transform;

        _audioclip = clip;
        _audiosource = source;
    }

    private void Start()
    {
        if(_audiosource != null && _audioclip != null)
        {
            _audiosource.clip = _audioclip;
            _audiosource.Play();
            StartCoroutine(TrackPlayTime());
        }
        else
        {
            Debug.LogWarning("A sound effect was played with a null audio source or clip");
            gameObject.SetActive(false);
        }
    }

    private IEnumerator TrackPlayTime()
    {
        for (float t = 0; t < _audioclip.length; t += Time.deltaTime)
        {
            if(_trackingTransform && _transform != null)
            {
                transform.position = _transform.position;
            }

            yield return null;
        }
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Destroy(this);
    }

    public void CullSource()
    {
        gameObject.SetActive(false);
    }
}

public class TrackedAudioSource : MonoBehaviour, IAudioSource
{
    private AudioClip _audioclip;
    private AudioSource _audiosource;

    private Transform _transform;

    private bool _trackingTransform;

    //tracked specific
    private int _key;
    private bool _loopable;
    private bool _paused = false;

    public void SetTrackedAudioSourcePosition(AudioClip clip, AudioSource source, int key)
    {
        _key = key;

        _trackingTransform = false;
        _transform = null;

        _audioclip = clip;
        _audiosource = source;
    }
    public void SetTrackedAudioSourceTransform(AudioClip clip, AudioSource source, Transform transform, int key)
    {
        _key = key;

        _trackingTransform = true;
        _transform = transform;

        _audioclip = clip;
        _audiosource = source;
    }

    private void Start()
    {
        if (_audiosource != null && _audioclip != null)
        {
            _audiosource.clip = _audioclip;
            _audiosource.Play();
        }
        else
        {
            Debug.LogWarning("A sound effect was played with a null audio source or clip");
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        Destroy(this);
    }

    //tracked audio source functions

    //loops the audio, prevents it from stopping and going back to the pool indefinitly (use with care)
    public void SetToLoop(bool loop)
    {
        _loopable = loop;
        _audiosource.loop = loop;
    }
    //moves the audio playhead within the clip to the specified time, clamped for saftey
    public void MoveAudioPlayHead(float jumpToTime)
    {
        jumpToTime = Mathf.Clamp(jumpToTime, 0, _audioclip.length);
        _audiosource.time = jumpToTime;
    }

    //pauses or unpauses the audio clip
    public void PauseAudio()
    {
        if(!_paused)
        {
            _audiosource.Pause();
        }
    }
    public void ResumeAudio()
    {
        if(_paused)
        {
            _audiosource.Play();
        }
    }

    public bool GetIsPlaying()
    {
        return _audiosource.isPlaying;
    }

    public AudioSource GetAudiosourceForOverride()
    {
        return _audiosource;
    }

    private void Update()
    {
        if(_trackingTransform && _transform)
        {
            transform.position = _transform.position;
        }

        if(_loopable)
        {
            return;
        }
        if(!_audiosource.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }

    public void CullSource()
    {
        gameObject.SetActive(false);
    }
}
