using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ISOMain : MonoBehaviour
{
    private AudioManager audioManager;
    private FlowManager flowManager;

    private static Dictionary<string, FShader> _shaders;

    public static bool doesDebugging = false;

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

        InitializePixelPerfectCamera(Futile.instance.camera, screenSize);

        audioManager = new AudioManager(this);
        flowManager = new FlowManager(this);
        flowManager.SwitchLoopFlow(new ISOGame());
    }

    private void Update()
    {
        flowManager.RawUpdate(Time.deltaTime);
    }

    private void LoadAtlases()
    {
        //Futile.atlasManager.LoadAtlas("Atlases/rainWorld");
        //Futile.atlasManager.LoadAtlas("Atlases/uiSprites");
        Futile.atlasManager.LoadAtlas("Atlases/isogame");
        Futile.atlasManager.LoadAtlas("Atlases/uiatlas");

        Futile.atlasManager.LoadAtlas("Atlases/fontAtlas");
        Futile.atlasManager.LoadFont("font", "font", "Atlases/font", 0f, 0f);
        Futile.atlasManager.LoadFont("DisplayFont", "DisplayFont", "Atlases/DisplayFont", 0f, 0f);
    }

    private void LoadShaders()
    {
        _shaders = new Dictionary<string, FShader>();

        FShader shader = FShader.CreateShader("WorldObject", Resources.Load<Shader>("Shaders/WorldObject"));
        _shaders.Add(shader.name, shader);
    }

    private void LoadTextures()
    {
        Shader.SetGlobalTexture("_NoiseTex", Resources.Load<Texture>("Textures/cloud"));
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