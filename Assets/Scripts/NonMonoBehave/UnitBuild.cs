using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class UnitBuild : IUnitBuild
{
    private string _name;
    public string Name { get { return _name == null || _name.Equals("") ? BaseUnit.Name: _name; } }
    public UnitType Type { get { return BaseUnit.Type; } }
    private int _level;
    public int Lvl { get { return _level; } }

    public void LevelUp() {
        _level++;
    }
    
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

    public void GainExp(IUnitBuild unitDestroyed) {
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

    public int Hp { get { return BaseUnit.HpAt(Lvl) + Weapon.Hp + EquipmentsStat(e => e.Hp); } }
    public int Ep { get { return BaseUnit.EpAt(Lvl) + Weapon.Ep + EquipmentsStat(e => e.Ep); } }
    public int Atk { get { return BaseUnit.AtkAt(Lvl) + Weapon.Atk + EquipmentsStat(e => e.Atk); } }
    public int Def { get { return BaseUnit.DefAt(Lvl) + Weapon.Def + EquipmentsStat(e => e.Def); } }
    public int Acc { get { return BaseUnit.AccAt(Lvl) + Weapon.Acc + EquipmentsStat(e => e.Acc); } }
    public int Eva { get { return BaseUnit.EvaAt(Lvl) + Weapon.Eva + EquipmentsStat(e => e.Eva); } }
    public int Spd { get { return BaseUnit.SpdAt(Lvl) + Weapon.Spd + EquipmentsStat(e => e.Spd); } }
    public int Rng { get { return BaseUnit.RngAt(Lvl) + Weapon.Rng + EquipmentsStat(e => e.Rng); } }
    public int Mv { get { return BaseUnit.Mv + Weapon.Mv + EquipmentsStat(e => e.Mv); } }

    private IBaseUnit _baseUnit;
    public IBaseUnit BaseUnit { get { return _baseUnit; } set { _baseUnit = value; } }
    private IWeapon _weapon;
    public IWeapon Weapon { get { return _weapon; } set { _weapon = value; } }
    private IEquipment[] _equipments;
    public IEquipment[] Equipments { get { return _equipments; } set { _equipments = value; } }
    private int EquipmentsStat(Func<IEquipment, int> statFunc) {
        return new List<IEquipment>(Equipments).Where(equip => equip != null).Select<IEquipment, int>(statFunc).Sum();
    }

    public float ElemRes(Element element) {
        throw new System.NotImplementedException();
    }

    public UnitBuild(string name, int level, int currentExp, IBaseUnit baseUnit, IWeapon weapon, IEquipment[] equipments) {
        _name = name;
        _level = level;
        c_Exp = currentExp;
        _baseUnit = baseUnit;
        _weapon = weapon;
        _equipments = equipments;
    }

    // Equals, GetHashCode
    public override bool Equals(object obj) {
        UnitBuild build = obj as UnitBuild;
        return build != null &&
               _name == build._name &&
               Name == build.Name &&
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
               EqualityComparer<IBaseUnit>.Default.Equals(_baseUnit, build._baseUnit) &&
               EqualityComparer<IBaseUnit>.Default.Equals(BaseUnit, build.BaseUnit) &&
               EqualityComparer<IWeapon>.Default.Equals(_weapon, build._weapon) &&
               EqualityComparer<IWeapon>.Default.Equals(Weapon, build.Weapon) &&
               EqualityComparer<IEquipment[]>.Default.Equals(_equipments, build._equipments) &&
               EqualityComparer<IEquipment[]>.Default.Equals(Equipments, build.Equipments);
    }

    public override int GetHashCode() {
        int hashCode = -2049291569;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_name);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
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
        hashCode = hashCode * -1521134295 + EqualityComparer<IBaseUnit>.Default.GetHashCode(_baseUnit);
        hashCode = hashCode * -1521134295 + EqualityComparer<IBaseUnit>.Default.GetHashCode(BaseUnit);
        hashCode = hashCode * -1521134295 + EqualityComparer<IWeapon>.Default.GetHashCode(_weapon);
        hashCode = hashCode * -1521134295 + EqualityComparer<IWeapon>.Default.GetHashCode(Weapon);
        hashCode = hashCode * -1521134295 + EqualityComparer<IEquipment[]>.Default.GetHashCode(_equipments);
        hashCode = hashCode * -1521134295 + EqualityComparer<IEquipment[]>.Default.GetHashCode(Equipments);
        return hashCode;
    }

}
