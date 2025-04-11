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
                // add audio source - charge stun
                _instance._chargeStunSource = newManager.AddComponent<AudioSource>();
                // add audio source - SFX
                _instance._sfxSource = newManager.AddComponent<AudioSource>();

                // load music files
                _instance.LoadMusic();
                // load footsteps
                _instance.LoadFootsteps();
                // load charge stun
                _instance.LoadChargeStun();
                // ensure all audio files are loaded from resources
                _instance.LoadSFX();
            }
            // return new/existing instance
            return _instance;
        }
    }

    private AudioSource _musicSource;
    private AudioSource _stepsSource;
    private AudioSource _chargeStunSource;
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

    #region Charge Stun
    private AudioClip _chargeStun;

    private void LoadChargeStun()
    {
        _chargeStun = Resources.Load<AudioClip>("SFX/ChargeStun");
    }

    public void PlayChargeStun()
    {
        // prevent stacking up many one-shot charge sounds
        _chargeStunSource.Stop();

        // start playing in normal order
        _chargeStunSource.pitch = 1;
        _chargeStunSource.PlayOneShot(_chargeStun, GameManager.GetSFXVolume());
    }

    public void StopChargeStunSFX()
    {
        _chargeStunSource.pitch = -1;
    }
    #endregion

    #region SFX
    private AudioClip _clickUI;
    private AudioClip _sliderClick;

    private AudioClip _tabCycle;
    private AudioClip _generalSoundHUD;
    private AudioClip _logOpen;
    private AudioClip _logClose;

    private AudioClip[] _pickups;

    private AudioClip _flashlightClickDown;
    private AudioClip _flashlightClickUp;
    private AudioClip _flashlightStun;

    private AudioClip[] _lightButtonPress;
    private AudioClip _terminalBoot;
    private AudioClip _terminalOpen;
    private AudioClip _terminalClose;
    private AudioClip _powerZone;
    private AudioClip _unpowerZone;
    private AudioClip _terminalArrow;
    private AudioClip _gridOverload;

    private AudioClip _boxOpen;
    private AudioClip _boxClose;
    private AudioClip _boxUnlatch;
    private AudioClip _boxLatch;
    private AudioClip _wireSelect;
    private AudioClip[] _generalZap;
    private AudioClip _removeZap;
    private AudioClip _boxFix;

    private void LoadSFX()
    {
        _clickUI = Resources.Load<AudioClip>("SFX/ClickUI");
        _sliderClick = Resources.Load<AudioClip>("SFX/SliderClick");

        _tabCycle = Resources.Load<AudioClip>("SFX/TabCycle");
        _generalSoundHUD = Resources.Load<AudioClip>("SFX/GeneralSoundHUD");
        _logOpen = Resources.Load<AudioClip>("SFX/LogOpen");
        _logClose = Resources.Load<AudioClip>("SFX/LogClose");

        _pickups = new AudioClip[4];
        _pickups[0] = Resources.Load<AudioClip>("SFX/Pickup1");
        _pickups[1] = Resources.Load<AudioClip>("SFX/Pickup2");
        _pickups[2] = Resources.Load<AudioClip>("SFX/Pickup3");
        _pickups[3] = Resources.Load<AudioClip>("SFX/Pickup4");

        _flashlightClickDown = Resources.Load<AudioClip>("SFX/FlashlightClickDown");
        _flashlightClickUp = Resources.Load<AudioClip>("SFX/FlashlightClickUp");
        _flashlightStun = Resources.Load<AudioClip>("SFX/FlashlightStun");

        _lightButtonPress = new AudioClip[3];
        _lightButtonPress[0] = Resources.Load<AudioClip>("SFX/LightButton1");
        _lightButtonPress[1] = Resources.Load<AudioClip>("SFX/LightButton2");
        _lightButtonPress[2] = Resources.Load<AudioClip>("SFX/LightButton3");
        _terminalBoot = Resources.Load<AudioClip>("SFX/TerminalBoot");
        _terminalOpen = Resources.Load<AudioClip>("SFX/TerminalOpen");
        _terminalClose = Resources.Load<AudioClip>("SFX/TerminalClose");
        _powerZone = Resources.Load<AudioClip>("SFX/PowerZone");
        _unpowerZone = Resources.Load<AudioClip>("SFX/UnpowerZone");
        _terminalArrow = Resources.Load<AudioClip>("SFX/TerminalArrow");
        _gridOverload = Resources.Load<AudioClip>("SFX/GridOverload");

        _boxOpen = Resources.Load<AudioClip>("SFX/BoxOpen");
        _boxClose = Resources.Load<AudioClip>("SFX/BoxClose");
        _boxUnlatch = Resources.Load<AudioClip>("SFX/BoxUnlatch");
        _boxLatch = Resources.Load<AudioClip>("SFX/BoxLatch");
        _wireSelect = Resources.Load<AudioClip>("SFX/WireSelect");
        _generalZap = new AudioClip[3];
        _generalZap[0] = Resources.Load<AudioClip>("SFX/Zap1");
        _generalZap[1] = Resources.Load<AudioClip>("SFX/Zap2");
        _generalZap[2] = Resources.Load<AudioClip>("SFX/Zap3");
        _removeZap = Resources.Load<AudioClip>("SFX/Zap4");
        _boxFix = Resources.Load<AudioClip>("SFX/BoxFix");
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

    public void PlayTabCycle()
    {
        _sfxSource.PlayOneShot(_tabCycle, GameManager.GetSFXVolume());
    }

    public void PlayGeneralSoundHUD()
    {
        _sfxSource.PlayOneShot(_generalSoundHUD, GameManager.GetSFXVolume());
    }

    public void PlayLogOpen()
    {
        _sfxSource.PlayOneShot(_logOpen, GameManager.GetSFXVolume());
    }

    public void PlayLogClose()
    {
        _sfxSource.PlayOneShot(_logClose, GameManager.GetSFXVolume());
    }

    public void PlayPickup()
    {
        _sfxSource.PlayOneShot(_pickups[Random.Range(0, _pickups.Length)], GameManager.GetSFXVolume());
    }

    public void PlayFlashlightClickDown()
    {
        _sfxSource.PlayOneShot(_flashlightClickDown, GameManager.GetSFXVolume());
    }

    public void PlayFlashlightClickUp()
    {
        _sfxSource.PlayOneShot(_flashlightClickUp, GameManager.GetSFXVolume());
    }

    public void PlayFlashlightStun()
    {
        _sfxSource.PlayOneShot(_flashlightStun, GameManager.GetSFXVolume());
    }

    public void PlayLightButtonPress()
    {
        _sfxSource.PlayOneShot(_lightButtonPress[Random.Range(0, _lightButtonPress.Length)], GameManager.GetSFXVolume());
    }
    public void PlayTerminalBoot()
    {
        _sfxSource.PlayOneShot(_terminalBoot, GameManager.GetSFXVolume());
    }

    public void PlayTerminalOpen()
    {
        _sfxSource.PlayOneShot(_terminalOpen, GameManager.GetSFXVolume());
    }

    public void PlayTerminalClose()
    {
        _sfxSource.PlayOneShot(_terminalClose, GameManager.GetSFXVolume());
    }

    public void PlayPowerZone()
    {
        _sfxSource.PlayOneShot(_powerZone, GameManager.GetSFXVolume());
    }

    public void PlayUnpowerZone()
    {
        _sfxSource.PlayOneShot(_unpowerZone, GameManager.GetSFXVolume());
    }

    public void PlayTerminalArrow()
    {
        _sfxSource.PlayOneShot(_terminalArrow, GameManager.GetSFXVolume());
    }

    public void PlayGridOverload()
    {
        _sfxSource.PlayOneShot(_gridOverload, GameManager.GetSFXVolume());
    }

    public void PlayBoxOpen()
    {
        _sfxSource.PlayOneShot(_boxOpen, GameManager.GetSFXVolume());
    }

    public void PlayBoxClose()
    {
        _sfxSource.PlayOneShot(_boxClose, GameManager.GetSFXVolume());
    }

    public void PlayBoxUnlatch()
    {
        _sfxSource.PlayOneShot(_boxUnlatch, GameManager.GetSFXVolume());
    }

    public void PlayBoxLatch()
    {
        _sfxSource.PlayOneShot(_boxLatch, GameManager.GetSFXVolume());
    }

    public void PlayWireSelect()
    {
        _sfxSource.PlayOneShot(_wireSelect, GameManager.GetSFXVolume());
    }

    public void PlayGeneralZap()
    {
        _sfxSource.PlayOneShot(_generalZap[Random.Range(0, _generalZap.Length)], GameManager.GetSFXVolume());
    }

    public void PlayRemoveZap()
    {
        _sfxSource.PlayOneShot(_removeZap, GameManager.GetSFXVolume());
    }

    public void PlayBoxFix()
    {
        _sfxSource.PlayOneShot(_boxFix, GameManager.GetSFXVolume());
    }
    #endregion
}