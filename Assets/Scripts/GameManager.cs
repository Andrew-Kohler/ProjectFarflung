using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores and manages player data saved between scenes.
/// Stores and manages save data saved between sessions.
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

    #region SCENE PERSISTENT DATA

    // Data saved between scenes
    [Serializable]
    public class ScenePersistentData
    {

    }

    // private stored inventory
    private ScenePersistentData _scenePersistent;

    // public accessor of inventory
    public ScenePersistentData ScenePersistent
    {
        get
        {
            // initialize if necessary and possible
            if (_scenePersistent == null)
            {
                ResetScenePersistentData();
            }
            // return new/existing inventory
            return _scenePersistent;
        }
        private set
        {
            _scenePersistent = value;
        }
    }

    /// <summary>
    /// initializes base stats of inventory.
    /// Used to reset inventory between runs.
    /// </summary>
    public void ResetScenePersistentData()
    {
        ScenePersistentData newScenePersistent = new ScenePersistentData();

        // Apply reset/initialized Inventory data to Instance
        Instance.ScenePersistent = newScenePersistent;
    }
    #endregion

    #region GAME PERSISTENT DATA
    // permanent upgrades, settings, etc. (saved between sessions)
    [Serializable]
    public class GamePersistentData
    {
        public float SFXVolume;
        public float MusicVolume;

        public float Brightness;
        public float CamSensitivity;
    }

    // private stored save data
    private GamePersistentData _gamePersistent;

    // public accessor for save data
    public GamePersistentData GamePersistent
    {
        get
        {
            // initialize if necessary and possible
            if (_gamePersistent == null)
            {
                InitializeSaveData();
            }

            return _gamePersistent;
        }
        private set
        {
            _gamePersistent = value;
        }
    }

    /// <summary>
    /// initializes base stats of save data (used for first time playing).
    /// Used both for reading existing save data AND for creating new save data if none is found.
    /// </summary>
    public void InitializeSaveData(bool deleteOldSave = false)
    {
        // new persistent data instance to initialize/load
        GamePersistentData newSaveData = new GamePersistentData();

        newSaveData.SFXVolume = 1f;
        newSaveData.MusicVolume = 1f;

        newSaveData.Brightness = 1f;
        newSaveData.CamSensitivity = .5f;


        // read save data, overriding existing data as it is found
        string filePath = Application.persistentDataPath + "/GameData.json";
        if (!deleteOldSave)
        {
            if (System.IO.File.Exists(filePath))
            {
                string saveData = System.IO.File.ReadAllText(filePath);
                newSaveData = JsonUtility.FromJson<GamePersistentData>(saveData);
                Instance.GamePersistent = newSaveData;
                return;
            }
        }

        // Apply read/initialized data to instance
        Instance.GamePersistent = newSaveData;
    }

    private void OnApplicationQuit()
    {
        // Saves data on quitting application
        string saveData = JsonUtility.ToJson(GamePersistent);
        string filePath = Application.persistentDataPath + "/GameData.json";
        System.IO.File.WriteAllText(filePath, saveData);
    }
    #endregion


}
