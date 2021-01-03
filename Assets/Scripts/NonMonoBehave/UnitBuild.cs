public class UnitBuild : IUnitBuild
{
    private string _name;

    public string Name { get { return _name == null || _name.Equals("") ? BaseUnit.Name: _name; } }
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
        while (c_Exp > ExpToNext) {
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

    public int Hp { get { return BaseUnit.HpAt(Lvl) + Weapon.Hp; } }
    public int Ep { get { return BaseUnit.EpAt(Lvl) + Weapon.Ep; } }
    public int Atk { get { return BaseUnit.AtkAt(Lvl) + Weapon.Atk; } }
    public int Def { get { return BaseUnit.DefAt(Lvl) + Weapon.Def; } }
    public int Acc { get { return BaseUnit.AccAt(Lvl) + Weapon.Acc; } }
    public int Eva { get { return BaseUnit.EvaAt(Lvl) + Weapon.Eva; } }
    public int Spd { get { return BaseUnit.SpdAt(Lvl) + Weapon.Spd; } }
    public int Rng { get { return BaseUnit.RngAt(Lvl) + Weapon.Rng; } }
    public int Mv { get { return BaseUnit.Mv + Weapon.Mv; } }

    private IBaseUnit _baseUnit;
    public IBaseUnit BaseUnit { get { return _baseUnit; } set { _baseUnit = value; } }
    private IWeapon _weapon;
    public IWeapon Weapon { get { return _weapon; } set { _weapon = value; } }
    private IEquipment[] _equipments;
    public IEquipment[] Equipments { get { return _equipments; } set { _equipments = value; } }

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
}
