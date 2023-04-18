using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundEffects : MonoBehaviour
{
    [Header("Sound Effects")]
    [Space(10)]
    [SerializeField] private List<AudioClip> playerSoundEffects = new List<AudioClip>();


    #region Audio
    private void PlayerSoundEffect(string clipName)
    {
        AudioClip clip = null;
        for (int i = 0; i < playerSoundEffects.Count; i++)
        {
            if (playerSoundEffects[i].name == clipName)
                clip = playerSoundEffects[i];
        }
        AudioManager.Instance.PlaySoundEffect(transform, SoundType.PlayerSoundEffect, "PlayerSoundEffects", clip);
    }
    private void PlayerSoundEffect(AudioClip clipToPlay)
    {
        AudioManager.Instance.PlaySoundEffect(transform, SoundType.PlayerSoundEffect, "PlayerSoundEffects", clipToPlay);
    }

    public void PlayFootstepAudio()
    {
        int selectedFootStep = Random.Range(2, 4);
        PlayerSoundEffect(playerSoundEffects[selectedFootStep]);
    }
    public void PlayJumpAudio()
    {
        PlayerSoundEffect("F.O.R-Jump1");
    }
    public void PlayLandAudio()
    {
        PlayerSoundEffect(playerSoundEffects[0]);
    }

    #endregion
}
