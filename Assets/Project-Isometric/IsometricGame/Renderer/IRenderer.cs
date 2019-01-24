public interface IRenderer
{
    void OnInitializeSprite(SpriteLeaser spriteLeaser, WorldCamera camera);
    void RenderUpdate(SpriteLeaser spriteLeaser, WorldCamera camera);

    bool GetShownByCamera(SpriteLeaser spriteLeaser, WorldCamera camera);
}