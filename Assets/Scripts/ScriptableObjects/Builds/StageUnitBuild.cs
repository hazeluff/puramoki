using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  version of UnitBuild that be saved as an asset
/// </summary>
[CreateAssetMenu(fileName = "EnemyUnitBuild", menuName = "Builds/EnemyUnitBuild", order = 1)]
public class StageUnitBuild : UnitBuild {
    [SerializeField]
    private CoreUnitBase _coreUnitBase;
    public CoreUnitBase CoreUnitBase => _coreUnitBase;
}
