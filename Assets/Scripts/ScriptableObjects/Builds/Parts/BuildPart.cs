using Newtonsoft.Json.Linq;
using UnityEngine;

public class BuildPart : ScriptablePart, IBuildPart, IDatabasePart {
    [SerializeField]
    private string _name;
    [SerializeField]
    private string _id;
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

    public string Name => _name;
    public string Id => _id;
    public int Hp => _hp;
    public int Ep => _mp;
    public int Atk => _atk;
    public int Def => _def;
    public int Acc => _acc;
    public int Eva => _eva;
    public int Spd => _spd;
    public int Rng => _rng;
    public int Mv => _mv;

    public float ElemRes(Element element) {
        return 0.0f;
    }

    [SerializeField]
    private GameObject _model;
    public GameObject Model => _model;

    public JObject ToSaveJson() {
        return new() {
            { "id", Id }
        };
    }
}
