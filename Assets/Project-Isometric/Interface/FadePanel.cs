using System;
using UnityEngine;
using Custom;

namespace Isometric.Interface
{
	public class FadePanel : InterfaceObject
	{
        private FSprite sprite;

		public FadePanel(PopupMenuFlow menu) : base(menu)
		{
            sprite = new FSprite("pixel");

            sprite.scaleX = MenuFlow.screenWidth;
            sprite.scaleY = MenuFlow.screenHeight;

            sprite.color = new Color(0f, 0f, 0f, 0.5f);
            sprite.alpha = 0f;

            container.AddChild(sprite);
        }

        public override void Update(float deltaTime)
        {
            sprite.alpha = CustomMath.Curve((menu as PopupMenuFlow).factor, -3f);

            base.Update(deltaTime);
        }
    }
}
