using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSaveManager : MonoBehaviour {
    [SerializeField]
    private SaveManager saveManager;

    private string fileName = "test";

    private void OnGUI() {
        GUI.Box(new Rect(10, 10, 300, 130), (saveManager.Data == null ? "null" : saveManager.Data.ToString()));
        fileName = GUI.TextField(new Rect(10, 150, 300, 30), fileName);
        if (GUI.Button(new Rect(10, 190, 300, 30), "Save")) {
            saveManager.Save(fileName);
        }
        if (GUI.Button(new Rect(10, 230, 300, 30), "Load")) {
            saveManager.Load(fileName);
        }
    }
}
