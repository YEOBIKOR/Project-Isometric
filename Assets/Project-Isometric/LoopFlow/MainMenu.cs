using UnityEngine;
using Custom;

using System.IO;

namespace Isometric.Interface
{
    public class MainMenu : MenuFlow
    {
        private OptionsMenu _optionsMenu;

        private FSprite _background;
        private FSprite[] _titleSprites;
        private GeneralButton[] _buttons;
        private GeneralButton _visitDevLog;
        private WorldSelect _worldSelect;

        public MainMenu() : base()
        {
            _optionsMenu = new OptionsMenu(this);

            _background = new FSprite("mainbackground");
            _background.scale = 1.2f * screenHeight / 270f;
            container.AddChild(_background);

            _titleSprites = new FSprite[3];
            _titleSprites[0] = new FSprite("titlei");
            _titleSprites[1] = new FSprite("titles");
            _titleSprites[2] = new FSprite("titleo");

            for (int i = _titleSprites.Length - 1; !(i < 0); i--)
            {
                _titleSprites[i].x = screenWidth * 0.5f - ((3 - i) * 40f);
                _titleSprites[i].y = -screenHeight;
                _titleSprites[i].scale = 2f;

                container.AddChild(_titleSprites[i]);
            }

            _buttons = new GeneralButton[3];
            _buttons[0] = new GeneralButton(this, "Start", OnWorldSelect);
            _buttons[1] = new GeneralButton(this, "Options", OpenOptions);
            _buttons[2] = new GeneralButton(this, "Quit", OnApplicationQuit);

            _buttons[0].position = new Vector2(0f, 88f - screenHeight * 0.5f);
            _buttons[0].size = new Vector2(48f, 48f);
            _buttons[1].position = new Vector2(0f, 48f - screenHeight * 0.5f);
            _buttons[1].size = new Vector2(48f, 16f);
            _buttons[2].position = new Vector2(0f, 24f - screenHeight * 0.5f);
            _buttons[2].size = new Vector2(48f, 16f);

            for (int index = 0; index < _buttons.Length; index++)
                AddElement(_buttons[index]);

            _worldSelect = new WorldSelect(this);

            _visitDevLog = new GeneralButton(this, "Wanna See Devlog?", OnVisitDevLog);
            _visitDevLog.position = new Vector2(0f, screenHeight * -0.5f + 24f);
            _visitDevLog.size = new Vector2(96f, 16f);
            AddElement(_visitDevLog);
        }

        public override void Update(float deltaTime)
        {
            Vector2 backgroundTargetPosition = -mousePosition * 0.03f +
                new Vector2(Mathf.PerlinNoise(time, 0f) - 0.5f, Mathf.PerlinNoise(0f, time) - 0.5f) * 5f;

            _background.SetPosition(Vector2.Lerp(_background.GetPosition(), backgroundTargetPosition, deltaTime * 3f));

            _background.alpha = Mathf.Lerp(0f, 0.5f, (time - 1f) * 0.5f);
            for (int i = 0; i < _titleSprites.Length; i++)
                _titleSprites[i].y = Mathf.Lerp(-screenHeight, screenHeight * 0.5f - 60f, CustomMath.Curve(time - (i * 0.2f), -3f)) + Mathf.Sin(time * 3f - i) * 4f;
            for (int i = 0; i < _buttons.Length; i++)
                _buttons[i].position = new Vector2(-screenWidth * 0.5f + Mathf.Lerp(-24f, 40f, CustomMath.Curve(time - 1f - (i * 0.2f), -3f)), _buttons[i].position.y);
            _visitDevLog.position = new Vector2(screenWidth * 0.5f + Mathf.Lerp(48f, -64f, CustomMath.Curve(time - 3f, -3f)), _visitDevLog.position.y);

            base.Update(deltaTime);
        }

        public override bool OnExecuteEscape()
        {
            OnApplicationQuit();

            return false;
        }

        public void OnWorldSelect()
        {
            if (_worldSelect.activated)
                RemoveElement(_worldSelect);
            else
                AddElement(_worldSelect);
        }

        public void OnGameStart(string worldFile)
        {
            IsometricGame game = new IsometricGame(worldFile);

            loopFlowManager.RequestSwitchLoopFlow(game);
        }

        public void OpenOptions()
        {
            AddSubLoopFlow(_optionsMenu);
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