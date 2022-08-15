using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MBLoadoutMenu : MonoBehaviour {
    [SerializeField]
    private SaveManager saveManager;

    [SerializeField]
    private GameObject promptGO;

    [SerializeField]
    private TMP_Text buildNameButtonLabel;
    [SerializeField]
    private TMP_InputField buildNamePromptInput;


    private UnitBuild _selectedBuild = null;
    public UnitBuild Selected => _selectedBuild;

    [SerializeField]
    private TMP_Text coreUnitNameButtonLabel;
    [SerializeField]
    private TMP_Text bodyNameButtonLabel;
    [SerializeField]
    private TMP_Text armsNameButtonLabel;
    [SerializeField]
    private TMP_Text lowerNameButtonLabel;
    [SerializeField]
    private TMP_Text weaponNameButtonLabel;


    private MenuState state = MenuState.PART_SELECT;
    private MenuState nextState;

    private enum MenuState {
        PART_SELECT,
        ENTER_NAME, 
        NEXT_WAIT // only for nextState to denote "null"
    }

    private void Start() {
        saveManager.Load("test");
        if (saveManager.Data.Builds.Count > 0) {
            SelectBuild(saveManager.Data.Builds[0]);
            EventSystem.current.SetSelectedGameObject(buildNameButtonLabel.transform.parent.gameObject);
        }
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

    private void SelectBuild(UnitBuild build) {
        this._selectedBuild = build;
        if (Selected != null) {
            buildNameButtonLabel.text = Selected.Name;
            coreUnitNameButtonLabel.text = Selected.CoreUnit.Name;
            bodyNameButtonLabel.text = Selected.BodyPart.Name;
            armsNameButtonLabel.text = Selected.ArmsPart.Name;
            lowerNameButtonLabel.text = Selected.LowerPart.Name;
            weaponNameButtonLabel.text = Selected.WeaponPart.Name;
        } else {
            buildNameButtonLabel.text = "<none selected>";
            coreUnitNameButtonLabel.text = "<core>";
            bodyNameButtonLabel.text = "<body>";
            armsNameButtonLabel.text = "<arms>";
            lowerNameButtonLabel.text = "<lower>";
            weaponNameButtonLabel.text = "<weapon>";
        }
    }

    private void DeselectBuilds() {
        _selectedBuild = null;
    }

    private void UpdateEnterName() {
        // Force focus on the input
        if (!buildNamePromptInput.isFocused) {
            buildNamePromptInput.Select();
            return;
        }
        InputManager input = InputManager.get();
        if (input.JOYSTICK_START || input.JOYSTICK_A) {
            State_ExitName(buildNamePromptInput.text);
            return;
        }
        if (input.KEYBOARD_ESC || input.JOYSTICK_B || input.JOYSTICK_BACK) {
            State_ExitName(null);
            return;
        }
    }

    public void State_ExitName(string newName) {
        if (newName != null) {
            // Save new name
            buildNameButtonLabel.text = newName;
            Selected.SetName(newName);
        }
        nextState = MenuState.PART_SELECT;
        EventSystem.current.SetSelectedGameObject(buildNameButtonLabel.transform.parent.gameObject);
        promptGO.SetActive(false);
    }

    public void State_EnterName() {
        buildNamePromptInput.text = Selected.Name;
        nextState = MenuState.ENTER_NAME;
        EventSystem.current.SetSelectedGameObject(null);
        promptGO.SetActive(true);
    }

    private void DeactivateInput(TMP_InputField inputField, bool clearInput) {
        inputField.DeactivateInputField(clearInput);
        inputField.interactable = false;
        inputField.interactable = true;
    }
}
