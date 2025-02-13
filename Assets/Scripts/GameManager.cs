using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // terminal/zone unlocked
        public bool[] TerminalUnlocks;
        // zone power toggled on/off - MUST be same size as TerminalUnlocks
        public bool[] PoweredZones;

        // List of light switch on/off states based on light switch index
        public bool[] PowerSwitches;

        // previous save terminal (or none in case of game start)
        // busted box fix states
        // picked up keycard list
        // narrative log pickup list
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
            NarrativeTimestamp = 0;                   
            RemainingLives = 9;

            Floor = 1;
            // Center, top, left, bottom, right
            VisitationList1F = new bool[5]; 
            for (int i = 0; i < VisitationList1F.Length; i++)
                VisitationList1F[i] = false;
            //VisitationList1F[0] = true; // Center room visited by default for now

            VisitationList2F = new bool[3];
            for (int i = 0; i < VisitationList2F.Length; i++)
                VisitationList2F[i] = false;
            VisitationList2F[0] = true; // Center room visited by default for now

            VisitationList3F = new bool[1];
            for (int i = 0; i < VisitationList3F.Length; i++)
                VisitationList3F[i] = false;

            // arrays must be initialized like this otherwise json lists will be empty instead of properly initialized

            TerminalUnlocks = new bool[12]; // 12 total power zones
            for (int i = 0; i < TerminalUnlocks.Length; i++)
                TerminalUnlocks[i] = false; // all locked by default

            PoweredZones = new bool[12]; // 12 total power zones
            for (int i = 0; i < PoweredZones.Length; i++)
                PoweredZones[i] = false; // all off by default
            PoweredZones[0] = true; // command enabled by default

            PowerSwitches = new bool[64];
            for (int i = 0; i < PowerSwitches.Length; i++)
                PowerSwitches[i] = true; // all switches are ON by default

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
            PowerSwitches = new bool[other.PowerSwitches.Length];
            for (int i = 0; i < PowerSwitches.Length; i++)
                PowerSwitches[i] = other.PowerSwitches[i];

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
    #endregion

    #region OPTIONS DATA
    // permanent upgrades, settings, etc. (saved between sessions)
    [System.Serializable]
    public class OptionsConfig
    {
        // SETTINGS DATA
        // Volume
        public float SFXVolume;
        public float MusicVolume;
        // Graphics
        public float Brightness;
        // Controls
        public float CamSensitivity;

        // --------------------------------------------------------- \\
        // TODO: Add additional options data types here
        // --------------------------------------------------------- \\

        /// <summary>
        /// Default constructor.
        /// Used only when existing options config data cannot be found.
        /// </summary>
        public OptionsConfig()
        {
            // default values in case of missing values from read file
            // Volume
            SFXVolume = 1f;
            MusicVolume = 1f;
            // Graphics
            Brightness = 1f;
            // Controls
            CamSensitivity = .5f;

            // --------------------------------------------------------- \\
            // TODO: Add default values for additional options data here
            // --------------------------------------------------------- \\
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

    // TODO: any custom public functions for options menu
    // reset SFX sliders to defaults
    // reset Controls to defaults
    // etc...
    //
    // Note: if those reset to defaults functions are made,
    // then the default initialization in the above function should be replaced by calls to the function where possible
    #endregion

    private void OnApplicationQuit()
    {
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
