using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageUnit", menuName = "Stage/Unit", order = 1)]
public class StageUnit : ScriptableObject, IStageUnit {
    public const int UNINITIALIZED_START_HP = -1;

    // Underlying serializable fields
    [SerializeField]
    private UnitProfile _unitProfile;
    [SerializeField]
    private Faction _factionOverride;

    [SerializeField]
    private MapCoordinate _position;

    [SerializeField]
    private List<IUserStatusEffect> _statusEffects;
    public int overrideHP = UNINITIALIZED_START_HP;
    private int _currentHP;

    public IUnitProfile UnitProfile { get { return _unitProfile; } }
    public Faction Faction { get { return _factionOverride != null ? _factionOverride : _unitProfile.Faction; } }

    public MapCoordinate Position { get { return _position; } }

    public List<IUserStatusEffect> StatusEffects { get { throw new System.NotImplementedException(); } }
    public int c_HP { get { return _currentHP; } }
    public int c_MP { get { return _unitProfile.MP; } }
    public int c_Atk { get { return _unitProfile.Atk; } }
    public int c_Def { get { return _unitProfile.Def; } }
    public int c_Int { get { return _unitProfile.Int; } }
    public int c_Res { get { return _unitProfile.Res; } }
    public int c_Hit { get { return _unitProfile.Hit; } }
    public int c_Spd { get { return _unitProfile.Spd; } }
    public int c_Mv { get { return _unitProfile.Mv; } }

    public float c_ElemRes(Element element) {
        throw new System.NotImplementedException();
    }

    public void Init() {
        if (overrideHP == StageUnit.UNINITIALIZED_START_HP) {
            _currentHP = UnitProfile.HP;
        } else {      
            _currentHP = overrideHP;
        }
    }

    // Actions
    public void Attack(IStageUnit target) {
        target.ReceiveAttack(new BasicAttackInstance(c_Atk, DamageType.PHYSICAL));
    }

    public void ReceiveAttack(IDamageSource source) {
        if ((this._currentHP = this._currentHP - source.Damage) < 0) {
            _currentHP = 0;
        }
    }
}
