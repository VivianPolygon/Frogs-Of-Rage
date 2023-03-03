using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private string soundSettings;

    private bool _isPlayingSound = false;

    private int soundKey;

    private void PlayClip()
    {
        //sound is set to looping here initialy
        soundKey = AudioManager.Instance.PlayTrackedAudio(transform, SoundType.LowPrioritySoundEffect, soundSettings, clip, true);
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();

        //button for starting audio
        if(!_isPlayingSound)
        {
            if (GUILayout.Button("Play Tracked Sound") && !_isPlayingSound)
            {
                PlayClip();
            }
        }
        if(_isPlayingSound)
        {
            GUILayout.BeginHorizontal();
            //button for ending loop
            if (GUILayout.Button("Stop Looping"))
            {
                AudioManager.SetTrackedAudioLooping(soundKey, false);
            }
            //button for starting loop
            if (GUILayout.Button("Start Looping"))
            {
                AudioManager.SetTrackedAudioLooping(soundKey, true);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            //restarts the audio from the beggining
            if (GUILayout.Button("Restart Audio"))
            {
                // restarts the audio from the begginuing, exact same thing as: AudioManager.ChangeAdjustableAudioPlayhead(soundkey, 0);
                AudioManager.RestartTrackedAudio(soundKey); 
            }
            //ends the audio
            if (GUILayout.Button("End Audio"))
            {
                AudioManager.CancelTrackedAudio(soundKey);
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            //restarts the audio from the beggining
            if (GUILayout.Button("Jump to 1 second"))
            {
                AudioManager.ChangeTrackedAudioPlayhead(soundKey, 1);
            }
            //ends the audio
            if (GUILayout.Button("Jump to 3 seconds"))
            {
                AudioManager.ChangeTrackedAudioPlayhead(soundKey, 3);
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUILayout.TextArea("You can also get direct acsess to the audio source");
            GUILayout.TextArea("Use conservativly, the sound setting scriptable objects do this easier");
            GUILayout.EndVertical();

            //example sof what you can do, any functions you can do with an audio source are acessible of course, but this should more be used to mess with these type of settings if you need a very specif one off sound setting, or to pitch/volume shift somthing gradualy
            //control of if an audio is playing or looping or where its at can be done directly too, but should be done using the audio manager functions, as they null check for you.

            #region "Example Direct Functions"
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Volume Down"))
            {
                AudioManager.GetAudioSource(soundKey).volume -= 0.1f;
            }
            if (GUILayout.Button("Volume Up"))
            {
                AudioManager.GetAudioSource(soundKey).volume += 0.1f;
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Pitch Down"))
            {
                AudioManager.GetAudioSource(soundKey).pitch -= 0.5f;
            }
            if (GUILayout.Button("Pitch Up"))
            {
                AudioManager.GetAudioSource(soundKey).pitch += 0.5f;
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Spacial Blend: 2D"))
            {
                AudioManager.GetAudioSource(soundKey).spatialBlend = 0;
            }
            if (GUILayout.Button("Spacial Blend: 3D"))
            {
                AudioManager.GetAudioSource(soundKey).spatialBlend = 1;
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Min Distance Down"))
            {
                AudioManager.GetAudioSource(soundKey).minDistance -= 5;
            }
            if (GUILayout.Button("Min Distance Up"))
            {
                AudioManager.GetAudioSource(soundKey).minDistance += 5;
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Max Distance Down"))
            {
                AudioManager.GetAudioSource(soundKey).maxDistance -= 5;
            }
            if (GUILayout.Button("Max Distance Up"))
            {
                AudioManager.GetAudioSource(soundKey).maxDistance += 5;
            }
            GUILayout.EndVertical();
            #endregion
        }

        GUILayout.EndHorizontal();
    }

    private void Update()
    {
        //checks if the sound from the soundkey is currently playing. used here for drawing appropiate ONGUI    
        _isPlayingSound = AudioManager.CheckTrackedAudioIsPlaying(soundKey);
    }

}
