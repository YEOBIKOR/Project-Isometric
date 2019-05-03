using System;
using UnityEngine;
using Custom;

namespace Isometric.Interface
{
    public class OptionsMenu : PopupMenuFlow
    {
        private FSprite _fadePanel;
        private RoundedRect _panel;

        private FLabel _label;

        public OptionsMenu(LoopFlow pausingTarget) : base(pausingTarget, true, 0.5f, 0.5f)
        {
            AddElement(new FadePanel(this));

            _panel = new RoundedRect(this);
            AddElement(_panel);

            _label = new FLabel("font", "Not yet.");
            container.AddChild(_label);
        }

        public override void Update(float deltaTime)
        {
            _panel.size = Vector2.Lerp(Vector2.zero, new Vector2(240f, 240f), CustomMath.Curve(factor, -3f));

            base.Update(deltaTime);
        }
    }
}