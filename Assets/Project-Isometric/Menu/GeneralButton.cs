using System;
using UnityEngine;

namespace Isometric.UI
{
    public class GeneralButton : ButtonBase
    {
        private RoundedRect rect1;
        private RoundedRect rect2;

        private FLabel label;

        private Action clickCallback;

        private float hoverfactor;

        public GeneralButton(Menu menu, string name, Action clickCallback) : base(menu)
        {
            rect1 = new RoundedRect(menu);
            rect2 = new RoundedRect(menu);

            AddElement(rect1);
            AddElement(rect2);

            label = new FLabel("font", name);
            label.scale = 0.5f;
            container.AddChild(label);

            this.clickCallback = clickCallback;
        }

        public override void OnActivate()
        {
            base.OnActivate();

            hoverfactor = 0f;
        }

        public override void Update(float deltaTime)
        {
            hoverfactor = Mathf.Lerp(hoverfactor, hovering && !pressing ? 1f : 0f, deltaTime * 16f);

            rect1.position = position;
            rect2.position = position;
            rect1.size = size + Vector2.Lerp(Vector2.zero, Vector2.one * 6f, hoverfactor);
            rect2.size = size + Vector2.Lerp(Vector2.zero, Vector2.one * 2f, hoverfactor);

            label.SetPosition(position);

            base.Update(deltaTime);
        }

        public override void OnPressUp()
        {
            base.OnPressUp();

            if (clickCallback != null)
                clickCallback();
        }
    }
}