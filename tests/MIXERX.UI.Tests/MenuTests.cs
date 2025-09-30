using Xunit;
using MIXERX.UI.ViewModels;

namespace MIXERX.UI.Tests
{
    public class MenuTests
    {
        [Fact]
        public void MainWindowViewModel_ShouldHaveMenuCommands()
        {
            var viewModel = new MainWindowViewModel();
            
            Assert.NotNull(viewModel.NewProjectCommand);
            Assert.NotNull(viewModel.OpenProjectCommand);
            Assert.NotNull(viewModel.SaveProjectCommand);
            Assert.NotNull(viewModel.ExitCommand);
        }
    }
}
