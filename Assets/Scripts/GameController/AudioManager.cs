using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public bool musicOn = true;
    public bool soundOn = true;

    private float _musicVolume = 1f;
    private float _soundVolume = 1f;

    private GameObject audioObject;
    private AudioSource mainMusic;
    private List<AudioSource> sounds = new List<AudioSource>();

    private static AudioManager instance;

    public float musicVolume
    {
        get { return _musicVolume; }
        set
        {
            if (value != _musicVolume)
            {
                _musicVolume = value;
                if (mainMusic != null)
                    mainMusic.volume = value;
            }
        }
    }

    public float soundVolume
    {
        get { return _soundVolume; }
        set
        {
            if (value != _soundVolume)
            {
                _soundVolume = value;
                foreach (AudioSource src in sounds)
                    src.volume = _soundVolume;
            }
        }
    }

    // ===== Singleton: Gọi từ bất kỳ đâu bằng AudioManager.GetInstance() =====
    public static AudioManager GetInstance()
    {
        if (instance == null)
        {
            GameObject obj = new GameObject("AudioManager");
            DontDestroyOnLoad(obj); // Không bị xóa khi chuyển Scene
            instance = obj.AddComponent<AudioManager>();
            instance.audioObject = obj;
            instance.mainMusic = obj.AddComponent<AudioSource>();
        }
        return instance;
    }

    // ===== Nhạc nền (Background Music) =====

    /// <summary>Phát nhạc nền, mặc định lặp vô hạn.</summary>
    public void PlayMusic(AudioClip music, bool loop = true)
    {
        if (music == null) return;

        mainMusic.Stop();
        mainMusic.clip = music;
        mainMusic.loop = loop;
        mainMusic.volume = musicVolume;

        if (musicOn && Time.timeScale != 0)
            mainMusic.Play();
    }

    public void StopMusic()
    {
        mainMusic.Stop();
    }

    public void PauseMusic()
    {
        mainMusic.Pause();
    }

    public void ResumeMusic()
    {
        if (musicOn && Time.timeScale != 0)
            mainMusic.UnPause();
    }

    public void SetMusicOn(bool value)
    {
        musicOn = value;
        if (musicOn)
            ResumeMusic();
        else
            PauseMusic();
    }

    // ===== Hiệu ứng âm thanh (Sound Effects) =====

    /// <summary>Phát một hiệu ứng âm thanh, tự hủy sau khi phát xong.</summary>
    public AudioSource PlaySound(AudioClip sound, bool loop = false)
    {
        if (sound == null) return null;

        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.clip = sound;
        source.volume = soundVolume;
        source.loop = loop;
        sounds.Add(source);

        if (soundOn && Time.timeScale != 0)
            source.Play();

        if (!loop)
            StartCoroutine(DestroyAfterPlay(source, sound.length));

        return source;
    }

    public void StopSound(AudioSource sound)
    {
        if (sound != null)
            StartCoroutine(DestroyAfterPlay(sound, 0f));
    }

    public void StopAllSounds()
    {
        foreach (AudioSource src in sounds)
        {
            if (src != null)
            {
                src.Stop();
                Destroy(src);
            }
        }
        sounds.Clear();
    }

    private IEnumerator DestroyAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (source != null)
        {
            sounds.Remove(source);
            Destroy(source);
        }
    }
}
