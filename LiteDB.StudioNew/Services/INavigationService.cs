using System;

namespace LiteDB.StudioNew.Services;

public interface INavigationService
{
    void NavigateToConnectionsListViewModel();
    void NavigateToAddConnectionViewModel();
    void NavigateToEditConnectionViewModel(Guid connectionGuid);
}