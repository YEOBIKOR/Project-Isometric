using System;
using UnityEngine;
using Custom;

namespace Isometric.Interface
{
    public class IntroRoll : MenuFlow
    {
        private FContainer _logoContainer;
        private FSprite _logoSprite;
        private FContainer _iconContainer;
        private FSprite[] _iconSprites;
        private FSprite[] _shadeSprites;

        public IntroRoll() : base()
        {
            _logoContainer = new FContainer();
            _iconContainer = new FContainer();
            _iconSprites = new FSprite[4];
            _shadeSprites = new FSprite[4];
            for (int i = 0; i < 4; i++)
            {
                _iconSprites[i] = new FSprite(string.Concat("intro", i + 1));
                _iconContainer.AddChild(_iconSprites[i]);

                _shadeSprites[i] = new FSprite("pixel");
                _shadeSprites[i].scale = 4f;
                _shadeSprites[i].color = Color.black;
                _iconContainer.AddChild(_shadeSprites[i]);
            }

            _iconSprites[0].y = 3f;
            _iconSprites[1].x = 3f;
            _iconSprites[2].x = -3f;
            _iconSprites[3].y = -3f;

            _shadeSprites[0].SetPosition(-7f, 3f);
            _shadeSprites[1].SetPosition(3f, 7f);
            _shadeSprites[2].SetPosition(-3f, -7f);
            _shadeSprites[3].SetPosition(7f, -3f);
            
            _logoContainer.AddChild(_iconContainer);

            _logoSprite = new FSprite("intro5");
            _logoSprite.y = -5f;
            _logoContainer.AddChild(_logoSprite);

            _logoContainer.scale = 2f;
            container.AddChild(_logoContainer);
        }

        public override void Update(float deltaTime)
        {
            _iconSprites[0].x = Mathf.Lerp(-7f, -3f, CustomMath.Curve(time - 0.0f, -5f));
            _iconSprites[1].y = Mathf.Lerp(7f, 3f, CustomMath.Curve(time - 0.1f, -5f));
            _iconSprites[2].y = Mathf.Lerp(-7f, -3f, CustomMath.Curve(time - 0.2f, -5f));
            _iconSprites[3].x = Mathf.Lerp(7f, 3f, CustomMath.Curve(time - 0.3f, -5f));

            _iconContainer.scale = Mathf.Lerp(4f, 2f, CustomMath.Curve(time, -2f));

            float t = (CustomMath.Curve(CustomMath.Factor(time, 0.5f, 0.5f), 2f) + CustomMath.Curve(CustomMath.Factor(time, 1f, 0.5f), -2f)) * 0.5f;
            _iconContainer.x = Mathf.Lerp(0f, -14f, t);
            float flicker = CustomMath.Curve(CustomMath.Factor(time, 0.5f, 1f), 1.5f);
            _logoSprite.alpha = Mathf.Lerp(0f, 1f, flicker * (flicker < 1f ? Mathf.PerlinNoise(time * 12f, 0f) : 1f));
            _logoSprite.x = Mathf.Lerp(0f, 11f, t);

            if (time > 4f && !loopFlowManager.transiting)
                loopFlowManager.RequestSwitchLoopFlow(new MainMenu());

            base.Update(deltaTime);
        }
    }
}
