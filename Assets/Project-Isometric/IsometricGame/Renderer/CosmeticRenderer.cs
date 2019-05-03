using System;
using UnityEngine;
using Custom;

public class CosmeticRenderer : IRenderer
{
    private World _world;
    public World world
    {
        get { return _world; }
    }

    private FAtlasElement _element;
    public virtual FAtlasElement element
    {
        get { return _element; }
        set { _element = value; }
    }

    private Vector3 _worldPosition;
    public Vector3 worldPosition
    {
        get { return _worldPosition; }
        set { _worldPosition = value; }
    }

    private Vector2 _positionOffset;
    public Vector2 positionOffset
    {
        get { return _positionOffset; }
        set { _positionOffset = value; }
    }

    private float _rotation;
    public float rotation
    {
        get { return _rotation; }
        set { _rotation = value; }
    }

    private Vector2 _scale;
    public Vector2 scale
    {
        get { return _scale; }
        set { _scale = value; }
    }

    private bool _doesFlip;
    public bool doesFlip
    {
        get { return _doesFlip; }
        set { _doesFlip = value; }
    }

    private float _viewAngle;
    public float viewAngle
    {
        get { return _viewAngle; }
        set { _viewAngle = CustomMath.ToAngle(value); }
    }

    private float _sortZOffset;
    public float sortZOffset
    {
        get { return _sortZOffset; }
        set { _sortZOffset = value; }
    }

    private Color _color;
    public Color color
    {
        get { return _color; }
        set { _color = value; }
    }

    private float _alpha;
    public float alpha
    {
        get
        { return _color.a; }
        set
        { _color.a = value; }
    }

    private FShader _shader;
    public FShader shader
    {
        get
        { return _shader; }
        set
        { _shader = value; }
    }

    public CosmeticRenderer(FAtlasElement element)
    {
        _element = element;
        _worldPosition = Vector3.zero;
        _positionOffset = Vector2.zero;
        _rotation = 0f;
        _doesFlip = true;
        _viewAngle = 0f;
        _sortZOffset = 0f;
        _color = Color.white;
        _scale = Vector2.one;
        _shader = FShader.defaultShader;
    }

    public virtual void OnShow(World world)
    {
        _world = world;
        _world.worldCamera.AddRenderer(this);
    }

    public virtual void Erase()
    {
        _world = null;
    }

    public virtual void Update(float deltaTime)
    {

    }

    public virtual void OnInitializeSprite(SpriteLeaser spriteLeaser, WorldCamera camera)
    {
        FSprite sprite = new FSprite(element == null ? Futile.whiteElement : element);
        sprite.shader = shader;

        spriteLeaser.sprites.Add(sprite);
    }

    public virtual void RenderUpdate(SpriteLeaser spriteLeaser, WorldCamera camera)
    {
        if (world != null)
        {
            FSprite sprite = spriteLeaser.sprites[0];

            if (element != null)
            {
                sprite.isVisible = true;
                sprite.element = element;
            }
            else
                sprite.isVisible = false;
            sprite.SetPosition(camera.GetScreenPosition(worldPosition) + _positionOffset);
            sprite.rotation = _rotation;
            sprite.scaleX = scale.x;
            sprite.scaleY = scale.y;
            if (doesFlip)
                sprite.scaleX *= (camera.GetFlipXByViewAngle(viewAngle) ? -1f : 1f);
            sprite.sortZ = camera.GetSortZ(worldPosition) + sortZOffset;
            sprite.color = color; // new Color(worldPosition.x, worldPosition.y, worldPosition.z);
        }
        else
            spriteLeaser.Erase();
    }

    public virtual bool GetShownByCamera(SpriteLeaser spriteLeaser, WorldCamera camera)
    {
        return spriteLeaser.InScreenRect(spriteLeaser.sprites[0]);
    }
}