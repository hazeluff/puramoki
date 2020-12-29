using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MBUnit : MBClickable, IMBUnit {

    private MeshRenderer _renderer;

    private MBStage stage;

    [SerializeField]
    private StageUnit _unit;
    public IStageUnit Unit { get { return _unit; } }
    [SerializeField]
    private bool _isPlayer;
    public bool IsPlayer { get { return _isPlayer; } }

    private void Awake() {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void setStage(MBStage stage) {
        this.stage = stage;
    }

    void Update() {
        if (stage == null) {
            return;
        }
        setColor(this.Equals(stage.CurrentUnit) ? Color.magenta : Color.white);
    }

    public void setColor(Color color) {
        _renderer.material.color = color;
    }

    public void Move(List<MapCoordinate> path) {
        MapCoordinate lastCoord = path[path.Count-1];
        gameObject.transform.localPosition = new Vector3(lastCoord.X, 0.25f + stage.Heights[lastCoord], lastCoord.Y);
        Unit.MoveTo(lastCoord);
    }

    public override void Click() {
        stage.ClickUnit(this);
    }
}
