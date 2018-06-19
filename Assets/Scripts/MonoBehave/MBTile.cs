
using UnityEngine;

public class MBTile : MonoBehaviour {
    private static CustomLogger LOGGER = new CustomLogger(typeof(MBTile).ToString());

    private MeshRenderer _renderer;
    
    private MBStage stage;

    private void Awake() {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void setStage(MBStage stage) {
        this.stage = stage;
    }

    public void setColor(Color color) {
        _renderer.material.color = color;
    }

    void OnTouchDown(Touch currentTouch) {
        stage.ClickTile(this);
    }

    void OnTouchOver() {

    }
}
