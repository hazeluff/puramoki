﻿using System.Linq;
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
    private ReversableDictionary<MapCoordinate, MBTile> tiles = new ReversableDictionary<MapCoordinate, MBTile>();
    public Dictionary<MapCoordinate, int> Heights { get; private set; }
    private ReversableDictionary<MBUnit, MapCoordinate> units = new ReversableDictionary<MBUnit, MapCoordinate>();
    public MapCoordinate GetUnitPos(MBUnit unit) {
        return units.Get(unit);
    }


    public bool IsOccupied(MapCoordinate coordinate) {
        return units.ContainsValue(coordinate);
    }

    private MapCoordinate cursorPos = new MapCoordinate(0, 0);
    
    public int TurnCount { get; private set; }
    private MBUnit currentUnit = null;
    private int currentUnitPrevCd = 0;
    private MapCoordinate selected = null;

    private HashSet<MapCoordinate> range;

    public bool AnySelected() {
        return selected != null;
    }

    public IStageUnit GetSelectedUnit() {
        return units.Get(selected).Unit;
    }


    private bool InBounds(MapCoordinate coordinate) {
        return tiles.ContainsKey(coordinate);
    }

    private void ResetRange() {
        foreach (MapCoordinate coord in range) {
            tiles.Get(coord).setDefaultColor();
        }
        range = null;
    }

    private bool InRange(MapCoordinate coordinate) {
        if (range == null) {
            return true;
        }
        return range.Contains(coordinate);
    }

    public bool IsSelected(MBUnit unit) {
        return selected != null && units.Get(selected) == unit;
    }

	void Awake () {
        menuUI.SetActive(false);
        unitMenuUI.SetActive(false);
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
        foreach (MBUnit mbUnit in FindObjectsOfType<MBUnit>()) {
            factions.Add(mbUnit.Unit.Faction);
        }

        FactionDiplomacy = new FactionDiplomacy(factions);

        // Register Units
        foreach (MBUnit mbUnit in FindObjectsOfType<MBUnit>()) {
            Vector3 unitPos = mbUnit.transform.position;
            MapCoordinate mapCoordinate = new MapCoordinate(Mathf.RoundToInt(unitPos.x), Mathf.RoundToInt(unitPos.z));
            units.Add(mbUnit, mapCoordinate);
            mbUnit.setStage(this);
            FactionDiplomacy.RegisterUnit(mbUnit);
        }

        // Init Units
        int fastestSpd = units.GetDictionary().Keys.Aggregate((agg, next) => next.Unit.c_Spd > agg.Unit.c_Spd ? next : agg).Unit.c_Spd;
        foreach (MBUnit mbUnit in units.GetDictionary().Keys) {
            mbUnit.Unit.InitCooldown(fastestSpd);
            if (mbUnit.IsPlayer) {
                mbUnit.Unit.SetLastTurn(-1);
            }
        }

        TurnCount = 0;
        FinishUnitTurn();
    }

    private void Start() {
        input = InputManager.get();
        MoveCursorTo(cursorPos);
    }

    void Update () {
        UpdateInput();
	}

    private void UpdateInput() {
        input = InputManager.get();

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

        switch (State) {
            case ControlState.DESELECTED:
                MoveCursor();

                if (input.A) {
                    if (selected == null && IsOccupied(cursorPos)) {
                        OpenUnitMenu();
                        return;
                    }
                }

                if (input.BACK) {
                    OpenMenu();
                }
                break;
            case ControlState.UNIT_MENU:
                break;
            case ControlState.UNIT_MOVE:
                MoveCursor();

                if (input.A) {
                    if (!IsOccupied(cursorPos) && InRange(cursorPos)) {
                        MoveUnit(units.Get(selected), new List<MapCoordinate>() { selected, cursorPos });
                        CloseRangeSelection();
                        OpenUnitMenu();
                        return;
                    }
                }

                if (input.B) {
                    CloseRangeSelection();
                    OpenUnitMenu();
                    return;
                }
                break;
            case ControlState.UNIT_ATTACK:
                if (input.A) {
                    if (!IsOccupied(cursorPos) && InRange(cursorPos)) {
                        ClickUnit(units.Get(cursorPos));
                        return;
                    }
                }

                if (input.B) {
                    CloseRangeSelection();
                    OpenUnitMenu();
                    return;
                }
                break;
            case ControlState.MENU:
                if (input.B || input.BACK) {
                    CloseMenu();
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

    public void StartMoveSelection() {
        if (!units.Get(selected).Unit.Moved) {
            ChangeState(ControlState.UNIT_MOVE);
            unitMenuUI.SetActive(false);
            range = FindMoveRange(currentUnit, selected);
            foreach (MapCoordinate coord in range) {
                tiles.Get(coord).setMoveRangeColor();
            }
        }
    }

    public void CloseRangeSelection() {
        cursorPos = selected;
        MoveCursorTo(cursorPos);
        ResetRange();
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

    public void StartAttackSelection() {
        ChangeState(ControlState.UNIT_ATTACK);
        unitMenuUI.SetActive(false);
        range = FindAttackRange(units.Get(selected), selected);
        foreach (MapCoordinate coord in range) {
            tiles.Get(coord).setAttackRangeColor();
        }
    }

    private HashSet<MapCoordinate> FindAttackRange(IMBUnit mbUnit, MapCoordinate origin) {
        int maxAttackRange = mbUnit.Unit.Profile.Weapon.RangeMax;
        int minAttackRange = mbUnit.Unit.Profile.Weapon.RangeMin;
        HashSet<MapCoordinate> range = new HashSet<MapCoordinate>();
        for (int xOffset = -maxAttackRange; xOffset <= maxAttackRange; xOffset++) {
            int yAbsRange = maxAttackRange - Math.Abs(xOffset);
            for (int yOffset = -yAbsRange; yOffset <= yAbsRange; yOffset++) {
                MapCoordinate offset = new MapCoordinate(xOffset, yOffset);
                MapCoordinate tile = origin + offset;
                if (tiles.ContainsKey(tile) &&
                    offset != new MapCoordinate(0,0) &&
                    (Math.Abs(xOffset) + Math.Abs(yOffset) >= minAttackRange)) {
                    range.Add(tile);
                }
            }
        }
        return range;
    }

    public void OpenUnitMenu() {
        ChangeState(ControlState.UNIT_MENU);
        unitMenuUI.SetActive(true);
        selected = cursorPos;
    }

    public void CloseUnitMenu() {
        unitMenuUI.SetActive(false);
        Deselect();
    }

    private void MoveUnit(MBUnit unit, List<MapCoordinate> path) {
        units.Remove(unit);
        units.Add(unit, path[path.Count - 1]);
        unit.Move(path);
    }

    private static readonly MapCoordinate[][] MAP_MOVEMENT_MAP = new MapCoordinate[][] {
        new MapCoordinate[] { MapCoordinate.UP , MapCoordinate.RIGHT, MapCoordinate.DOWN, MapCoordinate.LEFT },
        new MapCoordinate[] { MapCoordinate.LEFT, MapCoordinate.UP , MapCoordinate.RIGHT, MapCoordinate.DOWN },
        new MapCoordinate[] { MapCoordinate.DOWN, MapCoordinate.LEFT, MapCoordinate.UP , MapCoordinate.RIGHT },
        new MapCoordinate[] { MapCoordinate.RIGHT, MapCoordinate.DOWN , MapCoordinate.LEFT, MapCoordinate.UP }
    };

    private void MoveCursor(int direction) {
        MapCoordinate newCursorPos = cursorPos + MAP_MOVEMENT_MAP[mbCamera.Orientation][direction];
        MoveCursorTo(newCursorPos);
    }

    private void MoveCursorTo(MapCoordinate coordinate) {
        if (InBounds(coordinate) && InRange(coordinate)) {
            MapCoordinate oldPos = cursorPos;
            this.cursorPos = coordinate;
            cursor.localPosition = cursorPos.Vector3 + (Vector3.up * Heights[cursorPos]);
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

    public void FinishUnitTurn() {
        State = ControlState.WAIT;
        if (currentUnit != null) {
            currentUnit.Unit.SetLastTurn(TurnCount++);
        }
        currentUnit = NextUnitTurn();
        currentUnit.Unit.ResetForTurn();
        Debug.Log("Current Unit: " + currentUnit);
        MoveCursorTo(units.Get(currentUnit));
        if (currentUnit.IsPlayer) {
            State = ControlState.UNIT_MENU;
            OpenUnitMenu();
        } else {
            StartCoroutine("AutomateTurn");
        }
    }

    void AutomateTurn() {
        Debug.Log("Automate Unit: " + currentUnit.Unit.Profile.Name);
        // Do stuff
        currentUnit.Unit.AddCooldown(500);
        FinishUnitTurn();
    }

    private MBUnit NextUnitTurn() {
        MBUnit nextUnit = new List<MBUnit>(this.units.GetDictionary().Keys)
            .Aggregate((agg, next) => {
                if (next.Unit.Cooldown == agg.Unit.Cooldown) {
                    return next.Unit.LastTurn < agg.Unit.LastTurn ? next : agg;
                }
                return next.Unit.Cooldown < agg.Unit.Cooldown ? next : agg;
            });
        int cd = nextUnit.Unit.Cooldown;
        foreach (KeyValuePair<MBUnit, MapCoordinate> unit in this.units) {
            unit.Key.Unit.ReduceCooldown(cd);
        }
        return nextUnit;
    }

    public void ClickTile(MBTile tile) {
        MapCoordinate coordinate = tiles.Get(tile);
        MoveCursorTo(coordinate);
        switch (State) {
            case ControlState.DESELECTED:
                if (units.Get(currentUnit).Equals(coordinate)) {
                    OpenUnitMenu();
                } else {
                    CloseUnitMenu();
                }
                break;
            case ControlState.UNIT_MENU:
                if (!units.Get(currentUnit).Equals(coordinate)) {
                    CloseUnitMenu();
                }
                break;
            case ControlState.UNIT_MOVE:
                if (InRange(coordinate) && !IsOccupied(cursorPos)) {
                    MoveUnit(units.Get(selected), new List<MapCoordinate>() { selected, coordinate });
                    CloseRangeSelection();
                    Deselect();
                }
                break;
            case ControlState.UNIT_ATTACK:
                if (InRange(cursorPos) && IsOccupied(cursorPos)) {
                    ClickUnit(units.Get(cursorPos));                
                }
                break;
            default:
                break;
        }
    }

    public void ClickUnit(MBUnit unit) {
        MapCoordinate pos = units.Get(unit);
        MoveCursorTo(pos);
        switch (State) {
            case ControlState.DESELECTED:
            case ControlState.UNIT_MENU:
                MoveCursorTo(pos);
                if (currentUnit.Equals(unit)) {
                    OpenUnitMenu();
                } else {
                    CloseUnitMenu();
                }
                break;
            case ControlState.UNIT_ATTACK:
                MoveCursorTo(pos);
                GetSelectedUnit().Attack(unit.Unit);
                CloseRangeSelection();
                break;
            default:
                break;
        }
    }

    public void OpenMenu() {
        menuUI.SetActive(true);
    }

    private void CloseMenu() {
        menuUI.SetActive(false);
    }

    private void Deselect() {
        ChangeState(ControlState.DESELECTED);
        selected = null;
    }

    private void ChangeState(ControlState newState) {
        StartCoroutine(IEnumChangeState(newState));
    }

    IEnumerator IEnumChangeState(ControlState newState) {
        State = ControlState.WAIT;
        yield return null;
        State = newState;
    }
}
