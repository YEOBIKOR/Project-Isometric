using UnityEngine;
using Custom;

public class EntityPart : CosmeticRenderer
{
    public Entity owner { get; private set; }

    public EntityPart(Entity owner, string element) : this(owner, Futile.atlasManager.GetElementWithName(string.Concat("entities/", element)))
    {

    }

    public EntityPart(Entity owner, FAtlasElement element) : base(element)
    {
        this.owner = owner;
    }

    public override void RenderUpdate(SpriteLeaser spriteLeaser, WorldCamera camera)
    {
        base.RenderUpdate(spriteLeaser, camera);
    }
}

public class ZFlipEntityPart : EntityPart
{
    private FAtlasElement _flipedAtlasElement;

    public ZFlipEntityPart(Entity owner, string element, string flipedElement) : base(owner, element)
    {
        _flipedAtlasElement = Futile.atlasManager.GetElementWithName(string.Concat("entities/", flipedElement));
    }

    public override void RenderUpdate(SpriteLeaser spriteLeaser, WorldCamera camera)
    {
        base.RenderUpdate(spriteLeaser, camera);

        spriteLeaser.sprites[0].element = camera.GetFlipZByViewAngle(viewAngle) ? _flipedAtlasElement : element;
    }
}