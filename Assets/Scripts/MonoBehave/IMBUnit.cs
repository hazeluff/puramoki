using System.Collections.Generic;
using UnityEngine;

public interface IMBUnit {
    void Init(MBStage stage, IStageUnit unit, MapCoordinate pos);
    IStageUnit Unit { get; }
    bool IsPlayer { get; }

    // Character Model
    float ModelHeight { get; }

    // Actions
    void Move(List<MapCoordinate> path);
    void Attack(IMBUnit target);
    void ReceiveAttack();
    void Destroy();

    // Finish Turn
    void FinishTurn(int turnNum);
}
