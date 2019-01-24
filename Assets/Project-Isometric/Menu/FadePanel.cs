using System;
using UnityEngine;
using Custom;

namespace Isometric.UI
{
	public class FadePanel : UIObject
	{
        private FSprite sprite;

		public FadePanel(PopupMenu menu) : base(menu)
		{
            sprite = new FSprite("pixel");

            sprite.scaleX = Menu.screenWidth;
            sprite.scaleY = Menu.screenHeight;

            sprite.color = new Color(0f, 0f, 0f, 0.5f);
            sprite.alpha = 0f;

            container.AddChild(sprite);
        }

        public override void Update(float deltaTime)
        {
            sprite.alpha = CustomMath.Curve((menu as PopupMenu).factor, -3f);

            base.Update(deltaTime);
        }
    }
}
