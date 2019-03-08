using UnityEngine;
using System.Collections.Generic;
using Custom;

namespace Isometric.Interface
{
    public class CameraHUDMenu : Menu
    {
        private IsometricGame _game;

        private WorldCamera _camera;

        public CameraHUDMenu(IsometricGame game, WorldCamera camera) : base()
        {
            _game = game;
            _camera = camera;
        }

        public override void RawUpdate(float deltaTime)
        {
            if (!_game.paused)
                base.RawUpdate(deltaTime);
        }

        public void Speech(IPositionable behaviour, string text)
        {
            ChatBubble bubble = new ChatBubble(_camera, behaviour, text, this);
            AddElement(bubble);
        }

        public void IndicateDamage(Damage damage, Vector3 position)
        {
            DamageIndicator indicator = new DamageIndicator(this, damage);
            indicator.position = _camera.GetScreenPosition(position) + _camera.worldContainer.GetPosition();

            AddElement(indicator);
        }
    }
}