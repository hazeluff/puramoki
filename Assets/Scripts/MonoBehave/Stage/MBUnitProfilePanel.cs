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
    private Text unitFaction;
    [SerializeField]
    private Text unitLvl;
    [SerializeField]
    private Text unitExp;
    [SerializeField]
    private Text unitType;
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

    private int cursorUnitHash;

    void Update() {
        panel.SetActive(stage.IsOccupied(stage.CursorPos));
        if (!panel.activeSelf) {
            return;
        }
        IStageUnit cursorUnit = stage.CursorUnit;
        int hash = cursorUnit.GetHashCode();
        if (cursorUnitHash != hash) {
            cursorUnitHash = hash;
            unitName.text = cursorUnit.Build.Name;
            unitLvl.text = cursorUnit.Build.Lvl.ToString();
            unitExp.text = String.Format("{0} / {1}", cursorUnit.Build.ExpCurrent, cursorUnit.Build.ExpToNext);
            unitType.text = cursorUnit.Build.Type.ToString();
            unitFaction.text = cursorUnit.Faction.Name;
            unitHp.text = String.Format("{0} / {1}", cursorUnit.c_Hp, cursorUnit.Build.Hp);
            unitEp.text = String.Format("{0} / {1}", cursorUnit.c_Ep, cursorUnit.Build.Ep);
            unitMv.text = cursorUnit.c_Mv.ToString();
            unitAtk.text = cursorUnit.c_Atk.ToString();
            unitAcc.text = cursorUnit.c_Acc.ToString();
            unitSpd.text = cursorUnit.c_Spd.ToString();
            unitDef.text = cursorUnit.c_Def.ToString();
            unitEva.text = cursorUnit.c_Eva.ToString();
            unitRng.text = cursorUnit.c_Rng.ToString();
        }
    }
}
