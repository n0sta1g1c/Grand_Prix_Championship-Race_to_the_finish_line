using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Sound[] MusicSounds, SFXSounds;
    public AudioSource MusicSource, SFXSource;

    [SerializeField] private AudioMixer AudioMixer;

    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider CarVolumeSlider;

    private bool MusicIsPlaying;

    private Scene scene;

    private void Start()
    {


        scene = SceneManager.GetActiveScene();
        if (scene.buildIndex == 1)
        {
            PlayMusic("Selection_Theme");
        }
        else if (scene.buildIndex == 0)
        {
            InitializePlayerPrefsVolume();
            PlayMusic("MainMenu_Theme");
        }
        else if (scene.buildIndex > 1)
        {
            InitializePlayerPrefsVolume();
            StartCoroutine(DelayedPlayMusic(3));
        }
    }

    /*private void Update()
    {
        if (scene.buildIndex > 1)
        {
            CheckIfGameIsInCountdownState();
        }

    }*/

    private void InitializePlayerPrefsVolume()
    {
        if (PlayerPrefs.HasKey("MusicVolume") || PlayerPrefs.HasKey("MasterVolume") || PlayerPrefs.HasKey("SFXVolume") || PlayerPrefs.HasKey("CarVolume"))
        {
            LoadVolumes();
        }
        else
        {
            SetMasterVolume();
            SetMusicVolume();
            SetSFXVolume();
            SetCarVolume();
        }
    }

    public void SetMasterVolume()
    {
        float volume = MasterSlider.value;
        AudioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume()
    {
        float volume = MusicSlider.value;
        AudioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        AudioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetCarVolume()
    {
        float volume = CarVolumeSlider.value;
        AudioMixer.SetFloat("CarVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("CarVolume", volume);
    }

    public void LoadVolumes()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        MasterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        CarVolumeSlider.value = PlayerPrefs.GetFloat("CarVolume");

        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
        SetCarVolume();
    }

    public IEnumerator FadeOutAudio(AudioSource audioSource, float fadeDuration)
    {
        if (audioSource == null) yield break;

        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null; // Wait for the next frame
        }

        audioSource.volume = 0; // Ensure it's fully set to zero
        audioSource.mute = true; // Optionally mute the audio entirely
    }

    /*private void CheckIfGameIsInCountdownState()
    {
        if (!GameManager.Instance.CountdownState && !MusicIsPlaying)
        {
            //Debug.Log("Music is playing");
            PlayMusic("Game_Theme");
            //Debug.Log("Game is paused");
            StopMusic("Game_Theme");
        }
        else
        {
            if (!MusicIsPlaying)
            {

            }

        }
    }*/

    private IEnumerator DelayedPlayMusic(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Apply the brake
        PlayMusic("Game_Theme");

        //Debug.Log("Brake applied after delay.");
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(MusicSounds, x => x.Name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            MusicIsPlaying = true;
            MusicSource.clip = s.Clip;
            MusicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(SFXSounds, x => x.Name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            SFXSource.PlayOneShot(s.Clip);
        }
    }

    public void StopMusic(string name)
    {
        Sound s = Array.Find(MusicSounds, x => x.Name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            MusicIsPlaying = false;
            MusicSource.clip = s.Clip;
            MusicSource.Stop();
        }

    }

}
