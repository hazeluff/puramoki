using System;
using System.Collections.Generic;
using UnityEngine;

public class MBStageTest : MonoBehaviour {
    private static CustomLogger LOGGER = new CustomLogger(typeof(MBStageTest).ToString());

    private InputManager input;

    private Dictionary<MapCoordinate, MBTile> tiles = new Dictionary<MapCoordinate, MBTile>();
    private Dictionary<MBUnit, MapCoordinate> units = new Dictionary<MBUnit, MapCoordinate>();
    private Dictionary<MapCoordinate, MBUnit> unitsByCoord = new Dictionary<MapCoordinate, MBUnit>();

    [SerializeField]
    private Transform cursor;

    MapCoordinate cursorPos = new MapCoordinate(0, 0);

    ControlState controlState = ControlState.DESELECTED;
    MapCoordinate selected = null;

    public enum ControlState {
        DESELECTED, UNIT_MOVE
    }

	void Awake () {
        // Register Tiles
        MBTile[] tileList = FindObjectsOfType<MBTile>();
        foreach (MBTile tile in tileList) {
            Vector3 tilePos = tile.transform.localPosition;
            MapCoordinate mapCoordinate = new MapCoordinate(Mathf.RoundToInt(tilePos.x), Mathf.RoundToInt(tilePos.z));
            tile.setCoordinates(mapCoordinate);
            tiles.Add(mapCoordinate, tile);
            tile.setStage(this);
        }

        // Register Units
        MBUnit[] unitList = FindObjectsOfType<MBUnit>();
        foreach(MBUnit mbUnit in unitList) {
            Vector3 tilePos = mbUnit.transform.localPosition;
            MapCoordinate mapCoordinate = new MapCoordinate(Mathf.RoundToInt(tilePos.x), Mathf.RoundToInt(tilePos.z));
            units.Add(mbUnit, mapCoordinate);
            unitsByCoord.Add(mapCoordinate, mbUnit);
        }
    }

    private void Start() {
        input = InputManager.get();
        MoveCursorTo(cursorPos);
    }

    void Update () {
        updateCursor();
	}

    private void updateCursor() {
        input = InputManager.get();
        if(input.DPAD.UP || input.LEFT_STICK.UP) {
            MoveCursor(ControllerDirection.UP);
        } else if (input.DPAD.DOWN || input.LEFT_STICK.DOWN) {
            MoveCursor(ControllerDirection.DOWN);
        } else if (input.DPAD.LEFT || input.LEFT_STICK.LEFT) {
            MoveCursor(ControllerDirection.LEFT);
        } else if (input.DPAD.RIGHT || input.LEFT_STICK.RIGHT) {
            MoveCursor(ControllerDirection.RIGHT);
        }

        if(input.A) {
            if (controlState == ControlState.DESELECTED && selected == null && unitsByCoord.ContainsKey(cursorPos)) {
                LOGGER.info("Selected");
                controlState = ControlState.UNIT_MOVE;
                selected = cursorPos;
            } else if (controlState == ControlState.UNIT_MOVE && !unitsByCoord.ContainsKey(cursorPos)) {
                LOGGER.info("Moved");
                MoveUnit(unitsByCoord[selected], new List<MapCoordinate>() { selected, cursorPos });
                controlState = ControlState.DESELECTED;
                selected = null;
            }
        } else if(input.B) {
            LOGGER.info("Cancelled");
            controlState = ControlState.DESELECTED;
            selected = null;
        }
    }

    private void MoveUnit(MBUnit unit, List<MapCoordinate> path) {
        unitsByCoord.Remove(path[0]);
        units.Remove(unit);
        unitsByCoord.Add(path[path.Count - 1], unit);
        units.Add(unit, path[path.Count - 1]);
        unit.Move(path);
    }

    private enum ControllerDirection {
        UP, DOWN, LEFT, RIGHT
    }

    private void MoveCursor(ControllerDirection direction/*, CamDirection camDirection*/) {
        MapCoordinate newCursorPos;
        switch (direction) {
            case ControllerDirection.UP:
                newCursorPos = cursorPos + MapCoordinate.UP;
                break;
            case ControllerDirection.DOWN:
                newCursorPos = cursorPos + MapCoordinate.DOWN;
                break;
            case ControllerDirection.LEFT:
                newCursorPos = cursorPos + MapCoordinate.LEFT;
                break;
            case ControllerDirection.RIGHT:
                newCursorPos = cursorPos + MapCoordinate.RIGHT;
                break;
            default:
                throw new SystemException("Invalid direction: " + direction);
        }
        if(tiles.ContainsKey(newCursorPos)) {
            MoveCursorTo(newCursorPos);
        }
    }

    private void MoveCursorTo(MapCoordinate coordinate) {
        this.cursorPos = coordinate;
        cursor.localPosition = cursorPos.Vector3;

        MBTile tile = tiles[coordinate];
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
        MoveCursorTo(tile.Coordinates);
    }
}
