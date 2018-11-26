using UnityEngine;
using UnityEngine.U2D;

public class ISOMain : MonoBehaviour
{
    private AudioManager audioManager;
    private FlowManager flowManager;

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
        InitializePixelPerfectCamera(Futile.instance.camera, screenSize);

        audioManager = new AudioManager(this);
        flowManager = new FlowManager(this);
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

    private void InitializePixelPerfectCamera(Camera camera, Vector2 screenSize)
    {
        PixelPerfectCamera pixelPerfect = camera.gameObject.AddComponent<PixelPerfectCamera>();

        pixelPerfect.assetsPPU = 1;
        pixelPerfect.refResolutionX = (int)screenSize.x;
        pixelPerfect.refResolutionY = (int)screenSize.y;
        pixelPerfect.upscaleRT = true;
    }
}