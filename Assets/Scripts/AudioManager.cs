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
                // add audio source - footsteps
                _instance._stepsSource = newManager.AddComponent<AudioSource>();
                // add audio source - SFX
                _instance._sfxSource = newManager.AddComponent<AudioSource>();

                // load music files
                _instance.LoadMusic();
                // load footsteps
                _instance.LoadFootsteps();
                // ensure all audio files are loaded from resources
                _instance.LoadSFX();
            }
            // return new/existing instance
            return _instance;
        }
    }

    private AudioSource _musicSource;
    private AudioSource _stepsSource;
    private AudioSource _sfxSource;

    #region Music / Ambient
    private void LoadMusic()
    {

    }
    #endregion

    #region Footsteps
    private AudioClip[] _footsteps;
    private AudioClip _prevFootstep = null;

    private void LoadFootsteps()
    {
        _footsteps = new AudioClip[6];
        _footsteps[0] = Resources.Load<AudioClip>("SFX/Step1");
        _footsteps[1] = Resources.Load<AudioClip>("SFX/Step2");
        _footsteps[2] = Resources.Load<AudioClip>("SFX/Step3");
        _footsteps[3] = Resources.Load<AudioClip>("SFX/Step4");
        _footsteps[4] = Resources.Load<AudioClip>("SFX/Step5");
        _footsteps[5] = Resources.Load<AudioClip>("SFX/Step6");
    }

    /// <summary>
    /// Attempts to play footstep sounds.
    /// Will not play a sound unless the previous footstep has finished.
    /// Will never play a duplicate footstep sequentially.
    /// </summary>
    public void TryPlayFootsteps(bool alwaysPlay = false)
    {
        // prevent layering two footstep sounds when jumping
        if (alwaysPlay)
            _stepsSource.Stop();

        // only do new footstep logic if the previous footstep has fully ended
        if (!alwaysPlay && _stepsSource.isPlaying)
            return;

        // create new list WITHOUT prev footstep to ensure consistent variety
        AudioClip[] noDupeList;
        if (_prevFootstep is not null)
        {
            int currIndex = 0;
            noDupeList = new AudioClip[_footsteps.Length - 1];
            foreach (AudioClip clip in _footsteps)
            {
                if (clip != _prevFootstep)
                {
                    noDupeList[currIndex] = clip;
                    currIndex++;
                }
            }
        }
        else
        {
            // this is the first footstep so we don't care
            noDupeList = _footsteps;
        }

        AudioClip newStep = noDupeList[Random.Range(0, noDupeList.Length)];
        _stepsSource.PlayOneShot(newStep, GameManager.GetSFXVolume());
        _prevFootstep = newStep;
    }

    public void PlayJumpSound()
    {
        _stepsSource.PlayOneShot(_footsteps[0], GameManager.GetSFXVolume());
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