using System;
using UnityEngine;
using UnityEngine.UI;

public class MBUnitProfilePanel : MonoBehaviour {

    [SerializeField]
    private MBStage stage;
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private Text unitName;
    [SerializeField]
    private Text unitLvl;
    [SerializeField]
    private Text unitExp;
    [SerializeField]
    private Text unitFaction;
    [SerializeField]
    private Text unitHp;
    [SerializeField]
    private Text unitEp;
    [SerializeField]
    private Text unitMv;
    [SerializeField]
    private Text unitAtk;
    [SerializeField]
    private Text unitAcc;
    [SerializeField]
    private Text unitSpd;
    [SerializeField]
    private Text unitDef;
    [SerializeField]
    private Text unitEva;
    [SerializeField]
    private Text unitRng;

    private int selectedUnitHash;

    void Update() {
        panel.SetActive(stage.AnySelected());
        if (!stage.AnySelected()) {
            return;
        }
        IStageUnit selectedUnit = stage.GetSelectedUnit();
        int newSelectedUnitHash = selectedUnit.GetHashCode();
        if (selectedUnitHash != newSelectedUnitHash) {
            selectedUnitHash = newSelectedUnitHash;
            unitName.text = selectedUnit.Profile.Name;
            unitLvl.text = String.Format("LVL {0}", selectedUnit.Profile.Lvl);
            unitExp.text = String.Format("EXP {0} / {1}", selectedUnit.Profile.ExpCurrent, selectedUnit.Profile.ExpToNext);
            unitFaction.text = selectedUnit.Faction.Name;
            unitHp.text = String.Format("HP {0} / {1}", selectedUnit.c_HP, selectedUnit.Profile.HP);
            unitEp.text = String.Format("EP {0} / {1}", selectedUnit.c_EP, selectedUnit.Profile.EP);
            unitMv.text = String.Format("MV {0}", selectedUnit.c_Mv);
            unitAtk.text = String.Format("ATK {0}", selectedUnit.c_Atk);
            unitAcc.text = String.Format("ACC {0}", selectedUnit.c_Acc);
            unitSpd.text = String.Format("SPD {0}", selectedUnit.c_Spd);
            unitDef.text = String.Format("DEF {0}", selectedUnit.c_Def);
            unitEva.text = String.Format("EVA {0}", selectedUnit.c_Eva);
            unitRng.text = String.Format("RNG {0}", selectedUnit.c_Rng);
        }
    }
}
