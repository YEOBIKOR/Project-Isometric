using System;
using UnityEngine;
using Isometric.UI;

namespace Isometric.UI
{
    public class PopupMenu : Menu
    {
        private LoopFlow pausingTarget;

        private bool escToExit;
        private bool terminating;

        private float appearingTime;
        private float disappearingTime;

        private float _factor;
        public float factor
        {
            get
            { return _factor; }
        }

        public PopupMenu(LoopFlow pausingTarget, bool escToExit, float appearingTime = 0f, float disappearingTime = 0f) : base()
        {
            this.pausingTarget = pausingTarget;
            this.escToExit = escToExit;

            this.appearingTime = appearingTime;
            this.disappearingTime = disappearingTime;
        }

        public override void OnActivate()
        {
            base.OnActivate();
            
            terminating = false;
            _factor = 0f;

            if (pausingTarget != null)
                pausingTarget.paused = true;
        }

        public override void OnTerminate()
        {
            if (pausingTarget != null)
                pausingTarget.paused = false;

            base.OnTerminate();
        }

        public override void RawUpdate(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && escToExit && !paused && time > 0f)
                RequestTerminate();

            base.RawUpdate(deltaTime);
        }

        public override void Update(float deltaTime)
        {
            _factor = Mathf.Clamp01(_factor + (terminating ? deltaTime / -disappearingTime : deltaTime / appearingTime));
            if (!(_factor > 0f))
                Terminate();

            base.Update(deltaTime);
        }

        public void RequestTerminate()
        {
            terminating = true;
        }
    }
}
