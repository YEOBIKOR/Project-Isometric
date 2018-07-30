using System;
using UnityEngine;
using Custom;

namespace ISO.UI
{
    public class OptionsMenu : PopupMenu
    {
        private FSprite fadePanel;
        private RoundedRect panel;
        private FLabel label;

        public OptionsMenu(LoopFlow pausingTarget) : base(pausingTarget, true, 0.5f, 0.5f)
        {
            AddElement(new FadePanel(this));

            panel = new RoundedRect(this);
            AddElement(panel);

            label = new FLabel("font", "Not yet.");
            container.AddChild(label);
        }

        public override void Update(float deltaTime)
        {
            panel.size = Vector2.Lerp(Vector2.zero, new Vector2(240f, 240f), CustomMath.Curve(factor, -3f));

            base.Update(deltaTime);
        }
    }
}