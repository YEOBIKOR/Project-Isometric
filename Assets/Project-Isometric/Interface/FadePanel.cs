using System;
using UnityEngine;
using Custom;

namespace Isometric.Interface
{
	public class FadePanel : InterfaceObject
	{
        private FSprite _sprite;

		public FadePanel(PopupMenuFlow menu) : base(menu)
		{
            _sprite = new FSprite("pixel");

            _sprite.scaleX = MenuFlow.screenWidth;
            _sprite.scaleY = MenuFlow.screenHeight;

            _sprite.color = new Color(0f, 0f, 0f, 0.5f);
            _sprite.alpha = 0f;

            container.AddChild(_sprite);
        }

        public override void Update(float deltaTime)
        {
            _sprite.alpha = CustomMath.Curve((menu as PopupMenuFlow).factor, -3f);

            base.Update(deltaTime);
        }
    }
}
