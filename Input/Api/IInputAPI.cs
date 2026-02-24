using Microsoft.Xna.Framework;

namespace Haruka.MonoGameUtils.Input.Api;

public interface IInputAPI {

    public void EarlyUpdate(GameTime time);

    public void LateUpdate(GameTime time);
        
    public void Initialize();

    public void Start();

    public void Stop();
}