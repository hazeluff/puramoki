public interface IUnit
{
    string Name { get; }
    int Level { get; }
    UnitClass Class { get; }
    int ExpCurrent { get; }
    int ExpToNext { get; }
    void GainExp(int exp);

    // Base Stats
    int HP { get; }
    int CurrentHP { get; }
    int MP { get; }
    int CurrentMP { get; }
    int Atk { get; } // Double Atk over Def => Ignore Def
    int CurrentAtk { get; }
    int Def { get; } // Double Def over Atk => 0 dmg
    int CurrentDef { get; }
    int Int { get; } // Double Int over Res => Ignore Res
    int CurrentInt { get; }
    int Res { get; } // Double Res over Int => 0 dmg
    int CurrentRes { get; }
    int Hit { get; } // Double Hit stat vs => Crit
    int CurrentHit { get; }
    int Spd { get; } // Double Speed stat vs => Double Attack
    int CurrentSpd { get; }

    // Movement
    int Mv { get; }
    int CurrentMv { get; }

    // Elemental Resistance (as a %)
    float ElemRes(Element element);
    float CurrentElemRes(Element element);

    // Gear
    IWeapon Weapon { get; }
    IEquipment[] Equipments { get; }
}
