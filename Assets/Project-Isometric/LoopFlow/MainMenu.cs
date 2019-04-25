using UnityEngine;
using Custom;

using System.IO;

namespace Isometric.Interface
{
    public class MainMenu : MenuFlow
    {
        private OptionsMenu optionsMenu;

        private FSprite background;
        private FSprite[] titleSprites;
        private GeneralButton[] buttons;
        private GeneralButton visitDevLog;
        private WorldSelect worldSelect;

        public MainMenu() : base()
        {
            optionsMenu = new OptionsMenu(this);

            background = new FSprite("mainbackground");
            background.scale = 1.2f * screenHeight / 270f;
            container.AddChild(background);

            titleSprites = new FSprite[3];
            titleSprites[0] = new FSprite("titlei");
            titleSprites[1] = new FSprite("titles");
            titleSprites[2] = new FSprite("titleo");

            for (int i = titleSprites.Length - 1; !(i < 0); i--)
            {
                titleSprites[i].x = screenWidth * 0.5f - ((3 - i) * 40f);
                titleSprites[i].y = -screenHeight;
                titleSprites[i].scale = 2f;

                container.AddChild(titleSprites[i]);
            }

            buttons = new GeneralButton[3];
            buttons[0] = new GeneralButton(this, "Start", OnWorldSelect);
            buttons[1] = new GeneralButton(this, "Options", OpenOptions);
            buttons[2] = new GeneralButton(this, "Quit", OnApplicationQuit);

            buttons[0].position = new Vector2(0f, 88f - screenHeight * 0.5f);
            buttons[0].size = new Vector2(48f, 48f);
            buttons[1].position = new Vector2(0f, 48f - screenHeight * 0.5f);
            buttons[1].size = new Vector2(48f, 16f);
            buttons[2].position = new Vector2(0f, 24f - screenHeight * 0.5f);
            buttons[2].size = new Vector2(48f, 16f);

            for (int index = 0; index < buttons.Length; index++)
                AddElement(buttons[index]);

            worldSelect = new WorldSelect(this);
            worldSelect.visible = false;
            AddElement(worldSelect);

            visitDevLog = new GeneralButton(this, "Wanna See Devlog?", OnVisitDevLog);
            visitDevLog.position = new Vector2(0f, screenHeight * -0.5f + 24f);
            visitDevLog.size = new Vector2(96f, 16f);
            AddElement(visitDevLog);
        }

        public override void Update(float deltaTime)
        {
            Vector2 backgroundTargetPosition = -mousePosition * 0.03f +
                new Vector2(Mathf.PerlinNoise(time, 0f) - 0.5f, Mathf.PerlinNoise(0f, time) - 0.5f) * 5f;

            background.SetPosition(Vector2.Lerp(background.GetPosition(), backgroundTargetPosition, deltaTime * 3f));

            background.alpha = Mathf.Lerp(0f, 0.5f, (time - 1f) * 0.5f);
            for (int i = 0; i < titleSprites.Length; i++)
                titleSprites[i].y = Mathf.Lerp(-screenHeight, screenHeight * 0.5f - 60f, CustomMath.Curve(time - (i * 0.2f), -3f)) + Mathf.Sin(time * 3f - i) * 4f;
            for (int i = 0; i < buttons.Length; i++)
                buttons[i].position = new Vector2(-screenWidth * 0.5f + Mathf.Lerp(-24f, 40f, CustomMath.Curve(time - 1f - (i * 0.2f), -3f)), buttons[i].position.y);
            visitDevLog.position = new Vector2(screenWidth * 0.5f + Mathf.Lerp(48f, -64f, CustomMath.Curve(time - 3f, -3f)), visitDevLog.position.y);

            base.Update(deltaTime);
        }

        public override bool OnExecuteEscape()
        {
            OnApplicationQuit();

            return false;
        }

        public void OnWorldSelect()
        {
            worldSelect.visible = !worldSelect.visible;
        }

        public void OnGameStart(string worldFile)
        {
            IsometricGame game = new IsometricGame(worldFile);

            loopFlowManager.RequestSwitchLoopFlow(game);
        }

        public void OpenOptions()
        {
            AddSubLoopFlow(optionsMenu);
        }

        public void OnApplicationQuit()
        {
            Application.Quit();
        }

        public void OnVisitDevLog()
        {
            Application.OpenURL("https://twitter.com/i/moments/987507190041739264");
        }
    }

    public class WorldSelect : InterfaceObject
    {
        private string[] _worldNames;
        private string[] _worldPaths;

        private GeneralButton[] _worldSelects;

        public const int WorldNumber = 3;

        public WorldSelect(MainMenu menu) : base(menu)
        {
            _worldNames = new string[WorldNumber];
            _worldPaths = new string[WorldNumber];

            _worldSelects = new GeneralButton[WorldNumber];
            
            for (int index = 0; index < WorldNumber; index++)
            {
                string worldName = "World_" + index;
                string worldPath = "SaveData/" + worldName + ".dat";

                _worldNames[index] = worldName;
                _worldPaths[index] = worldPath;

                _worldSelects[index] = new GeneralButton(menu, "World_#", delegate { menu.OnGameStart(worldPath); } );
                _worldSelects[index].position = new Vector2(-MenuFlow.screenWidth * 0.5f + 40f + index * 64f, 144f - MenuFlow.screenHeight * 0.5f);
                _worldSelects[index].size = new Vector2(48f, 48f);

                AddElement(_worldSelects[index]);
            }
        }

        public override void OnActivate()
        {
            base.OnActivate();

            for (int index = 0; index < WorldNumber; index++)
            {
                string displayString = File.Exists(_worldPaths[index]) ? _worldNames[index] : "Create New";
                _worldSelects[index].text = displayString;
            }
        }
    }
}