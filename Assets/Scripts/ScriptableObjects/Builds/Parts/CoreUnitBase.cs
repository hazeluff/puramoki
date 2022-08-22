using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CoreUnitBase", menuName = "Builds/Parts/Core", order = 1)]
public class CoreUnitBase : ScriptableObject, IDatabasePart {
    [SerializeField]
    private string _name;
    [SerializeField]
    private string _id;
    [SerializeField]
    private UnitType _type;

    [SerializeField]
    private int _baseHp;
    [SerializeField]
    private int _baseEp;
    [SerializeField]
    private int _baseAtk;
    [SerializeField]
    private int _baseDef;
    [SerializeField]
    private int _baseAcc;
    [SerializeField]
    private int _baseEva;
    [SerializeField]
    private int _baseSpd;
    [SerializeField]
    private int _baseRng;
    [SerializeField]
    private int _baseMv;

    public string Name => _name;
    public string Id => _id;
    public UnitType Type => _type;

    public int BaseHp => _baseHp;
    public int HpAt(int lvl) { return BaseHp + (int)Mathf.Floor(BaseHp * Mathf.Pow(1.1f, lvl - 1)); }

    public int BaseEp => _baseEp;
    public int EpAt(int lvl) { return BaseEp + (int) Mathf.Floor(BaseEp * Mathf.Pow(1.1f, lvl - 1)); }
    
    public int BaseAtk => _baseAtk;
    public int AtkAt(int lvl) { return BaseAtk + (int) Mathf.Floor(BaseAtk * Mathf.Pow(1.1f, lvl - 1)); }
    
    public int BaseDef => _baseDef;
    public int DefAt(int lvl) { return BaseDef + (int) Mathf.Floor(BaseDef * Mathf.Pow(1.1f, lvl - 1)); }
    
    public int BaseAcc => _baseAcc;
    public int AccAt(int lvl) { return BaseAcc + (int) Mathf.Floor(BaseAcc * Mathf.Pow(1.1f, lvl - 1)); }
    
    public int BaseEva => _baseEva;
    public int EvaAt(int lvl) { return BaseEva + (int) Mathf.Floor(BaseEva * Mathf.Pow(1.1f, lvl - 1)); }
    
    public int BaseSpd => _baseSpd;
    public int SpdAt(int lvl) { return BaseSpd + (int) Mathf.Floor(BaseSpd * Mathf.Pow(1.1f, lvl - 1)); }
    
    public int BaseRng => _baseRng;
    public int RngAt(int lvl) { return BaseRng + (int) Mathf.Floor(BaseRng * Mathf.Pow(1.1f, lvl - 1)); }

    public int Mv => _baseMv;

    [SerializeField]
    private GameObject _model;
    public GameObject Model => _model;
}
