using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitBuild", menuName = "Builds/UnitBuild", order = 1)]
public class UnitBuild : ScriptableObject {
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
        _name = name;
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
    public int ExpCurrent { get { return c_Exp; } }
    public int ExpToNext { get { return 100 + (Lvl-1) * 10; } }

    public void GainExp(int exp) {
        c_Exp += exp;
        while (c_Exp >= ExpToNext) {
            c_Exp -= ExpToNext;
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

    public int Hp { get { return CoreStats(unit => unit.HpAt(Lvl)) + WeaponStats(wpn => wpn.Hp) + PartsStats(pt => pt.Hp); } }
    public int Ep { get { return CoreStats(unit => unit.EpAt(Lvl)) + WeaponStats(wpn => wpn.Ep) + PartsStats(pt => pt.Ep); } }
    public int Atk { get { return CoreStats(unit => unit.AtkAt(Lvl)) + WeaponStats(wpn => wpn.Atk) + PartsStats(pt => pt.Atk); } }
    public int Def { get { return CoreStats(unit => unit.DefAt(Lvl)) + WeaponStats(wpn => wpn.Def) + PartsStats(pt => pt.Def); } }
    public int Acc { get { return CoreStats(unit => unit.AccAt(Lvl)) + WeaponStats(wpn => wpn.Acc) + PartsStats(pt => pt.Acc); } }
    public int Eva { get { return CoreStats(unit => unit.EvaAt(Lvl)) + WeaponStats(wpn => wpn.Eva) + PartsStats(pt => pt.Eva); } }
    public int Spd { get { return CoreStats(unit => unit.SpdAt(Lvl)) + WeaponStats(wpn => wpn.Spd) + PartsStats(pt => pt.Spd); } }
    public int Rng { get { return CoreStats(unit => unit.RngAt(Lvl)) + WeaponStats(wpn => wpn.Rng) + PartsStats(pt => pt.Rng); } }
    public int Mv { get { return CoreStats(e => CoreUnit.Mv) + WeaponStats(wpn => wpn.Mv) + PartsStats(pt => pt.Mv); } }

    [SerializeField]
    private CoreUnit _coreUnit;
    public CoreUnit CoreUnit { get { return _coreUnit; } set { _coreUnit = value; } }
    private int CoreStats(Func<CoreUnit, int> statFunc) {
        return CoreUnit != null ? statFunc.Invoke(CoreUnit) : 0;
    }

    private int CoreStats(Func<CoreUnit, int, int> statFunc) {
        return CoreUnit != null ? statFunc.Invoke(CoreUnit, Lvl) : 0;
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
    private Weapon _weapon;
    public Weapon Weapon { get { return _weapon; } set { _weapon = value; } }
    private int WeaponStats(Func<IBuildPart, int> statFunc) {
        return Weapon != null ? statFunc.Invoke(Weapon) : 0;
    }

    public float ElemRes(Element element) {
        throw new System.NotImplementedException();
    }

    public UnitBuild(string name, int level, int currentExp, 
        CoreUnit coreUnit, BodyPart bodyPart, ArmsPart armsPart, LowerPart lowerPart, Weapon weapon) {
        _name = name;
        _level = level;
        c_Exp = currentExp;
        _coreUnit = coreUnit;
        _bodyPart = bodyPart;
        _armsPart = armsPart;
        _lowerPart = lowerPart;
        _weapon = weapon;
    }

    // Assets
    public float Height { get { return 0.25f; } }

    public GameObject InstantiateModel() {
        GameObject model = new GameObject("Model");
        new GameObject("Core").transform.parent = model.transform;
        new GameObject("Body").transform.parent = model.transform;
        new GameObject("Arms").transform.parent = model.transform;
        new GameObject("Lower").transform.parent = model.transform;
        if (LowerPart != null) {
            GameObject.Instantiate(LowerPart.Model).transform.parent = model.transform;
        }
        new GameObject("Weapon").transform.parent = model.transform;
        return model;
    }

    public GameObject InstantiateStageObject(MBStage stage, IStageUnit unit, bool isPlayer, MapCoordinate pos) {
        GameObject stageObject = new GameObject(Name);
        stageObject.AddComponent<TestMBUnit>().Init(stage, unit, true, pos);
        InstantiateModel().transform.parent = stageObject.transform;
        return stageObject;
    }

    //

    public override bool Equals(object obj) {
        var build = obj as UnitBuild;
        return build != null &&
               base.Equals(obj) &&
               _name == build._name &&
               Name == build.Name &&
               Type == build.Type &&
               _level == build._level &&
               Lvl == build.Lvl &&
               c_Exp == build.c_Exp &&
               ExpCurrent == build.ExpCurrent &&
               ExpToNext == build.ExpToNext &&
               Hp == build.Hp &&
               Ep == build.Ep &&
               Atk == build.Atk &&
               Def == build.Def &&
               Acc == build.Acc &&
               Eva == build.Eva &&
               Spd == build.Spd &&
               Rng == build.Rng &&
               Mv == build.Mv &&
               EqualityComparer<CoreUnit>.Default.Equals(_coreUnit, build._coreUnit) &&
               EqualityComparer<CoreUnit>.Default.Equals(CoreUnit, build.CoreUnit) &&
               EqualityComparer<BodyPart>.Default.Equals(_bodyPart, build._bodyPart) &&
               EqualityComparer<BodyPart>.Default.Equals(BodyPart, build.BodyPart) &&
               EqualityComparer<ArmsPart>.Default.Equals(_armsPart, build._armsPart) &&
               EqualityComparer<ArmsPart>.Default.Equals(ArmsPart, build.ArmsPart) &&
               EqualityComparer<LowerPart>.Default.Equals(_lowerPart, build._lowerPart) &&
               EqualityComparer<LowerPart>.Default.Equals(LowerPart, build.LowerPart) &&
               EqualityComparer<IBuildPart[]>.Default.Equals(Parts, build.Parts) &&
               EqualityComparer<Weapon>.Default.Equals(_weapon, build._weapon) &&
               EqualityComparer<Weapon>.Default.Equals(Weapon, build.Weapon);
    }

    public override int GetHashCode() {
        var hashCode = -19346833;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_name);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
        hashCode = hashCode * -1521134295 + Type.GetHashCode();
        hashCode = hashCode * -1521134295 + _level.GetHashCode();
        hashCode = hashCode * -1521134295 + Lvl.GetHashCode();
        hashCode = hashCode * -1521134295 + c_Exp.GetHashCode();
        hashCode = hashCode * -1521134295 + ExpCurrent.GetHashCode();
        hashCode = hashCode * -1521134295 + ExpToNext.GetHashCode();
        hashCode = hashCode * -1521134295 + Hp.GetHashCode();
        hashCode = hashCode * -1521134295 + Ep.GetHashCode();
        hashCode = hashCode * -1521134295 + Atk.GetHashCode();
        hashCode = hashCode * -1521134295 + Def.GetHashCode();
        hashCode = hashCode * -1521134295 + Acc.GetHashCode();
        hashCode = hashCode * -1521134295 + Eva.GetHashCode();
        hashCode = hashCode * -1521134295 + Spd.GetHashCode();
        hashCode = hashCode * -1521134295 + Rng.GetHashCode();
        hashCode = hashCode * -1521134295 + Mv.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<CoreUnit>.Default.GetHashCode(_coreUnit);
        hashCode = hashCode * -1521134295 + EqualityComparer<CoreUnit>.Default.GetHashCode(CoreUnit);
        hashCode = hashCode * -1521134295 + EqualityComparer<BodyPart>.Default.GetHashCode(_bodyPart);
        hashCode = hashCode * -1521134295 + EqualityComparer<BodyPart>.Default.GetHashCode(BodyPart);
        hashCode = hashCode * -1521134295 + EqualityComparer<ArmsPart>.Default.GetHashCode(_armsPart);
        hashCode = hashCode * -1521134295 + EqualityComparer<ArmsPart>.Default.GetHashCode(ArmsPart);
        hashCode = hashCode * -1521134295 + EqualityComparer<LowerPart>.Default.GetHashCode(_lowerPart);
        hashCode = hashCode * -1521134295 + EqualityComparer<LowerPart>.Default.GetHashCode(LowerPart);
        hashCode = hashCode * -1521134295 + EqualityComparer<IBuildPart[]>.Default.GetHashCode(Parts);
        hashCode = hashCode * -1521134295 + EqualityComparer<Weapon>.Default.GetHashCode(_weapon);
        hashCode = hashCode * -1521134295 + EqualityComparer<Weapon>.Default.GetHashCode(Weapon);
        return hashCode;
    }
}
