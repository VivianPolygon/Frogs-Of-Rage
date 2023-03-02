using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] [Min(0)] private int _lowPrioritySoundEffectLimit = 10;
    [SerializeField] [Min(0)] private int _playerSoundEffectLimit = 4;
    [Space(10)]
    [SerializeField] private SoundSettings[] _soundSettings;
    [SerializeField] private SoundSettings _defaultSoundSettings;

    #region "Sound Pool Variables"
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

    Dictionary<int, TrackedAudioSource> _adjustableAudioDictionary;

    public override void Awake()
    {
        base.Awake();
    }

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
            if(!poolParent.GetChild(i).gameObject.activeSelf)
            {
                return poolParent.GetChild(i).gameObject;
            }
        }
        if(createNewWhenFull)
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

    private SoundSettings GetSoundSettingsByString(string settingsName)
    {
        for (int i = 0; i < _soundSettings.Length; i++)
        {
            if(settingsName == _soundSettings[i].name)
            {
                return _soundSettings[i];
            }
        }
        Debug.LogWarning("No Sound Setting by the name: <b>" + settingsName + "</b>");
        return _defaultSoundSettings;
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

    #region "Playing Sounds Functions"


    public void PlaySoundEffect(Vector3 position, SoundType soundType, string soundSettings, AudioClip audioClip)
    {
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


    public int PlayAdjustableAudio(Vector3 position, SoundType soundType, bool loops, string soundSettings, AudioClip audioClip)
    {
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


        //adds the basic sound source script and sets it
        BasicAudioSource source = currentSFXObject.AddComponent<BasicAudioSource>();
        source.SetBasicAudioSourcePosition(audioClip, audioSource);


        //sets object to true to run
        currentSFXObject.SetActive(true);

        return 0;
    }
    public int PlayAdjustableAudio(Transform soundParent, SoundType soundType, bool loops, string soundSettings, AudioClip audioClip)
    {


        return 0;
    }

    #endregion


    #region "Adjustable Audio adjustment Functions"
    public void StopAdjustableAudio(int adjustableAudioIntKey)
    {

    }
    public void RestartAdjustableAudio(int adjustableAudioIntKey)
    {

    }
    public void ChangeAdjustableAudioPlayhead(int adjustableAudioIntKey, float timeStamp)
    {

    }
    public void SetAdjustableAudioToLoop(int adjustableAudioIntKey, bool loop)
    {

    }

    #endregion

}

public class BasicAudioSource : MonoBehaviour
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
}

public class TrackedAudioSource : MonoBehaviour
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
        if (_audiosource != null && _audioclip != null)
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
            if (_trackingTransform && _transform != null)
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
}
