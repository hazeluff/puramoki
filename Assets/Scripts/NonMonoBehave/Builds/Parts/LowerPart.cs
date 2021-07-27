using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "LowerPart", menuName = "Builds/Parts/Lower", order = 1)]
public class LowerPart : BuildPart {
    [SerializeField]
    private Vector3 _joinPoint;
    public Vector3 JoinPoint { get { return _joinPoint; } }
}
