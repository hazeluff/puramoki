using UnityEngine;
using UnityEngine.UI;

public class MBUnitProfilePanel : MonoBehaviour {

    [SerializeField]
    private MBStage stage;
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private Text unitName;

    private int selectedUnitHash;

    void Update() {
        panel.SetActive(stage.AnySelected());
        if (!stage.AnySelected()) {
            return;
        }
        IStageUnit selectedUnit = stage.GetSelected();
        int newSelectedUnitHash = selectedUnit.GetHashCode();
        if (selectedUnitHash != newSelectedUnitHash) {
            selectedUnitHash = newSelectedUnitHash;
            unitName.text = selectedUnit.UnitProfile.Name;
        }
    }
}
