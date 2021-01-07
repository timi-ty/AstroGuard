using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager instance { get; private set; }
    #endregion

    #region Unity Runtime
    private void Awake()
    {
        #region Singleton
        if (!instance)
        {
            instance = this;
        }
        else if (!instance.Equals(this))
        {
            Destroy(gameObject);
        }
        #endregion
    }
    #endregion

    #region Components
    public AudioSource uiAudioSource;
    public AudioSource bgMusicSource;
    public AudioSource gamePlayAudioSource;
    public AudioSource ambienceAudioSource;
    #endregion

    #region Audio Clips
    public AudioClip uiButttonClip;
    public AudioClip uiPopAudioClip;
    public AudioClip celebrationClip;
    #endregion

    #region Properties
    private static bool isSfxEnabled => Settings.isSfxEnabled;
    private static bool isBgMusicEnabled => Settings.isBgMusicEnabled;
    #endregion

    private void Start()
    {
        CheckAudioSettings();
    }

    public void PlayUIButtonSFX()
    {
        if (!isSfxEnabled) return;

        uiAudioSource.PlayOneShot(uiButttonClip);
    }

    public void PlayUIPopSFX()
    {
        if (!isSfxEnabled) return;

        uiAudioSource.PlayOneShot(uiPopAudioClip);
    }

    public static void PlayUIClip(AudioClip audioClip)
    {
        if (!isSfxEnabled) return;

        instance.uiAudioSource.PlayOneShot(audioClip);
    }

    public static void PlayCelebrationSFX()
    {
        if (!isSfxEnabled) return;

        instance.uiAudioSource.PlayOneShot(instance.celebrationClip);
    }

    public static void PlayUILooping(AudioClip audioClip)
    {
        if (!isSfxEnabled) return;

        instance.uiAudioSource.clip = audioClip;
        instance.uiAudioSource.loop = true;
        instance.uiAudioSource.Play();
    }

    public static void StopUILooping()
    {
        instance.uiAudioSource.Stop();
        instance.uiAudioSource.loop = false;
        instance.uiAudioSource.clip = null;
    }

    public static void PlayGameClip(AudioClip audioClip)
    {
        if (!isSfxEnabled) return;

        instance.gamePlayAudioSource.PlayOneShot(audioClip);
    }

    public static void TurnUpBgMusic()
    {
        if (!isBgMusicEnabled) return;

        instance.StopAllCoroutines();

        instance.StartCoroutine(instance.FadeInAudio(instance.bgMusicSource, 0.4f));
    }

    public static void TurnDownBgMusic()
    {
        if (!isBgMusicEnabled) return;

        instance.StopAllCoroutines();

        instance.StartCoroutine(instance.FadeOutAudio(instance.bgMusicSource, 0.1f));
    }

    #region Utility Methods
    public static void CheckAudioSettings()
    {
        instance.bgMusicSource.enabled = isBgMusicEnabled;

        instance.uiAudioSource.enabled = isSfxEnabled;

        instance.ambienceAudioSource.enabled = isSfxEnabled;

        instance.gamePlayAudioSource.enabled = isSfxEnabled;
    }

    public static void FreezeAudio()
    {
        instance.bgMusicSource.enabled = false;

        instance.uiAudioSource.enabled = false;

        instance.ambienceAudioSource.enabled = false;

        instance.gamePlayAudioSource.enabled = false;
    }

    public static void UnfreezeAudio()
    {
        CheckAudioSettings();
    }

    private IEnumerator FadeInAudio(AudioSource audioSource, float final)
    {
        while(audioSource.volume < final)
        {
            audioSource.volume += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = final;
    }

    private IEnumerator FadeOutAudio(AudioSource audioSource, float final)
    {
        while (audioSource.volume > final)
        {
            audioSource.volume -= Time.deltaTime;
            yield return null;
        }
        audioSource.volume = final;
    }
    #endregion
}
