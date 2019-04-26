using System.Collections.Generic;
using UnityEngine;

namespace Isometric.Interface
{
    public abstract class InterfaceObject
    {
        private MenuFlow _menu;
        public virtual MenuFlow menu
        {
            get
            { return _menu; }
        }

        private InterfaceObject _parent;

        private List<InterfaceObject> _elements;

        public bool activated
        {
            get
            { return container.container != null; }
        }
        
        public bool visible
        {
            get
            { return container.isVisible; }
            set
            { container.isVisible = value; }
        }

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

        public InterfaceObject(MenuFlow menu)
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

        public virtual void OnDeactivate()
        {
            for (int index = 0; index < _elements.Count; index++)
                _elements[index].OnDeactivate();
        }

        public virtual void Update(float deltaTime)
        {
            for (int index = 0; index < _elements.Count; index++)
                _elements[index].Update(deltaTime);
        }

        public void AddElement(InterfaceObject element)
        {
            if (visible)
                element.OnActivate();

            element._parent = this;

            _elements.Add(element);
            _container.AddChild(element.container);
        }

        public FNode AddElement(FNode element)
        {
            _container.AddChild(element);

            return element;
        }

        public void RemoveElement(InterfaceObject element)
        {
            int index = _elements.IndexOf(element);

            if (index < 0)
                return;

            if (visible)
                element.OnDeactivate();

            element._parent = null;

            _elements.RemoveAt(index);
            _container.RemoveChild(element.container);
        }

        public FNode RemoveElement(FNode element)
        {
            _container.RemoveChild(element);

            return element;
        }

        public void RemoveSelf()
        {
            if (visible)
            {
                if (_parent == null)
                    _menu.RemoveElement(this);
                else
                    _parent.RemoveElement(this);
            }

        }

        public bool mouseOn
        {
            get
            {
                Vector2 mousePos = MenuFlow.mousePosition;

                return mousePos.x > position.x - size.x * 0.5f && mousePos.x < position.x + size.x * 0.5f &&
                    mousePos.y > position.y - size.y * 0.5f && mousePos.y < position.y + size.y * 0.5f;
            }
        }
    }
}