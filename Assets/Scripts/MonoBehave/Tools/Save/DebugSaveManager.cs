using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSaveManager : MonoBehaviour {
    [SerializeField]
    private SaveManager saveManager;

    private string fileName = "test";

    private void OnGUI() {
        GUI.skin.box.wordWrap = true;
        GUI.Box(new Rect(10, 500, 300, 130), (saveManager.Data == null ? "null" : saveManager.Data.ToString()));
        fileName = GUI.TextField(new Rect(10, 640, 300, 30), fileName);
        if (GUI.Button(new Rect(10, 680, 300, 30), "Save")) {
            saveManager.Save(fileName);
        }
        if (GUI.Button(new Rect(10, 720, 300, 30), "Load")) {
            saveManager.Load(fileName);
        }
    }
}
