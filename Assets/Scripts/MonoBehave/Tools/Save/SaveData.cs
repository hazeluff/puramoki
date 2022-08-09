using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public class SaveData {

    // Common
    readonly static string[] PART_TYPES = new string[] { "core", "arm", "body", "lower", "weapon" };

    // Saved Data
    private long lastSave;
    Dictionary<string, List<string>> partsCollection;

    // Accessors
    public long LastSave { get { return lastSave; } }
    public DateTime LastSaveDT { get { return DateTimeOffset.FromUnixTimeMilliseconds(lastSave).DateTime; } }

    private SaveData(long lastSave, Dictionary<string, List<string>> partsCollection) {
        this.lastSave = lastSave;
        this.partsCollection = partsCollection;
    }

    public static SaveData NewSave() {
        long lastSave = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Dictionary<string, List<string>> partsCollection = new Dictionary<string, List<string>>();
        foreach (string partType in PART_TYPES) {
            partsCollection.Add(partType, new List<string>());
        }
        return new SaveData(lastSave, partsCollection);
    }

    public static SaveData Parse(JObject json) {
        long lastSave = (long) json["lastSave"];

        Dictionary<string, List<string>> partsCollection = new Dictionary<string, List<string>>();
        foreach (string partType in PART_TYPES) {
            partsCollection.Add(partType, json["partsCollection"][partType].Values<string>().ToList());
        }
        return new SaveData(lastSave, partsCollection);
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


        return json;
    }

    public override string ToString() {
        return ToJson().ToString(Formatting.None);
    }
}
