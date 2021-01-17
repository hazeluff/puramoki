using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MBStageLoader : MonoBehaviour {
    private List<IUnitBuild> _units;

    public List<IUnitBuild> UserBuilds { get { return _units; } }

    public void LoadStage() {
        _units = new List<IUnitBuild> {
            new UnitBuild(
                "Hazeluff", 1, 0,
                Resources.Load<BaseUnit>("ScriptableObjects/Database/BaseUnit/Fighter"),
                Resources.Load<Weapon>("ScriptableObjects/Database/Weapon/BeamSable"),
                new IEquipment[3]{ Resources.Load<Equipment>("ScriptableObjects/Database/Equipment/Midas"), null, null})
        };
        SceneManager.LoadScene("Stage");
    }

    private void Awake() {
        DontDestroyOnLoad(this);
    }
}
