namespace Haruka.MonoGameUtils.UI.Elements;

public interface ITouchable {

    public void OnTouch(int x, int y);
    public bool ShouldDoOriginCheck();
}