using System;
using System.Collections.Generic;
using UnityEngine;

public class MBStageTest : MonoBehaviour {
    private static CustomLogger LOGGER = new CustomLogger(typeof(MBStageTest).ToString());

    private InputManager input;

    private Dictionary<MapCoordinate, MBTileTest> tiles = new Dictionary<MapCoordinate, MBTileTest>();

    [SerializeField]
    private Transform cursor;
    MapCoordinate cursorPos = new MapCoordinate(0, 0);

	void Awake () {
        MBTileTest[] tileList = FindObjectsOfType<MBTileTest>();
        foreach (MBTileTest tile in tileList) {
            Vector3 tilePos = tile.transform.localPosition;
            MapCoordinate mapCoordinate = new MapCoordinate(Mathf.RoundToInt(tilePos.x), Mathf.RoundToInt(tilePos.z));
            tile.setCoordinates(mapCoordinate);
            tiles.Add(mapCoordinate, tile);
            tile.setStage(this);
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

        MBTileTest tile = tiles[coordinate];
        if(tile != null) {
            tile.setColor(Color.red);
            foreach (KeyValuePair<MapCoordinate, MBTileTest> tileEntry in tiles) {
                MBTileTest mapTile = tileEntry.Value;
                if (mapTile != tile) {
                    mapTile.setColor(Color.white);
                }
            }
        }
    }

    public void ClickTile(MBTileTest tile) {
        MoveCursorTo(tile.Coordinates);
    }
}
