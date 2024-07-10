using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class VolumeSettings : MonoBehaviour
{
    public Slider musicSlider;
    public Slider effectSlider;
    bool sampleCooldown = false;
    
    void Start()
    {
        musicSlider.onValueChanged.AddListener(OnMusicSliderChange);
        effectSlider.onValueChanged.AddListener(OnAmbientSliderChange);
    }

    private void OnEnable()
    {
        musicSlider.value = GlobalSettings.musicVolume;
        effectSlider.value = GlobalSettings.effectVolume;
    }

    public void OnMusicSliderChange(float value)
    {
        GlobalSettings.musicVolume = value;
        GlobalSettingsManager.SaveSettings();
    }

    public void OnAmbientSliderChange(float value)
    {
        GlobalSettings.effectVolume = value;
        GlobalSettingsManager.SaveSettings();

        PlayAudioSample(() => AudioManager.PlaySoundEffect("Debug"));
    }

    public void PlayAudioSample(Action method)
    {
        if (!sampleCooldown)
        {
            method(); // Call the method passed as a parameter
            sampleCooldown = true;
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(1);
        sampleCooldown = false;
    }
}
