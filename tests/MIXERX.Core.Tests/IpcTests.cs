using Xunit;
using MIXERX.Core.IPC;

namespace MIXERX.Core.Tests;

public class IpcTests
{
    [Fact]
    public void IpcClient_Connect_Succeeds()
    {
        var client = new IpcClient();
        var result = client.Connect("test-pipe");
        
        Assert.True(result);
    }

    [Fact]
    public void IpcClient_SendCommand_ReceivesResponse()
    {
        var client = new IpcClient();
        client.Connect("test-pipe");
        
        var command = new IpcCommand { Type = "Play", DeckId = 1 };
        var response = client.SendCommand(command);
        
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [Fact]
    public void IpcServer_HandleCommand_ExecutesCorrectly()
    {
        var server = new IpcServer();
        var commandExecuted = false;
        
        server.OnCommand += (cmd) => commandExecuted = true;
        server.Start("test-pipe");
        
        var command = new IpcCommand { Type = "Play", DeckId = 1 };
        server.HandleCommand(command);
        
        Assert.True(commandExecuted);
    }
}
