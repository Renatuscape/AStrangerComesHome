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

    public static void SkipNow()
    {
        if (instance != null)
        {
            instance.musicPlayer.Stop();
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
        var clipList = FindObjectOfType<AudioManager>().soundEffects;

        if (clipList != null)
        {
            AudioClip sound;

            if (soundName.ToLower() == "debug")
            {
                sound = clipList[Random.Range(1, clipList.Count)];
            }
            else
            {
                sound = clipList.FirstOrDefault(x => x.name == soundName);
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
