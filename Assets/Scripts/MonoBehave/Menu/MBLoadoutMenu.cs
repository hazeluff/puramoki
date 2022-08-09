using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MBLoadoutMenu : MonoBehaviour {
    [SerializeField]
    private GameObject promptGO;

    [SerializeField]
    private Text buildNameLabel;
    [SerializeField]
    private TMP_Text buildNamePromptInput;

    private MenuState State = MenuState.PART_SELECT;

    private enum MenuState {
        PART_SELECT, ENTER_NAME
    }

    private void Update() {
        switch (State) {
            case MenuState.ENTER_NAME:
                UpdateEnterName();
                break;
            default:
                break;
        }
        
    }

    private void UpdateEnterName() {
        InputManager input = InputManager.get();
        if (input.START) {
            State_ExitName(buildNamePromptInput.text);
            return;
        }
        if (input.BACK) {
            State_ExitName(null);
            return;
        }

    }

    public void State_ExitName(string newName) {
        if (newName != null) {
            buildNameLabel.text = newName;
        }
        State = MenuState.PART_SELECT;
        promptGO.SetActive(false);
    }

    public void State_EnterName() {
        State = MenuState.ENTER_NAME;
        promptGO.SetActive(true);
    }
}
