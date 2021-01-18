using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MBUnit : MBClickable, IMBUnit {

    private MBStage _stage;
    
    protected IStageUnit _unit;
    public IStageUnit Unit { get { return _unit; } }

    public void Init(MBStage stage, IStageUnit unit, MapCoordinate pos) {
        _stage = stage;
        if (unit != null) {
            _unit = unit;
        }
        _unit.Init(pos);
        transform.position = new Vector3(pos.X, 0.0f, pos.Y);
    }

    [SerializeField]
    private bool _isPlayer;
    public bool IsPlayer { get { return _isPlayer; } }

    protected virtual void Awake() {

    }

    void Update() {
        if (_stage == null || _unit == null) {
            return;
        }
        transform.localPosition = new Vector3(Unit.Position.X, _stage.Heights[Unit.Position], Unit.Position.Y);
    }

    public void Move(List<MapCoordinate> path) {
        MapCoordinate lastCoord = path[path.Count-1];
        Unit.MoveTo(lastCoord);
    }

    public void Attack(IMBUnit target) {
        Unit.Attack(target.Unit);
        target.ReceiveAttack();
    }

    public void ReceiveAttack() {

    }

    public void FinishTurn(int turnNum) {
        Unit.FinishTurn(turnNum);        
    }

    public void Destroy() {
        // Play animation
        Destroy(this.gameObject);
    }

    public override void Click() {
        _stage.ClickUnit(this);
    }
}
