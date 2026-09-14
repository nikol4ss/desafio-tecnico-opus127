namespace Opus127.Dengue.UnitTests.TestDoubles;

internal sealed class FixedTimeProvider(DateTimeOffset value) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => value;
}
