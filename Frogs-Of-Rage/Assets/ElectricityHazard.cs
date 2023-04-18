using System.Collections.Generic;
using UnityEngine;

public class ElectricityHazard : MonoBehaviour
{
    public List<AudioClip> hazardSounds;
    public SphereCollider otherObjectCollider;
    public List<Light> lights;
    public List<Animator> animator;

    private AudioSource audioSource;
    private bool isHazardActive;
    private int currentSoundIndex;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        isHazardActive = true;
        currentSoundIndex = 0;
        PlayNextHazardSound();
        SetLightsActive(true);
    }

    void Update()
    {
        if (otherObjectCollider.enabled && isHazardActive)
        {
            ToggleHazard();
        }
        else if (!otherObjectCollider.enabled && !isHazardActive)
        {
            ToggleHazard();
        }

        if (isHazardActive && !audioSource.isPlaying)
        {
            PlayNextHazardSound();
        }
    }

    private void ToggleHazard()
    {
        isHazardActive = !isHazardActive;

        if (isHazardActive)
        {
            PlayNextHazardSound();
            SetLightsActive(true);
        }
        else
        {
            audioSource.Stop();
            SetLightsActive(false);
        }
    }

    private void PlayNextHazardSound()
    {
        audioSource.clip = hazardSounds[currentSoundIndex];
        audioSource.Play();

        currentSoundIndex++;
        if (currentSoundIndex >= hazardSounds.Count)
        {
            currentSoundIndex = 0;
        }
    }

    private void SetLightsActive(bool active)
    {
        foreach (Light light in lights)
        {
            light.enabled = active;
        }

        foreach (Animator animator in animator)
        {
            animator.enabled = active;
        }
    }
}
