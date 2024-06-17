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

    public List<AudioSource> audioPool;
    public List<AudioSource> playersInUse;

    float cooldownTimer;
    bool effectAudioCooldown = true;

    private void Start()
    {
        instance = this;
        CreateAudioPool(200);
    }

    private void OnEnable()
    {
        musicPlayer.Stop();
        cooldownTimer = 0;
        effectAudioCooldown = true;
        StartCoroutine(EffectAudioCooldown());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        musicPlayer.Stop();
        effectAudioCooldown = true;
    }

    IEnumerator EffectAudioCooldown()
    {

        while (effectAudioCooldown)
        {
            yield return null;
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer > 1.5f)
            {
                effectAudioCooldown = false;
            }
        }
    }

    void CreateAudioPool(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var audioSource = new GameObject().AddComponent<AudioSource>();
            audioSource.gameObject.name = "AudioSource";
            audioSource.gameObject.transform.SetParent(transform, false);
            audioPool.Add(audioSource);
        }
    }

    void Update()
    {
        if (TransientDataScript.GameState != GameState.MainMenu && TransientDataScript.GameState != GameState.Loading)
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

    public static AudioClip GetSoundEffect(string soundName)
    {
        if (instance != null)
        {
            var clip = instance.soundEffects.FirstOrDefault(x => x.name == soundName);

            if (clip == null)
            {
                Debug.Log("Could not find sound by the name " + soundName);
            }
            return clip;
        }
        else return null;
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

    public static void PlaySoundEffect(string soundName, float volumeAdjustment = 0)
    {
        if (instance != null && !instance.effectAudioCooldown)
        {
            instance.PlaySoundEffect(soundName, "ui", volumeAdjustment);
        }
    }

    //public static void PlayAmbientSound(string soundName, float volumeAdjustment = 0)
    //{
    //    if (instance != null)
    //    {
    //        instance.PlaySoundEffect(soundName, "ambient", volumeAdjustment);
    //    }
    //}

    void PlaySoundEffect(string soundName, string type, float volumeAdjustment)
    {
        if (!gameObject.scene.isLoaded) return;

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
                if (audioPool.Count > 0)
                {
                    var audioSource = audioPool[0];
                    audioPool.RemoveAt(0);
                    playersInUse.Add(audioSource);

                    audioSource.clip = sound;

                    if (type == "ambient")
                    {
                        audioSource.volume = GlobalSettings.ambientVolume + volumeAdjustment;
                    }
                    else
                    {
                        audioSource.volume = GlobalSettings.uiVolume + volumeAdjustment;
                    }

                    audioSource.Play();
                }

                if (audioPool.Count < 10)
                {
                    RecycleAudio();
                }
            }
        }
    }

    void RecycleAudio()
    {
        for (int i = playersInUse.Count - 1; i >= 0; i--)
        {
            var audio = playersInUse[i];

            if (!audio.isPlaying)
            {
                playersInUse.Remove(audio);
                audioPool.Add(audio);
            }
        }
    }
}
