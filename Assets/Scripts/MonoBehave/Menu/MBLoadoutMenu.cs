using System.Collections;
using System.Collections.Generic;
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


    [SerializeField]
    private string buildName;
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

    private void Update() {
        UpdateButtonLabels();

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

    private void UpdateButtonLabels() {
        if (buildName == null) {
            UpdateButtonsToBlank();
            return;
        }
        if (saveManager.Data == null) {
            UpdateButtonsToBlank();
            return;
        }
        if (!saveManager.Builds.ContainsKey(buildName)) {
            UpdateButtonsToBlank();
            return;
        }
        UnitBuild selectedBuild = saveManager.Builds[buildName];
        coreUnitNameButtonLabel.text = selectedBuild.CoreUnit.Name;
        bodyNameButtonLabel.text = selectedBuild.BodyPart.Name ;
        armsNameButtonLabel.text = selectedBuild.ArmsPart.Name;
        lowerNameButtonLabel.text = selectedBuild.LowerPart.Name;
        weaponNameButtonLabel.text = selectedBuild.WeaponPart.Name;
    }

    private void UpdateButtonsToBlank() {
        coreUnitNameButtonLabel.text = "<core>";
        bodyNameButtonLabel.text = "<body>";
        armsNameButtonLabel.text = "<arms>";
        lowerNameButtonLabel.text = "<lower>";
        weaponNameButtonLabel.text = "<weapon>";

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
            buildNameButtonLabel.text = newName;
        }
        nextState = MenuState.PART_SELECT;
        EventSystem.current.SetSelectedGameObject(buildNameButtonLabel.transform.parent.gameObject);
        promptGO.SetActive(false);
    }

    public void State_EnterName() {
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
