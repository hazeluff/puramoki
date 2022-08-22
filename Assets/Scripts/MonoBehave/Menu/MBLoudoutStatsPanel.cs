using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MBLoudoutStatsPanel : MonoBehaviour
{
    [SerializeField]
    private Button buildNameButton;
    [SerializeField]
    private Button coreUnitButton;
    [SerializeField]
    private Button bodyButton;
    [SerializeField]
    private Button armsButton;
    [SerializeField]
    private Button lowerButton;
    [SerializeField]
    private Button weaponButton;

    [SerializeField]
    private TMP_Text statsLabel;

    [SerializeField]
    private MBLoadoutSelectionPanel selectionPanel;

    private void Update() {
        UnitBuild selectedBuild = selectionPanel.Selected;
        if (selectedBuild == null) {
            // Nothing selected
            return;
        }
        GameObject selectedGO = EventSystem.current.currentSelectedGameObject;
        if (selectedGO == null || selectedGO == buildNameButton.gameObject) {
            UpdateBuildStats(selectedBuild);
            return;
        }
        if (selectedGO == coreUnitButton.gameObject) {
            UpdateCoreStats(selectedBuild.CoreUnitPart);
            return;
        }
        if (selectedGO == bodyButton.gameObject) {
            UpdatePartStats(selectedBuild.BodyPart);
            return;
        }
        if (selectedGO == armsButton.gameObject) {
            UpdatePartStats(selectedBuild.ArmsPart);
            return;
        }
        if (selectedGO == lowerButton.gameObject) {
            UpdatePartStats(selectedBuild.LowerPart);
            return;
        }
        if (selectedGO == lowerButton.gameObject) {
            UpdatePartStats(selectedBuild.WeaponPart);
            return;
        }
    }

    private void UpdateBuildStats(UnitBuild selectedBuild) {
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

    private void UpdateCoreStats(CoreUnitPart coreUnit) {
        statsLabel.text = coreUnit.Name + "\n" +
            // "Level: " + coreUnit.Lvl + "\n" +
            // "Exp: " + coreUnit.ExpCurrent + "/" + coreUnit.NextLevelExp + "\n" +
            "HP:" + coreUnit.Hp + "\n" +
            "EP:" + coreUnit.Ep + "\n" +
            "ATK:" + coreUnit.Atk + "\n" +
            "DEF:" + coreUnit.Def + "\n" +
            "ACC:" + coreUnit.Acc + "\n" +
            "EVA:" + coreUnit.Eva + "\n" +
            "SPD:" + coreUnit.Spd + "\n" +
            "RNG:" + coreUnit.Rng + "\n" +
            "MV:" + coreUnit.Mv + "\n";
    }

    private void UpdatePartStats(BuildPart buildPart) {
        statsLabel.text = buildPart.Name + "\n" +
            "\n" +
            "\n" +
            "HP:" + buildPart.Hp + "\n" +
            "EP:" + buildPart.Ep + "\n" +
            "ATK:" + buildPart.Atk + "\n" +
            "DEF:" + buildPart.Def + "\n" +
            "ACC:" + buildPart.Acc + "\n" +
            "EVA:" + buildPart.Eva + "\n" +
            "SPD:" + buildPart.Spd + "\n" +
            "RNG:" + buildPart.Rng + "\n" +
            "MV:" + buildPart.Mv + "\n";

    }
}
