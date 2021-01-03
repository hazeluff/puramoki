public interface IBaseUnit {
    string Name { get; }
    UnitClass Class { get; }

    // Base Stats
    int BaseHp { get; }
    int HpAt(int level);
    
    int BaseEp { get; }
    int EpAt(int level);
    
    int BaseAtk { get; }
    int AtkAt(int level);
    
    int BaseDef { get; }
    int DefAt(int level);
    
    int BaseAcc { get; }
    int AccAt(int level);
    
    int BaseEva { get; }
    int EvaAt(int level);
    
    int BaseSpd { get; }
    int SpdAt(int level);
    
    int BaseRng { get; }
    int RngAt(int level);

    // Movement
    int Mv { get; }

    // Elemental Resistance (as a %)
    float ElemRes(Element element);
}
