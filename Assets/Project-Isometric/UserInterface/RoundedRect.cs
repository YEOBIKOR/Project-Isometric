using System;
using UnityEngine;

namespace Isometric.UI
{
	public class RoundedRect : UIObject
	{
        private FSprite[] sprites;

		public RoundedRect(Menu menu) : base(menu)
		{
            sprites = new FSprite[8];

            sprites[0] = new FSprite("roundedrectcorner", true);
            sprites[1] = new FSprite("roundedrectcorner", true);
            sprites[1].scaleX = -1f;
            sprites[2] = new FSprite("roundedrectcorner", true);
            sprites[2].scaleY = -1f;
            sprites[3] = new FSprite("roundedrectcorner", true);
            sprites[3].scaleX = -1f;
            sprites[3].scaleY = -1f;
            sprites[4] = new FSprite("pixel", true);
            sprites[5] = new FSprite("pixel", true);
            sprites[6] = new FSprite("pixel", true);
            sprites[7] = new FSprite("pixel", true);

            for (int i = 0; i < this.sprites.Length; i++)
                container.AddChild(sprites[i]);
        }

        public override void Update(float deltaTime)
        {
            Vector2 cornerSize = new Vector2(sprites[0].width, sprites[0].height);
            Vector2 cornerPosition = (size * 0.5f) - (cornerSize * 0.5f);

            sprites[0].SetPosition(position + new Vector2(-cornerPosition.x, cornerPosition.y));
            sprites[1].SetPosition(position + new Vector2(cornerPosition.x, cornerPosition.y));
            sprites[2].SetPosition(position + new Vector2(-cornerPosition.x, -cornerPosition.y));
            sprites[3].SetPosition(position + new Vector2(cornerPosition.x, -cornerPosition.y));

            float edgeThickness = 1f;
            Vector2 edgeLength = size - cornerSize * 2f;

            sprites[4].x = position.x;
            sprites[4].y = position.y + size.y * 0.5f - 0.5f;
            sprites[4].scaleX = edgeLength.x;
            sprites[4].scaleY = edgeThickness;
            sprites[5].x = position.x;
            sprites[5].y = position.y + size.y * -0.5f + 0.5f;
            sprites[5].scaleX = edgeLength.x;
            sprites[5].scaleY = edgeThickness;
            sprites[6].x = position.x + size.x * 0.5f - 0.5f;
            sprites[6].y = position.y;
            sprites[6].scaleX = edgeThickness;
            sprites[6].scaleY = edgeLength.y;
            sprites[7].x = position.x + size.x * -0.5f + 0.5f;
            sprites[7].y = position.y;
            sprites[7].scaleX = edgeThickness;
            sprites[7].scaleY = edgeLength.y;

            base.Update(deltaTime);
        }
    }
}
