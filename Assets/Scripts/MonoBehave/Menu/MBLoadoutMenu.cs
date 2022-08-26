using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBLoadoutMenu : MonoBehaviour {
    [SerializeField]
    MBLoadoutSelectionPanel loadoutSelectionPanel;

    // Start is called before the first frame update
    void Start() {
        SaveManager.Get().Load("test");
        if (SaveManager.Get().Data.Builds.Count > 0) {
            loadoutSelectionPanel.SelectBuild(SaveManager.Get().Data.Builds[0]);
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
