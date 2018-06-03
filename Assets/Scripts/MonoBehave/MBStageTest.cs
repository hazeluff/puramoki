﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class MBStageTest : MonoBehaviour {
    private static CustomLogger LOGGER = new CustomLogger(typeof(MBStageTest).ToString());

    private InputManager input;

    private ReversableDictionary<MapCoordinate, MBTile> tiles = new ReversableDictionary<MapCoordinate, MBTile>();
    private ReversableDictionary<MBUnit, MapCoordinate> units = new ReversableDictionary<MBUnit, MapCoordinate>();

    [SerializeField]
    private Transform cursor;
    public MBStageCamera mbCamera;

    MapCoordinate cursorPos = new MapCoordinate(0, 0);

    ControlState controlState = ControlState.DESELECTED;
    MapCoordinate selected = null;

    public bool IsSelected(MBUnit unit) {        
        return selected != null && units.Get(selected) == unit;
    }

    public enum ControlState {
        DESELECTED, UNIT_MOVE
    }

	void Awake () {
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

        if(input.DPAD.UP || input.LEFT_STICK.UP) {
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

        if(input.A) {
            if (controlState == ControlState.DESELECTED && selected == null && units.ContainsKey(cursorPos)) {
                controlState = ControlState.UNIT_MOVE;
                selected = cursorPos;
                return;
            } else if (controlState == ControlState.UNIT_MOVE && !units.ContainsKey(cursorPos)) {
                MoveUnit(units.Get(selected), new List<MapCoordinate>() { selected, cursorPos });
                return;
            }
        } else if(input.B) {
            controlState = ControlState.DESELECTED;
            selected = null;
            return;
        }
    }

    private void MoveUnit(MBUnit unit, List<MapCoordinate> path) {
        LOGGER.info("Moved Unit");
        units.Remove(unit);
        units.Add(unit, path[path.Count - 1]);
        unit.Move(path);
        controlState = ControlState.DESELECTED;
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
        MoveCursorTo(coordinate);
        if(controlState == ControlState.UNIT_MOVE) {
            if(!units.ContainsKey(coordinate)) {
                MoveUnit(units.Get(selected), new List<MapCoordinate>() { selected, coordinate });
            }
        }
    }

    public void ClickUnit(MBUnit unit) {
        MoveCursorTo(units.Get(unit));
        if(controlState == ControlState.DESELECTED) {
            selected = units.Get(unit);
            controlState = ControlState.UNIT_MOVE;
        }
    }
}
