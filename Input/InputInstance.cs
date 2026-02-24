using Haruka.MonoGameUtils.Input.Api;
using Microsoft.Extensions.Logging;

namespace Haruka.MonoGameUtils.Input;

public class InputInstance {

    public IInputAPI API { get; }
    public InputErrorInformation Error { get; private set; }

    public InputInstance(IInputAPI api) {
        API = api;
    }

    public bool HasError() {
        return Error != null;
    }

    internal void SetError(string message, Exception exception = null) {
        InputManager.InputLog.LogError("Error in input instance " + API + ": " + message, exception);
        Error = new InputErrorInformation(message, exception);
    }

    public void Reset() {
        API.Stop();
        API.Start();
    }
}