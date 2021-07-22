using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MBStageLoader : MonoBehaviour {
    [SerializeField]
    private List<UnitBuild> _units;

    public List<UnitBuild> UserBuilds { get { return _units; } }

    public void LoadStage() {
        SceneManager.LoadScene("Stage");
    }

    private void Awake() {
        DontDestroyOnLoad(this);
    }

    public void LoadStageModel() {
        _units[0].InstantiateModel();
    }
}
