using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MBUnitMenu : MonoBehaviour {
    private static CustomLogger LOGGER = new CustomLogger(typeof(MBUnitMenu).ToString());

    private static readonly Color selectedColor = new Color32(0x00, 0xF9, 0xFF, 0xFF);

    public Image[] options;
    private MBStage stage;

    private int selection = 0;

    public void setStage(MBStage stage) {
        this.stage = stage;
    }

    void Awake() {
        Disable();
    }

    public void Enable() {
        this.gameObject.SetActive(true);
        selection = 0;
    }

    public void Disable() {
        this.gameObject.SetActive(false);
    }

    public void Up() {
        if (--selection < 0) {
            selection = options.Length - 1;
        }
        UpdateSelection();
    }

    public void Down() {
        if (++selection >= options.Length) {
            selection = 0;
        }
        UpdateSelection();
    }

    public int getSelection() {
        return selection;
    }

    private void UpdateSelection() {
        for (int i = 0; i < options.Length; i++) {
            options[i].color = selection == i ? selectedColor : Color.white;
        }
    }
}
