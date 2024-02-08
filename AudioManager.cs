using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public DataManagerScript dataManager;
    public AudioSource musicPlayer;
    public List<AudioClip> bgMusic;
    public List<AudioClip> uiEffects;
    public List<AudioClip> ambientEffects;

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

                if (musicPlayer.clip != null)
                {
                    musicPlayer.Play();
                }
            }
        }
    }

    public static void PlayUISound(string soundName)
    {
        var clipList = FindObjectOfType<AudioManager>().uiEffects;
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
            audioSource.volume = GlobalSettings.uiVolume;
            Destroy(audioSource.gameObject, sound.length);
        }
    }

    public static void PlayAmbientSound(string soundName)
    {
        var clipList = FindObjectOfType<AudioManager>().ambientEffects;
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
            audioSource.volume = GlobalSettings.ambientVolume;
            Destroy(audioSource.gameObject, sound.length);
        }
    }
}
