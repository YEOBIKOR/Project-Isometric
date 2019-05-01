using System;
using UnityEngine;

namespace Isometric.Interface
{
	public class RoundedRect : InterfaceObject
	{
        private FSliceSprite _sliceSprite;

		public RoundedRect(MenuFlow menu, bool solid = false) : base(menu)
		{
            _sliceSprite = new FSliceSprite(solid ? "solidroundedrect" : "roundedrect", 16, 16, 8, 8, 8, 8);

            AddElement(_sliceSprite);
        }

        public override void Update(float deltaTime)
        {
            _sliceSprite.width = size.x + 16;
            _sliceSprite.height = size.y + 16;

            base.Update(deltaTime);
        }
    }
}
