﻿using System.Collections.Generic;
using UnityEngine;

namespace Isometric.UI
{
    public class UIObject
    {
        private Menu _menu;
        public virtual Menu menu
        {
            get
            { return _menu; }
        }

        private List<UIObject> elements;

        private Vector2 _position;
        public Vector2 position
        {
            get
            { return _position; }
            set
            { _position = value; }
        }

        private Vector2 _size;
        public Vector2 size
        {
            get
            { return _size; }
            set
            { _size = value; }
        }

        public virtual FContainer container
        {
            get
            { return _menu.container; }
        }

        public UIObject(Menu menu)
        {
            elements = new List<UIObject>();

            _menu = menu;
            _position = Vector2.zero;
            _size = Vector2.zero;
        }

        public virtual void OnActivate()
        {
            for (int index = 0; index < elements.Count; index++)
                elements[index].OnActivate();
        }

        public virtual void Update(float deltaTime)
        {
            for (int index = 0; index < elements.Count; index++)
                elements[index].Update(deltaTime);
        }

        public UIObject AddElement(UIObject element)
        {
            elements.Add(element);
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