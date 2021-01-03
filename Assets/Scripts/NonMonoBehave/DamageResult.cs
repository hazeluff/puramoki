public class DamageResult {
    public int DamageReduced { get; private set; }
    public int DamageDealt { get; private set; }
    public bool IsKill { get; private set; }

    public DamageResult(int damageReduced, int damageDealt, bool isKill) {
        DamageReduced = damageReduced;
        DamageDealt = damageDealt;
        IsKill = isKill;
    }
}
