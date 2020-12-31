using System.Collections.Generic;

public interface IMBUnit {
    IStageUnit Unit { get; }
    bool IsPlayer { get; }

    // Actions
    void Move(List<MapCoordinate> path);
    void Attack(IMBUnit target);
    void ReceiveAttack();
    void Destroy();

    // Finish Turn
    void FinishTurn(int lastTurnCount);
}
