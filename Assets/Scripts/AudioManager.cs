using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                // add audio sources - footsteps, charge stun, creature
                _instance._stepsSource = newManager.AddComponent<AudioSource>();
                _instance._chargeStunSource = newManager.AddComponent<AudioSource>();
                _instance._creatureSource = newManager.AddComponent<AudioSource>();
                // add audio source - SFX
                _instance._sfxSource = newManager.AddComponent<AudioSource>();

                // load ALL files: music, footsteps, stun, creature, SFX
                _instance.LoadMusic();
                _instance.LoadFootsteps();
                _instance.LoadChargeStun();
                _instance.LoadCreature();
                _instance.LoadSFX();
            }
            // return new/existing instance
            return _instance;
        }
    }

    private AudioSource _musicSource;
    private AudioSource _stepsSource;
    private AudioSource _chargeStunSource;
    private AudioSource _creatureSource;
    private AudioSource _sfxSource;

    #region Music / Ambient
    private AudioClip _startMusic;
    private AudioClip _ambientTrack;

    private void LoadMusic()
    {
        _startMusic = Resources.Load<AudioClip>("Music_Ambient/HowIWonder");
        _ambientTrack = Resources.Load<AudioClip>("Music_Ambient/AmbientTrack");
    }

    /// <summary>
    /// Queues new track, but does NOTHING if that track is already playing
    /// </summary>
    private void QueueTrack(AudioClip track)
    {
        if (_currTrack != track)
            _queueTrack = track;
    }

    public void CutMusic()
    {
        // stop music and clear queue
        _currTrack = null;
        _queueTrack = null;
    }

    public void QueueStartMusic()
    {
        QueueTrack(_startMusic);
    }

    public void QueueAmbientTrack()
    {
        QueueTrack(_ambientTrack);
    }

    private const float VOLUME_CHANGE_RATE = 1.25f;  // rate at which volume fades & increases back when switching track

    // for queuing track change and managing current music / ambient track
    private AudioClip _queueTrack = null;
    private AudioClip _currTrack = null;
    private bool _isPaused = false;

    float _prevTime = 0f;

    private void Update()
    {
        // this is necessary because 
        float deltaTime = Time.realtimeSinceStartup - _prevTime;
        _prevTime = Time.realtimeSinceStartup; // update prev time for next frame

        // transition out to new queued track
        if (_queueTrack is not null)
        {
            // slowly decrement volume down
            _musicSource.volume -= VOLUME_CHANGE_RATE * deltaTime;

            if (_musicSource.volume <= 0)
            {
                // switch track
                _musicSource.volume = 0;
                _musicSource.clip = _queueTrack;
                _musicSource.Play(); // start playing new looping track

                // clear queue
                _currTrack = _queueTrack;
                _queueTrack = null;
            }
        }
        // standard track looping behavior
        else if (_currTrack is not null)
        {
            // move towards 25% configured volume during pause
            if (_isPaused)
            {
                if (_musicSource.volume > GameManager.GetMusicVolume() / 4f)
                {
                    float newVol = _musicSource.volume - VOLUME_CHANGE_RATE * deltaTime;
                    if (newVol < GameManager.GetMusicVolume() / 4f)
                        newVol = GameManager.GetMusicVolume() / 4f;
                    _musicSource.volume = newVol;
                }
                // increasing to 25% should only occur on instant pause upon loading new track
                else if (_musicSource.volume < GameManager.GetMusicVolume() / 4f)
                {
                    float newVol = _musicSource.volume +VOLUME_CHANGE_RATE * deltaTime;
                    if (newVol > GameManager.GetMusicVolume() / 4f)
                        newVol = GameManager.GetMusicVolume() / 4f;
                    _musicSource.volume = newVol;
                }
            }
            // ensure at full configuredvolume when NOT paused
            else
            {
                // ensure at full volume
                float newVol = _musicSource.volume + VOLUME_CHANGE_RATE * deltaTime;
                if (newVol > GameManager.GetMusicVolume())
                    newVol = GameManager.GetMusicVolume();
                _musicSource.volume = newVol;
            }
        }
        // no track to play - or music was stopped
        else
        {
            if (_musicSource.volume > 0)
            {
                // decrease volume to zero
                float newVol = _musicSource.volume - VOLUME_CHANGE_RATE * deltaTime;
                if (newVol < 0f)
                {
                    _musicSource.volume = 0f;
                    // also stop track since it has finished fading out
                    _musicSource.Stop();
                }
                else
                    _musicSource.volume = newVol;
            }
        }
    }

    private void OnEnable()
    {
        PauseControls.onPauseOpen += SetPauseOpen;
        PauseControls.onPauseClose += SetPauseClose;
        SceneManager.sceneLoaded += SetPauseOnLoad;
    }

    private void OnDisable()
    {
        PauseControls.onPauseOpen -= SetPauseOpen;
        PauseControls.onPauseClose -= SetPauseClose;
        SceneManager.sceneLoaded -= SetPauseOnLoad;
    }

    private void SetPauseOpen()
    {
        _isPaused = true;
    }

    private void SetPauseClose()
    {
        _isPaused = false;
    }

    private void SetPauseOnLoad(Scene scene, LoadSceneMode mode)
    {
        // ensure pause saved state properly reset between scenes for the sake of music/ambient reduction
        _isPaused = false;
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

    #region Creature
    AudioClip _creatureSpawn;
    AudioClip _creatureDespawn;
    AudioClip _creatureStun;

    private void LoadCreature()
    {
        _creatureSpawn = Resources.Load<AudioClip>("Creature/CreatureSpawn");
        _creatureDespawn = Resources.Load<AudioClip>("Creature/CreatureDespawn");
        _creatureStun = Resources.Load<AudioClip>("Creature/CreatureStun");
    }

    public void PlayCreatureSpawn()
    {
        // prevent multiple creature sounds at once
        _creatureSource.Stop();
        _creatureSource.PlayOneShot(_creatureSpawn, GameManager.GetSFXVolume());
    }

    public void PlayCreatureDespawn()
    {
        _creatureSource.Stop(); // prevent playing multiple creature sounds at once
        _creatureSource.PlayOneShot(_creatureDespawn, GameManager.GetSFXVolume());
    }

    public void PlayCreatureStun()
    {
        _creatureSource.Stop(); // prevent playing multiple creature sounds at once
        _creatureSource.PlayOneShot(_creatureStun, GameManager.GetSFXVolume());
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

    private AudioClip _doorOpen;
    private AudioClip _doorClose;
    private AudioClip _doorLocked;

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

        _doorOpen = Resources.Load<AudioClip>("SFX/DoorOpen");
        _doorClose = Resources.Load<AudioClip>("SFX/DoorClose");
        _doorLocked = Resources.Load<AudioClip>("SFX/DoorLocked");
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

    public void PlayDoorOpen()
    {
        _sfxSource.PlayOneShot(_doorOpen, GameManager.GetSFXVolume());
    }

    public void PlayDoorClose()
    {
        _sfxSource.PlayOneShot(_doorClose, GameManager.GetSFXVolume());
    }

    public void PlayDoorLocked()
    {
        _sfxSource.PlayOneShot(_doorLocked, GameManager.GetSFXVolume());
    }
    #endregion
}