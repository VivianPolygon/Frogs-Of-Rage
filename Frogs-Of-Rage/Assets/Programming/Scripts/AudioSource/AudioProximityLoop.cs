using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioProximityLoop : MonoBehaviour
{
    [Space(10)]
    public bool sourceOn;
    private bool _sourceOnTracking;

    private enum ProximityLoopSoundTypes
    {
        LowPriority,
        HighPriority
    }

    [SerializeField] ProximityLoopSoundTypes _soundPriority;
    private SoundType _soundPriorityCoverted;

    [Header("Hearing Ranges")]
    [SerializeField] [Min(1)] [Tooltip("Distance the player needs to be for audio source to start")]
    private float _startAudioRange = 1;

    [Header("Sound Settings")]
    [Space(10)]
    [SerializeField] private string _soundSettings;
    [SerializeField] private AudioClip _loopingClip;

    [Header("Range Detection")]
    [Space(10)]
    [SerializeField] [Range(1f, 10f)] private int _checksPerSecond = 2;


    private int _storedKey;
    private bool _audioIsPlaying;

    private WaitForSeconds _checkDelay;
    private Coroutine _distanceCheckCoroutine;


    private void Awake()
    {
        _soundPriorityCoverted = ConvertLoopType(_soundPriority);
        _checkDelay = new WaitForSeconds((11 - (float)_checksPerSecond) / 10);
        _sourceOnTracking = !sourceOn;
    }
    private void Update()
    {
        UpdateChecking();
    }

    private void UpdateChecking()
    {
        if(sourceOn != _sourceOnTracking)
        {
            if(sourceOn) // begins tracking
            {
                if(_distanceCheckCoroutine != null)
                {
                    StopCoroutine(_distanceCheckCoroutine);
                    _distanceCheckCoroutine = null;
                }
                StartCoroutine(DistanceCheckDelay());
            }
            else // stops tracking, stops audio
            {
                if (_distanceCheckCoroutine != null)
                {
                    StopCoroutine(_distanceCheckCoroutine);
                    _distanceCheckCoroutine = null;
                }
                StopAudio();
            }
        }
        _sourceOnTracking = sourceOn;
    }
    private IEnumerator DistanceCheckDelay()
    {
        while(sourceOn)
        {
            CheckDistances();

            yield return _checkDelay;
        }
    }
    private void CheckDistances()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _startAudioRange);

        if(colliders != null)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if(colliders[i].gameObject.tag == "Player")
                {
                    if(Vector3.Distance(colliders[i].transform.position, transform.position) <= _startAudioRange) // player is in range for audio start
                    {
                        StartAudio();
                        return;
                    }
                    else // player is in range for audio stop
                    {
                        StopAudio();
                        return;
                    }
                }
            }
        }

        StopAudio(); // stops if the player is outside the range
    }
    private void StartAudio()
    {
        if(!_audioIsPlaying)
        {
            _audioIsPlaying = true;
            _storedKey = AudioManager.Instance.PlayTrackedAudio(transform, _soundPriorityCoverted, _soundSettings, _loopingClip, true);
        }
    }
    private void StopAudio()
    {
        if(_audioIsPlaying)
        {
            _audioIsPlaying = false;
            AudioManager.CancelTrackedAudio(_storedKey);
        }
    }

    private SoundType ConvertLoopType(ProximityLoopSoundTypes proximityType)
    {
        switch (proximityType)
        {
            case ProximityLoopSoundTypes.LowPriority:
                return SoundType.LowPrioritySoundEffect;
            case ProximityLoopSoundTypes.HighPriority:
                return SoundType.HighPrioritySoundEffect;
            default:
                return SoundType.LowPrioritySoundEffect;
        }
    }
    private void OnDrawGizmos()
    {
        Color newColor = Color.yellow;
        newColor.a = 0.25f;

        Gizmos.color = newColor;
        Gizmos.DrawSphere(transform.position, _startAudioRange);
    }

    private void OnDisable()
    {
        StopAudio();
    }
}
