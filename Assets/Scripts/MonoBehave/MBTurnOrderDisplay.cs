using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MBTurnOrderDisplay : MonoBehaviour {


    [SerializeField]
    private MBStage stage;
    [SerializeField]
    private Text text;

    private void Update() {
        List<string> unitCooldowns = stage.GetTurnOrder()
            .ConvertAll<string>(mbUnit => string.Format("{{{0}:{1}}}", mbUnit.Unit.Build.Name, mbUnit.Unit.Cooldown));
            
        text.text = string.Format("Turn #: {0}\nTurn Order: [{1}]\nCurrent Unit: {2}", 
            stage.TurnCount, String.Join(",", unitCooldowns.ToArray()), stage.CurrentUnit.Unit);
    }
}
