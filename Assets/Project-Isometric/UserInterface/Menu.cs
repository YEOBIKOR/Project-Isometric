using System.Collections.Generic;
using UnityEngine;

namespace Isometric.UI
{
    public class Menu : LoopFlow
    {
        private FContainer _container;
        public FContainer container
        {
            get
            { return _container; }
        }

        private List<UIObject> _elements;

        public Menu() : base()
        {
            _container = new FContainer();
            _elements = new List<UIObject>();
        }

        public override void OnActivate()
        {
            base.OnActivate();

            for (int index = 0; index < _elements.Count; index++)
                _elements[index].OnActivate();

            Futile.stage.AddChild(_container);
        }

        public override void OnTerminate()
        {
            _container.RemoveFromContainer();

            base.OnTerminate();
        }

        public override void Update(float deltaTime)
        {
            for (int index = 0; index < _elements.Count; index++)
                _elements[index].Update(deltaTime);

            base.Update(deltaTime);
        }

        public UIObject AddElement(UIObject element)
        {
            _elements.Add(element);
            return element;
        }

        public static float screenWidth
        {
            get
            { return Futile.screen.width; }
        }

        public static float screenHeight
        {
            get
            { return Futile.screen.height; }
        }

        public static Vector2 leftUp
        {
            get
            { return new Vector2(screenWidth * -0.5f, screenHeight * 0.5f); }
        }
        
        public static Vector2 rightUp
        {
            get
            { return new Vector2(screenWidth * 0.5f, screenHeight * 0.5f); }
        }
        
        public static Vector2 leftDown
        {
            get
            { return new Vector2(screenWidth * -0.5f, screenHeight * -0.5f); }
        }
        
        public static Vector2 rightDown
        {
            get
            { return new Vector2(screenWidth * 0.5f, screenHeight * -0.5f); }
        }

        public static Vector2 mousePosition
        {
            get
            { return ( Input.mousePosition - new Vector3(Screen.width, Screen.height) * 0.5f) / Futile.displayScale; }
        }
    }
}