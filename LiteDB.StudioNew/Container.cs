using LiteDB.StudioNew.Services;

namespace LiteDB.StudioNew;

public static class Container
{
    static Container()
    {
        ConnectionRepository = new ConnectionRepository("connections.json");
        NavigationService = new NavigationService();
    }

    public static IConnectionRepository ConnectionRepository { get; }
    public static INavigationService NavigationService { get; }
}