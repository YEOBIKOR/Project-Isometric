using UnityEngine;
using Isometric.Items;
using Custom;

namespace Isometric.Interface
{
    public enum CursorType
    {
        None,
        Construct,
        Destruct,
        Target
    }

    public class PlayerInterface : MenuFlow
    {
        private Player _player;
        public Player player
        {
            get { return _player; }
        }
        public WorldCamera worldCamera
        {
            get { return _player.worldCamera; }
        }

        private ItemSelect _itemSelect;
        private InventoryMenu _inventoryMenu;

        private WorldCursor[] _cursors;
        private WorldCursor _currentCursor;

        private CommandDelegate _playerCommand;

        public PlayerInterface(Player player) : base()
        {
            _player = player;

            _itemSelect = new ItemSelect(this);
            _inventoryMenu = new InventoryMenu(this);

            _playerCommand = new CommandDelegate();

            _playerCommand.Add("inventory", new CommandCallback(delegate
            {
                if (_inventoryMenu.activated)
                    _inventoryMenu.RequestTerminate();
                else
                    AddSubLoopFlow(_inventoryMenu);
            }));
        }

        public override void OnActivate()
        {
            base.OnActivate();

            _cursors = new WorldCursor[]
            {
                null,
                new ConstructCursor(this, worldCamera),
                new DestructCursor(this, worldCamera),
                new TargetCursor(this)
            };

            SetCursor(CursorType.None);
        }

        public void SetCursor(CursorType cursorType)
        {
            WorldCursor newCursor = _cursors[(int)cursorType];

            if (_currentCursor == newCursor)
                return;

            if (_currentCursor != null)
                _currentCursor.RemoveSelf();

            _currentCursor = newCursor;

            if (_currentCursor != null)
                AddElement(_currentCursor);
        }

        public override void RawUpdate(float deltaTime)
        {
            if (!player.game.paused)
            {
                _playerCommand.Update(deltaTime);

                base.RawUpdate(deltaTime);
            }
        }

        public override void Update(float deltaTime)
        {
            if (!_inventoryMenu.activated)
            {
                if (Input.mouseScrollDelta.y != 0f && !_itemSelect.activated)
                    AddSubLoopFlow(_itemSelect);

                if (_currentCursor != null)
                    _currentCursor.CursorUpdate(player.world, player, MenuFlow.mousePosition);
            }

            base.Update(deltaTime);
        }

        public class ItemSelect : MenuFlow
        {
            public const int Length = 8;

            private PlayerInterface _playerInterface;

            public Player player
            {
                get
                { return _playerInterface.player; }
            }
            
            private ItemContainerVisualizer[] _visualizer;
            private int _selectedIndex;

            private float _factor;
            private float _sleepTime;

            private float _scrollAmount;
            private float _selectSpriteAngle;

            private FLabel _selectedItemLabel;
            private FLabel _selectedItemLabelShadow;

            public ItemSelect(PlayerInterface playerInterface) : base()
            {
                this._playerInterface = playerInterface;

                _visualizer = new ItemContainerVisualizer[Length];
                for (int index = 0; index < Length; index++)
                {
                    _visualizer[index] = new ItemContainerVisualizer(this, player.inventory[index]);
                    AddElement(_visualizer[index]);
                }

                _factor = 0f;
                _sleepTime = 0f;
                _scrollAmount = 0f;
                _selectSpriteAngle = 0f;

                _selectedItemLabel = new FLabel("font", string.Empty);
                _selectedItemLabelShadow = new FLabel("font", string.Empty);
                _selectedItemLabelShadow.color = Color.black;

                container.AddChild(_selectedItemLabelShadow);
                container.AddChild(_selectedItemLabel);
            }

            public override void Update(float deltaTime)
            {
                Vector2 anchorPosition = player.screenPosition + Vector2.up * 16f;
                float scrollDelta = Input.mouseScrollDelta.y;

                _sleepTime = scrollDelta != 0 ? 1f : _sleepTime - deltaTime;
                _factor = Mathf.Clamp01(_factor + (_sleepTime > 0f ? deltaTime : -deltaTime) * 5f);

                if (!(_factor > 0f))
                    Terminate();

                _scrollAmount += scrollDelta;

                int oldSelectedIndex = _selectedIndex;
                _selectedIndex = (int)Mathf.Repeat(-_scrollAmount, Length);

                _selectSpriteAngle = Mathf.LerpAngle(_selectSpriteAngle, (float)_selectedIndex / Length * 360f - 90f, deltaTime * 10f);

                for (int index = 0; index < Length; index++)
                {
                    bool selected = index == _selectedIndex;
                    float radian = ((float)index / Length - _selectSpriteAngle / 360f) * Mathf.PI * 2f;

                    _visualizer[index].position = anchorPosition + (new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * _factor * (selected ? 40f : 32f));
                    _visualizer[index].scale = Vector2.Lerp(_visualizer[index].scale, Vector2.one * (selected ? 2f : 1f), deltaTime * 10f);
                    _visualizer[index].container.alpha = _factor;
                }

                ItemContainer pickedItemContainer = player.inventory[_selectedIndex];

                if (pickedItemContainer.blank)
                    _selectedItemLabel.text = string.Empty;
                else
                    _selectedItemLabel.text = pickedItemContainer.itemStack.item.name;

                _selectedItemLabelShadow.text = _selectedItemLabel.text;

                _selectedItemLabel.SetPosition(anchorPosition + new Vector2(0f, 24f));
                _selectedItemLabelShadow.SetPosition(anchorPosition + new Vector2(0f, 23f));

                _selectedItemLabel.alpha = _factor;
                _selectedItemLabelShadow.alpha = _factor;

                if (oldSelectedIndex != _selectedIndex)
                    player.PickItem(pickedItemContainer);

                base.Update(deltaTime);
            }
        }
    }
}