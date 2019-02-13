using System;
using UnityEngine;

namespace Isometric.Interface
{
	public class RoundedRect : InterfaceObject
	{
        private FSprite[] sprites;

		public RoundedRect(Menu menu) : base(menu)
		{
            sprites = new FSprite[8];

            FAtlasElement cornerElement = Futile.atlasManager.GetElementWithName("roundedrectcorner");
            FAtlasElement pixelElement = Futile.atlasManager.GetElementWithName("uipixel");

            sprites[0] = new FSprite(cornerElement, true);
            sprites[1] = new FSprite(cornerElement, true);
            sprites[1].scaleX = -1f;
            sprites[2] = new FSprite(cornerElement, true);
            sprites[2].scaleY = -1f;
            sprites[3] = new FSprite(cornerElement, true);
            sprites[3].scaleX = -1f;
            sprites[3].scaleY = -1f;
            sprites[4] = new FSprite(pixelElement, true);
            sprites[5] = new FSprite(pixelElement, true);
            sprites[6] = new FSprite(pixelElement, true);
            sprites[7] = new FSprite(pixelElement, true);

            for (int i = 0; i < this.sprites.Length; i++)
                container.AddChild(sprites[i]);
        }

        public override void Update(float deltaTime)
        {
            Vector2 cornerSize = new Vector2(sprites[0].width, sprites[0].height);
            Vector2 cornerPosition = (size * 0.5f) - (cornerSize * 0.5f);

            sprites[0].SetPosition(-cornerPosition.x, cornerPosition.y);
            sprites[1].SetPosition(cornerPosition.x, cornerPosition.y);
            sprites[2].SetPosition(-cornerPosition.x, -cornerPosition.y);
            sprites[3].SetPosition(cornerPosition.x, -cornerPosition.y);

            float edgeThickness = 1f;
            Vector2 edgeLength = size - cornerSize * 2f;
            
            sprites[4].y = size.y * 0.5f - 0.5f;
            sprites[4].scaleX = edgeLength.x;
            sprites[4].scaleY = edgeThickness;
            sprites[5].y = size.y * -0.5f + 0.5f;
            sprites[5].scaleX = edgeLength.x;
            sprites[5].scaleY = edgeThickness;
            sprites[6].x = size.x * 0.5f - 0.5f;
            sprites[6].scaleX = edgeThickness;
            sprites[6].scaleY = edgeLength.y;
            sprites[7].x = size.x * -0.5f + 0.5f;
            sprites[7].scaleX = edgeThickness;
            sprites[7].scaleY = edgeLength.y;

            base.Update(deltaTime);
        }
    }
}
