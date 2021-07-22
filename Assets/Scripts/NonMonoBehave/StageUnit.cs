using UnityEngine;
using System.Collections.Generic;

public class StageUnit : IStageUnit {

    public string Name { get { return Build.Name; } }
    private Faction _faction;
    public Faction Faction { get { return _faction; } }


    private UnitBuild _build;
    public UnitBuild Build { get { return _build; } }

    private List<IUserStatusEffect> _statusEffects;
    public List<IUserStatusEffect> StatusEffects { get { return new List<IUserStatusEffect>(); } }

    public const int UNINITIALIZED_START_HP = -1;
    private int _currentHp = UNINITIALIZED_START_HP;
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

    public StageUnit(Faction faction, UnitBuild build) {
        _faction = faction;
        _build = build;
        _currentHp = _build != null ? _build.Hp : 1;
    }

    public StageUnit(Faction faction, string name, int lvl, int currentExp, 
        CoreUnit coreUnit,  BodyPart bodyPart, ArmsPart armsPart, LowerPart lowerPart, Weapon weapon) 
        :this(faction, new UnitBuild(
            name, lvl, currentExp, 
            coreUnit, bodyPart, armsPart, lowerPart, weapon)){

    }

    public void Init(MapCoordinate pos) {
        _lastPos = pos;
        _currentPos = pos;
    }

    // State
    public const int MINIMUM_COOLDOWN = 200;
    private int _cooldown = 0;
    public int Cooldown { get { return _cooldown; } }
    private int _lastTurn = -1;
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
    public void FinishTurn(int turnNum) {
        _lastTurn = turnNum;
        AddCooldown(FinishTurnCooldown);
    }

    public int FinishTurnCooldown {
        get {
            int cd = 1000 - c_Spd;
            return cd < 300 ? 300 : cd;
        }
    }

    // Equals, GetHashCode
    public override bool Equals(object obj) {
        var unit = obj as StageUnit;
        return unit != null &&
               Name == unit.Name &&
               EqualityComparer<Faction>.Default.Equals(_faction, unit._faction) &&
               EqualityComparer<Faction>.Default.Equals(Faction, unit.Faction) &&
               EqualityComparer<UnitBuild>.Default.Equals(_build, unit._build) &&
               EqualityComparer<UnitBuild>.Default.Equals(Build, unit.Build) &&
               EqualityComparer<List<IUserStatusEffect>>.Default.Equals(_statusEffects, unit._statusEffects) &&
               EqualityComparer<List<IUserStatusEffect>>.Default.Equals(StatusEffects, unit.StatusEffects) &&
               _currentHp == unit._currentHp &&
               c_Hp == unit.c_Hp &&
               c_Ep == unit.c_Ep &&
               c_Mv == unit.c_Mv &&
               c_Atk == unit.c_Atk &&
               c_Acc == unit.c_Acc &&
               c_Spd == unit.c_Spd &&
               c_Def == unit.c_Def &&
               c_Eva == unit.c_Eva &&
               c_Rng == unit.c_Rng &&
               _cooldown == unit._cooldown &&
               Cooldown == unit.Cooldown &&
               _lastTurn == unit._lastTurn &&
               LastTurn == unit.LastTurn &&
               _moved == unit._moved &&
               Moved == unit.Moved &&
               EqualityComparer<MapCoordinate>.Default.Equals(_currentPos, unit._currentPos) &&
               EqualityComparer<MapCoordinate>.Default.Equals(Position, unit.Position) &&
               EqualityComparer<MapCoordinate>.Default.Equals(_lastPos, unit._lastPos) &&
               EqualityComparer<MapCoordinate>.Default.Equals(LastPos, unit.LastPos) &&
               _attacked == unit._attacked &&
               Attacked == unit.Attacked &&
               FinishTurnCooldown == unit.FinishTurnCooldown;
    }

    public override int GetHashCode() {
        var hashCode = -838004479;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
        hashCode = hashCode * -1521134295 + EqualityComparer<Faction>.Default.GetHashCode(_faction);
        hashCode = hashCode * -1521134295 + EqualityComparer<Faction>.Default.GetHashCode(Faction);
        hashCode = hashCode * -1521134295 + EqualityComparer<UnitBuild>.Default.GetHashCode(_build);
        hashCode = hashCode * -1521134295 + EqualityComparer<UnitBuild>.Default.GetHashCode(Build);
        hashCode = hashCode * -1521134295 + EqualityComparer<List<IUserStatusEffect>>.Default.GetHashCode(_statusEffects);
        hashCode = hashCode * -1521134295 + EqualityComparer<List<IUserStatusEffect>>.Default.GetHashCode(StatusEffects);
        hashCode = hashCode * -1521134295 + _currentHp.GetHashCode();
        hashCode = hashCode * -1521134295 + c_Hp.GetHashCode();
        hashCode = hashCode * -1521134295 + c_Ep.GetHashCode();
        hashCode = hashCode * -1521134295 + c_Mv.GetHashCode();
        hashCode = hashCode * -1521134295 + c_Atk.GetHashCode();
        hashCode = hashCode * -1521134295 + c_Acc.GetHashCode();
        hashCode = hashCode * -1521134295 + c_Spd.GetHashCode();
        hashCode = hashCode * -1521134295 + c_Def.GetHashCode();
        hashCode = hashCode * -1521134295 + c_Eva.GetHashCode();
        hashCode = hashCode * -1521134295 + c_Rng.GetHashCode();
        hashCode = hashCode * -1521134295 + _cooldown.GetHashCode();
        hashCode = hashCode * -1521134295 + Cooldown.GetHashCode();
        hashCode = hashCode * -1521134295 + _lastTurn.GetHashCode();
        hashCode = hashCode * -1521134295 + LastTurn.GetHashCode();
        hashCode = hashCode * -1521134295 + _moved.GetHashCode();
        hashCode = hashCode * -1521134295 + Moved.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<MapCoordinate>.Default.GetHashCode(_currentPos);
        hashCode = hashCode * -1521134295 + EqualityComparer<MapCoordinate>.Default.GetHashCode(Position);
        hashCode = hashCode * -1521134295 + EqualityComparer<MapCoordinate>.Default.GetHashCode(_lastPos);
        hashCode = hashCode * -1521134295 + EqualityComparer<MapCoordinate>.Default.GetHashCode(LastPos);
        hashCode = hashCode * -1521134295 + _attacked.GetHashCode();
        hashCode = hashCode * -1521134295 + Attacked.GetHashCode();
        hashCode = hashCode * -1521134295 + FinishTurnCooldown.GetHashCode();
        return hashCode;
    }


}
