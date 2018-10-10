using UnityEngine;
using Custom;

namespace Isometric.UI
{
    public class MainMenu : Menu
    {
        private OptionsMenu optionsMenu;

        private FSprite backGround;
        private FSprite[] titleSprites;
        private GeneralButton[] buttons;
        private GeneralButton visitDevLog;

        public MainMenu() : base()
        {
            optionsMenu = new OptionsMenu(this);

            backGround = new FSprite("mainbackground");
            backGround.scale = 1.2f * screenHeight / 270f;
            container.AddChild(backGround);

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
            buttons[0] = new GeneralButton(this, "Start", OnGameStart);
            buttons[1] = new GeneralButton(this, "Options", OpenOptions);
            buttons[2] = new GeneralButton(this, "Quit", Application.Quit);

            buttons[0].position = new Vector2(0f, 88f - screenHeight * 0.5f);
            buttons[0].size = new Vector2(48f, 48f);
            buttons[1].position = new Vector2(0f, 48f - screenHeight * 0.5f);
            buttons[1].size = new Vector2(48f, 16f);
            buttons[2].position = new Vector2(0f, 24f - screenHeight * 0.5f);
            buttons[2].size = new Vector2(48f, 16f);

            for (int index = 0; index < buttons.Length; index++)
                AddElement(buttons[index]);

            visitDevLog = new GeneralButton(this, "Wanna See Devlog?", OnVisitDevLog);
            visitDevLog.position = new Vector2(0f, screenHeight * -0.5f + 24f);
            visitDevLog.size = new Vector2(96f, 16f);
            AddElement(visitDevLog);
        }

        public override void Update(float deltaTime)
        {
            backGround.SetPosition(-mousePosition * 0.03f +
                new Vector2(Mathf.PerlinNoise(time, 0f) - 0.5f, Mathf.PerlinNoise(0f, time) - 0.5f) * 5f);

            backGround.alpha = Mathf.Lerp(0f, 0.5f, (time - 1f) * 0.5f);
            for (int i = 0; i < titleSprites.Length; i++)
                titleSprites[i].y = Mathf.Lerp(-screenHeight, screenHeight * 0.5f - 60f, CustomMath.Curve(time - (i * 0.2f), -3f)) + Mathf.Sin(time * 3f - i) * 4f;
            for (int i = 0; i < buttons.Length; i++)
                buttons[i].position = new Vector2(-screenWidth * 0.5f + Mathf.Lerp(-24f, 40f, CustomMath.Curve(time - 1f - (i * 0.2f), -3f)), buttons[i].position.y);
            visitDevLog.position = new Vector2(screenWidth * 0.5f + Mathf.Lerp(48f, -64f, CustomMath.Curve(time - 3f, -3f)), visitDevLog.position.y);

            base.Update(deltaTime);
        }

        public void OnGameStart()
        {
            flowManager.RequestSwitchLoopFlow(new ISOGame());
        }

        public void OpenOptions()
        {
            AddSubLoopFlow(optionsMenu);
        }

        public void OnVisitDevLog()
        {
            Application.OpenURL("https://twitter.com/i/moments/987507190041739264");
        }
    }
}