using LiteDB.StudioNew.Services;

namespace LiteDB.StudioNew;

public static class Container
{
    public static IConnectionRepository ConnectionRepository { get; } = new ConnectionRepository("");
}