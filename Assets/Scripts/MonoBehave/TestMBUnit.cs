using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestMBUnit : MBUnit, IMBUnit {

    // Init Unit
    [SerializeField]
    private string _name;
    [SerializeField]
    private Faction _faction;
    [SerializeField]
    private int _lvl = 1;
    [SerializeField]
    private int _currentExp = 0;
    [SerializeField]
    private BaseUnit _baseUnit;
    [SerializeField]
    private Weapon _weapon;
    [SerializeField]
    private Equipment[] _equipment = new Equipment[3];

    protected override void Awake() {
        base.Awake();
        _unit = new StageUnit(_name, _faction, _lvl, _currentExp, _baseUnit, _weapon, _equipment);
    }
}
