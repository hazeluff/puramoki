using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

public class CoreUnitPart : ScriptableObject, IBuildPart {
    public static CoreUnitPart CreateInstance(CoreUnitBase coreBase, int level, int currentExp) {
        CoreUnitPart core = ScriptableObject.CreateInstance<CoreUnitPart>();
        core._coreBase = coreBase;
        core._level = level;
        core._currentExp = currentExp;
        return core;
    }

    [SerializeField]
    private CoreUnitBase _coreBase;

    public string Name => _coreBase.Name;
    public string Id => _coreBase.Id;

    private int _level;
    public int Lvl => _level;

    public void LevelUp() {
        _level++;
    }

    private int _currentExp;
    public int ExpCurrent => _currentExp;
    public int ExpTillNext => ExpCurrent >= NextLevelExp ? 0 : NextLevelExp - ExpCurrent;
    public int NextLevelExp => 100 + (Lvl - 1) * 10;

    public void GainExp(int exp) {
        _currentExp += exp;
        while (_currentExp >= NextLevelExp) {
            _currentExp -= NextLevelExp;
            LevelUp();
        }
    }

    public void GainExp(UnitBuild unitDestroyed) {
        int levelDiff = unitDestroyed.Lvl - this.Lvl;
        GainExp(EvalDestroyExp(levelDiff));
    }

    private int EvalDestroyExp(int levelDiff) {
        switch (levelDiff) {
            case 5:
                return 35;
            case 4:
                return 30;
            case 3:
                return 25;
            case 2:
                return 20;
            case 1:
                return 15;
            case 0:
            case -1:
            case -2:
            case -3:
                return 10;
            case -4:
                return 5;
            case -5:
                return 2;
            default:
                return levelDiff > 5 ? 35 : 1;
        }
    }

    public UnitType Type { get { return _coreBase.Type; } }

    public int Hp => _coreBase.HpAt(Lvl);
    public int Ep => _coreBase.EpAt(Lvl);
    public int Atk => _coreBase.AtkAt(Lvl);
    public int Def => _coreBase.DefAt(Lvl);
    public int Acc => _coreBase.AccAt(Lvl);
    public int Eva => _coreBase.EvaAt(Lvl);
    public int Spd => _coreBase.SpdAt(Lvl);
    public int Rng => _coreBase.RngAt(Lvl);
    public int Mv => _coreBase.Mv;

    public int HpAt(int lvl) { return Hp + (int)Mathf.Floor(Hp * Mathf.Pow(1.1f, lvl - 1)); }
    public int EpAt(int lvl) { return Ep + (int)Mathf.Floor(Ep * Mathf.Pow(1.1f, lvl - 1)); }
    public int AtkAt(int lvl) { return Atk + (int) Mathf.Floor(Atk * Mathf.Pow(1.1f, lvl - 1)); }
    public int DefAt(int lvl) { return Def + (int) Mathf.Floor(Def * Mathf.Pow(1.1f, lvl - 1)); }
    public int AccAt(int lvl) { return Acc + (int) Mathf.Floor(Acc * Mathf.Pow(1.1f, lvl - 1)); }
    public int EvaAt(int lvl) { return Eva + (int) Mathf.Floor(Eva * Mathf.Pow(1.1f, lvl - 1)); }
    public int SpdAt(int lvl) { return Spd + (int) Mathf.Floor(Spd * Mathf.Pow(1.1f, lvl - 1)); }
    public int RngAt(int lvl) { return Rng + (int) Mathf.Floor(Rng * Mathf.Pow(1.1f, lvl - 1)); }

    public float ElemRes(Element element) {
        return 0.0f;
    }

    public GameObject Model => _coreBase.Model;

    public JObject ToSaveJson() {
        JObject json = new();
        json.Add("id", Id);
        if (Lvl > 0) {
            json.Add("lvl", Lvl);
        }
        if (Lvl > 0 || (Lvl == 0 && ExpCurrent > 0)) {
            json.Add("xp", ExpCurrent);
        }
        return json;
    }
}
