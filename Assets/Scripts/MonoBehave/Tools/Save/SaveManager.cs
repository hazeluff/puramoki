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
    public CoreUnitDatabase CoreUnitDatabase => _coreUnitDatabase;
    [SerializeField]
    private BodyPartDatabase _bodyPartDatabase;
    public BodyPartDatabase BodyPartDatabase => _bodyPartDatabase;
    [SerializeField]
    private ArmsPartDatabase _armsPartDatabase;
    public ArmsPartDatabase ArmsPartDatabase => _armsPartDatabase;
    [SerializeField]
    private LowerPartDatabase _lowerPartDatabase;
    public LowerPartDatabase LowerPartDatabase => _lowerPartDatabase;
    [SerializeField]
    private WeaponPartDatabase _weaponPartDatabase;
    public WeaponPartDatabase WeaponPartDatabase => _weaponPartDatabase;

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
    private SaveData Parse(string strJsonSave) {
        JObject json = JObject.Parse(strJsonSave);
        long lastSave = (long)json["lastSave"];

        List <CoreUnitPart> coreUnitParts = json["progress"]["collection"]["core"]
            .ToList()
            .Select(jsonCore => {
                string id = (string) jsonCore["id"];
                int lvl = JsonUtils.GetJsonValue(jsonCore, "lvl", 0);
                int xp  = JsonUtils.GetJsonValue(jsonCore, "xp", 0);
                CoreUnitBase coreBase = _coreUnitDatabase.Get(id);
                return CoreUnitPart.CreateInstance(coreBase, lvl, xp); 
            })
            .ToList();
        List<BodyPart> bodyParts = json["progress"]["collection"]["body"]
            .Select(jsonBody => _bodyPartDatabase.Get((string)jsonBody["id"]))
            .ToList();
        List<ArmsPart> armsParts = json["progress"]["collection"]["arms"]
            .Select(jsonArms => _armsPartDatabase.Get((string)jsonArms["id"]))
            .ToList();
        List<LowerPart> lowerParts = json["progress"]["collection"]["lower"]
            .Select(jsonLower => _lowerPartDatabase.Get((string)jsonLower["id"]))
            .ToList();
        List<WeaponPart> weaponParts = json["progress"]["collection"]["weapon"]
            .Select(jsonWeapon => _weaponPartDatabase.Get((string)jsonWeapon["id"]))
            .ToList();

        List<UnitBuild> builds = json["builds"].Select(jsonBuild => {
                CoreUnitBase coreBase = _coreUnitDatabase.Get((string)jsonBuild["core"]);
                return UnitBuild.CreateInstance(
                    (string)jsonBuild["name"],
                    GetCoreUnitPart(coreUnitParts, (string)jsonBuild["core"]),
                    _bodyPartDatabase.Get((string)jsonBuild["body"]),
                    _armsPartDatabase.Get((string)jsonBuild["arms"]),
                    _lowerPartDatabase.Get((string)jsonBuild["lower"]),
                    _weaponPartDatabase.Get((string)jsonBuild["weapon"])
                );
            })
            .ToList();
        return new SaveData(lastSave, coreUnitParts, bodyParts, armsParts, lowerParts, weaponParts, builds);
    }

    private static CoreUnitPart GetCoreUnitPart(List<CoreUnitPart> cores, string id) {
        return cores.First(core => core.Id.Equals(id));
    }

    public static SaveData NewSave() {
        long lastSave = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        return new SaveData(
            lastSave, 
            new List<CoreUnitPart>(), new List<BodyPart>(), new List<ArmsPart>(), new List<LowerPart>(), new List<WeaponPart>(), 
            new List<UnitBuild>()
        );
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
