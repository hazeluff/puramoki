using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageUnit", menuName = "Stage/Unit", order = 1)]
public class StageUnit : ScriptableObject, IStageUnit {

    // Underlying serializable fields
    [SerializeField]
    private UnitProfile _profile;
    [SerializeField]
    private Faction _faction;

    public const int UNINITIALIZED_START_HP = -1;
    [SerializeField]
    private List<IUserStatusEffect> _statusEffects;
    public int overrideHP = UNINITIALIZED_START_HP;
    private int _currentHP;


    public IUnitProfile Profile { get { return _profile; } }
    public Faction Faction { get { return _faction; } }

    public List<IUserStatusEffect> StatusEffects { get { throw new System.NotImplementedException(); } }
    public int c_HP { get { return _currentHP; } }
    public int c_EP { get { return _profile.EP; } }
    public int c_Atk { get { return _profile.Atk + _profile.Weapon.Atk; } }
    public int c_Def { get { return _profile.Def; } }
    public int c_Acc { get { return _profile.Acc; } }
    public int c_Eva { get { return _profile.Eva; } }
    public int c_Spd { get { return _profile.Spd; } }
    public int c_Rng { get { return _profile.Rng; } }
    public int c_Mv { get { return _profile.Mv; } }

    public float c_ElemRes(Element element) {
        throw new System.NotImplementedException();
    }

    public void Init(MapCoordinate pos) {
        if (overrideHP == StageUnit.UNINITIALIZED_START_HP) {
            _currentHP = Profile.HP;
        } else {      
            _currentHP = overrideHP;
        }
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
        AddCooldown(100 + (1000 - c_Spd));
        _moved = true;
    }
    // Attack
    private bool _attacked = false;
    public bool Attacked { get { return _attacked; } }

    public void Attack(IStageUnit target) {
        target.ReceiveAttack(new BasicAttackInstance(c_Atk, DamageType.PHYSICAL));
        AddCooldown(_profile.Weapon.Cooldown - c_Spd);
        _attacked = true;
    }

    public void ReceiveAttack(IDamageSource source) {
        int reducedDmg = source.Damage - c_Def;
        if ((this._currentHP = this._currentHP - reducedDmg) < 0) {
            _currentHP = 0;
        }
    }

    public void ResetForTurn() {
        _moved = false;
        _attacked = false;
    }
}
