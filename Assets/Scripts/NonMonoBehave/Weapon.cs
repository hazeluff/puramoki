using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Equipment/Weapon", order = 1)]
public class Weapon : ScriptableObject, IWeapon {
    [SerializeField]
    private string _name;
    [SerializeField]
    private WeaponType _type;
    [SerializeField]
    private int _rangeMin;
    [SerializeField]
    private int _rangeMax;
    [SerializeField]
    private int  _hp;
    [SerializeField]
    private int _mp;
    [SerializeField]
    private int _atk;
    [SerializeField]
    private int _def;
    [SerializeField]
    private int _acc;
    [SerializeField]
    private int _eva;
    [SerializeField]
    private int _rng;
    [SerializeField]
    private int _spd;
    [SerializeField]
    private int _mv;
    [SerializeField]
    private int _cooldown;

    public string Name { get { return _name; } }
    public WeaponType Type { get { return _type; } }
    public int RangeMin { get { return _rangeMin; } }
    public int RangeMax { get { return _rangeMax; } }
    public int HP { get { return _hp; } }
    public int EP { get { return _mp; } }
    public int Atk { get { return _atk; } }
    public int Def { get { return _def; } }
    public int Acc { get { return _acc; } }
    public int Eva { get { return _eva; } }
    public int Spd { get { return _spd; } }
    public int Rng { get { return _rng; } }
    public int Mv { get { return _mv; } }

    public int Cooldown { get { return _cooldown; } }

    public float ElemRes(Element element) {
        return 0.0f;
    }
}
