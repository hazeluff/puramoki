using UnityEngine;
using System.Collections.Generic;

public class StageUnit : IStageUnit {

    public string Name { get { return Build.Name; } }
    private Faction _faction;
    public Faction Faction { get { return _faction; } }


    private IUnitBuild _build;
    public IUnitBuild Build { get { return _build; } }

    private List<IUserStatusEffect> _statusEffects;
    public List<IUserStatusEffect> StatusEffects { get { throw new System.NotImplementedException(); } }

    public const int UNINITIALIZED_START_HP = -1;
    private int _currentHp;
    public int c_Hp { get { return _currentHp; } }
    public int c_Ep { get { return Build.Ep; } }
    public int c_Mv { get { return Build.Mv; } }

    public int c_Atk { get { return Build.Atk + Build.Weapon.Atk; } }
    public int c_Acc { get { return Build.Acc; } }
    public int c_Spd { get { return Build.Spd; } }

    public int c_Def { get { return Build.Def; } }
    public int c_Eva { get { return Build.Eva; } }
    public int c_Rng { get { return Build.Rng; } }

    public float c_ElemRes(Element element) {
        throw new System.NotImplementedException();
    }

    public StageUnit(string name, Faction faction, int lvl, int currentExp, IBaseUnit baseUnit, IWeapon weapon, IEquipment[] equipment) {
        _faction = faction;
        _build = new UnitBuild(name, lvl, currentExp, baseUnit, weapon, equipment);
        _currentHp = Build.Hp;
    }

    public void Init(MapCoordinate pos) {
        _lastPos = pos;
        _currentPos = pos;
    }

    // State
    public const int MINIMUM_COOLDOWN = 200;
    private int _cooldown = 0;
    public int Cooldown { get { return _cooldown; } }
    private int _lastTurn = 0;
    public int LastTurn { get { return _lastTurn; } }

    public void InitCooldown(int highestUnitSpd) {
        _cooldown = highestUnitSpd - c_Spd;
    }

    public void ResetCooldown() {
        _cooldown = MINIMUM_COOLDOWN;
    }

    public void AddCooldown(int amt) {
        _cooldown += amt;
    }

    public void ReduceCooldown(int amt) {
        _cooldown -= amt;
    }

    public void SetLastTurn(int turn) {
        _lastTurn = turn;
    }

    // Actions
    // Move
    private bool _moved = false;
    public bool Moved { get { return _moved; } }
    MapCoordinate _currentPos = null;
    public MapCoordinate Position { get { return _currentPos; } }
    MapCoordinate _lastPos = null;
    public MapCoordinate LastPos { get { return _lastPos; } }

    public void MoveTo(MapCoordinate newPos) {
        _lastPos = _currentPos;
        _currentPos = newPos;
        AddCooldown(100);
        _moved = true;
    }

    // Attack
    private bool _attacked = false;
    public bool Attacked { get { return _attacked; } }

    public void Attack(IStageUnit target) {
        Debug.Log(Build.Name + ": Attack [" + target.ToString() + "]");
        DamageResult damageResult = target.ReceiveAttack(new BasicAttackInstance(this, c_Atk, DamageType.PHYSICAL));
        if (damageResult.IsKill) {
            Build.GainExp(target.Build);
        }
        int atkCd = Build.Weapon.Cooldown - c_Spd;
        AddCooldown(atkCd < 100 ? 100 : atkCd);
        _attacked = true;
    }

    public DamageResult ReceiveAttack(IDamageInstance source) {
        Debug.Log(Build.Name + ": Receive Attack [" + source.ToString() + "]");
        int startHP = _currentHp;
        int damageReduced = c_Def;
        int reducedDmg = source.Damage - c_Def;
        if ((_currentHp = _currentHp - reducedDmg) < 0) {
            _currentHp = 0;
        }
        int damageDealt = startHP - _currentHp;
        return new DamageResult(damageReduced, damageDealt, startHP > 0 && _currentHp <= 0);
    }

    // Start of turn
    public void ResetForTurn() {
        _moved = false;
        _attacked = false;
    }

    // Finish Turn
    public void FinishTurn() {
        AddCooldown(FinishTurnCooldown);
    }

    public int FinishTurnCooldown {
        get {
            int cd = 1000 - c_Spd;
            return cd < 300 ? 300 : cd;
        }
    }

}
