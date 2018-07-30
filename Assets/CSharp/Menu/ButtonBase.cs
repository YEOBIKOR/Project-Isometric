using System;
using UnityEngine;

namespace ISO.UI
{
	public class ButtonBase : UIObject
	{
        public bool hovering { get; private set; }
        public bool pressing { get; private set; }

        public ButtonBase(Menu menu) : base(menu)
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
                    hovering = false;
                    pressing = false;
                }
            }
            else if (!hovering && mouseOn)
            {
                OnMouseHover();
                hovering = true;
            }

            base.Update(deltaTime);
        }

        public virtual void OnMouseHover()
        {

        }

        public virtual void OnPressDown()
        {

        }

        public virtual void OnPressUp()
        {

        }
    }
}
