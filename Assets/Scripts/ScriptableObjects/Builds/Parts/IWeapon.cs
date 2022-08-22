public interface IWeapon : IBuildPart {
    WeaponType Type { get; }
    int RangeMax { get; }
    int RangeMin { get; }

    int Cooldown { get; }
}
