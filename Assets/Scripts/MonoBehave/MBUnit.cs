using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBUnit : MonoBehaviour, IMBUnit {

    public void Move(List<MapCoordinate> path) {
        MapCoordinate lastCoord = path[path.Count-1];
        gameObject.transform.localPosition = new Vector3(lastCoord.X, 0.25f, lastCoord.Y);
    }
}
