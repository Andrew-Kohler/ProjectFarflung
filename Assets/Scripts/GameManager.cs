using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Stores and manages progression data saved between scenes and sessions.
/// Stores and manages options data saved between sessions.
/// </summary>
public class GameManager : MonoBehaviour
{
    // private singleton instance
    private static GameManager _instance;

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
            }
            // return new/existing instance
            return _instance;
        }
    }

    #region PROGRESSION DATA
    [Serializable]
    public class ProgressionData
    {
        // previous save terminal (or none in case of game start)
        // terminal unlock states (zone unlock states on power map)
        // activated light zones (through terminal)
        // light switch states (physical switches)
        // busted box fix states
        // picked up keycard list
        // narrative log pickup list
        // etc...

        // --------------------------------------------------------- \\
        // TODO: Add additional progression data types here
        // --------------------------------------------------------- \\
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
        ResetGameData();
        ProgressionData newSaveData = Instance.GameData;

        // read save data, overriding existing data as it is found
        string filePath = Application.persistentDataPath + "/ProgressionData.json";
        if (System.IO.File.Exists(filePath))
        {
            string saveData = System.IO.File.ReadAllText(filePath);
            newSaveData = JsonUtility.FromJson<ProgressionData>(saveData);
        }

        // Apply read/initialized data to instance
        Instance.GameData = newSaveData;
    }

    /// <summary>
    /// Sets all save data to its default values.
    /// Useful for when creating a new game save.
    /// </summary>
    public void ResetGameData()
    {
        // new persistent data instance to initialize/load
        ProgressionData newSaveData = new ProgressionData();

        // e.g.
        // newSaveData.SaveTerminal = 0;

        // --------------------------------------------------------- \\
        // TODO: Add default values for additional progression data here
        // --------------------------------------------------------- \\

        // reset BOTH scene and game data to be safe
        Instance.GameData = newSaveData;
        Instance.SceneData = newSaveData;
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
                Instance.SceneData = Instance.GameData;
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
        Instance.GameData = Instance.SceneData;
    }
    #endregion

    #region OPTIONS DATA
    // permanent upgrades, settings, etc. (saved between sessions)
    [Serializable]
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

        // default values in case of missing values from read file
        // Volume
        newSaveData.SFXVolume = 1f;
        newSaveData.MusicVolume = 1f;
        // Graphics
        newSaveData.Brightness = 1f;
        // Controls
        newSaveData.CamSensitivity = .5f;

        // --------------------------------------------------------- \\
        // TODO: Add default values for additional options data here
        // --------------------------------------------------------- \\

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
