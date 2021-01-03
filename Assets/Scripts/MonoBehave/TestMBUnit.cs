using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestMBUnit : MBClickable, IMBUnit {

    private MeshRenderer _renderer;

    private MBStage stage;
    
    private StageUnit _unit;
    public IStageUnit Unit { get { return _unit; } }
    [SerializeField]
    private bool _isPlayer;
    public bool IsPlayer { get { return _isPlayer; } }

    // Init Unit
    [SerializeField]
    private string _name;
    [SerializeField]
    private Faction _faction;
    [SerializeField]
    private int _lvl = 1;
    [SerializeField]
    private int _currentExp = 0;
    [SerializeField]
    private BaseUnit _baseUnit;
    [SerializeField]
    private Weapon _weapon;
    [SerializeField]
    private Equipment[] _equipment = new Equipment[3];

    private void Awake() {
        _renderer = GetComponent<MeshRenderer>();
        _unit = new StageUnit(_name, _faction, _lvl, _currentExp, _baseUnit, _weapon, _equipment);

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

    public void Attack(IMBUnit target) {
        Unit.Attack(target.Unit);
        target.ReceiveAttack();
    }

    public void ReceiveAttack() {

    }

    public void FinishTurn(int lastTurnCount) {
        Unit.SetLastTurn(lastTurnCount);
        Unit.FinishTurn();        
    }

    public void Destroy() {
        // Play animation
        Destroy(this.gameObject);
    }

    public override void Click() {
        stage.ClickUnit(this);
    }
}
