using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestMBUnit : MBUnit, IMBUnit {

    // Init Unit
    [SerializeField]
    private Faction _faction;
    [SerializeField]
    private UnitBuild _build;

    protected override void Awake() {
        base.Awake();
        _unit = new StageUnit(_faction, _build);
    }
}
