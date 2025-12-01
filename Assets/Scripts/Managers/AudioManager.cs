using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;
    public AudioSource musicSource; // Müzik kaynaðý burada ama fonksiyonu yoktu.

    [Header("UI Sounds")]
    public AudioClip uiHoverClip;
    public AudioClip uiClickClip;
    public AudioClip levelUpClip;
    public AudioClip battleStartClip;
    public AudioClip victoryClip;
    public AudioClip defeatClip;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void PlayHover() => PlaySFX(uiHoverClip);
    public void PlayClick() => PlaySFX(uiClickClip);
    public void PlayLevelUp() => PlaySFX(levelUpClip);
    public void PlayBattleStart() => PlaySFX(battleStartClip);
    public void PlayVictory() => PlaySFX(victoryClip);
    public void PlayDefeat() => PlaySFX(defeatClip);
}