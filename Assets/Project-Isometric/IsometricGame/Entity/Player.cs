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

    private ICommand[] _commands;
    
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

            inventory[index].SetItem(new ItemStack(items[index], 30));
        }

        CreateCommand();
    }

    private void CreateCommand()
    {
        _commands = new ICommand[]
        {
            new CommandPlayerMove(this, Vector2.up),
            new CommandPlayerMove(this, Vector2.left),
            new CommandPlayerMove(this, Vector2.down),
            new CommandPlayerMove(this, Vector2.right),
            new CommandPlayerJump(this),
        };
    }

    private void AddCommand()
    {
        InputManager inputManager = InputManager.Instance;

        inputManager.AddCommand("move_up", _commands[0]);
        inputManager.AddCommand("move_left", _commands[1]);
        inputManager.AddCommand("move_down", _commands[2]);
        inputManager.AddCommand("move_right", _commands[3]);
        inputManager.AddCommand("jump", _commands[4]);
    }

    private void RemoveCommand()
    {
        InputManager inputManager = InputManager.Instance;

        for (int index = 0; index < _commands.Length; index++)
        {
            inputManager.RemoveCommand(_commands[index]);
        }
    }

    public override void OnSpawn(Chunk chunk, Vector3 position)
    {
        base.OnSpawn(chunk, position);

        game.AddSubLoopFlow(_playerInterface);

        AddCommand();
    }

    public override void OnDespawn()
    {
        RemoveCommand();

        _playerInterface.Terminate();

        base.OnDespawn();
    }

    public override void Update(float deltaTime)
    {
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
        moveSpeed = Input.GetKey(KeyCode.LeftShift) ? 8f : 4f;

        if (Input.GetKey(KeyCode.Return))
            KillCreature();

        if (Input.GetKeyDown(KeyCode.T))
            DropItem(_pickedItemContainer);

        if (_moveDirectionByScreen != Vector2.zero)
        {
            Vector2 moveDirection = worldCamera.ScreenToWorldDirection(_moveDirectionByScreen);

            MoveTo(moveDirection, 50f * deltaTime);
            viewAngle = Mathf.LerpAngle(viewAngle, Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg, deltaTime * 10f);

            _moveDirectionByScreen = Vector2.zero;
        }
    }

    public void AcquireItem(ItemStack itemStack)
    {
        ItemStack returnItemStack = itemStack;

        for (int i = 0; i < inventorySize; i++)
        {
            if (inventory[i].blank || inventory[i].itemStack.item == itemStack.item)
                returnItemStack = inventory[i].SetItem(returnItemStack);

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

            itemContainer.SetItem(null);
        }
    }

    public void PickItem(ItemContainer itemContainer)
    {
        _pickedItemContainer = itemContainer;
    }

    public void UseItem(RayTrace rayTrace, bool clicked)
    {
        Vector2 viewDirection = new Vector2(rayTrace.hitPosition.x - worldPosition.x, rayTrace.hitPosition.z - worldPosition.z);
        viewAngle = Mathf.Atan2(viewDirection.y, viewDirection.x) * Mathf.Rad2Deg;

        if (pickItemStack != null)
        {
            if (!(_itemUseCoolTime > 0f) && (pickItemStack.item.repeatableUse || clicked))
            {
                pickItemStack.OnUseItem(world, this, rayTrace);

                _itemUseCoolTime = pickItemStack.item.useCoolTime;
            }
        }
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

        private Player player;
        private List<EntityPart> entityParts
        {
            get
            { return player.entityParts; }
        }

        private float runfactor;

        private AnimationRigBipedal _bodyRig;
        private AnimationRigHandle _handleRig;

        public PlayerGraphics(Player player)
        {
            this.player = player;

            runfactor = 0f;

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
            Vector3 playerPosition = player.worldPosition;

            _bodyRig.worldPosition = playerPosition;
            _bodyRig.viewAngle = player.viewAngle;
            _bodyRig.moveSpeed = new Vector2(player.velocity.x, player.velocity.z).magnitude * deltaTime * 1.45f;
            _bodyRig.landed = player._physics.landed;

            _handleRig.worldPosition = playerPosition;
            _handleRig.viewAngle = player.viewAngle;
            _handleRig.cameraViewAngle = player.worldCamera.viewAngle;

            GetEntityPart(PartType.Scarf).worldPosition = Vector3.Lerp(GetEntityPart(PartType.Body).worldPosition, GetEntityPart(PartType.Head).worldPosition, 0.3f);
            GetEntityPart(PartType.Tail).worldPosition = playerPosition + CustomMath.HorizontalRotate(new Vector3(-0.3f, 0.75f + Mathf.Sin(runfactor * 2f) * -0.05f, Mathf.Sin(runfactor * 2f) * 0.05f), player.viewAngle);

            for (int index = 0; index < entityParts.Count; index++)
                player.entityParts[index].viewAngle = player.viewAngle;

            HoldType holdType = HoldType.None;

            EntityPart item = GetEntityPart(PartType.Item);
            item.element = null;

            if (!player._pickedItemContainer.blank)
            {
                Item pickingItem = player.pickItemStack.item;

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
}