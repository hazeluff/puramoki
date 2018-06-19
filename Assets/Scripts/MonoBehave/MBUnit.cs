using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBUnit : MonoBehaviour, IMBUnit {

    private MeshRenderer _renderer;

    private MBStage stage;

    private void Awake() {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void setStage(MBStage stage) {
        this.stage = stage;
    }

    void Update() {
        setColor(stage.IsSelected(this) ? Color.magenta : Color.white);
    }

    public void setColor(Color color) {
        _renderer.material.color = color;
    }

    public void Move(List<MapCoordinate> path) {
        MapCoordinate lastCoord = path[path.Count-1];
        gameObject.transform.localPosition = new Vector3(lastCoord.X, 0.25f, lastCoord.Y);
    }

    void OnTouchDown(Touch currentTouch) {
        stage.ClickUnit(this);
    }
}
