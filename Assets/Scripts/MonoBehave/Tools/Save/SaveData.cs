using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public class SaveData {

    // Common
    readonly static string[] PART_TYPES = new string[] { "core", "arms", "body", "lower", "weapon" };

    // Saved Data
    private readonly long _lastSave;
    private readonly Dictionary<string, List<string>> _partsCollection;
    private readonly Dictionary<string, UnitBuild> _builds;

    // Accessors
    public long LastSave => _lastSave;
    public DateTime LastSaveDT => DateTimeOffset.FromUnixTimeMilliseconds(_lastSave).DateTime;
    public Dictionary<string, UnitBuild> Builds => _builds;


    private SaveData(long lastSave, Dictionary<string, List<string>> partsCollection, Dictionary<string, UnitBuild> builds) {
        this._lastSave = lastSave;
        this._partsCollection = partsCollection;
        this._builds = builds;
    }

    public static SaveData NewSave() {
        long lastSave = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Dictionary<string, List<string>> partsCollection = new Dictionary<string, List<string>>();
        foreach (string partType in PART_TYPES) {
            partsCollection.Add(partType, new List<string>());
        }
        return new SaveData(lastSave, partsCollection, new Dictionary<string, UnitBuild>());
    }

    public static SaveData Parse(JObject json) {
        long lastSave = (long) json["lastSave"];

        Dictionary<string, List<string>> partsCollection = new Dictionary<string, List<string>>();
        foreach (string partType in PART_TYPES) {
            partsCollection.Add(partType, json["partsCollection"][partType].Values<string>().ToList());
        }

        Dictionary<string, UnitBuild> builds = json["builds"]
            .Select(jsonBuild => new UnitBuild(
                (string)jsonBuild["name"],
                (int)jsonBuild["level"],
                (int)jsonBuild["currentExp"],
                (string)jsonBuild["core"],
                (string)jsonBuild["arms"],
                (string)jsonBuild["body"],
                (string)jsonBuild["lower"],
                (string)jsonBuild["weapon"]
            ))
            .ToDictionary(
                unitBuild => unitBuild.name,
                unitBuild => unitBuild
            );
        return new SaveData(lastSave, partsCollection, builds);
    }

    public JObject ToJson() {
        JObject json = new() {
            { "lastSave", _lastSave }
        };

        // Parts Collection
        JObject jsonPartsCollection = new();
        foreach (KeyValuePair<string, List<string>> entry in _partsCollection) {
            JArray jsonCollection = JArray.FromObject(entry.Value);
            jsonPartsCollection.Add(entry.Key, jsonCollection);
        }
        json.Add("partsCollection", jsonPartsCollection);

        // Builds
        json.Add("builds", JArray.FromObject(_builds.Select(buildEntry => {
            UnitBuild build = buildEntry.Value;
            JObject jsonBuild = new();
            jsonBuild.Add("name", build.name);
            jsonBuild.Add("level", build.level);
            jsonBuild.Add("currentExp", build.currentExp);
            jsonBuild.Add("core", build.coreUnitId);
            jsonBuild.Add("arms", build.armsId);
            jsonBuild.Add("body", build.bodyId);
            jsonBuild.Add("lower", build.lowerId);
            jsonBuild.Add("weapon", build.weaponId);
            return jsonBuild;
        })));


        return json;
    }

    public override string ToString() {
        return ToJson().ToString(Formatting.None);
    }

    // Structs to represent internal save data
    public struct UnitBuild {
        public string name;
        public int level;
        public int currentExp;
        public string coreUnitId;
        public string armsId;
        public string bodyId;
        public string lowerId;
        public string weaponId;

        public UnitBuild(string name, int level, int currentExp, string coreUnitId, string armsId, string bodyId, string lowerId, string weaponId) {
            this.name = name;
            this.level = level;
            this.currentExp = currentExp;
            this.coreUnitId = coreUnitId;
            this.armsId = armsId;
            this.bodyId = bodyId;
            this.lowerId = lowerId;
            this.weaponId = weaponId;
        }
    }
}
