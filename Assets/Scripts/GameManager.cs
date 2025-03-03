using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Stores and manages progression data saved between scenes and sessions.
/// Stores and manages options data saved between sessions.
/// </summary>
public class GameManager : MonoBehaviour
{
    // private singleton instance
    private static GameManager _instance;

    // Constants
    public const int MAX_LIVES = 9;

    // Current list of player's Log objects
    public List<Log> FoundLogs;

    // public accessor of instance
    public static GameManager Instance
    {
        get
        {
            // setup GameManager as a singleton class
            if (_instance == null)
            {
                // create new game manager object
                GameObject newManager = new();
                newManager.name = "[Game Manager]";
                newManager.AddComponent<GameManager>();
                DontDestroyOnLoad(newManager);
                _instance = newManager.GetComponent<GameManager>();
                GameManager.DefaultSceneData(); // set scene data to defaults

                // ensures controls are updated with player overrides
                // loaded here so it always happens at the start and not after rebindings are needed
                string rebindsJson = PlayerPrefs.GetString("rebinds");
                InputSystem.actions.LoadBindingOverridesFromJson(rebindsJson);

                _instance.InitializeGameData();
                _instance.InitializeOptionsData();
            }
            // return new/existing instance
            return _instance;
        }
    }

    #region SCENE DATA
    // Static non-saved data (between scenes)
    public static float FlashlightCharge;

    private static void DefaultSceneData()
    {
        // since terminals are save points battery charge can always be full at start of a new session (no need to be saved)
        FlashlightCharge = 1f; // 1 = full charge; 0 = no charge
    }
    #endregion

    #region PROGRESSION DATA
    [System.Serializable]
    public class ProgressionData
    {
        // whether there is data to be overriden - only used for start menu
        public bool NewGameStarted;

        // HEADS-UP DISPLAY (HUD)
        // Narrative timestamp (conveying to the player that time has past - represented as an integer 0 through 4)
        public int NarrativeTimestamp;
        // Player remaining lives
        public int RemainingLives;

        // What floor the player is on (used by HUD, 1-3)
        public int Floor;
        // Lists of rooms the players have visited in the form of booleans (we'll be assigning the order)
        public bool[] VisitationList1F;
        public bool[] VisitationList2F;
        public bool[] VisitationList3F;

        // List of log names the player posesses (used to load actual log data) and their read state (because changing the contents of ScrObs is bad)
        public List<string> FoundLogNames;
        public List<bool> FoundLogReadStatus;
        // Index of the log the player currently has selected
        public int LogIndex;

        // POWER MANAGEMENT
        // terminal/zone unlocked
        public bool[] TerminalUnlocks;
        // zone power toggled on/off - MUST be same size as TerminalUnlocks
        public bool[] PoweredZones;
        // List of light switch on/off states based on light switch index
        public List<string> PowerSwitches;

        // BUSTED WIRE BOXES
        // list of names of boxes that have been fixed (since states only added and not ever removed - it is easier to use a List)
        public List<string> FixedWireBoxes;

        // KEYCARDS
        // List of names of keys the player has picked up
        public List<string> Keys;

        // previous save terminal (or none in case of game start)
        // etc...

        // --------------------------------------------------------- \\
        // TODO: Add additional progression data types here
        // --------------------------------------------------------- \\

        /// <summary>
        /// Default constructor.
        /// Used for default initialization OR resetting of progression data.
        /// </summary>
        public ProgressionData()
        {
            NewGameStarted = false;

            // HEADS-UP DISPLAY (HUD)

            NarrativeTimestamp = 0;                   
            RemainingLives = 9;

            Floor = 1;
            // Center, top, left, bottom, right
            VisitationList1F = new bool[5]; 
            for (int i = 0; i < VisitationList1F.Length; i++)
                VisitationList1F[i] = false;

            VisitationList2F = new bool[3];
            for (int i = 0; i < VisitationList2F.Length; i++)
                VisitationList2F[i] = false;
            VisitationList2F[0] = true; // Center room visited by default for now

            VisitationList3F = new bool[1];
            for (int i = 0; i < VisitationList3F.Length; i++)
                VisitationList3F[i] = false;

            FoundLogNames = new List<string>(); // Found log file names
            FoundLogReadStatus = new List<bool>();
            LogIndex = 0;                       // Index in the HUD list that the player has selected

            // POWER MANAGEMENT
            // arrays must be initialized like this otherwise json lists will be empty instead of properly initialized

            TerminalUnlocks = new bool[12]; // 12 total power zones
            for (int i = 0; i < TerminalUnlocks.Length; i++)
                TerminalUnlocks[i] = false; // all locked by default

            PoweredZones = new bool[12]; // 12 total power zones
            for (int i = 0; i < PoweredZones.Length; i++)
                PoweredZones[i] = false; // all off by default
            PoweredZones[0] = true; // command enabled by default

            PowerSwitches = new List<string>();

            // BUSTED WIRE BOXES
            FixedWireBoxes = new List<string>(); // new empty list (expandable)

            // KEYCARDS
            Keys = new List<string>();

            // --------------------------------------------------------- \\
            // TODO: Add default values for additional progression data here
            // --------------------------------------------------------- \\
        }

        /// <summary>
        /// Copy constructor for ProgressionData.
        /// Used for copying data from scene data to game data BY VALUE and not by reference.
        /// </summary>
        public ProgressionData(ProgressionData other)
        {
            NewGameStarted = other.NewGameStarted;

            // HEADS-UP DISPLAY (HUD)
            NarrativeTimestamp = other.NarrativeTimestamp;                    
            RemainingLives = other.RemainingLives;

            Floor = other.Floor;
            VisitationList1F = new bool[5]; 
            for (int i = 0; i<VisitationList1F.Length; i++)
                VisitationList1F[i] = other.VisitationList1F[i];

            VisitationList2F = new bool[3];
            for (int i = 0; i<VisitationList2F.Length; i++)
                VisitationList2F[i] = other.VisitationList2F[i];

            VisitationList3F = new bool[1];
            for (int i = 0; i<VisitationList3F.Length; i++)
                VisitationList3F[i] = other.VisitationList3F[i];

            FoundLogNames = new List<string>(other.FoundLogNames);
            FoundLogReadStatus = new List<bool>(other.FoundLogReadStatus);
            LogIndex = other.LogIndex;

        
            // POWER MANAGEMENT
            // arrays are reference based, so MUST be assigned like this

            // terminal/zone unlocked
            TerminalUnlocks = new bool[other.TerminalUnlocks.Length];
            for (int i = 0; i < TerminalUnlocks.Length; i++)
                TerminalUnlocks[i] = other.TerminalUnlocks[i];

            // zone power toggled on/off
            PoweredZones = new bool[other.PoweredZones.Length];
            for (int i = 0; i < PoweredZones.Length; i++)
                PoweredZones[i] = other.PoweredZones[i];

            // light switch on/off states
            PowerSwitches = new List<string>(other.PowerSwitches);

            // BUSTED WIRE BOXES
            FixedWireBoxes = new List<string>(other.FixedWireBoxes);

            // KEYS
            Keys = new List<string>(other.Keys);
            // --------------------------------------------------------- \\
            // TODO: Add additional progression data value copies here
            // --------------------------------------------------------- \\
        }
    }

    // private stored save data
    private ProgressionData _gameData;

    /// <summary>
    /// accessor for save data.
    /// CAUTION: should generally not interface here in other files, instead use SceneData
    /// </summary>
    private ProgressionData GameData // currently being hidden from other files to prevent misuse of interfacing with progression data
    {
        get
        {
            // initialize if necessary and possible
            if (_gameData == null)
            {
                InitializeGameData();
            }

            return _gameData;
        }
        set
        {
            _gameData = value;
        }
    }

    /// <summary>
    /// initializes base stats of save data (used for first time playing).
    /// Used both for reading existing save data AND for creating new save data if none is found.
    /// </summary>
    private void InitializeGameData()
    {
        // initialize to default values before reading from file
        ProgressionData newSaveData = new ProgressionData();
        FoundLogs = new List<Log>();

        // read save data, overriding existing data as it is found
        string filePath = Application.persistentDataPath + "/ProgressionData.json";
        if (System.IO.File.Exists(filePath))
        {
            string saveData = System.IO.File.ReadAllText(filePath);
            newSaveData = JsonUtility.FromJson<ProgressionData>(saveData);
        }

        // Apply read/initialized data to instance
        // should be a copy of the data, not the same reference
        Instance.GameData = new ProgressionData(newSaveData);
        LoadLogData();
    }

    /// <summary>
    /// Sets all save data to its default values.
    /// Useful for when creating a new game save.
    /// </summary>
    public void ResetGameData()
    {
        // Resets BOTH game and scene data to properly reset progress
        Instance.GameData = new ProgressionData();
        Instance.SceneData = new ProgressionData();
    }

    // private stored scene data
    private ProgressionData _sceneData;

    /// <summary>
    /// public accessor for scene data (unsaved version of progression data)
    /// This is where progression data should be interfaced throughout the project in nearly every case.
    /// </summary>
    public ProgressionData SceneData
    {
        get
        {
            // initialize if necessary and possible
            if (_sceneData == null)
            {
                // if no scene data is found, update it to match save data
                // should occur on application start
                Instance.SceneData = new ProgressionData(Instance.GameData);
            }
            // return new/existing inventory
            return _sceneData;
        }
        private set
        {
            _sceneData = value;
        }
    }

    /// <summary>
    /// Transfers progression data from scene data to game data, so that it will be saved to file.
    /// Should be called whenever interacting with or exiting a terminal (save points).
    /// </summary>
    public void SaveSceneDataToGameData()
    {
        // copy of scene data, NOT using same reference
        Instance.GameData = new ProgressionData(Instance.SceneData);
    }

    /// <summary>
    /// Loads in log data from resources based on the stored names of the files the player owns.
    /// Scriptable objects cannot be stored in JSON data as they'll be kept only via reference.
    /// Called when save data is loaded into a play session.
    /// </summary>
    private void LoadLogData()
    {
        if(GameData.FoundLogNames.Count > 0)
        {
            for (int i = 0; i < GameData.FoundLogNames.Count; i++)
            {
                FoundLogs.Add((Log)Resources.Load(GameData.FoundLogNames[i]));
            }
        }
        
    }

    /// <summary>
    /// Adds newly acquired log data to both the name list and the current data list in the correct order.
    /// Then, invokes a delegate that lets the HUD UI know (if it's active) to refresh the list.
    /// </summary>
    public void AddLogData(Log log, string resourceName)
    {
        // Adds log to lists in correct order
        if (FoundLogs.Count > 0)
        {
            if (!log.hasDate) // If a log doesn't have a date, it goes at the end of the list. Undated logs should have a date of 999.999 to make sure no dated materials get put with them.
            {
                FoundLogs.Add(log);
                SceneData.FoundLogNames.Add(resourceName);
                SceneData.FoundLogReadStatus.Add(false);
                return;
            }
            for (int i = 0; i < FoundLogs.Count; i++)
            {
                
                if (log.date.x < FoundLogs[i].date.x) // If the month is earlier, then the log we're adding is currently the last one of its month
                {
                    FoundLogs.Insert(i, log);
                    SceneData.FoundLogNames.Insert(i, resourceName);
                    SceneData.FoundLogReadStatus.Insert(i, false);
                    return;
                }
                else if (log.date.x == FoundLogs[i].date.x) // If the month is the same
                {
                    if (log.date.y <= FoundLogs[i].date.y)
                    {
                        FoundLogs.Insert(i, log);
                        SceneData.FoundLogNames.Insert(i, resourceName);
                        SceneData.FoundLogReadStatus.Insert(i, false);
                        return;
                    }
                }
            }
            // If there are no logs left after this one, this log is at the end of the list
            // If an undated log enters the list, this condition becomes irrelevant
            if (!FoundLogs.Find(x => x.filename == log.filename)) 
            {
                FoundLogs.Add(log);
                SceneData.FoundLogNames.Add(resourceName);
                SceneData.FoundLogReadStatus.Add(false);
                return;
            }
        }
        else // If this is the first log to be added, no need to sort
        {
            FoundLogs.Add(log);
            SceneData.FoundLogNames.Add(resourceName);
            SceneData.FoundLogReadStatus.Add(false);
        }

    }
    #endregion

    #region OPTIONS DATA
    // permanent upgrades, settings, etc. (saved between sessions)
    [System.Serializable]
    public class OptionsConfig
    {
        // Volume
        public float MainVolume;
        public float SFXVolume;
        public float MusicVolume;
        public float LogVolume;

        // Camera
        public float Brightness;
        public int FoV;
        public float Sensitivity;
        public bool CameraBobbing;

        /// <summary>
        /// Default constructor.
        /// Used only when existing options config data cannot be found.
        /// </summary>
        public OptionsConfig()
        {
            // default values in case of missing values from read file

            // Volume
            ResetVolumeToDefaults();

            // Camera
            ResetBrightness();
            ResetFoV();
            ResetSensitivity();
            ResetBobbing();
        }

        /// <summary>
        /// Can be called to reset all volume sliders to their default values
        /// </summary>
        public void ResetVolumeToDefaults()
        {
            MainVolume = 0.8f;
            SFXVolume = 0.8f;
            MusicVolume = 0.8f;
            LogVolume = 0.8f;
        }

        public void ResetBrightness()
        {
            Brightness = 1f;
        }

        public void ResetFoV()
        {
            FoV = 60;
        }

        public void ResetSensitivity()
        {
            Sensitivity = 3f;
        }

        public void ResetBobbing()
        {
            CameraBobbing = true;
        }
    }

    // private stored save data
    private OptionsConfig _optionsData;

    // public accessor for save data
    public OptionsConfig OptionsData
    {
        get
        {
            // initialize if necessary and possible
            if (_optionsData == null)
            {
                InitializeOptionsData();
            }

            return _optionsData;
        }
        private set
        {
            _optionsData = value;
        }
    }

    /// <summary>
    /// initializes base stats of save data (used for first time playing).
    /// Used both for reading existing save data AND for creating new save data if none is found.
    /// </summary>
    public void InitializeOptionsData()
    {
        // new persistent data instance to initialize/load
        OptionsConfig newSaveData = new OptionsConfig();

        // Read options data file, overriding defaults as data is found
        string filePath = Application.persistentDataPath + "/OptionsData.json";
        if (System.IO.File.Exists(filePath))
        {
            string saveData = System.IO.File.ReadAllText(filePath);
            newSaveData = JsonUtility.FromJson<OptionsConfig>(saveData);
        }

        // Apply read/initialized data to instance
        Instance.OptionsData = newSaveData;
    }

    /// <summary>
    /// Returns volume level for SFX. Accounts for main volume reduction.
    /// </summary>
    public static float GetSFXVolume()
    {
        return Instance.OptionsData.SFXVolume * Instance.OptionsData.MainVolume;
    }

    /// <summary>
    /// Returns volume level for Music. Accounts for main volume reduction.
    /// </summary>
    public static float GetMusicVolume()
    {
        return Instance.OptionsData.MusicVolume * Instance.OptionsData.MainVolume;
    }

    /// <summary>
    /// Returns volume level for Audio Logs. Accounts for main volume reduction.
    /// </summary>
    public static float GetLogVolume()
    {
        return Instance.OptionsData.LogVolume * Instance.OptionsData.MainVolume;
    }
    #endregion

    private void OnApplicationQuit()
    {
        // save controls rebindings
        string rebindsJson = InputSystem.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebindsJson);

        // Save OPTIONS DATA to file
        string optionsData = JsonUtility.ToJson(Instance.OptionsData);
        string optionsPath = Application.persistentDataPath + "/OptionsData.json";
        System.IO.File.WriteAllText(optionsPath, optionsData);

        // Save PROGRESSION DATA to file
        string saveData = JsonUtility.ToJson(Instance.GameData);
        string savePath = Application.persistentDataPath + "/ProgressionData.json";
        System.IO.File.WriteAllText(savePath, saveData);
    }
}
