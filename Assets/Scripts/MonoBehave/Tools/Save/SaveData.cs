using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public class SaveData {

    // Common
    readonly static string[] PART_TYPES = new string[] { "core", "arms", "body", "lower", "weapon" };

    // Saved Data
    private readonly long lastSave;
    private readonly Dictionary<string, List<string>> partsCollection;
    private readonly List<UnitBuild> builds; 

    // Accessors
    public long LastSave { get { return lastSave; } }
    public DateTime LastSaveDT { get { return DateTimeOffset.FromUnixTimeMilliseconds(lastSave).DateTime; } }

    private SaveData(long lastSave, Dictionary<string, List<string>> partsCollection, List<UnitBuild> builds) {
        this.lastSave = lastSave;
        this.partsCollection = partsCollection;
        this.builds = builds;
    }

    public static SaveData NewSave() {
        long lastSave = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Dictionary<string, List<string>> partsCollection = new Dictionary<string, List<string>>();
        foreach (string partType in PART_TYPES) {
            partsCollection.Add(partType, new List<string>());
        }
        return new SaveData(lastSave, partsCollection, new List<UnitBuild>());
    }

    public static SaveData Parse(JObject json) {
        long lastSave = (long) json["lastSave"];

        Dictionary<string, List<string>> partsCollection = new Dictionary<string, List<string>>();
        foreach (string partType in PART_TYPES) {
            partsCollection.Add(partType, json["partsCollection"][partType].Values<string>().ToList());
        }

        List<UnitBuild> builds = json["builds"]
            .Select(jsonBuild => new UnitBuild(
                (string) jsonBuild["name"],
                (int) jsonBuild["level"],
                (int)jsonBuild["currentExp"],
                (string)jsonBuild["core"],
                (string)jsonBuild["arms"],
                (string)jsonBuild["body"],
                (string)jsonBuild["lower"],
                (string)jsonBuild["weapon"]
            ))
            .ToList();
        return new SaveData(lastSave, partsCollection, builds);
    }

    public JObject ToJson() {
        JObject json = new() {
            { "lastSave", lastSave }
        };

        // Parts Collection
        JObject jsonPartsCollection = new();
        foreach (KeyValuePair<string, List<string>> entry in partsCollection) {
            JArray jsonCollection = JArray.FromObject(entry.Value);
            jsonPartsCollection.Add(entry.Key, jsonCollection);
        }
        json.Add("partsCollection", jsonPartsCollection);

        // Builds
        json.Add("builds", JArray.FromObject(builds.Select(build => {
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
