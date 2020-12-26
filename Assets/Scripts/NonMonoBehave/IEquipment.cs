public interface IEquipment {
    int HP { get; }
    int EP { get; }
    int Atk { get; }
    int Def { get; }
    int Acc { get; }
    int Eva { get; }
    int Spd { get; }
    int Rng { get; }

    int Mv { get; }

    float ElemRes(Element element);
}
