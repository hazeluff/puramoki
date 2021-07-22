using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MBUnit : MBClickable, IMBUnit {

    private MBStage _stage;
    
    protected IStageUnit _unit;
    public IStageUnit Unit { get { return _unit; } }

    [SerializeField]
    private bool _isPlayer = false;
    public bool IsPlayer { get { return _isPlayer; } }

    public float Height { get { return Unit.Build.Height; } }

    public void Init(MBStage stage, IStageUnit unit, bool isPlayer, MapCoordinate pos) {
        _stage = stage;
        if (unit != null) {
            _unit = unit;
        }
        _isPlayer = isPlayer;
        _unit.Init(pos);
    }

    protected virtual void Awake() {

    }

    void Update() {
        if (_stage == null || _unit == null) {
            return;
        }
        SetModelPosition();
    }

    void SetModelPosition() {
        float mapHeight = _stage.Heights[Unit.Position];
        transform.localPosition = new Vector3(Unit.Position.X, mapHeight + Height, Unit.Position.Y);
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
