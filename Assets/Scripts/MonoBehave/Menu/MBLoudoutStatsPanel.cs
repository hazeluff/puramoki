using TMPro;
using UnityEngine;

public class MBLoudoutStatsPanel : MonoBehaviour {
    [SerializeField]
    private TMP_Text statsLabel;

    [SerializeField]
    private MBLoadoutSelectionPanel selectionPanel;

    private void Update() {
        UnitBuild selectedBuild = selectionPanel.Selected;
        if (selectedBuild != null) {
            statsLabel.text = selectedBuild.Name + "\n" +
                "Level: " + selectedBuild.Lvl + "\n" +
                "Exp: " + selectedBuild.ExpCurrent + "/" + selectedBuild.NextLevelExp + "\n" +
                "HP:" + selectedBuild.Hp + "\n" +
                "EP:" + selectedBuild.Ep + "\n" +
                "ATK:" + selectedBuild.Atk + "\n" +
                "DEF:" + selectedBuild.Def + "\n" +
                "ACC:" + selectedBuild.Acc + "\n" +
                "EVA:" + selectedBuild.Eva + "\n" +
                "SPD:" + selectedBuild.Spd + "\n" +
                "RNG:" + selectedBuild.Rng + "\n" +
                "MV:" + selectedBuild.Mv + "\n";
        }
    }
}
