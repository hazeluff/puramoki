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
        panel.SetActive(stage.IsOccupied(stage.CursorPos));
        if (!panel.activeSelf) {
            return;
        }
        IStageUnit cursorUnit = stage.CursorUnit;
        int newSelectedUnitHash = cursorUnit.GetHashCode();
        if (selectedUnitHash != newSelectedUnitHash) {
            selectedUnitHash = newSelectedUnitHash;
            unitName.text = cursorUnit.Build.Name;
            unitLvl.text = String.Format("LVL {0}", cursorUnit.Build.Lvl);
            unitExp.text = String.Format("EXP {0} / {1}", cursorUnit.Build.ExpCurrent, cursorUnit.Build.ExpToNext);
            unitFaction.text = cursorUnit.Faction.Name;
            unitHp.text = String.Format("HP {0} / {1}", cursorUnit.c_Hp, cursorUnit.Build.Hp);
            unitEp.text = String.Format("EP {0} / {1}", cursorUnit.c_Ep, cursorUnit.Build.Ep);
            unitMv.text = String.Format("MV {0}", cursorUnit.c_Mv);
            unitAtk.text = String.Format("ATK {0}", cursorUnit.c_Atk);
            unitAcc.text = String.Format("ACC {0}", cursorUnit.c_Acc);
            unitSpd.text = String.Format("SPD {0}", cursorUnit.c_Spd);
            unitDef.text = String.Format("DEF {0}", cursorUnit.c_Def);
            unitEva.text = String.Format("EVA {0}", cursorUnit.c_Eva);
            unitRng.text = String.Format("RNG {0}", cursorUnit.c_Rng);
        }
    }
}
