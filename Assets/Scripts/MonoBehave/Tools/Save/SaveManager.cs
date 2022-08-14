using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour {
    [SerializeField]
    private CoreUnitDatabase _coreUnitDatabase;
    [SerializeField]
    private BodyPartDatabase _bodyPartDatabase;
    [SerializeField]
    private ArmsPartDatabase _armsPartDatabase;
    [SerializeField]
    private LowerPartDatabase _lowerPartDatabase;
    [SerializeField]
    private WeaponDatabase _weaponDatabase;

    // Singleton Behaviour
    private static SaveManager _saveManager;

    void Awake() {
        if (_saveManager == null) {
            DontDestroyOnLoad(gameObject);
            _saveManager = this;
        } else if (_saveManager != this) {
            Destroy(gameObject);
        }
    }

    public static SaveManager Get() {
        return _saveManager;
    }

    // Filesystem/Data management
    SaveData saveData;
    public SaveData Data => saveData;


    public void Load(string saveName) {
        ReadSave(saveName);
    }

    private void ReadSave(String saveName) {
        string path = GetFilePath();
        string fileName = GetFileName(saveName);

        if (File.Exists(fileName)) {
            using FileStream fileStream = File.Open(fileName, FileMode.Open);
            using StreamReader streamReader = new(fileStream);
            string strSave = streamReader.ReadToEnd();
            this.saveData = SaveData.Parse(JObject.Parse(strSave));
        } else {
            Debug.Log("No save exists.");
        }
    }

    public void Save(string saveName) {
        WriteSave(saveName, Data);
    }

    public void WriteSave(string saveName, SaveData saveData) {
        string path = GetFilePath();
        string fileName = GetFileName(saveName);

        Directory.CreateDirectory(path);
        using FileStream fileStream = File.Open(fileName, FileMode.Create);
        using StreamWriter streamWriter = new(fileStream);
        streamWriter.Write(saveData.ToJson().ToString(Formatting.Indented));
    }

    private string GetFileName(string saveName) {
        return GetFilePath() + saveName + ".json";
    }

    private string GetFilePath() {
        return Application.persistentDataPath + "/saves/";
    }

    // Accessors
    public long LastSave => Data.LastSave;
    public DateTime LastSaveDT => Data.LastSaveDT;

    private Dictionary<string, UnitBuild> _builds;
    public Dictionary<string, UnitBuild> Builds { 
        get {
            if (_builds != null) {
                return _builds;
            }
            return LoadBuilds();
        } 
    }

    private Dictionary<string, UnitBuild> LoadBuilds() {
        return Data.Builds.ToDictionary(
            saveBuildEntry => saveBuildEntry.Value.name, 
            saveBuildEntry => LoadBuild(saveBuildEntry.Value)
        );
    }

    private UnitBuild LoadBuild(SaveData.UnitBuild saveBuild) {
        return UnitBuild.CreateInstance(
            saveBuild.name,
            saveBuild.level,
            saveBuild.currentExp,
            _coreUnitDatabase.Get(saveBuild.coreUnitId),
            _bodyPartDatabase.Get(saveBuild.bodyId),
            _armsPartDatabase.Get(saveBuild.armsId),
            _lowerPartDatabase.Get(saveBuild.lowerId),
            _weaponDatabase.Get(saveBuild.weaponId)
        );
    }
}
