namespace Haruka.MonoGameUtils.Input;

public class InputErrorInformation {

    public String Message { get; }
    public Exception Exception { get; }
    public DateTime DateTime { get; }

    public InputErrorInformation(string message, Exception exception) {
        Message = message;
        Exception = exception;
        DateTime = DateTime.Now;
    }
}