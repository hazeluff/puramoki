using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {
    private static InputManager _inputManager;

    void Awake() {
        if (_inputManager == null) {
            DontDestroyOnLoad(gameObject);
            _inputManager = this;
        } else if (_inputManager != this) {
            Destroy(gameObject);
        }
    }

    public static InputManager get() {
        return _inputManager;
    }

    /// <summary>
    /// The layers we want to touch. (NOT IGNORE)
    /// </summary>
    public LayerMask touchLayers;
    public Vector3 MOUSE { get { return Input.mousePosition; } }
    public bool LEFT_CLICK { get { return Input.GetMouseButtonDown(0); } }
    public bool RIGHT_CLICK { get { return Input.GetMouseButtonDown(1); } }

    public bool AnyButton { get { return A || B || X || Y || RB || LB || BACK || START; } }
    public bool AnyInput { get { return A || B || X || Y || RB || LB || BACK || START || DPAD.AnyInput || LEFT_STICK.AnyInput || RIGHT_STICK.AnyInput || RT.Pressed || LT.Pressed; } }

    public readonly Stick D_PAD = new KeyboardStick();
    public readonly AnalogueStick LEFT_STICK = new AnalogueStick("Horizontal", "Vertical");
    public readonly AnalogueStick RIGHT_STICK = new AnalogueStick("R_Horizontal", "R_Vertical");
    public readonly Stick DPAD = new XBoxDPad("D_Horizontal", "D_Vertical");
    public readonly Trigger LT = new Trigger("LT");
    public readonly Trigger RT = new Trigger("RT");
    public bool SCROLL_UP { get { return Input.GetAxis("MouseScroll") > 0.0f; } }
    public bool SCROLL_DOWN { get { return Input.GetAxis("MouseScroll") < 0.0f; } }


    // Buttons
#if UNITY_WEBPLAYER || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
    public bool A { get { return JOYSTICK_A || KEYBOARD_A; } }
    public bool JOYSTICK_A { get { return Input.GetKeyDown("joystick 1 button 0"); } }
    public bool KEYBOARD_A { get { return Input.GetKeyDown(KeyCode.J); } }
    public bool B { get { return JOYSTICK_B || KEYBOARD_B; } }
    public bool JOYSTICK_B { get { return Input.GetKeyDown("joystick 1 button 1"); } }
    public bool KEYBOARD_B { get { return Input.GetKeyDown(KeyCode.K); } }
    public bool X { get { return JOYSTICK_X || KEYBOARD_X; } }
    public bool JOYSTICK_X { get { return Input.GetKeyDown("joystick 1 button 2"); } }
    public bool KEYBOARD_X { get { return Input.GetKeyDown(KeyCode.U); } }
    public bool Y { get { return JOYSTICK_Y || KEYBOARD_Y; } }
    public bool JOYSTICK_Y { get { return Input.GetKeyDown("joystick 1 button 3"); } }
    public bool KEYBOARD_Y { get { return Input.GetKeyDown(KeyCode.I); } }

    public bool LB { get { return JOYSTICK_LB || KEYBOARD_LB; } }
    public bool JOYSTICK_LB { get { return Input.GetKeyDown("joystick 1 button 4"); } }
    public bool KEYBOARD_LB { get { return Input.GetKeyDown(KeyCode.L); } }
    public bool RB { get { return JOYSTICK_RB || KEYBOARD_RB; } }
    public bool JOYSTICK_RB { get { return Input.GetKeyDown("joystick 1 button 5"); } }
    public bool KEYBOARD_RB { get { return Input.GetKeyDown(KeyCode.Semicolon); } }

    public bool BACK { get { return JOYSTICK_BACK || KEYBOARD_ESC || KEYBOARD_BACK; } }
    public bool JOYSTICK_BACK { get { return Input.GetKeyDown("joystick 1 button 6"); } }
    public bool KEYBOARD_ESC { get { return Input.GetKeyDown(KeyCode.Escape); } }
    public bool KEYBOARD_BACK { get { return Input.GetKeyDown(KeyCode.Backspace); } }
    public bool START { get { return JOYSTICK_START || KEYBOARD_START; } }
    public bool JOYSTICK_START { get { return Input.GetKeyDown("joystick 1 button 7"); } }
    public bool KEYBOARD_START { get { return Input.GetKeyDown(KeyCode.Return); } }
#elif UNITY_STANDALONE_OSX
    public bool A { get { return Input.GetKeyDown("joystick 1 button 16"); } }
    public bool B { get { return Input.GetKeyDown("joystick 1 button 17"); } }
    public bool Y { get { return Input.GetKeyDown("joystick 1 button 18"); } }
    public bool X { get { return Input.GetKeyDown("joystick 1 button 19"); } }

    public bool LB { get { return Input.GetKeyDown("joystick 1 button 13"); } }
    public bool RB { get { return Input.GetKeyDown("joystick 1 button 14"); } }
    
    public bool BACK { get { return Input.GetKeyDown("joystick 1 button 10"); } }
    public bool START { get { return Input.GetKeyDown("joystick 1 button 9"); } }
#endif

    void LateUpdate() {
        LEFT_STICK.UpdateState();
        RIGHT_STICK.UpdateState();
        DPAD.UpdateState();
        LT.UpdateState();
        RT.UpdateState();
    }


}
