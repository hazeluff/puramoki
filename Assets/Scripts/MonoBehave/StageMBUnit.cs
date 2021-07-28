using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageMBUnit : MBUnit, IMBUnit {

    // Init Unit
    [SerializeField]
    private string _name;
    [SerializeField]
    private Faction _faction;
    [SerializeField]
    private UnitBuild _build;

    protected override void Awake() {
        base.Awake();
        UnitBuild buildInstance = ScriptableObject.Instantiate(_build);
        buildInstance.SetName(_name);
        _unit = new StageUnit(_faction, buildInstance);
    }
}
