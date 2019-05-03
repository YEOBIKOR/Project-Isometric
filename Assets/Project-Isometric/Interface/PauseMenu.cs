using UnityEngine;
using Custom;

namespace Isometric.Interface
{
    public class PauseMenu : PopupMenuFlow
    {
        private LoopFlow _pauseTarget;
        private OptionsMenu _optionsMenu;

        private FSprite[] _cinematicEdge;

        private GeneralButton[] _buttons;

        private static AudioClip _onPauseAudio;

        public PauseMenu(LoopFlow pauseTarget) : base(null, true, 0.5f, 0.5f)
        {
            _pauseTarget = pauseTarget;

            _optionsMenu = new OptionsMenu(this);

            AddElement(new FadePanel(this));

            _cinematicEdge = new FSprite[2];
            for (int index = 0; index < _cinematicEdge.Length; index++)
            {
                _cinematicEdge[index] = new FSprite("pixel");

                _cinematicEdge[index].width = Futile.screen.width;
                _cinematicEdge[index].color = Color.black;
                _cinematicEdge[index].y = Futile.screen.halfHeight * (index > 0 ? 1f : -1f);
            }

            container.AddChild(_cinematicEdge[0]);
            container.AddChild(_cinematicEdge[1]);

            _buttons = new GeneralButton[3];
            _buttons[0] = new GeneralButton(this, "Resume", RequestTerminate);
            _buttons[1] = new GeneralButton(this, "To Menu", BackToMenu);
            _buttons[2] = new GeneralButton(this, "Options", OpenOptions);

            for (int index = 0; index < _buttons.Length; index++)
            {
                _buttons[index].size = new Vector2(48f, 16f);
                AddElement(_buttons[index]);
            }

            _buttons[0].position = MenuFlow.rightDown + Vector2.right * -30f;
            _buttons[1].position = MenuFlow.leftDown + Vector2.right * 30f;
            _buttons[2].position = MenuFlow.leftDown + Vector2.right * 90f;

            if (_onPauseAudio == null)
            {
                _onPauseAudio = Resources.Load<AudioClip>("SoundEffects/UIMetalHit");
            }
        }

        public override void Update(float deltaTime)
        {
            _pauseTarget.paused = !(factor < 1f);
            _pauseTarget.timeScale = 1f - Mathf.Clamp01(factor * 2f);

            for (int index = 0; index < _cinematicEdge.Length; index++)
                _cinematicEdge[index].scaleY = Mathf.Lerp(0f, 72f, CustomMath.Curve(factor, -3f));

            for (int index = 0; index < _buttons.Length; index++)
                _buttons[index].position = new Vector2(_buttons[index].position.x,MenuFlow.screenHeight * -0.5f + Mathf.Lerp(-16f, 16f, CustomMath.Curve(factor * 4f - (index + 1), -1f)));

            base.Update(deltaTime);
        }

        public override void OnActivate()
        {
            base.OnActivate();

            AudioEngine.PlaySound(_onPauseAudio);
        }

        public override void OnTerminate()
        {
            _pauseTarget.paused = false;
            _pauseTarget.timeScale = 1f;

            base.OnTerminate();
        }

        public void BackToMenu()
        {
            loopFlowManager.RequestSwitchLoopFlow(new MainMenu());
        }

        public void OpenOptions()
        {
            AddSubLoopFlow(_optionsMenu);
        }
    }
}