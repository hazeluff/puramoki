using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "LowerPart", menuName = "Builds/Parts/Lower", order = 1)]
public class LowerPart : BuildPart {
    [SerializeField]
    private float _height;
    [SerializeField]
    private Vector3 _joinPoint;

    public float Height { get { return _height; } }
    public Vector3 JoinPoint { get { return _joinPoint; } }
}
