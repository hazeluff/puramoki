public class BasicAttackInstance : IDamageInstance {
    public IStagePermanent Source { get; private set; }
    public int Damage { get; private set; }
    public DamageType Type { get; private set; }

    public BasicAttackInstance(IStagePermanent source, int damage, DamageType type) {
        Source = source;
        Damage = damage;
        Type = type;
    }
}