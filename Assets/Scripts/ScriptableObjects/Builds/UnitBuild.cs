using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitBuild", menuName = "Builds/UnitBuild", order = 1)]
public class UnitBuild : ScriptableObject {

    public static UnitBuild CreateInstance(string name, int level, int currentExp, CoreUnit coreUnit, BodyPart bodyPart, ArmsPart armsPart, LowerPart lowerPart, WeaponPart weaponPart) {
        UnitBuild build = ScriptableObject.CreateInstance<UnitBuild>();
        build._name = name;
        build._level = level;
        build.c_Exp = currentExp;
        build._coreUnit = coreUnit;
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
            if (CoreUnit != null) {
                return CoreUnit.Name;
            }

            return "null";
        }
    }
    public void SetName(string name) {
        this._name = name;
    }

    public UnitType Type { get { return CoreUnit.Type; } }
    [SerializeField]
    private int _level;
    public int Lvl { get { return _level; } }

    public void LevelUp() {
        _level++;
    }

    [SerializeField]
    private int c_Exp;
    public int ExpCurrent => c_Exp;
    public int ExpTillNext => ExpCurrent >= NextLevelExp ? 0 : NextLevelExp - ExpCurrent;
    public int NextLevelExp => 100 + (Lvl - 1) * 10;

    public void GainExp(int exp) {
        c_Exp += exp;
        while (c_Exp >= NextLevelExp) {
            c_Exp -= NextLevelExp;
            LevelUp();
        }
    }

    public void GainExp(UnitBuild unitDestroyed) {
        int levelDiff = unitDestroyed.Lvl - this.Lvl;
        GainExp(EvalDestroyExp(levelDiff));
    }

    private int EvalDestroyExp(int levelDiff) {
        switch (levelDiff) {
            case 5:
                return 35;
            case 4:
                return 30;
            case 3:
                return 25;
            case 2:
                return 20;
            case 1:
                return 15;
            case 0:
            case -1:
            case -2:
            case -3:
                return 10;
            case -4:
                return 5;
            case -5:
                return 2;
            default:
                return levelDiff > 5 ? 35 : 1;
        }
}

    public int Hp { get { return HpAt(_level); } }
    public int Ep { get { return EpAt(_level); } }
    public int Atk { get { return AtkAt(_level); } }
    public int Def { get { return DefAt(_level); } }
    public int Acc { get { return AccAt(_level); } }
    public int Eva { get { return EvaAt(_level); } }
    public int Spd { get { return SpdAt(_level); } }
    public int Rng { get { return RngAt(_level); } }
    public int Mv { get { return MvAt(_level); } }

    public int HpAt(int lvl) {
        return CoreStats(core => core.HpAt(lvl)) + WeaponStats(wpn => wpn.Hp) + PartsStats(pt => pt.Hp);
    }
    public int EpAt(int lvl) {
        return CoreStats(core => core.EpAt(lvl)) + WeaponStats(wpn => wpn.Ep) + PartsStats(pt => pt.Ep);
    }
    public int AtkAt(int lvl) {
        return CoreStats(core => core.AtkAt(lvl)) + WeaponStats(wpn => wpn.Atk) + PartsStats(pt => pt.Atk);
    }
    public int DefAt(int lvl) {
        return CoreStats(core => core.DefAt(lvl)) + WeaponStats(wpn => wpn.Def) + PartsStats(pt => pt.Def);
    }
    public int AccAt(int lvl) {
        return CoreStats(core => core.AccAt(lvl)) + WeaponStats(wpn => wpn.Acc) + PartsStats(pt => pt.Acc);
    }
    public int EvaAt(int lvl) {
        return CoreStats(core => core.EvaAt(lvl)) + WeaponStats(wpn => wpn.Eva) + PartsStats(pt => pt.Eva);
    }
    public int SpdAt(int lvl) {
        return CoreStats(core => core.SpdAt(lvl)) + WeaponStats(wpn => wpn.Spd) + PartsStats(pt => pt.Spd);
    }
    public int RngAt(int lvl) {
        return CoreStats(core => core.DefAt(lvl)) + WeaponStats(wpn => wpn.Rng) + PartsStats(pt => pt.Rng);
    }
    public int MvAt(int lvl) {
        return CoreStats(core => core.Mv) + WeaponStats(wpn => wpn.Def) + PartsStats(pt => pt.Mv);
    }

    [SerializeField]
    private CoreUnit _coreUnit;
    public CoreUnit CoreUnit { get { return _coreUnit; } set { _coreUnit = value; } }
    private int CoreStats(Func<CoreUnit, int> statFunc) {
        return CoreUnit != null ? statFunc.Invoke(CoreUnit) : 0;
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

    public IBuildPart[] Parts { get { return new IBuildPart[] { BodyPart, ArmsPart, LowerPart }; } }
    private int PartsStats(Func<IBuildPart, int> statFunc) {
        return new List<IBuildPart>(Parts).Where(part => part != null).Select<IBuildPart, int>(statFunc).Sum();
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
        if (CoreUnit != null)
        {
            GameObject.Instantiate(CoreUnit.Model).transform.parent = Core.transform;
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
