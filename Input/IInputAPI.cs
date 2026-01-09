using Microsoft.Xna.Framework;

namespace Haruka.MonoGameUtils.Input;

public interface IInputAPI {

    public void EarlyUpdate(GameTime time);

    public void LateUpdate(GameTime time);
        
    public void Initialize();

    public Exception GetError();
    public DateTime? GetErrorTime();
    public void ResetError();
}