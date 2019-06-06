using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class IsometricMain : MonoBehaviour
{
    private InputManager _inputManager;
    private AudioEngine _audioEngine;
    private IsometricLoopFlowManager _flowManager;

    private static Camera _camera;
    public static new Camera camera
    {
        get { return _camera; }
    }

    private static Dictionary<string, FShader> _shaders;

    [SerializeField]
    private bool _pixelPerfect;

    private void Start()
    {
        Vector2 screenSize;
        screenSize.y = 270f;
        screenSize.x = screenSize.y / Screen.height * Screen.width;

        FutileParams futileParams = new FutileParams(true, true, false, false);
        futileParams.AddResolutionLevel(screenSize.x, 1f, 1f, string.Empty);
        futileParams.backgroundColor = Color.black;

        Futile.instance.Init(futileParams);

        LoadAtlases();
        LoadShaders();
        LoadTextures();

        _camera = Futile.instance.camera;
        if (_pixelPerfect)
            InitializePixelPerfectCamera(_camera, screenSize);

        _inputManager = new InputManager();
        _audioEngine = new AudioEngine();
        _flowManager = new IsometricLoopFlowManager();

        _flowManager.SwitchLoopFlow(new Isometric.Interface.IntroRoll());

        string path = "SaveData/";

        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);
    }

    private void Update()
    {
        float timeScale =
                Input.GetKey(KeyCode.Keypad1) ? 0f :
                Input.GetKey(KeyCode.Keypad2) ? 0.3f :
                Input.GetKey(KeyCode.Keypad3) ? 3f :
                1f;

        if (Input.GetKeyDown(KeyCode.Home))
            _flowManager.RequestSwitchLoopFlow(new IsometricGame("SaveData/World_0.dat"));

        float deltaTime = Time.deltaTime * timeScale;

        _flowManager.RawUpdate(deltaTime);
    }

    private void OnDestroy()
    {
        _flowManager.OnTerminate();
    }

    private void LoadAtlases()
    {
        Futile.atlasManager.LoadAtlas("Atlases/isogame");
        Futile.atlasManager.LoadAtlas("Atlases/uiatlas");

        Futile.atlasManager.LoadAtlas("Atlases/fontatlas");
        Futile.atlasManager.LoadFont("font", "font", "Atlases/font", 0f, 0f);
    }

    private void LoadShaders()
    {
        _shaders = new Dictionary<string, FShader>();

        FShader[] array = new FShader[]
        {
            FShader.CreateShader("WorldObject", Resources.Load<Shader>("Shaders/WorldObject")),
            FShader.CreateShader("DroppedItem", Resources.Load<Shader>("Shaders/DroppedItem"))
        };

        foreach (var shader in array)
            _shaders.Add(shader.name, shader);
    }

    private void LoadTextures()
    {
        Shader.SetGlobalTexture("_NoiseTex", Resources.Load<Texture>("Textures/noise"));
    }

    private void InitializePixelPerfectCamera(Camera camera, Vector2 screenSize)
    {
        PixelPerfectCamera pixelPerfect = camera.gameObject.AddComponent<PixelPerfectCamera>();

        pixelPerfect.assetsPPU = 1;
        pixelPerfect.refResolutionX = (int)screenSize.x;
        pixelPerfect.refResolutionY = (int)screenSize.y;
        pixelPerfect.upscaleRT = true;
    }

    public static FShader GetShader(string shaderName)
    {
        return _shaders[shaderName];
    }
}