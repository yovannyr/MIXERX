using CommunityToolkit.Mvvm.Input;
using MIXERX.App.Models;

namespace MIXERX.App.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}