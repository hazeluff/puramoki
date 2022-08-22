using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class UnitBuild : ScriptableObject {
    public static UnitBuild CreateInstance(string name, CoreUnitPart coreUnit, BodyPart bodyPart, ArmsPart armsPart, LowerPart lowerPart, WeaponPart weaponPart) {
        UnitBuild build = ScriptableObject.CreateInstance<UnitBuild>();
        build._name = name;
        build._coreUnitPart = coreUnit;
        build._bodyPart = bodyPart;
        build._armsPart = armsPart;
        build._lowerPart = lowerPart;
        build._weaponPart = weaponPart;
        return build;
    }

    [SerializeField]
    private string _name;
    public string Name {
        get {
            if (_name != null && !_name.Equals("")) {
                return _name;
            }
            if (CoreUnitPart != null) {
                return CoreUnitPart.Name;
            }

            return "null";
        }
    }
    public void SetName(string name) {
        this._name = name;
    }

    public UnitType Type { get { return CoreUnitPart.Type; } }

    public int Lvl => CoreUnitPart.Lvl;
    public int ExpCurrent => CoreUnitPart.ExpCurrent;
    public int ExpTillNext => CoreUnitPart.ExpTillNext;
    public int NextLevelExp => CoreUnitPart.NextLevelExp;

    public int Hp => CoreUnitPart.Hp + PartsStats(pt => pt.Hp) + WeaponStats(wpn => wpn.Hp);
    public int Ep => CoreUnitPart.Ep + PartsStats(pt => pt.Ep) + WeaponStats(wpn => wpn.Ep);
    public int Atk => CoreUnitPart.Atk + PartsStats(pt => pt.Atk) + WeaponStats(wpn => wpn.Atk);
    public int Def => CoreUnitPart.Def + PartsStats(pt => pt.Def) + WeaponStats(wpn => wpn.Def);
    public int Acc => CoreUnitPart.Acc + PartsStats(pt => pt.Acc) + WeaponStats(wpn => wpn.Acc);
    public int Eva => CoreUnitPart.Eva + PartsStats(pt => pt.Eva) + WeaponStats(wpn => wpn.Eva);
    public int Spd => CoreUnitPart.Spd + PartsStats(pt => pt.Spd) + WeaponStats(wpn => wpn.Spd);
    public int Rng => CoreUnitPart.Rng + PartsStats(pt => pt.Rng) + WeaponStats(wpn => wpn.Rng);
    public int Mv => CoreUnitPart.Mv + PartsStats(pt => pt.Mv) + WeaponStats(wpn => wpn.Def);

    public int HpAt(int lvl) {
        return CoreStats(core => core.HpAt(lvl)) + PartsStats(pt => pt.Hp) + WeaponStats(wpn => wpn.Hp);
    }
    public int EpAt(int lvl) {
        return CoreStats(core => core.EpAt(lvl)) + PartsStats(pt => pt.Ep) + WeaponStats(wpn => wpn.Ep);
    }
    public int AtkAt(int lvl) {
        return CoreStats(core => core.AtkAt(lvl)) + PartsStats(pt => pt.Atk) + WeaponStats(wpn => wpn.Atk);
    }
    public int DefAt(int lvl) {
        return CoreStats(core => core.DefAt(lvl)) + PartsStats(pt => pt.Def) + WeaponStats(wpn => wpn.Def);
    }
    public int AccAt(int lvl) {
        return CoreStats(core => core.AccAt(lvl)) + PartsStats(pt => pt.Acc) + WeaponStats(wpn => wpn.Acc);
    }
    public int EvaAt(int lvl) {
        return CoreStats(core => core.EvaAt(lvl)) + PartsStats(pt => pt.Eva) + WeaponStats(wpn => wpn.Eva);
    }
    public int SpdAt(int lvl) {
        return CoreStats(core => core.SpdAt(lvl)) + PartsStats(pt => pt.Spd) + WeaponStats(wpn => wpn.Spd);
    }
    public int RngAt(int lvl) {
        return CoreStats(core => core.RngAt(lvl)) + PartsStats(pt => pt.Rng) + WeaponStats(wpn => wpn.Rng);
    }

    [SerializeField]
    private CoreUnitBase _baseCoreUnit;
    [SerializeField]
    protected CoreUnitPart _coreUnitPart;
    public CoreUnitPart CoreUnitPart { get { return _coreUnitPart; } set { _coreUnitPart = value; } }
    private int CoreStats(Func<CoreUnitPart, int> statFunc) {
        return CoreUnitPart != null ? statFunc.Invoke(CoreUnitPart) : 0;
     }

    [SerializeField]
    private BodyPart _bodyPart;
    public BodyPart BodyPart { get { return _bodyPart; } set { _bodyPart = value; } }
    [SerializeField]
    private ArmsPart _armsPart;
    public ArmsPart ArmsPart { get { return _armsPart; } set { _armsPart = value; } }
    [SerializeField]
    private LowerPart _lowerPart;
    public LowerPart LowerPart { get { return _lowerPart; } set { _lowerPart = value; } }

    private int PartsStats(Func<IBuildPart, int> statFunc) {
        return new List<IBuildPart>(new IBuildPart[] { BodyPart, ArmsPart, LowerPart })
            .Where(part => part != null)
            .Select<IBuildPart, int>(statFunc)
            .Sum();
    }

    [SerializeField]
    private WeaponPart _weaponPart;
    public WeaponPart WeaponPart { get { return _weaponPart; } set { _weaponPart = value; } }

    private int WeaponStats(Func<IBuildPart, int> statFunc) {
        return WeaponPart != null ? statFunc.Invoke(WeaponPart) : 0;
    }

    public float ElemRes(Element element) {
        throw new System.NotImplementedException();
    }

    // Assets
    public GameObject InstantiateModel() {
        GameObject model = new("Model");

        GameObject Lower = new("Lower");
        Lower.transform.parent = model.transform;
        if (LowerPart != null)
        {
            GameObject.Instantiate(LowerPart.Model).transform.parent = Lower.transform;
        }

        GameObject Body = new("Body");
        Body.transform.parent = model.transform;
        if (BodyPart != null) {
            GameObject.Instantiate(BodyPart.Model).transform.parent = Body.transform;
            Body.transform.localPosition = LowerPart != null ? LowerPart.JoinPoint : Vector3.zero;
        }

        GameObject Arms = new("Arms");
        Arms.transform.parent = model.transform;
        if (ArmsPart != null)
        {
            GameObject ArmsModel = GameObject.Instantiate(ArmsPart.Model);
            Transform LeftArmModelTransform = ArmsModel.transform.Find("LeftArm");
            LeftArmModelTransform.parent = Arms.transform;
            LeftArmModelTransform.localPosition = Body.transform.position + (BodyPart != null ? BodyPart.LeftArmJoinPoint : Vector3.zero);
            Transform RightArmModelTransform = ArmsModel.transform.Find("RightArm");
            RightArmModelTransform.parent = Arms.transform;
            RightArmModelTransform.localPosition = Body.transform.position + (BodyPart != null ? BodyPart.RightArmJoinPoint : Vector3.zero);
            GameObject.Destroy(ArmsModel); // Remove container of arms models
        }

        GameObject Core = new("Core");
        Core.transform.parent = model.transform;
        if (CoreUnitPart != null)
        {
            GameObject.Instantiate(CoreUnitPart.Model).transform.parent = Core.transform;
            Core.transform.localPosition = Body.transform.position + (BodyPart != null ? BodyPart.CoreJoinPoint : Vector3.zero);
        }

        GameObject Weapon = new("Weapon");
        Weapon.transform.parent = model.transform;
        if (WeaponPart != null) {
            // GameObject.Instantiate(WeaponPart.Model).transform.parent = Weapon.transform;
        }
        return model;
    }

    public GameObject InstantiateStageObject(MBStage stage, IStageUnit unit, bool isPlayer, MapCoordinate pos) {
        GameObject stageObject = new(Name);
        stageObject.AddComponent<UserMBUnit>().Init(stage, unit, isPlayer, pos);
        InstantiateModel().transform.parent = stageObject.transform;
        return stageObject;
    }
}
