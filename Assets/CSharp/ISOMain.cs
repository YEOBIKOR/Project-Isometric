using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISOMain : MonoBehaviour
{
    private AudioManager audioManager;
    private FlowManager flowManager;

    public static bool doesDebugging = false;

    private void Start()
    {
        FutileParams futileParams = new FutileParams(true, true, false, false);
        futileParams.AddResolutionLevel(270f / Screen.height * Screen.width, 1f, 1f, string.Empty);
        futileParams.backgroundColor = Color.black;

        Futile.instance.Init(futileParams);
        
        //Futile.atlasManager.LoadAtlas("Atlases/rainWorld");
        //Futile.atlasManager.LoadAtlas("Atlases/uiSprites");
        Futile.atlasManager.LoadAtlas("Atlases/isogame");
        Futile.atlasManager.LoadAtlas("Atlases/uiatlas");
        
        Futile.atlasManager.LoadAtlas("Atlases/fontAtlas");
        Futile.atlasManager.LoadFont("font", "font", "Atlases/font", 0f, 0f);
        Futile.atlasManager.LoadFont("DisplayFont", "DisplayFont", "Atlases/DisplayFont", 0f, 0f);

        audioManager = new AudioManager(this);
        flowManager = new FlowManager(this);
    }

    private void Update()
    {
        flowManager.RawUpdate(Time.deltaTime);
    }
}