using UnityEngine;

public class BuildPart : ScriptablePart, IBuildPart {
    [SerializeField]
    private string _name;
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
    private GameObject _model;

    public string Name { get { return _name; } }
    public int Hp { get { return _hp; } }
    public int Ep { get { return _mp; } }
    public int Atk { get { return _atk; } }
    public int Def { get { return _def; } }
    public int Acc { get { return _acc; } }
    public int Eva { get { return _eva; } }
    public int Spd { get { return _spd; } }
    public int Rng { get { return _rng; } }
    public int Mv { get { return _mv; } }

    public float ElemRes(Element element) {
        return 0.0f;
    }

    public GameObject Model { get { return _model; } }
}
