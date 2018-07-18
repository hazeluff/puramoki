using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBStage : MonoBehaviour {
    private static CustomLogger LOGGER = new CustomLogger(typeof(MBStage).ToString());

    [SerializeField]
    private Transform cursor;
    [SerializeField]
    private MBStageCamera mbCamera;
    [SerializeField]
    private GameObject unitMenu;

    private InputManager input;

    public ControlState State { get; private set; }

    private ReversableDictionary<MapCoordinate, MBTile> tiles = new ReversableDictionary<MapCoordinate, MBTile>();
    public Dictionary<MapCoordinate, int> Heights { get; private set; }

    private ReversableDictionary<MBUnit, MapCoordinate> units = new ReversableDictionary<MBUnit, MapCoordinate>();

    public bool IsOccupied (MapCoordinate coordinate) {
        return units.ContainsKey(coordinate);
    }

    private MapCoordinate cursorPos = new MapCoordinate(0, 0);

    private MapCoordinate selected = null;

    private HashSet<MapCoordinate> range;
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

    public enum ControlState {
        DESELECTED, UNIT_MENU, UNIT_MOVE, WAIT
    }

	void Awake () {
        unitMenu.SetActive(false);
        State = ControlState.DESELECTED;
        Heights = new Dictionary<MapCoordinate, int>();

        // Register Tiles
        MBTile[] tileList = FindObjectsOfType<MBTile>();
        foreach (MBTile tile in tileList) {
            Vector3 tilePos = tile.transform.position;
            MapCoordinate mapCoordinate = new MapCoordinate(Mathf.RoundToInt(tilePos.x), Mathf.RoundToInt(tilePos.z));
            tiles.Add(mapCoordinate, tile);
            Heights.Add(mapCoordinate, Mathf.RoundToInt(tilePos.y - tile.transform.localPosition.y));
            tile.setStage(this);
        }

        // Register Units
        MBUnit[] unitList = FindObjectsOfType<MBUnit>();
        foreach(MBUnit mbUnit in unitList) {
            Vector3 unitPos = mbUnit.transform.position;
            MapCoordinate mapCoordinate = new MapCoordinate(Mathf.RoundToInt(unitPos.x), Mathf.RoundToInt(unitPos.z));
            units.Add(mbUnit, mapCoordinate);
            mbUnit.setStage(this);
        }
    }

    private void Start() {
        input = InputManager.get();
        MoveCursorTo(cursorPos);
    }

    void Update () {
        UpdateCursor();
	}

    private void UpdateCursor() {
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
                break;
            case ControlState.UNIT_MENU:
                break;
            case ControlState.UNIT_MOVE:
                MoveCursor();

                if (input.A) {
                    if (!IsOccupied(cursorPos) && InRange(cursorPos)) {
                        MoveUnit(units.Get(selected), new List<MapCoordinate>() { selected, cursorPos });
                        return;
                    }
                } else if (input.B) {
                    CancelMoveSelection();
                    OpenUnitMenu();
                    return;
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
        ChangeState(ControlState.UNIT_MOVE);
        unitMenu.SetActive(false);
        range = FindRange(units.Get(selected).Unit, selected);
        foreach (MapCoordinate coord in range) {
            tiles.Get(coord).setMoveRangeColor();
        }
    }

    public void CancelMoveSelection() {
        cursorPos = selected;
        MoveCursorTo(cursorPos);
        ResetRange();
        OpenUnitMenu();
    }

    private HashSet<MapCoordinate> FindRange(IUnit unit, MapCoordinate origin) {
        int unitMove = unit.CurrentMv;
        HashSet<MapCoordinate> range = new HashSet<MapCoordinate>();
        FindRange(unit, range, unitMove, origin);
        return range;
    }

    private void FindRange(IUnit unit, HashSet<MapCoordinate> listToAddTo, int mvLeft, MapCoordinate currentNode) {
        if (!tiles.ContainsKey(currentNode)) {
            return;
        }

        if (listToAddTo.Contains(currentNode)) {
            return;
        }

        ITile currentTile = tiles.Get(currentNode).Tile;
        if (!currentTile.Traversable) {
            return;
        }

        listToAddTo.Add(currentNode);

        if (listToAddTo.Count > 0) {
            mvLeft -= currentTile.MoveCost(unit);
        }

        if (mvLeft < 0) {
            return;
        }

        listToAddTo.Add(currentNode);
        foreach (MapCoordinate direction in new MapCoordinate[] { MapCoordinate.UP, MapCoordinate.DOWN, MapCoordinate.LEFT, MapCoordinate.RIGHT }) {
            MapCoordinate adjacentNode = currentNode + direction;
            FindRange(unit, listToAddTo, mvLeft, adjacentNode);
        }
    }

    public void OpenUnitMenu() {
        ChangeState(ControlState.UNIT_MENU);
        unitMenu.SetActive(true);
        selected = cursorPos;
    }

    public void CancelUnitMenu() {
        ChangeState(ControlState.DESELECTED);
        unitMenu.SetActive(false);
        selected = null;
    }

    private void MoveUnit(MBUnit unit, List<MapCoordinate> path) {
        ResetRange();        

        units.Remove(unit);
        units.Add(unit, path[path.Count - 1]);
        unit.Move(path);
        ChangeState(ControlState.DESELECTED);
        selected = null;
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
        if (InRange(coordinate)) {
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

    public void ClickTile(MBTile tile) {
        MapCoordinate coordinate = tiles.Get(tile);
        switch (State) {
            case ControlState.DESELECTED:
                MoveCursorTo(coordinate);
                break;
            case ControlState.UNIT_MOVE:
                if (InRange(coordinate)) {
                    MoveUnit(units.Get(selected), new List<MapCoordinate>() { selected, coordinate });
                } else {
                    CancelMoveSelection();
                    CancelUnitMenu();
                }
                MoveCursorTo(coordinate);
                break;
            case ControlState.UNIT_MENU:
                if (InRange(coordinate)) {
                    OpenUnitMenu();
                } else {
                    CancelUnitMenu();
                }
                MoveCursorTo(coordinate);
                break;

            default:
                break;
        }
        MoveCursorTo(coordinate);
    }

    public void ClickUnit(MBUnit unit) {
        MoveCursorTo(units.Get(unit));
        if(State == ControlState.DESELECTED || State == ControlState.UNIT_MENU) {
            OpenUnitMenu();
        }
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
