using System.Collections.Generic;
using UnityEngine;
using Custom;
using Isometric.Items;
using Isometric.Interface;

public class Player : EntityCreature
{
    private PlayerGraphics _playerGraphics;
    private PlayerInterface _playerInterface;

    private ItemContainer[] _inventory;
    public ItemContainer[] inventory
    {
        get
        { return _inventory; }
    }
    public int inventorySize
    {
        get
        { return inventory.Length; }
    }
    private ItemContainer _pickedItemContainer;
    public ItemStack pickItemStack
    {
        get
        { return _pickedItemContainer.itemStack; }
    }

    private Damage _playerAttackDamage;

    private float _itemUseCoolTime;

    private Vector2 _moveDirectionByScreen;

    private CommandDelegate _playerCommand;
    
    public Player() : base(0.3f, 2.0f, 100f)
    {
        _physics.airControl = true;

        _inventory = new ItemContainer[36];

        for (int index = 0; index < _inventory.Length; index++)
            _inventory[index] = new ItemContainer();

        _playerGraphics = new PlayerGraphics(this);
        _playerInterface = new PlayerInterface(this);

        _pickedItemContainer = inventory[0];

        _playerAttackDamage = new Damage(this);

        Item[] items = Item.GetItemAll();
        for (int index = 0; index < inventorySize; index++)
        {
            if (index >= items.Length)
                break;

            inventory[index].Apply(new ItemStack(items[index], 30));
        }

        CreateCommand();
    }

    private void CreateCommand()
    {
        _playerCommand = new CommandDelegate();

        _playerCommand.Add("move_up", new CommandPlayerMove(this, Vector2.up));
        _playerCommand.Add("move_left", new CommandPlayerMove(this, Vector2.left));
        _playerCommand.Add("move_down", new CommandPlayerMove(this, Vector2.down));
        _playerCommand.Add("move_right", new CommandPlayerMove(this, Vector2.right));
        _playerCommand.Add("jump", new CommandPlayerJump(this));
        _playerCommand.Add("sprint", new CommandPlayerSprint(this));
        _playerCommand.Add("drop_item", new CommandCallback(delegate { DropItem(_pickedItemContainer); } ));
    }

    public override void OnSpawn()
    {
        base.OnSpawn();

        game.AddSubLoopFlow(_playerInterface);
    }

    public override void OnDespawn()
    {
        _playerInterface.Terminate();

        base.OnDespawn();
    }

    public override void Update(float deltaTime)
    {
        _playerCommand.Update(deltaTime);

        UpdateMovement(deltaTime);

        _itemUseCoolTime = Mathf.Max(_itemUseCoolTime - deltaTime, 0f);

        CursorType type = CursorType.None;

        if (!_pickedItemContainer.blank)
        {
            Item pickingItem = pickItemStack.item;
            type = pickingItem.cursorType;
        }

        _playerInterface.SetCursor(type);

        base.Update(deltaTime);

        _playerGraphics.Update(deltaTime);
    }

    public void Jump()
    {
        _physics.AddForce(new Vector3(0f, 12f, 0f));
    }

    private void UpdateMovement(float deltaTime)
    {
        if (_moveDirectionByScreen != Vector2.zero)
        {
            Vector2 moveDirection = worldCamera.ScreenToWorldDirection(_moveDirectionByScreen);

            MoveTo(moveDirection, 50f * deltaTime);
            viewAngle = Mathf.LerpAngle(viewAngle, Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg, deltaTime * 10f);

            _moveDirectionByScreen = Vector2.zero;
        }

        moveSpeed = 4f;
    }

    public void AcquireItem(ItemStack itemStack)
    {
        ItemStack returnItemStack = itemStack;

        for (int i = 0; i < inventorySize; i++)
        {
            if (inventory[i].blank || inventory[i].itemStack.item == itemStack.item)
                returnItemStack = inventory[i].Apply(returnItemStack);

            if (returnItemStack == null)
                break;
        }
    }

    public void DropItem(ItemContainer itemContainer)
    {
        if (!itemContainer.blank)
        {
            DroppedItem droppedItem = new DroppedItem(itemContainer.itemStack);
            droppedItem.velocity = CustomMath.HorizontalRotate(Vector3.right * 10f, viewAngle);

            world.SpawnEntity(droppedItem, worldPosition + Vector3.up);

            itemContainer.Apply(null);
        }
    }

    public void PickItem(ItemContainer itemContainer)
    {
        _pickedItemContainer = itemContainer;
    }

    public void UseItem(Vector3Int tilePosition, bool clicked)
    {
        SetViewDirection(tilePosition + Vector3.one * 0.5f);

        if (GetItemUsable(clicked))
        {
            pickItemStack.item.OnUseItem(world, this, _pickedItemContainer, tilePosition);

            _itemUseCoolTime = pickItemStack.item.useCoolTime;
        }
    }

    public void UseItem(Vector3 targetPosition, bool clicked)
    {
        SetViewDirection(targetPosition);
        
        if (GetItemUsable(clicked))
        {
            pickItemStack.item.OnUseItem(world, this, _pickedItemContainer, targetPosition);

            _itemUseCoolTime = pickItemStack.item.useCoolTime;
        }
    }

    private void SetViewDirection(Vector3 targetPosition)
    {
        Vector2 viewDirection = new Vector2(targetPosition.x - worldPosition.x, targetPosition.z - worldPosition.z);
        viewAngle = Mathf.Atan2(viewDirection.y, viewDirection.x) * Mathf.Rad2Deg;
    }

    private bool GetItemUsable(bool clicked)
    {
        if (pickItemStack == null)
            return false;

        return !(_itemUseCoolTime > 0f) && (pickItemStack.item.repeatableUse || clicked);
    }

    public override string name
    {
        get
        { return "Player"; }
    }

    public class PlayerGraphics
    {
        private enum PartType
        {
            Body,
            Head,
            Scarf,
            RLeg,
            LLeg,
            RFoot,
            LFoot,
            RArm,
            LArm,
            Tail,
            Item,
        }

        private Player _player;
        private List<EntityPart> entityParts
        {
            get
            { return _player._entityParts; }
        }

        private float _runfactor;

        private AnimationRigBipedal _bodyRig;
        private AnimationRigHandle _handleRig;

        public PlayerGraphics(Player player)
        {
            this._player = player;

            _runfactor = 0f;

            EntityPart body = new EntityPart(player, "entityplayerbody");
            EntityPart head = new ZFlipEntityPart(player, "entityplayerhead", "entityplayerheadback");
            EntityPart rArm = new EntityPart(player, "entityplayerarm");
            EntityPart lArm = new EntityPart(player, "entityplayerarm");
            EntityPart rLeg = new EntityPart(player, "entityplayerleg1");
            EntityPart lLeg = new EntityPart(player, "entityplayerleg1");
            EntityPart rFoot = new EntityPart(player, "entityplayerleg2");
            EntityPart lFoot = new EntityPart(player, "entityplayerleg2");
            EntityPart item = new EntityPart(player, null as FAtlasElement);

            _bodyRig = new AnimationRigBipedal(body, head, rArm, lArm, rLeg, lLeg, rFoot, lFoot);
            _handleRig = new AnimationRigHandle(rArm, lArm, item);

            entityParts.Add(body);
            entityParts.Add(head);
            entityParts.Add(new EntityPart(player, "entityplayerscarf"));
            entityParts.Add(rLeg);
            entityParts.Add(lLeg);
            entityParts.Add(rFoot);
            entityParts.Add(lFoot);
            entityParts.Add(rArm);
            entityParts.Add(lArm);
            entityParts.Add(new EntityPart(player, "entityplayertail"));
            entityParts.Add(item);

            GetEntityPart(PartType.Scarf).sortZOffset = 0.6f;
            GetEntityPart(PartType.Body).sortZOffset = -0.5f;
            GetEntityPart(PartType.RLeg).sortZOffset = 0.1f;
            GetEntityPart(PartType.LLeg).sortZOffset = 0.1f;
            GetEntityPart(PartType.RFoot).sortZOffset = 0.5f;
            GetEntityPart(PartType.LFoot).sortZOffset = 0.5f;
            GetEntityPart(PartType.Item).scale = new Vector2(-1f, 1f);
        }

        public void Update(float deltaTime)
        {
            Vector3 playerPosition = _player.worldPosition;

            _bodyRig.worldPosition = playerPosition;
            _bodyRig.viewAngle = _player.viewAngle;
            _bodyRig.moveSpeed = new Vector2(_player.velocity.x, _player.velocity.z).magnitude * deltaTime * 1.45f;
            _bodyRig.landed = _player._physics.landed;

            _handleRig.worldPosition = playerPosition;
            _handleRig.viewAngle = _player.viewAngle;
            _handleRig.cameraViewAngle = _player.worldCamera.viewAngle;

            GetEntityPart(PartType.Scarf).worldPosition = Vector3.Lerp(GetEntityPart(PartType.Body).worldPosition, GetEntityPart(PartType.Head).worldPosition, 0.3f);
            GetEntityPart(PartType.Tail).worldPosition = playerPosition + CustomMath.HorizontalRotate(new Vector3(-0.3f, 0.75f + Mathf.Sin(_runfactor * 2f) * -0.05f, Mathf.Sin(_runfactor * 2f) * 0.05f), _player.viewAngle);

            for (int index = 0; index < entityParts.Count; index++)
                _player._entityParts[index].viewAngle = _player.viewAngle;

            HoldType holdType = HoldType.None;

            EntityPart item = GetEntityPart(PartType.Item);
            item.element = null;

            if (!_player._pickedItemContainer.blank)
            {
                Item pickingItem = _player.pickItemStack.item;

                item.element = pickingItem.element;
                holdType = pickingItem.holdType;
            }

            _handleRig.ChangeHoldState(holdType);

            _bodyRig.Update(deltaTime);
            _handleRig.Update(deltaTime);
        }

        private EntityPart GetEntityPart(PartType type)
        {
            return entityParts[(int)type];
        }
    }

    public class CommandPlayerMove : ICommand
    {
        private Player _player;

        private Vector2 _moveDirection;

        public CommandPlayerMove(Player player, Vector2 moveDirection)
        {
            _player = player;

            _moveDirection = moveDirection;
        }

        public void OnKey()
        {
            _player._moveDirectionByScreen += _moveDirection;
        }

        public void OnKeyDown()
        {

        }

        public void OnKeyUp()
        {

        }
    }

    public class CommandPlayerJump : ICommand
    {
        private Player _player;

        public CommandPlayerJump(Player player)
        {
            _player = player;
        }

        public void OnKey()
        {
            if (_player.physics.landed)
                _player.Jump();
        }

        public void OnKeyDown()
        {

        }

        public void OnKeyUp()
        {

        }
    }

    public class CommandPlayerSprint : ICommand
    {
        private Player _player;

        public CommandPlayerSprint(Player player)
        {
            _player = player;
        }

        public void OnKey()
        {
            _player.moveSpeed = 8f;
        }

        public void OnKeyDown()
        {

        }

        public void OnKeyUp()
        {

        }
    }
}