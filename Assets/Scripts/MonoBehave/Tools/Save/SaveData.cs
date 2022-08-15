using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public class SaveData {

    // Saved Data
    private long _lastSave;
    private readonly Dictionary<string, List<string>> _partsCollection;
    private readonly List<UnitBuild> _builds;

    // Accessors
    public void UpdateSave(long saveTime) {
        this._lastSave = saveTime;
    }
    public long LastSave => _lastSave;
    public DateTime LastSaveDT => DateTimeOffset.FromUnixTimeMilliseconds(_lastSave).DateTime;
    public List<UnitBuild> Builds => _builds;
    public UnitBuild GetBuild(string buildName) {
        return Builds.First(build => build.name.Equals(buildName));
    }

    public SaveData(long lastSave, Dictionary<string, List<string>> partsCollection, List<UnitBuild> builds) {
        this._lastSave = lastSave;
        this._partsCollection = partsCollection;
        this._builds = builds;
    }

    public JObject ToJson(long saveTime) {
        JObject json = new() {
            { "lastSave", saveTime }
        };

        // Parts Collection
        JObject jsonPartsCollection = new();
        foreach (KeyValuePair<string, List<string>> entry in _partsCollection) {
            JArray jsonCollection = JArray.FromObject(entry.Value);
            jsonPartsCollection.Add(entry.Key, jsonCollection);
        }
        json.Add("partsCollection", jsonPartsCollection);

        // Builds
        json.Add("builds", JArray.FromObject(_builds.Select(build => {
            JObject jsonBuild = new();
            jsonBuild.Add("name", build.Name);
            jsonBuild.Add("level", build.Lvl);
            jsonBuild.Add("currentExp", build.ExpCurrent);
            jsonBuild.Add("core", build.CoreUnit.Id);
            jsonBuild.Add("arms", build.ArmsPart.Id);
            jsonBuild.Add("body", build.BodyPart.Id);
            jsonBuild.Add("lower", build.LowerPart.Id);
            jsonBuild.Add("weapon", build.WeaponPart.Id);
            return jsonBuild;
        })));


        return json;
    }

    public override string ToString() {
        return ToJson(LastSave).ToString(Formatting.None);
    }
}
