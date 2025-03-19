using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioClip[] _musicClips, _soundEffectClips;
    [SerializeField] private AudioClip[] _loopMusic;

    [SerializeField] private AudioSource _musicSource, _soundEffectSource;

    private bool _isFocused = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (_isFocused && !_musicSource.isPlaying)
        {
            PlayNewMusic();
        }
    }

    public void PlayMusic(string name)
    {
        AudioClip clip = Array.Find(_musicClips, s => s.name == name);

        if (clip == null)
        {
            print("clip not found");
        }
        else
        {
            _musicSource.clip = clip;
            _musicSource.Play();
        }
    }

    public void PlaySoundEffect(string name)
    {
        AudioClip clip = Array.Find(_soundEffectClips, s => s.name == name);

        if (clip == null)
        {
            print("clip not found");
        }
        else
        {
            _soundEffectSource.pitch = 1.0f;
            _soundEffectSource.clip = clip;
            _soundEffectSource.Play();
        }
    }

    public void PlaySoundEffect(string name, float pitchBendRange = 0.0f)
    {
        //should only really pass in pitchBendRange values < 0.3f

        pitchBendRange = Mathf.Clamp(pitchBendRange, 0.0f, 3.0f);
        AudioClip clip = Array.Find(_soundEffectClips, s => s.name == name);

        if (clip == null)
        {
            print("clip not found");
        }
        else
        {
            float randomPitchBend = UnityEngine.Random.Range(-1 * pitchBendRange, pitchBendRange);
            _soundEffectSource.pitch = 1.0f + randomPitchBend;
            _soundEffectSource.clip = clip;
            _soundEffectSource.Play();
        }
    }

    public AudioClip[] GetMusicClips()
    {
        return _musicClips;
    }

    public AudioClip[] GetLoopsOneThroughEightAudioClips()
    {
        return _loopMusic;
    }

    private void OnApplicationFocus(bool focusState)
    {
        _isFocused = focusState;
    }

    public void PlayNewMusic()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            PlayMusic("the_heron");
        }
        else
        {
            AudioClip musicClip = _loopMusic[UnityEngine.Random.Range(0, _loopMusic.Length)];
            PlayMusic(musicClip.name);
        }
    }
}
