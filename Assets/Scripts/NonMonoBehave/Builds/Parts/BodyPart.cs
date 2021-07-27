using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "BodyPart", menuName = "Builds/Parts/Body", order = 1)]
public class BodyPart : BuildPart {
    [SerializeField]
    private Vector3 _leftArmJoinPoint;
    [SerializeField]
    private Vector3 _rightArmJoinPoint;
    [SerializeField]
    private Vector3 _coreJoinPoint;

    public Vector3 LeftArmJoinPoint { get { return _leftArmJoinPoint; } }
    public Vector3 RightArmJoinPoint { get { return _rightArmJoinPoint; } }
    public Vector3 CoreJoinPoint { get { return _coreJoinPoint; } }

}
