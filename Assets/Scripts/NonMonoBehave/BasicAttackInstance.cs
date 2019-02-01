public class BasicAttackInstance : IAttackInstance
{
    private readonly IStageUnit _source;
    private readonly int _damage;

    public IStageUnit Source { get { return Source; } }
    public int Damage { get { return _damage; } }

    public BasicAttackInstance(IStageUnit source, int damage) {
        this._source = source;
        this._damage = damage;
    }
}
