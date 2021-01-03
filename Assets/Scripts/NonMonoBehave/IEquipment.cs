public interface IEquipment {
    int Hp { get; }
    int Ep { get; }
    int Atk { get; }
    int Def { get; }
    int Acc { get; }
    int Eva { get; }
    int Spd { get; }
    int Rng { get; }

    int Mv { get; }

    float ElemRes(Element element);
}
