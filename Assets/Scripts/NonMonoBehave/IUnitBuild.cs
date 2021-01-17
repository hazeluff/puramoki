public interface IUnitBuild {
    string Name { get; }
    UnitType Type { get; }
    int Lvl { get; }
    void LevelUp();
    int ExpCurrent { get; }
    int ExpToNext { get; }
    void GainExp(int exp);
    void GainExp(IUnitBuild unitDestroyed);

    // Base Stats
    int Hp { get; }
    int Ep { get; }
    int Atk { get; }
    int Def { get; }
    int Acc { get; }
    int Eva { get; }
    int Spd { get; }
    int Rng { get; }

    // Movement
    int Mv { get; }

    // Elemental Resistance (as a %)
    float ElemRes(Element element);

    // Gear
    IBaseUnit BaseUnit { get; }
    IWeapon Weapon { get; }
    IEquipment[] Equipments { get; }
}
