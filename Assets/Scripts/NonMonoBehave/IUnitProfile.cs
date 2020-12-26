public interface IUnitProfile {
    string Name { get; }
    int Level { get; }
    UnitClass Class { get; }
    int ExpCurrent { get; }
    int ExpToNext { get; }
    void GainExp(int exp);

    // Base Stats
    int HP { get; }
    int EP { get; }
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
    Weapon Weapon { get; }
    IEquipment[] Equipments { get; }
}
