using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MBStage : MonoBehaviour {
    private static CustomLogger LOGGER = new CustomLogger(typeof(MBStage).ToString());

    [SerializeField]
    private Transform cursor;
    [SerializeField]
    private MBStageCamera mbCamera;
    [SerializeField]
    private GameObject unitMenuUI;
    [SerializeField]
    private GameObject menuUI;

    private InputManager input;

    public ControlState State { get; private set; }

    public enum ControlState {
        DESELECTED, UNIT_MENU, UNIT_MOVE, UNIT_ATTACK, MENU, WAIT
    }

    FactionDiplomacy FactionDiplomacy;

    /*
     * Map
     */
    private ReversableDictionary<MapCoordinate, MBTile> tiles = new ReversableDictionary<MapCoordinate, MBTile>();
    public Dictionary<MapCoordinate, int> Heights { get; private set; }

    private bool InBounds(MapCoordinate coordinate) {
        return tiles.ContainsKey(coordinate);
    }

    /*
     * Units
     */
    private ReversableDictionary<IMBUnit, MapCoordinate> units = new ReversableDictionary<IMBUnit, MapCoordinate>();

    public void RemoveUnit(IMBUnit unit) {
        units.Remove(unit);
    }

    public MapCoordinate GetUnitPos(IMBUnit unit) {
        return units.Get(unit);
    }

    public bool IsOccupied(MapCoordinate coordinate) {
        return units.ContainsValue(coordinate);
    }

    private bool InRange(MapCoordinate coordinate) {
        if (_range == null) {
            return false;
        }
        return _range.Contains(coordinate);
    }

    private void UpdateCamera() {
        if (!mbCamera.Rotating) {
            if (input.RB) {
                mbCamera.Rotate(true);
                return;
            } else if (input.LB) {
                mbCamera.Rotate(false);
                return;
            }
        }

        if (!mbCamera.Tilting) {
            if (input.LT.Pressed) {
                mbCamera.Zoom(true);
                return;
            }
        }

        if (!mbCamera.Zooming) {
            if (input.RT.Pressed) {
                mbCamera.Tilt(true);
                return;
            }
        }

        if (mbCamera.Rotating || mbCamera.Tilting || mbCamera.Zooming) {
            return;
        }
    }

    /*
     * Game Logic
     */

    // Game State Update
    /**
     * Maintains the game state by continually validating and fixing game state issues. Puts events on the stack
     */
    private void MaintainGameState() {
        ValidateAndFixGameState();
        while (events.Count > 0) {
            Action evnt = events.Pop();
            evnt();
            ValidateAndFixGameState();
        }
    }

    /**
     *  Validates and fixes the game state and adds animations/events to the event stack
     */
    private void ValidateAndFixGameState() {
        List<IMBUnit> deadUnits = units.GetDictionary().Keys.Where(mbUnit => mbUnit.Unit.c_Hp <= 0).ToList();
        foreach (IMBUnit mbUnit in deadUnits) {
            if (mbUnit.Unit.c_Hp <= 0) {
                RemoveUnit(mbUnit);
                events.Push(() => mbUnit.Destroy());
            }
        }
    }

    // Turn Order
    public int TurnCount { get; private set; }
    private IMBUnit _currentUnit = null;
    public IMBUnit CurrentUnit { get { return _currentUnit; } }
    public MapCoordinate CurrentUnitPos { get { return units.Get(CurrentUnit); } }

    public List<IMBUnit> GetTurnOrder() {
        List<IMBUnit> units = this.units.GetDictionary()
            .ToList<KeyValuePair<IMBUnit, MapCoordinate>>()
            .ConvertAll<IMBUnit>(pair => pair.Key)
            .OrderBy(unit => unit.Unit.Cooldown)
            .ThenBy(unit => unit.Unit.LastTurn)
            .ToList<IMBUnit>();
        return units;
    }

    private IMBUnit NextUnitTurn() {
        IMBUnit nextUnit = new List<IMBUnit>(this.units.GetDictionary().Keys)
            .Aggregate((agg, next) => {
                if (next.Unit.Cooldown == agg.Unit.Cooldown) {
                    return next.Unit.LastTurn < agg.Unit.LastTurn ? next : agg;
                }
                return next.Unit.Cooldown < agg.Unit.Cooldown ? next : agg;
            });
        int cd = nextUnit.Unit.Cooldown;
        foreach (KeyValuePair<IMBUnit, MapCoordinate> unit in this.units) {
            unit.Key.Unit.ReduceCooldown(cd);
        }
        return nextUnit;
    }

    private void FinishUnitTurn() {
        if (_currentUnit != null) {
            _currentUnit.FinishTurn(TurnCount++);
        }
        _currentUnit = NextUnitTurn();
        StartUnitTurn();
    }

    // Bot/AI
    IEnumerator AutomateTurn() {
        Debug.Log("Automate Unit: " + _currentUnit.Unit.Build.Name);
        // Do stuff
        yield return new WaitForSeconds(3);
        CurrentUnit.Move(new List<MapCoordinate>() { CurrentUnitPos });
        UA_FinishUnitTurn();
    }

    /*
     * User Actions (Controller/UI Interaction invoke these)
     */
    public void UA_OpenMenu() {
        menuUI.SetActive(true);
    }

    private void UA_CloseMenu() {
        menuUI.SetActive(false);
    }

    public void UA_OpenUnitMenu() {
        ChangeState(ControlState.UNIT_MENU);
        unitMenuUI.SetActive(true);
    }

    public void UA_CloseUnitMenu() {
        unitMenuUI.SetActive(false);
        Deselect();
    }

    public void UA_CloseRangeSelection() {
        MoveCursorTo(CurrentUnitPos);
        if (_range != null) {
            // Reset tile colors
            foreach (MapCoordinate coord in _range) {
                tiles.Get(coord).setDefaultColor();
            }
        }
        _range = null;
    }

    // Unit Move Range
    public void UA_StartMoveSelection() {
        if (!CurrentUnit.Unit.Moved) {
            ChangeState(ControlState.UNIT_MOVE);
            unitMenuUI.SetActive(false);
            _range = FindMoveRange(CurrentUnit, CurrentUnitPos);
            // Set tile colors
            foreach (MapCoordinate coord in _range) {
                tiles.Get(coord).setMoveRangeColor();
            }
        }
    }

    private HashSet<MapCoordinate> FindMoveRange(IMBUnit mbUnit, MapCoordinate origin) {
        int unitMove = mbUnit.Unit.c_Mv;
        Dictionary<MapCoordinate, int> range = new Dictionary<MapCoordinate, int>();
        FindMoveRange(mbUnit, range, unitMove, origin);
        return new HashSet<MapCoordinate>(range.Keys);
    }

    private void FindMoveRange(IMBUnit mbUnit, Dictionary<MapCoordinate, int> rangeOutput, int mvLeft, MapCoordinate currentNode) {
        if (!tiles.ContainsKey(currentNode)) {
            return;
        }

        // If the node has been added/checked already and had more mvLeft
        if (rangeOutput.ContainsKey(currentNode) && mvLeft <= rangeOutput[currentNode]) {
            return;
        }

        ITile currentTile = tiles.Get(currentNode).Tile;
        if (!currentTile.Traversable) {
            return;
        }

        // Remove mvLeft when it is not the unit's current tile
        if (rangeOutput.Count > 0) {
            mvLeft -= currentTile.MoveCost(mbUnit.Unit);
        }

        if (mvLeft < 0) {
            return;
        }

        rangeOutput[currentNode] = mvLeft;

        foreach (MapCoordinate direction in new MapCoordinate[] { MapCoordinate.UP, MapCoordinate.DOWN, MapCoordinate.LEFT, MapCoordinate.RIGHT }) {
            MapCoordinate adjacentNode = currentNode + direction;
            FindMoveRange(mbUnit, rangeOutput, mvLeft, adjacentNode);
        }
    }

    // Move
    bool UA_MoveUnit(MapCoordinate mapPos) {
        if (IsOccupied(mapPos)) {
            return false;
        }

        if (!InRange(mapPos)) {
            return false;
        }

        units.Remove(CurrentUnit);
        units.Add(CurrentUnit, mapPos);
        Debug.Log(units.Get(mapPos));

        CurrentUnit.Move(new List<MapCoordinate>() { CurrentUnitPos, mapPos });

        MaintainGameState();
        return true;
    }

    // Unit Attack Range
    public void UA_StartAttackSelection() {
        ChangeState(ControlState.UNIT_ATTACK);
        unitMenuUI.SetActive(false);
        _range = FindAttackRange(CurrentUnit, CurrentUnitPos);
        foreach (MapCoordinate coord in _range) {
            tiles.Get(coord).setAttackRangeColor();
        }
    }

    private HashSet<MapCoordinate> FindAttackRange(IMBUnit mbUnit, MapCoordinate origin) {
        int maxAttackRange = mbUnit.Unit.Build.Weapon.RangeMax;
        int minAttackRange = mbUnit.Unit.Build.Weapon.RangeMin;
        HashSet<MapCoordinate> range = new HashSet<MapCoordinate>();
        for (int xOffset = -maxAttackRange; xOffset <= maxAttackRange; xOffset++) {
            int yAbsRange = maxAttackRange - Math.Abs(xOffset);
            for (int yOffset = -yAbsRange; yOffset <= yAbsRange; yOffset++) {
                MapCoordinate offset = new MapCoordinate(xOffset, yOffset);
                MapCoordinate tile = origin + offset;
                if (tiles.ContainsKey(tile) &&
                    offset != new MapCoordinate(0, 0) &&
                    (Math.Abs(xOffset) + Math.Abs(yOffset) >= minAttackRange)) {
                    range.Add(tile);
                }
            }
        }
        return range;
    }

    // Attack
    bool UA_AttackTarget(MapCoordinate targetPos) {
        if (!IsOccupied(CursorPos)) {
            return false;
        }

        if (!InRange(CursorPos)) {
            return false;
        }

        CurrentUnit.Attack(units.Get(targetPos));

        UA_CloseRangeSelection();

        MaintainGameState();

        return true;
    }
    
    // Finish Turn
    public void UA_FinishUnitTurn() {
        UA_CloseUnitMenu();
        State = ControlState.WAIT;
        FinishUnitTurn();
    }

    /*
     * Controller Interactions
     */
    private void UpdateControllerInput() {
        switch (State) {
            case ControlState.DESELECTED:
                MoveCursor();

                if (input.A) {
                    if (CursorPos.Equals(CurrentUnitPos)) {
                        UA_OpenUnitMenu();
                        return;
                    }
                }

                if (input.BACK) {
                    UA_OpenMenu();
                }
                break;
            case ControlState.UNIT_MENU:
                break;
            case ControlState.UNIT_MOVE:
                MoveCursor();

                if (input.A) {
                    if (UA_MoveUnit(CursorPos)) {
                        UA_CloseRangeSelection();
                        UA_OpenUnitMenu();
                    }
                    return;
                }

                if (input.B) {
                    UA_CloseRangeSelection();
                    UA_OpenUnitMenu();
                    return;
                }
                break;
            case ControlState.UNIT_ATTACK:
                if (input.A) {
                    if (UA_AttackTarget(CursorPos)) {
                        MoveCursorTo(CurrentUnitPos);
                        UA_CloseRangeSelection();
                        UA_OpenUnitMenu();
                    }
                }

                if (input.B) {
                    UA_CloseRangeSelection();
                    UA_OpenUnitMenu();
                    return;
                }
                break;
            case ControlState.MENU:
                if (input.B || input.BACK) {
                    UA_CloseMenu();
                }
                break;
            default:
                break;
        }
    }

    public void MoveCursor() {
        if (input.DPAD.UP || input.LEFT_STICK.UP) {
            MoveCursor(0);
            return;
        } else if (input.DPAD.RIGHT || input.LEFT_STICK.RIGHT) {
            MoveCursor(1);
            return;
        } else if (input.DPAD.DOWN || input.LEFT_STICK.DOWN) {
            MoveCursor(2);
            return;
        } else if (input.DPAD.LEFT || input.LEFT_STICK.LEFT) {
            MoveCursor(3);
            return;
        }
    }

    /*
     * UI Interactions
     */
    public void ClickTile(MBTile tile) {
        MapCoordinate tilePos = tiles.Get(tile);
        MoveCursorTo(tilePos);
        switch (State) {
            case ControlState.DESELECTED:
                if (units.Get(_currentUnit).Equals(tilePos)) {
                    UA_OpenUnitMenu();
                } else {
                    UA_CloseUnitMenu();
                }
                break;
            case ControlState.UNIT_MENU:
                if (!units.Get(_currentUnit).Equals(tilePos)) {
                    UA_CloseUnitMenu();
                }
                break;
            case ControlState.UNIT_MOVE:
                if (UA_MoveUnit(tilePos)) {
                    UA_CloseRangeSelection();
                    UA_OpenUnitMenu();
                }
                break;
            case ControlState.UNIT_ATTACK:
                if (UA_AttackTarget(tilePos)) {
                    MoveCursorTo(CurrentUnitPos);
                    UA_CloseRangeSelection();
                    UA_OpenUnitMenu();
                }
                break;
            default:
                break;
        }
    }

    public void ClickUnit(TestMBUnit unit) {
        MapCoordinate unitPos = units.Get(unit);
        MoveCursorTo(unitPos);
        switch (State) {
            case ControlState.DESELECTED:
            case ControlState.UNIT_MENU:
                MoveCursorTo(unitPos);
                if (_currentUnit.Equals(unit)) {
                    UA_OpenUnitMenu();
                } else {
                    UA_CloseUnitMenu();
                }
                break;
            case ControlState.UNIT_ATTACK:
                if (UA_AttackTarget(CursorPos)) {
                    MoveCursorTo(CurrentUnitPos);
                    UA_CloseRangeSelection();
                    UA_OpenUnitMenu();
                }
                break;
            default:
                break;
        }
    }

    /*
     * UI
     */

     // Cursor
    private MapCoordinate _cursorPos;
    public MapCoordinate CursorPos { get { return _cursorPos; } }
    public IStageUnit CursorUnit { get { return units.ContainsValue(CursorPos) ? units.Get(CursorPos).Unit : null; } }

    private static readonly MapCoordinate[][] MAP_MOVEMENT_MAP = new MapCoordinate[][] {
        new MapCoordinate[] { MapCoordinate.UP , MapCoordinate.RIGHT, MapCoordinate.DOWN, MapCoordinate.LEFT },
        new MapCoordinate[] { MapCoordinate.LEFT, MapCoordinate.UP , MapCoordinate.RIGHT, MapCoordinate.DOWN },
        new MapCoordinate[] { MapCoordinate.DOWN, MapCoordinate.LEFT, MapCoordinate.UP , MapCoordinate.RIGHT },
        new MapCoordinate[] { MapCoordinate.RIGHT, MapCoordinate.DOWN , MapCoordinate.LEFT, MapCoordinate.UP }
    };

    private void MoveCursor(int direction) {
        MapCoordinate newCursorPos = CursorPos + MAP_MOVEMENT_MAP[mbCamera.Orientation][direction];
        MoveCursorTo(newCursorPos);
    }

    private void MoveCursorTo(MapCoordinate coordinate) {
        if (InBounds(coordinate)) {
            MapCoordinate oldPos = CursorPos;
            _cursorPos = coordinate;

            cursor.localPosition = CursorPos.Vector3 + (Vector3.up * Heights[CursorPos]);
            mbCamera.MoveTo(coordinate.Vector3);

            MBTile tile = tiles.Get(coordinate);
            if (tile != null) {
                tile.setSelectedColor();
                if (InRange(oldPos) && State == ControlState.UNIT_MOVE) {
                    tiles.Get(oldPos).setMoveRangeColor();
                } else {
                    tiles.Get(oldPos).setDefaultColor();
                }
            }
        }
    }

    // Range
    private HashSet<MapCoordinate> _range;
    public HashSet<MapCoordinate> Range { get { return _range; } }

    /*
     * State Changes
     */
    private void Deselect() {
        ChangeState(ControlState.DESELECTED);
    }

    private void ChangeState(ControlState newState) {
        StartCoroutine(IEnumChangeState(newState));
    }

    IEnumerator IEnumChangeState(ControlState newState) {
        State = ControlState.WAIT;
        yield return null;
        State = newState;
    }

    private void StartUnitTurn() {
        CurrentUnit.Unit.ResetForTurn();
        MoveCursorTo(CurrentUnitPos);
        if (CurrentUnit.IsPlayer) {
            ChangeState(ControlState.UNIT_MENU);
            UA_OpenUnitMenu();
        } else {
            StartCoroutine("AutomateTurn");
        }
    }

    /*
     * Events
     */
    Stack<Action> events = new Stack<Action>();

    void AddEvent(Action evnt) {
        events.Push(evnt);
    }


    /*
     * MonoBehaviour Awake, Start, Update
     */
    void Awake() {
        menuUI.SetActive(false);
        unitMenuUI.SetActive(false);
        _cursorPos = new MapCoordinate(0, 0);
        State = ControlState.DESELECTED;

        // Register Tiles
        MBTile[] tileList = FindObjectsOfType<MBTile>();
        Heights = new Dictionary<MapCoordinate, int>();
        foreach (MBTile tile in tileList) {
            Vector3 tilePos = tile.transform.position;
            MapCoordinate mapCoordinate = new MapCoordinate(Mathf.RoundToInt(tilePos.x), Mathf.RoundToInt(tilePos.z));
            tiles.Add(mapCoordinate, tile);
            Heights.Add(mapCoordinate, Mathf.RoundToInt(tilePos.y - tile.transform.localPosition.y));
            tile.setStage(this);
        }

        // Register Factions
        HashSet<Faction> factions = new HashSet<Faction>();
        foreach (TestMBUnit mbUnit in FindObjectsOfType<TestMBUnit>()) {
            factions.Add(mbUnit.Unit.Faction);
        }

        FactionDiplomacy = new FactionDiplomacy(factions);

        // Register Units
        foreach (TestMBUnit mbUnit in FindObjectsOfType<TestMBUnit>()) {
            Vector3 unitPos = mbUnit.transform.position;
            MapCoordinate mapCoordinate = new MapCoordinate(Mathf.RoundToInt(unitPos.x), Mathf.RoundToInt(unitPos.z));
            units.Add(mbUnit, mapCoordinate);
            mbUnit.setStage(this);
            mbUnit.Unit.Init(mapCoordinate);
            FactionDiplomacy.RegisterUnit(mbUnit);
        }

        // Init Units
        int fastestSpd = units.GetDictionary().Keys.Aggregate((agg, next) => next.Unit.c_Spd > agg.Unit.c_Spd ? next : agg).Unit.c_Spd;
        foreach (TestMBUnit mbUnit in units.GetDictionary().Keys) {
            mbUnit.Unit.InitCooldown(fastestSpd);
            if (mbUnit.IsPlayer) {
                mbUnit.Unit.SetLastTurn(-1);
            }
        }

        TurnCount = 0;
        _currentUnit = NextUnitTurn();
        StartUnitTurn();
    }

    private void Start() {
        input = InputManager.get();
        MoveCursorTo(CursorPos);
    }

    void Update() {
        UpdateCamera();
        UpdateControllerInput();
    }
}
