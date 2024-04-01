using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public DataManagerScript dataManager;
    public AudioSource musicPlayer;
    public List<AudioClip> bgMusic;
    public List<AudioClip> soundEffects;

    private void Start()
    {
        instance = this;
    }

    private void OnEnable()
    {
        musicPlayer.Stop();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        musicPlayer.Stop();
    }

    void Update()
    {
        if (TransientDataScript.GameState != GameState.MainMenu)
        {
            musicPlayer.volume = GlobalSettings.musicVolume;

            if (!musicPlayer.isPlaying)
            {
                var time = dataManager.timeOfDay;

                if (dataManager.currentRegion == "REGION2")
                {
                    musicPlayer.clip = bgMusic.FirstOrDefault(x => x.name == "Contemplating Mountains");
                }
                
                if (dataManager.currentRegion == "REGION0" || dataManager.currentRegion == "REGION1")
                {
                    if (time >= 0.3f && time < 0.6f)
                    {
                        musicPlayer.clip = bgMusic.FirstOrDefault(x => x.name == "Bright Morning");
                    }
                    else if (time >= 0.6f && time < 0.9f)
                    {
                        musicPlayer.clip = bgMusic.FirstOrDefault(x => x.name == "Sweet Evening");
                    }
                    else
                    {
                        musicPlayer.clip = bgMusic.FirstOrDefault(x => x.name == "Pensive Night");
                    }
                }

                if (musicPlayer.clip != null)
                {
                    musicPlayer.Play();
                }
            }
        }
    }

    public static void FadeToStop()
    {
        if (instance != null)
        {
            instance.FadeVolume();
        }
    }

    IEnumerator FadeVolume()
    {
        while (musicPlayer.isPlaying)
        {
            yield return new WaitForSeconds(0.2f);
            musicPlayer.volume -= 0.1f;

            if (musicPlayer.volume <= 0)
            {
                instance.musicPlayer.Stop();
            }
        }
    }

    public static void PlayUISound(string soundName)
    {
        instance.PlaySoundEffect(soundName, "ui");
    }

    public static void PlayAmbientSound(string soundName)
    {
        if (instance != null)
        {
            instance.PlaySoundEffect(soundName, "ambient");
        }
    }

    void PlaySoundEffect(string soundName, string type)
    {
        if (soundEffects != null)
        {
            AudioClip sound;

            if (soundName.ToLower() == "debug")
            {
                sound = soundEffects[Random.Range(1, soundEffects.Count)];
            }
            else
            {
                sound = soundEffects.FirstOrDefault(x => x.name == soundName);
            }


            if (sound != null)
            {
                var audioSource = new GameObject().AddComponent<AudioSource>();
                audioSource.clip = sound;
                audioSource.Play();

                if (type == "ambient")
                {
                    audioSource.volume = GlobalSettings.ambientVolume;
                }
                else
                {
                    audioSource.volume = GlobalSettings.uiVolume;
                }
                Destroy(audioSource.gameObject, sound.length);
            }
        }
    }
}
