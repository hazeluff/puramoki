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
    public GameObject unitMenu;

    private InputManager input;

    private ReversableDictionary<MapCoordinate, MBTile> tiles = new ReversableDictionary<MapCoordinate, MBTile>();
    private ReversableDictionary<MBUnit, MapCoordinate> units = new ReversableDictionary<MBUnit, MapCoordinate>();

    MapCoordinate cursorPos = new MapCoordinate(0, 0);
    
    public ControlState State { get; private set; }
    MapCoordinate selected = null;

    public bool IsSelected(MBUnit unit) {        
        return selected != null && units.Get(selected) == unit;
    }

    public enum ControlState {
        DESELECTED, UNIT_MENU, UNIT_MOVE, WAIT
    }

	void Awake () {
        unitMenu.SetActive(false);
        State = ControlState.DESELECTED;

        // Register Tiles
        MBTile[] tileList = FindObjectsOfType<MBTile>();
        foreach (MBTile tile in tileList) {
            Vector3 tilePos = tile.transform.localPosition;
            MapCoordinate mapCoordinate = new MapCoordinate(Mathf.RoundToInt(tilePos.x), Mathf.RoundToInt(tilePos.z));
            tiles.Add(mapCoordinate, tile);
            tile.setStage(this);
        }

        // Register Units
        MBUnit[] unitList = FindObjectsOfType<MBUnit>();
        foreach(MBUnit mbUnit in unitList) {
            Vector3 tilePos = mbUnit.transform.localPosition;
            MapCoordinate mapCoordinate = new MapCoordinate(Mathf.RoundToInt(tilePos.x), Mathf.RoundToInt(tilePos.z));
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

        if (input.RB) {
            mbCamera.Rotate(true);
            return;
        } else if (input.LB) {
            mbCamera.Rotate(false);
            return;
        }

        if (mbCamera.Rotating) {
            return;
        }

        switch (State) {
            case ControlState.DESELECTED:
                MoveCursor();

                if (input.A) {
                    if (selected == null && units.ContainsKey(cursorPos)) {
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
                    if (!units.ContainsKey(cursorPos)) {
                        MoveUnit(units.Get(selected), new List<MapCoordinate>() { selected, cursorPos });
                        return;
                    }
                } else if (input.B) {
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
        MapCoordinate newCursorPos = cursorPos + MAP_MOVEMENT_MAP[mbCamera.Rotation][direction];        
        if(tiles.ContainsKey(newCursorPos)) {
            MoveCursorTo(newCursorPos);
        }
        mbCamera.MoveTo(newCursorPos.Vector3);
    }

    private void MoveCursorTo(MapCoordinate coordinate) {
        this.cursorPos = coordinate;
        cursor.localPosition = cursorPos.Vector3;

        MBTile tile = tiles.Get(coordinate);
        if(tile != null) {
            tile.setColor(Color.red);
            foreach (KeyValuePair<MapCoordinate, MBTile> tileEntry in tiles) {
                MBTile mapTile = tileEntry.Value;
                if (mapTile != tile) {
                    mapTile.setColor(Color.white);
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
                MoveCursorTo(coordinate);
                if (!units.ContainsKey(coordinate)) {
                    MoveUnit(units.Get(selected), new List<MapCoordinate>() { selected, coordinate });
                }
                break;
            case ControlState.UNIT_MENU:
                if (units.ContainsKey(coordinate)) {
                    MoveCursorTo(coordinate);
                    OpenUnitMenu();
                }
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
