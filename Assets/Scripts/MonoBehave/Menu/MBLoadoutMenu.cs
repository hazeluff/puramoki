using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MBLoadoutMenu : MonoBehaviour {
    [SerializeField]
    private GameObject promptGO;
    [SerializeField]
    private TMP_Text buildNameButtonLabel;
    [SerializeField]
    private TMP_InputField buildNamePromptInput;

    private MenuState state = MenuState.PART_SELECT;
    private MenuState nextState;

    private enum MenuState {
        PART_SELECT,
        ENTER_NAME, 
        NEXT_WAIT // only for nextState to denote "null"
    }

    private void Update() {
        switch (state) {
            case MenuState.ENTER_NAME:
                UpdateEnterName();
                break;
            default:
                break;
        }

        if (nextState != MenuState.NEXT_WAIT) {
            state = nextState;
            nextState = MenuState.NEXT_WAIT;
        }
    }

    private void UpdateEnterName() {
        InputManager input = InputManager.get();
        if (input.START || input.A) {
            State_ExitName(buildNamePromptInput.text);
            return;
        }
        if (input.BACK || input.B) {
            State_ExitName(null);
            return;
        }

    }

    public void State_ExitName(string newName) {
        if (newName != null) {
            buildNameButtonLabel.text = newName;
        }
        nextState = MenuState.PART_SELECT;
        EventSystem.current.SetSelectedGameObject(buildNameButtonLabel.transform.parent.gameObject);
        promptGO.SetActive(false);
    }

    public void State_EnterName() {
        nextState = MenuState.ENTER_NAME;
        EventSystem.current.SetSelectedGameObject(buildNamePromptInput.gameObject);
        promptGO.SetActive(true);
    }
}
