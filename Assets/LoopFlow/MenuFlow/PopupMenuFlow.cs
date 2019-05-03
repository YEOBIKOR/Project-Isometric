using System;
using UnityEngine;
using Isometric.Interface;

namespace Isometric.Interface
{
    public class PopupMenuFlow : MenuFlow
    {
        private LoopFlow _pausingTarget;

        private bool _escToExit;
        private bool _terminating;

        private float _appearingTime;
        private float _disappearingTime;

        private float _factor;
        public float factor
        {
            get
            { return _factor; }
        }

        public PopupMenuFlow(LoopFlow pausingTarget, bool escToExit, float appearingTime = 0f, float disappearingTime = 0f) : base()
        {
            _pausingTarget = pausingTarget;
            _escToExit = escToExit;

            _appearingTime = appearingTime;
            _disappearingTime = disappearingTime;
        }

        public override void OnActivate()
        {
            base.OnActivate();
            
            _terminating = false;
            _factor = 0f;

            if (_pausingTarget != null)
                _pausingTarget.paused = true;
        }

        public override void OnTerminate()
        {
            if (_pausingTarget != null)
                _pausingTarget.paused = false;

            base.OnTerminate();
        }

        public override void Update(float deltaTime)
        {
            _factor = Mathf.Clamp01(_factor + (_terminating ? deltaTime / -_disappearingTime : deltaTime / _appearingTime));
            if (!(_factor > 0f))
                Terminate();

            base.Update(deltaTime);
        }

        public void RequestTerminate()
        {
            _terminating = true;
        }

        public override bool OnExecuteEscape()
        {
            if (_escToExit)
                RequestTerminate();

            return true;
        }
    }
}
