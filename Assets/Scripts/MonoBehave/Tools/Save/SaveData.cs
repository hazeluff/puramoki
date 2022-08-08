using System;
using UnityEngine;

[System.Serializable]
public class SaveData {

    public long lastSave;

    public long LastSave { get { return lastSave; } }
    public DateTime LastSaveDT { get { return DateTimeOffset.FromUnixTimeMilliseconds(lastSave).DateTime; } }

    public static SaveData CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<SaveData>(jsonString);
    }

    public override string ToString() {
        return "[" + 
            "lastSave:" + lastSave.ToString() + 
        "]";
    }
}
