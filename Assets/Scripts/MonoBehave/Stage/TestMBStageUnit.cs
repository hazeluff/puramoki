using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestMBStageUnit : MBUnit, IMBUnit {
    // Init Unit
    [SerializeField]
    private string _name;

    [SerializeField]
    private Faction _faction;
    public Faction Faction => _faction;

    [SerializeField]
    private StageUnitBuild _build;
    public StageUnitBuild Build => _build;

    [SerializeField]
    private int _level;
    [SerializeField]
    private int _currentExp;

    protected override void Awake() {
        base.Awake();
        _build = ScriptableObject.Instantiate(_build);
        _build.SetName(_name);
        _build.CoreUnitPart = CoreUnitPart.CreateInstance(_build.CoreUnitBase, _level, _currentExp);
    }
}
