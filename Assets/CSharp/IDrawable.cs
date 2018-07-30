public interface IDrawable
{
    void OnInitializeSprite(SpriteLeaser spriteLeaser, WorldCamera camera);
    void GraphicUpdate(SpriteLeaser spriteLeaser, WorldCamera camera);

    bool GetShownByCamera(SpriteLeaser spriteLeaser, WorldCamera camera);
}