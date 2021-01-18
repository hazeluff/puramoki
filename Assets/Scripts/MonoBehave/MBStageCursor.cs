
using UnityEngine;
using UnityEngine.EventSystems;

public class MBStageCursor : MonoBehaviour {
    [SerializeField]
    private MBStage stage;


    private static readonly float FLOAT_HEIGHT = 0.15f; // seconds
    private static readonly float PERIOD = 1.0f; // seconds
    private static readonly float PI_x2 = Mathf.PI * 2.0f;
    private float periodCycle = 0.0f; // 2pi for full revolution
    private void Update() {
        MapCoordinate cursorPos = stage.CursorPos;
        periodCycle += Time.deltaTime / PERIOD;
        periodCycle = periodCycle % PI_x2;
        float offset = Mathf.Sin(periodCycle) + 1;
        transform.localPosition = cursorPos.Vector3 + (Vector3.up * (stage.Heights[cursorPos] + offset * FLOAT_HEIGHT));
    }
}
