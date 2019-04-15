using System.Collections.Generic;
using UnityEngine;

namespace Isometric.Interface
{
    public abstract class MenuFlow : LoopFlow
    {
        private FContainer _container;
        public FContainer container
        {
            get
            { return _container; }
        }
        
        private List<InterfaceObject> _elements;

        public MenuFlow() : base()
        {
            _container = new FContainer();
            _elements = new List<InterfaceObject>();
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
            for (int index = 0; index < _elements.Count; index++)
                _elements[index].OnDeactivate();

            _container.RemoveFromContainer();

            base.OnTerminate();
        }

        public override void Update(float deltaTime)
        {
            for (int index = 0; index < _elements.Count; index++)
                _elements[index].Update(deltaTime);

            base.Update(deltaTime);
        }

        public void AddElement(InterfaceObject element)
        {
            _elements.Add(element);
            _container.AddChild(element.container);

            if (activated)
                element.OnActivate();
        }

        public void RemoveElement(InterfaceObject element)
        {
            int index = _elements.IndexOf(element);

            if (index < 0)
                return;

            if (activated)
                element.OnDeactivate();

            _elements.RemoveAt(index);
            _container.RemoveChild(element.container);
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
            { return (Input.mousePosition - new Vector3(Screen.width, Screen.height) * 0.5f) * Futile.displayScaleInverse; }
        }
    }
}