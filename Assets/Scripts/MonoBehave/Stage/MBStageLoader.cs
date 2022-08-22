using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MBStageLoader : MonoBehaviour {

    public List<UnitBuild> UserBuilds => SaveManager.Get().Data.Builds;

    public void LoadStage() {
        SaveManager.Get().Load("test");
        SceneManager.LoadScene("Stage");
    }

    private void Awake() {
        DontDestroyOnLoad(this);
    }
}
