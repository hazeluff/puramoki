using System.Collections.Generic;
using UnityEngine;

public interface IMBUnit {
    IStageUnit Unit { get; }
    bool IsPlayer { get; }

    void Init(MBStage stage, IStageUnit unit, bool isPlayer, MapCoordinate pos);

    // Actions
    void Move(List<MapCoordinate> path);
    void Attack(IMBUnit target);
    void ReceiveAttack();
    void Destroy();

    // Finish Turn
    void FinishTurn(int turnNum);
}
