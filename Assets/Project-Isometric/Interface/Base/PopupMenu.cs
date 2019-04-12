using System;
using UnityEngine;
using Isometric.Interface;

namespace Isometric.Interface
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

            if (escToExit)
                flowManager.AddPopup(this);

            if (pausingTarget != null)
                pausingTarget.paused = true;
        }

        public override void OnTerminate()
        {
            if (pausingTarget != null)
                pausingTarget.paused = false;

            flowManager.RemovePopup(this);

            base.OnTerminate();
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
