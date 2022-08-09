using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour {
    private static SaveManager _saveManager;

    void Awake() {
        if (_saveManager == null) {
            DontDestroyOnLoad(gameObject);
            _saveManager = this;        
        } else if(_saveManager != this) {
            Destroy(gameObject);
        }
    }

    public static SaveManager get() {
        return _saveManager;
    }

    SaveData saveData;

    public SaveData Data { get { return saveData; } }

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
            saveData = SaveData.Parse(JObject.Parse(strSave));
        } else {
            Debug.Log("No save exists.");
        }
    }

    public void Save(string saveName) {
        SaveData data = SaveData.NewSave();
        WriteSave(saveName, data);
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
}
