
using UnityEngine;

public class MBTile : MonoBehaviour {
    private static CustomLogger LOGGER = new CustomLogger(typeof(MBTile).ToString());

    private MeshRenderer _renderer;
    
    private MBStageTest stage;

    public MapCoordinate Coordinates { get; private set; }

    private void Awake() {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void setStage(MBStageTest stage) {
        this.stage = stage;
    }

    public void setColor(Color color) {
        _renderer.material.color = color;
    }

    public void setCoordinates(MapCoordinate mapCoordinate) {
        Coordinates = mapCoordinate;
    }

    void OnTouchDown(Touch currentTouch) {
        stage.ClickTile(this);
    }

    void OnTouchOver() {

    }
}
