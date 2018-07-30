using System;
using UnityEngine;
using Custom;

namespace ISO.UI
{
    public class IntroRoll : Menu
    {
        private FContainer logoContainer;
        private FSprite logoSprite;
        private FContainer iconContainer;
        private FSprite[] iconSprites;
        private FSprite[] shadeSprites;

        public IntroRoll() : base()
        {
            logoContainer = new FContainer();

            iconContainer = new FContainer();
            iconSprites = new FSprite[4];
            shadeSprites = new FSprite[4];
            for (int i = 0; i < 4; i++)
            {
                iconSprites[i] = new FSprite(string.Concat("intro", i + 1));
                iconContainer.AddChild(iconSprites[i]);

                shadeSprites[i] = new FSprite("pixel");
                shadeSprites[i].scale = 4f;
                shadeSprites[i].color = Color.black;
                iconContainer.AddChild(shadeSprites[i]);
            }

            iconSprites[0].y = 3f;
            iconSprites[1].x = 3f;
            iconSprites[2].x = -3f;
            iconSprites[3].y = -3f;

            shadeSprites[0].SetPosition(-7f, 3f);
            shadeSprites[1].SetPosition(3f, 7f);
            shadeSprites[2].SetPosition(-3f, -7f);
            shadeSprites[3].SetPosition(7f, -3f);
            
            logoContainer.AddChild(iconContainer);

            logoSprite = new FSprite("intro5");
            logoSprite.y = -4.5f;
            logoContainer.AddChild(logoSprite);

            logoContainer.scale = 2f;
            container.AddChild(logoContainer);
        }

        public override void Update(float deltaTime)
        {
            iconSprites[0].x = Mathf.Lerp(-7f, -3f, CustomMath.Curve(time - 0.0f, -5f));
            iconSprites[1].y = Mathf.Lerp(7f, 3f, CustomMath.Curve(time - 0.1f, -5f));
            iconSprites[2].y = Mathf.Lerp(-7f, -3f, CustomMath.Curve(time - 0.2f, -5f));
            iconSprites[3].x = Mathf.Lerp(7f, 3f, CustomMath.Curve(time - 0.3f, -5f));

            iconContainer.scale = Mathf.Lerp(4f, 2f, CustomMath.Curve(time, -2f));

            float t = (CustomMath.Curve(CustomMath.Factor(time, 0.5f, 0.5f), 2f) + CustomMath.Curve(CustomMath.Factor(time, 1f, 0.5f), -2f)) * 0.5f;
            iconContainer.x = Mathf.Lerp(0f, -14f, t);
            float flicker = CustomMath.Curve(CustomMath.Factor(time, 0.5f, 1f), 1.5f);
            logoSprite.alpha = Mathf.Lerp(0f, 1f, flicker * (flicker < 1f ? Mathf.PerlinNoise(time * 12f, 0f) : 1f));
            logoSprite.x = Mathf.Lerp(0f, 11f, t);

            if (time > 4f)
                flowManager.RequestSwitchLoopFlow(new MainMenu());

            base.Update(deltaTime);
        }
    }
}
