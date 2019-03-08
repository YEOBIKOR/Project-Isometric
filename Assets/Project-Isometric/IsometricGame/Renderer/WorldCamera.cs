using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Custom;

public enum CameraViewDirection : byte
{
    NE,
    NW,
    SW,
    SE
}

public class WorldCamera
{
    private World _world;

    private WorldMicrophone _worldMicrophone;
    public WorldMicrophone worldMicrophone
    {
        get
        { return _worldMicrophone; }
    }

    private Queue<IRenderer> _renderersQueue;
    private List<SpriteLeaser> _spriteLeasers;
    
    public FContainer worldContainer { get; private set; }
    private FContainer _debugContainer;

    private IPositionable _cameraTarget;
    private Vector3 _targetPosition;

    private CameraViewDirection _viewDirection;
    public CameraViewDirection viewDirection
    {
        get { return _viewDirection; }
    }
    private float _viewAngle;
    public float viewAngle
    {
        get { return _viewAngle; }
        set { _viewAngle = CustomMath.ToAngle(value); }
    }
    private float _lastViewAngle;
    private float _turn;

    private float _shake;

    public const int PixelsPerTile = 24;
    public const int PixelsPerHalfTile = PixelsPerTile / 2;
    public const int PixelsPerQuaterTile = PixelsPerTile / 4;

    private WorldCameraUI worldCameraUI;

    public WorldCamera(World world)
    {
        this._world = world;

        _worldMicrophone = new WorldMicrophone();

        _renderersQueue = new Queue<IRenderer>();
        _spriteLeasers = new List<SpriteLeaser>();

        worldContainer = new FContainer();
        worldContainer.shouldSortByZ = true;
        Futile.stage.AddChild(worldContainer);

        _debugContainer = new FContainer();
        Futile.stage.AddChild(_debugContainer);

        _targetPosition = Vector3.zero;

        _viewDirection = CameraViewDirection.NE;
        viewAngle = GetAngle(_viewDirection);
        _lastViewAngle = viewAngle;
        _turn = 0f;

        _shake = 0f;

        worldCameraUI = new WorldCameraUI(this);
    }

    public void GraphicUpdate(float deltaTime)
    {
        if (Input.GetKeyDown(KeyCode.Q))
            TurnViewDirection(false);
        else if (Input.GetKeyDown(KeyCode.E))
            TurnViewDirection(true);

        viewAngle = Mathf.LerpAngle(_lastViewAngle, GetAngle(_viewDirection), CustomMath.Curve(1f - _turn, -3f));
        _turn -= deltaTime;

        _shake = Mathf.Lerp(_shake, 0f, deltaTime * 10f);

        if (_cameraTarget != null)
            _targetPosition = Vector3.Lerp(_targetPosition, _cameraTarget.worldPosition, deltaTime * 10f);
        Vector2 cameraPosition = -GetScreenPosition(_targetPosition) + Random.insideUnitCircle * _shake;
        worldContainer.SetPosition(cameraPosition);
        _debugContainer.SetPosition(cameraPosition);

        while (_renderersQueue.Count > 0)
        {
            IRenderer renderer = _renderersQueue.Peek();

            if (renderer != null)
            {
                SpriteLeaser spriteLeaser = new SpriteLeaser(this, renderer);
                renderer.OnInitializeSprite(spriteLeaser, this);

                _spriteLeasers.Add(spriteLeaser);
            }
            _renderersQueue.Dequeue();
        }

        for (int index = 0; index < _spriteLeasers.Count; index++)
        {
            _spriteLeasers[index].GraphicUpdate();

            if (_spriteLeasers[index].removeInNextFrame)
            {
                _spriteLeasers[index].RemoveFromContainer();
                _spriteLeasers.RemoveAt(index);
                index--;
            }
        }

        Color skyColor = new Color(0.35f, 0.85f, 0.97f); // Color.Lerp(new Color(0.35f, 0.85f, 0.97f), Color.black, Mathf.Sin(Time.time * 0.1f) * 0.5f + 0.5f);

        Shader.SetGlobalVector("_CameraPosition", _targetPosition);
        Shader.SetGlobalVector("_SkyColor", skyColor);

        _worldMicrophone.worldPosition = _targetPosition;
        _worldMicrophone.viewAngle = _viewAngle;

        _worldMicrophone.Update(deltaTime);

        worldCameraUI.Update(deltaTime);
    }

    public RayTrace GetRayAtScreenPosition(Vector2 position)
    {
        Vector3 startRaytracePosition = CustomMath.HorizontalRotate(new Vector3(
            (position.x + position.y * 2f) / PixelsPerTile - Chunk.Height,
            Chunk.Height,
            (position.y * 2f - position.x) / PixelsPerTile - Chunk.Height), _viewAngle - 45f);
        return _world.RayTraceTile(startRaytracePosition, CustomMath.HorizontalRotate(new Vector3(1f, -1f, 1f), _viewAngle - 45f), 0f);
    }

    public void SetCameraTarget(IPositionable target)
    {
        _cameraTarget = target;
    }

    public void TurnViewDirection(bool right)
    {
        if (right)
            _viewDirection = (_viewDirection == CameraViewDirection.NE) ? CameraViewDirection.SE : _viewDirection - 1;
        else
            _viewDirection = (_viewDirection == CameraViewDirection.SE) ? CameraViewDirection.NE : _viewDirection + 1;

        _lastViewAngle = viewAngle;
        _turn = 1f;
    }

    public Vector2 GetScreenPosition(Vector3 position)
    {
        if (!turning)
        {
            switch (_viewDirection)
            {
                case CameraViewDirection.NE:
                    return new Vector2(PixelsPerHalfTile * (position.x - position.z), PixelsPerQuaterTile * (position.x + position.z) + PixelsPerHalfTile * position.y);

                case CameraViewDirection.NW:
                    return new Vector2(PixelsPerHalfTile * (position.x + position.z), PixelsPerQuaterTile * -(position.x - position.z) + PixelsPerHalfTile * position.y);

                case CameraViewDirection.SW:
                    return new Vector2(PixelsPerHalfTile * -(position.x - position.z), PixelsPerQuaterTile * -(position.x + position.z) + PixelsPerHalfTile * position.y);

                case CameraViewDirection.SE:
                    return new Vector2(PixelsPerHalfTile * -(position.x + position.z), PixelsPerQuaterTile * (position.x - position.z) + PixelsPerHalfTile * position.y);

                default:
                    return Vector2.zero;
            }
        }
        else
            return new Vector2(
                PixelsPerHalfTile * (cosViewAngle * -position.z + sinViewAngle * position.x),
                PixelsPerQuaterTile * (sinViewAngle * position.z + cosViewAngle * position.x) + PixelsPerHalfTile * position.y);
    }

    public float GetSortZ(Vector3 position)
    {
        if (!turning)
        {
            switch (_viewDirection)
            {
                case CameraViewDirection.NE:
                    return position.y - (position.x + position.z);

                case CameraViewDirection.NW:
                    return position.y + (position.x - position.z);

                case CameraViewDirection.SW:
                    return position.y + (position.x + position.z);

                case CameraViewDirection.SE:
                    return position.y - (position.x - position.z);

                default:
                    return 0f;
            }
        }
        else
            return position.y - (cosViewAngle * position.x + sinViewAngle * position.z);
    }

    public bool GetFlipXByViewAngle(float viewAngle)
    {
        return CustomMath.ToAngle(viewAngle - this.viewAngle) < 0f;
    }

    public bool GetFlipZByViewAngle(float viewAngle)
    {
        return Mathf.Abs(CustomMath.ToAngle(viewAngle - this.viewAngle)) < 90f;
    }

    public Color GetTint(Vector3 position)
    {
        return Color.black;
    }

    public static float GetAngle(CameraViewDirection viewDirection)
    {
        switch (viewDirection)
        {
            case CameraViewDirection.NE:
                return 45f;

            case CameraViewDirection.NW:
                return 135f;

            case CameraViewDirection.SW:
                return -135f;

            case CameraViewDirection.SE:
                return -45f;

            default:
                return 0f;
        }
    }

    public Vector2 ScreenToWorldDirection(Vector2 direction)
    {
        return Quaternion.AngleAxis(viewAngle - 90f, Vector3.forward) * direction.normalized;
    }

    public void ShakeCamera(float amount)
    {
        _shake += amount;
    }

    public void AddRenderer(IRenderer renderer)
    {
        SpriteLeaser spriteLeaser = new SpriteLeaser(this, renderer);
        renderer.OnInitializeSprite(spriteLeaser, this);

        _spriteLeasers.Add(spriteLeaser);
    }

    private void InitializeRenderer()
    {
        do
        {
            IRenderer renderer = _renderersQueue.Peek();

            if (renderer != null)
            {
                SpriteLeaser spriteLeaser = new SpriteLeaser(this, renderer);
                renderer.OnInitializeSprite(spriteLeaser, this);

                _spriteLeasers.Add(spriteLeaser);
            }
            _renderersQueue.Dequeue();

        } while (_renderersQueue.Count > 0);
    }

    public void AddDebugRenderer(FNode node)
    {
        _debugContainer.AddChild(node);
    }

    public void CleanUp()
    {
        worldContainer.RemoveFromContainer();
        _debugContainer.RemoveFromContainer();

        worldCameraUI.CleanUp();
    }

    public bool turning
    {
        get { return _turn > -0f; }
    }

    private float sinViewAngle
    {
        get { return Mathf.Sin(viewAngle * Mathf.Deg2Rad) * 1.414213f; }
    }

    private float cosViewAngle
    {
        get { return Mathf.Cos(viewAngle * Mathf.Deg2Rad) * 1.414213f; }
    }

    public class WorldCameraUI
    {
        private WorldCamera worldCamera;

        private FContainer axisContainer;
        private FSprite[] axisSprites;
        private float axisShowFactor;

        private FLabel debugLabel;
        private FLabel tutorialLabel;
        private const string tutorialText =
            "W A S D : Move the character\nSpace : Jump the character\nQ, E : Move the camera\nEsc : Exit the game";

        public WorldCameraUI(WorldCamera worldCamera)
        {
            this.worldCamera = worldCamera;

            axisContainer = new FContainer();
            axisContainer.SetPosition(Futile.screen.halfWidth - 36f, Futile.screen.halfHeight - 20f);
            axisContainer.scaleY = 0.5f;
            Futile.stage.AddChild(axisContainer);

            axisSprites = new FSprite[4];
            for (int index = 0; index < axisSprites.Length; index++)
            {
                axisSprites[index] = new FSprite("Futile_White");
                axisSprites[index].color = index == 3 ? Color.red : Color.black;
                axisSprites[index].anchorY = 0f;
                axisSprites[index].scaleY = 0f;
                axisSprites[index].scaleX = 0.15f;
                axisContainer.AddChild(axisSprites[index]);
            }

            axisShowFactor = 0f;

            if (IsometricMain.doesDebugging)
            {
                debugLabel = new FLabel("font", "DEBUG");
                debugLabel.color = Color.red;

                Futile.stage.AddChild(debugLabel);
            }

            tutorialLabel = new FLabel("font", tutorialText);
            tutorialLabel.alignment = FLabelAlignment.Left;
            tutorialLabel.SetPosition(-Futile.screen.halfWidth + 4f, -Futile.screen.halfHeight + 32f);
            tutorialLabel.scale = 1f;
            // Futile.stage.AddChild(tutorialLabel);
        }

        public void Update(float deltaTime)
        {
            axisShowFactor = Mathf.Clamp01(axisShowFactor + (worldCamera.turning ? deltaTime : -deltaTime) * 2f);

            for (int index = 0; index < axisSprites.Length; index++)
            {
                axisSprites[index].scaleY = Mathf.Lerp(0f, 2f, CustomMath.Curve(axisShowFactor, -2f));

                if (axisShowFactor > 0f)
                {
                    float angle = (worldCamera.viewAngle + (90f * index));
                    axisSprites[index].rotation = angle;
                }
            }

            if (IsometricMain.doesDebugging)
                debugLabel.text = string.Concat("view angle : ", worldCamera.viewAngle);
        }

        public void CleanUp()
        {
            axisContainer.RemoveFromContainer();
            tutorialLabel.RemoveFromContainer();
        }
    }
}

public class SpriteLeaser
{
    private WorldCamera camera;

    public IRenderer owner { get; private set; }
    public List<FSprite> sprites { get; private set; }
    public int spriteIndex { get; set; }
    public bool removeInNextFrame { get; set; }

    private bool showing;

    public SpriteLeaser(WorldCamera camera, IRenderer owner)
    {
        this.camera = camera;
        this.owner = owner;

        sprites = new List<FSprite>();
        spriteIndex = 0;

        showing = false;
    }

    public void GraphicUpdate()
    {
        owner.RenderUpdate(this, camera);

        if (showing && !owner.GetShownByCamera(this, camera))
            RemoveFromContainer();
        else if (!showing && owner.GetShownByCamera(this, camera))
            AddToContainer();
    }

    public void AddToContainer()
    {
        for (int index = 0; index < sprites.Count; index++)
            camera.worldContainer.AddChild(sprites[index]);

        showing = true;
    }

    public void RemoveFromContainer()
    {
        for (int index = 0; index < sprites.Count; index++)
            sprites[index].RemoveFromContainer();

        showing = false;
    }

    public void Erase()
    {
        removeInNextFrame = true;
    }

    public bool InScreenRect(FSprite sprite)
    {
        Vector2 position = sprite.GetPosition() + camera.worldContainer.GetPosition();
        Vector2 halfSize = new Vector2(sprite.width * sprite.scaleX, sprite.height * sprite.scaleY) * 0.5f;

        return (position.x + halfSize.x > -Futile.screen.halfWidth && position.x - halfSize.x < Futile.screen.halfWidth) &&
        (position.y + halfSize.y > -Futile.screen.halfHeight && position.y - halfSize.y < Futile.screen.halfHeight);
    }
}