using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundEffects : MonoBehaviour
{
    [Header("Sound Effects")]
    [Space(10)]
    [SerializeField] private List<AudioClip> playerSoundEffects = new List<AudioClip>();


    #region Audio
    private void PlayerSoundEffect(string clipName, string soundSettings)
    {
        AudioClip clip = null;
        for (int i = 0; i < playerSoundEffects.Count; i++)
        {
            if (playerSoundEffects[i].name == clipName)
                clip = playerSoundEffects[i];
        }
        AudioManager.Instance.PlaySoundEffect(transform, SoundType.PlayerSoundEffect, soundSettings, clip);
    }
    private void PlayerSoundEffect(AudioClip clipToPlay, string soundSettings)
    {
        AudioManager.Instance.PlaySoundEffect(transform, SoundType.PlayerSoundEffect, soundSettings, clipToPlay);
    }

    public void PlayFootstepAudio()
    {
        float selectedFootStep = Random.Range(1f, 2f);
        PlayerSoundEffect(playerSoundEffects[(int)selectedFootStep], "PlayerFootStepSoundEffects");
    }
    public void PlayJumpAudio()
    {
        PlayerSoundEffect("F.O.R-Jump1", "PlayerJumpSoundEffects");
    }
    public void PlayLandAudio()
    {
        PlayerSoundEffect(playerSoundEffects[1], "PlayerLandSoundEffects");
    }
    public void PlayDamageAudio()
    {
        PlayerSoundEffect("TubbyDamage", "PlayerDamageSoundEffects");
    }
    public void PlayDeathAudio()
    {
        PlayerSoundEffect("TubbyDeath", "PlayerDeathSoundEffects");
    }
    #endregion
}
