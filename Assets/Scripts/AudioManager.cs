using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows for static calls for all sound play calls
/// </summary>
public class AudioManager : MonoBehaviour
{
    // private singleton instance
    private static AudioManager _instance;

    // public accessor of instance
    public static AudioManager Instance
    {
        get
        {
            // setup GameManager as a singleton class
            if (_instance == null)
            {
                // create new game manager object
                GameObject newManager = new();
                newManager.name = "[Audio Manager]";
                DontDestroyOnLoad(newManager);
                newManager.AddComponent<AudioManager>();
                _instance = newManager.GetComponent<AudioManager>();

                // add audio source - Music
                _instance._musicSource = newManager.AddComponent<AudioSource>();
                _instance._musicSource.volume = GameManager.GetMusicVolume();
                _instance._musicSource.loop = true;
                // add audio source - SFX
                _instance._sfxSource = newManager.AddComponent<AudioSource>();

                // load music files
                _instance.LoadMusic();
                // ensure all audio files are loaded from resources
                _instance.LoadSFX();
            }
            // return new/existing instance
            return _instance;
        }
    }

    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    #region Music / Ambient
    private void LoadMusic()
    {

    }
    #endregion

    #region SFX
    private AudioClip _clickUI;
    private AudioClip _sliderClick;

    private void LoadSFX()
    {
        _clickUI = Resources.Load<AudioClip>("SFX/ClickUI");
        _sliderClick = Resources.Load<AudioClip>("SFX/SliderClick");
    }

    public void PlayClickUI()
    {
        _sfxSource.PlayOneShot(_clickUI, GameManager.GetSFXVolume());
    }

    public void PlaySliderClick()
    {
        // only play slider click if not already playing a sound - this prevents insane click speed sounds that get REALLY loud
        if (!_sfxSource.isPlaying)
            _sfxSource.PlayOneShot(_sliderClick, GameManager.GetSFXVolume());
    }
    #endregion
}