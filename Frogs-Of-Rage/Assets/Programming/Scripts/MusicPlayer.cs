using UnityEngine;
using System.Collections.Generic;

public class MusicPlayer : MonoBehaviour
{
    public UIManager uiManager;
    public List<AudioClip> mainGameMusic;
    public List<AudioClip> hatMenuMusic;
    public List<AudioClip> mainMenuMusic;
    public List<AudioClip> winGameMusic;

    private AudioSource audioSource;
    private List<AudioClip> currentMusicList;
    private CanvasState previousState;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentMusicList = mainMenuMusic;
        previousState = uiManager.state;
        UpdateMusicList();
        PlayRandomClip();
    }

    void Update()
    {
        UpdateMusicList();

        if (!audioSource.isPlaying && (previousState != CanvasState.Paused || uiManager.state != CanvasState.Player))
        {
            PlayRandomClip();
        }
    }

    void UpdateMusicList()
    {
        CanvasState currentState = uiManager.state;

        if (currentState != previousState)
        {
            switch (currentState)
            {
                case CanvasState.Start:
                    currentMusicList = mainMenuMusic;
                    PlayRandomClip();
                    break;
                case CanvasState.Player:
                    if (previousState != CanvasState.Paused)
                    {
                        currentMusicList = mainGameMusic;
                        PlayRandomClip();
                    }
                    break;
                case CanvasState.HatMenu:
                    currentMusicList = hatMenuMusic;
                    PlayRandomClip();
                    break;
                case CanvasState.Win:
                    currentMusicList = winGameMusic;
                    PlayRandomClip();
                    break;
                default:
                    currentMusicList = mainGameMusic;
                    break;
            }
            previousState = currentState;
        }
    }

    void PlayRandomClip()
    {
        int randomIndex = Random.Range(0, currentMusicList.Count);
        audioSource.clip = currentMusicList[randomIndex];
        audioSource.Play();
    }
}
