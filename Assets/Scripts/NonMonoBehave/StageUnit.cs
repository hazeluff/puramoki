using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageUnit", menuName = "Stage/Unit", order = 1)]
public class StageUnit : ScriptableObject, IStageUnit {
    public const int UNINITIALIZED_START_HP = -1;

    // Underlying serializable fields
    [SerializeField]
    private UnitProfile _unitProfile;

    [SerializeField]
    private MapCoordinate _position;

    [SerializeField]
    private List<IUserStatusEffect> _statusEffects;
    [SerializeField]
    private int _currentHP = UNINITIALIZED_START_HP;

    public IUnitProfile UnitProfile { get { return _unitProfile; } }

    public MapCoordinate Position { get { return _position; } }

    public List<IUserStatusEffect> StatusEffects { get { throw new System.NotImplementedException(); } }
    public int HP_Current { get { return _currentHP; } }
    public int MP_Current { get { return _unitProfile.MP; } }
    public int Atk_Current { get { return _unitProfile.Atk; } }
    public int Def_Current { get { return _unitProfile.Def; } }
    public int Int_Current { get { return _unitProfile.Int; } }
    public int Res_Current { get { return _unitProfile.Res; } }
    public int Hit_Current { get { return _unitProfile.Hit; } }
    public int Spd_Current { get { return _unitProfile.Spd; } }
    public int Mv_Current { get { return _unitProfile.Mv; } }

    public float ElemRes_Current(Element element) {
        throw new System.NotImplementedException();
    }

    // Actions
    public void Attack(IStageUnit target) {
        target.ReceiveAttack(new BasicAttackInstance(this, UnitProfile.Atk));
    }

    public void ReceiveAttack(IAttackInstance attack) {
        if ((this._currentHP = this._currentHP - attack.Damage) < 0) {
            _currentHP = 0;
        }
    }



    void Awake() {
        if (_currentHP == UNINITIALIZED_START_HP) {
            _currentHP = UnitProfile.HP;
        }
    }
}
