using System;
using System.Threading.Tasks;

namespace LiteDB.StudioNew.Services;

public interface INavigationService
{
    Task<object> NavigateToConnectionsListViewModel(bool isSelectionMode);
    void NavigateToAddConnectionViewModel();
    void NavigateToEditConnectionViewModel(Guid connectionGuid);
    void Close();
}