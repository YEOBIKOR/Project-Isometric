using System;
using UnityEngine;

namespace Isometric.Interface
{
    public class GeneralButton : ButtonBase
    {
        private RoundedRect _rect1;
        private RoundedRect _rect2;

        private FLabel _label;

        private Action _clickCallback;

        private float _hoverfactor;
        private bool _pressAudio;

        private static AudioClip _onHoverAudio;
        private static AudioClip _onPressAudio;

        public string text
        {
            get
            { return _label.text; }
            set
            { _label.text = value; }
        }

        public GeneralButton(MenuFlow menu, string name, Action clickCallback, bool pressAudio = true) : base(menu)
        {
            _rect1 = new RoundedRect(menu);
            _rect2 = new RoundedRect(menu);

            AddElement(_rect1);
            AddElement(_rect2);

            _label = new FLabel("font", name);
            container.AddChild(_label);

            this._clickCallback = clickCallback;
            this._pressAudio = pressAudio;

            if (_onHoverAudio == null)
            {
                _onHoverAudio = Resources.Load<AudioClip>("SoundEffects/UIMetal3");
                _onPressAudio = Resources.Load<AudioClip>("SoundEffects/UIArp");
            }
        }

        public override void OnActivate()
        {
            base.OnActivate();

            _hoverfactor = 0f;
        }

        public override void Update(float deltaTime)
        {
            _hoverfactor = Mathf.Lerp(_hoverfactor, hovering && !pressing ? 1f : 0f, deltaTime * 16f);

            Vector2 rectSize = size - Vector2.one * 12f;

            _rect1.size = rectSize + Vector2.Lerp(Vector2.zero, Vector2.one * 6f, _hoverfactor);
            _rect2.size = rectSize + Vector2.Lerp(Vector2.zero, Vector2.one * 2f, _hoverfactor);

            base.Update(deltaTime);
        }

        public override void OnHoverIn()
        {
            AudioEngine.PlaySound(_onHoverAudio);
        }

        public override void OnHover()
        {

        }

        public override void OnHoverOut()
        {

        }

        public override void OnPressUp()
        {
            if (_clickCallback != null)
                _clickCallback();

            if (_pressAudio)
                AudioEngine.PlaySound(_onPressAudio);
        }

        public override void OnPressDown()
        {

        }
    }
}