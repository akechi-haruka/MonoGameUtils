namespace Haruka.MonoGameUtils.Input;

public class InputException : Exception {
    public InputException(string message) : base(message) {
    }

    public InputException(string message, Exception innerException) : base(message, innerException) {
    }
}