public interface IWeapon : IBuildPart {
    string Name { get; }
    WeaponType Type { get; }
    int RangeMax { get; }
    int RangeMin { get; }

    int Cooldown { get; }
}
