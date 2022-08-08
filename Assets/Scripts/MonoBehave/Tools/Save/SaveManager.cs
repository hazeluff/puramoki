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
        string fileName = path + saveName + ".json";
        
        if (File.Exists(fileName)) {
            using FileStream fileStream = File.Open(fileName, FileMode.Open);
            using BinaryReader binaryReader = new(fileStream);
            string strSave = binaryReader.ReadString();
            saveData = SaveData.CreateFromJSON(strSave);
        } else {
            Debug.Log("No save exists.");
        }
    }

    public void Save(string saveName) {
        SaveData data = new SaveData();
        data.lastSave = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        WriteSave(saveName, data);
    }

    private void WriteSave(string saveName, SaveData saveData) {
        string path = GetFilePath();
        string fileName = path + saveName + ".json";

        Directory.CreateDirectory(path);
        using FileStream fileStream = File.Open(fileName, FileMode.Create);
        using BinaryWriter binaryWriter = new(fileStream);
        binaryWriter.Write(JsonUtility.ToJson(saveData, true));
    }

    private string GetSaveFileName(string saveName) {
        return GetFilePath() + saveName + ".json";
    }

    private string GetFilePath() {
        return Application.persistentDataPath + "/saves/";
    }
}
