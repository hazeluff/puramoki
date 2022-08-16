using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Builds/Parts/Weapon", order = 1)]
public class WeaponPart : BuildPart, IWeapon {
    [SerializeField]
    private WeaponType _type;
    [SerializeField]
    private int _rangeMin;
    [SerializeField]
    private int _rangeMax;

    [SerializeField]
    private int _cooldown;

    public WeaponType Type { get { return _type; } }
    public int RangeMin { get { return _rangeMin; } }
    public int RangeMax { get { return _rangeMax; } }

    public int Cooldown { get { return _cooldown; } }
}
