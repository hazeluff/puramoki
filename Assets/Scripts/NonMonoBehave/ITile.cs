using System.Collections.Generic;

public interface ITile {
    bool Traversable { get; }
    int MoveCost(IUnit unit);
    List<ITileEffect> Effects { get; }
}