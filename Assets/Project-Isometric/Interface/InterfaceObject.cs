using System.Collections.Generic;
using UnityEngine;

namespace Isometric.Interface
{
    public class InterfaceObject
    {
        private Menu _menu;
        public virtual Menu menu
        {
            get
            { return _menu; }
        }

        private List<InterfaceObject> _elements;
        
        public Vector2 position
        {
            get
            { return _container.GetPosition(); }
            set
            { _container.SetPosition(value); }
        }

        private Vector2 _size;
        public Vector2 size
        {
            get
            { return _size; }
            set
            { _size = value; }
        }

        public Vector2 scale
        {
            get
            { return new Vector2(_container.scaleX, _container.scaleY); }
            set
            { _container.scaleX = value.x; _container.scaleY = value.y; }
        }

        private FContainer _container;
        public virtual FContainer container
        {
            get
            { return _container; }
        }

        public InterfaceObject(Menu menu)
        {
            _menu = menu;
            _elements = new List<InterfaceObject>();
            _size = new Vector2(100f, 100f);
            _container = new FContainer();
        }

        public virtual void OnActivate()
        {
            for (int index = 0; index < _elements.Count; index++)
                _elements[index].OnActivate();
        }

        public virtual void Update(float deltaTime)
        {
            for (int index = 0; index < _elements.Count; index++)
                _elements[index].Update(deltaTime);
        }

        public InterfaceObject AddElement(InterfaceObject element)
        {
            _elements.Add(element);
            _container.AddChild(element.container);

            return element;
        }

        public bool mouseOn
        {
            get
            {
                Vector2 mousePos = Menu.mousePosition;

                return mousePos.x > position.x - size.x * 0.5f && mousePos.x < position.x + size.x * 0.5f &&
                    mousePos.y > position.y - size.y * 0.5f && mousePos.y < position.y + size.y * 0.5f;
            }
        }
    }
}