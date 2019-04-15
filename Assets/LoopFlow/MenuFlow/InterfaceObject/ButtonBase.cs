using System;
using UnityEngine;

namespace Isometric.Interface
{
	public abstract class ButtonBase : InterfaceObject
	{
        public bool hovering { get; private set; }
        public bool pressing { get; private set; }

        public ButtonBase(MenuFlow menu) : base(menu)
		{
            hovering = false;
            pressing = false;
        }

        public override void Update(float deltaTime)
        {
            bool mouseOn = this.mouseOn;
            
            if (hovering)
            {
                bool keyDown = Input.GetKey(KeyCode.Mouse0);

                if (!pressing && keyDown)
                {
                    OnPressDown();
                    pressing = true;
                }
                else if (pressing && !keyDown)
                {
                    OnPressUp();
                    pressing = false;
                }

                if (!mouseOn)
                {
                    OnHoverOut();
                    hovering = false;
                    pressing = false;
                }
                else
                {
                    OnHover();
                }
            }
            else if (!hovering && mouseOn)
            {
                OnHoverIn();
                hovering = true;
            }

            base.Update(deltaTime);
        }

        public abstract void OnHoverIn();

        public abstract void OnHover();

        public abstract void OnHoverOut();

        public abstract void OnPressDown();

        public abstract void OnPressUp();
    }
}
