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
    private WeaponPartDatabase _weaponPartDatabase;

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
            string strJsonSave = streamReader.ReadToEnd();
            this.saveData = Parse(strJsonSave);
        } else {
            Debug.Log("No save exists.");
        }
    }

    // Json Parsing + Save Creation
    readonly static string[] JSON_PART_TYPES = new string[] { "core", "arms", "body", "lower", "weapon" };

    private SaveData Parse(string strJsonSave) {
        JObject json = JObject.Parse(strJsonSave);
        long lastSave = (long)json["lastSave"];

        Dictionary<string, List<string>> partsCollection = new Dictionary<string, List<string>>();
        foreach (string partType in JSON_PART_TYPES) {
            partsCollection.Add(partType, json["partsCollection"][partType].Values<string>().ToList());
        }

        List<UnitBuild> builds = json["builds"]
            .Select(jsonBuild => UnitBuild.CreateInstance(
                (string)jsonBuild["name"],
                (int)jsonBuild["level"],
                (int)jsonBuild["currentExp"],
                _coreUnitDatabase.Get((string)jsonBuild["core"]),
                _bodyPartDatabase.Get((string)jsonBuild["body"]),
                _armsPartDatabase.Get((string)jsonBuild["arms"]),
                _lowerPartDatabase.Get((string)jsonBuild["lower"]),
                _weaponPartDatabase.Get((string)jsonBuild["weapon"])
            ))
            .ToList();
        return new SaveData(lastSave, partsCollection, builds);
    }

    public static SaveData NewSave() {
        long lastSave = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Dictionary<string, List<string>> partsCollection = new();
        foreach (string partType in JSON_PART_TYPES) {
            partsCollection.Add(partType, new List<string>());
        }
        return new SaveData(lastSave, partsCollection, new List<UnitBuild>());
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

        long currentTs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        try {
            streamWriter.Write(saveData.ToJson(currentTs).ToString(Formatting.Indented));
            saveData.UpdateSave(currentTs);
        } catch (Exception e) {
            Debug.Log("Failed to write save.\n" + e.Message);
        }
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
}
