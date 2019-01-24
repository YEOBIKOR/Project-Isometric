using System.Collections.Generic;
using UnityEngine;
using Custom;
using Isometric.Items;
using Isometric.UI;

public class Player : EntityCreature
{
    private PlayerGraphics playerGraphics;
    private PlayerInterface playerInterface;

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
    private ItemContainer pickedItemContainer;
    public ItemStack pickItemStack
    {
        get
        { return pickedItemContainer.itemStack; }
    }

    private Damage playerAttackDamage;

    private float itemUseCoolTime;
    
    public Player() : base(0.3f, 2.0f, 100f)
    {
        airControl = true;

        _inventory = new ItemContainer[32];

        for (int index = 0; index < _inventory.Length; index++)
            _inventory[index] = new ItemContainer();

        playerGraphics = new PlayerGraphics(this);
        playerInterface = new PlayerInterface(this);

        pickedItemContainer = inventory[0];

        playerAttackDamage = new Damage(this);

        Item[] items = Item.GetItemAll();
        for (int index = 0; index < inventorySize; index++)
        {
            if (index >= items.Length)
                break;

            inventory[index].SetItem(new ItemStack(items[index], 1));
        }
    }

    public override void OnSpawn(Chunk chunk, Vector3 position)
    {
        base.OnSpawn(chunk, position);

        game.AddSubLoopFlow(playerInterface);
        worldCamera.SetCameraTarget(this);
    }

    public override void OnDespawn()
    {
        playerInterface.Terminate();

        base.OnDespawn();
    }

    public override void Update(float deltaTime)
    {
        moveSpeed = Input.GetKey(KeyCode.LeftShift) ? 8f : 4f;

        if (Input.GetKey(KeyCode.Space) && landed)
            AddForce(new Vector3(0f, 12f, 0f));

        if (Input.GetKey(KeyCode.Return))
            KillCreature();

        if (Input.GetKeyDown(KeyCode.T))
            DropItem(pickedItemContainer);

        Vector2 moveDirectionByScreen = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
            moveDirectionByScreen += Vector2.up;
        if (Input.GetKey(KeyCode.S))
            moveDirectionByScreen += Vector2.down;
        if (Input.GetKey(KeyCode.A))
            moveDirectionByScreen += Vector2.left;
        if (Input.GetKey(KeyCode.D))
            moveDirectionByScreen += Vector2.right;

        if (moveDirectionByScreen != Vector2.zero)
        {
            Vector2 moveDirection = worldCamera.ScreenToWorldDirection(moveDirectionByScreen);

            MoveTo(moveDirection, 50f * deltaTime);
            viewAngle = Mathf.LerpAngle(viewAngle, Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg, deltaTime * 10f);
        }

        itemUseCoolTime = Mathf.Max(itemUseCoolTime - deltaTime, 0f);

        base.Update(deltaTime);

        playerGraphics.Update(deltaTime);
    }

    public void AcquireItem(ItemStack itemStack)
    {
        ItemStack returnItemStack = itemStack;

        for (int i = 0; i < inventorySize; i++)
        {
            if (inventory[i].blank)
                returnItemStack = inventory[i].SetItem(returnItemStack);

            else if (itemStack == inventory[i].itemStack)
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
        pickedItemContainer = itemContainer;
    }

    public void UseItem(RayTrace rayTrace, bool clicked)
    {
        if (pickItemStack != null)
        {
            if (!(itemUseCoolTime > 0f) && (pickItemStack.item.repeatableUse || clicked))
            {
                pickItemStack.OnUseItem(this, rayTrace);
                itemUseCoolTime = pickItemStack.item.useCoolTime;
            }
        }

        playerGraphics.useTime = 0f;
    }

    public class PlayerGraphics
    {
        private enum PartType
        {
            Head,
            Scarf,
            Body,
            RLeg,
            LLeg,
            RFoot,
            LFoot,
            RArm,
            LArm,
            Tail,
            Item,
        }

        public enum MoveState
        {
            Idle,
            Walk,
            Run
        }

        private Player player;
        private List<EntityPart> entityParts
        {
            get
            { return player.entityParts; }
        }

        public float useTime
        {
            get
            { return _useTime; }
            set
            { _useTime = value; }
        }

        public MoveState moveState
        {
            get
            { return _moveState; }
            set
            { _moveState = value; }
        }

        private float runfactor;
        private MoveState _moveState;

        private float _useTime;

        private PlayerArm rightArm, leftArm;
        private PlayerLeg rightLeg, leftLeg;
        private HoldItem holdItem;

        public PlayerGraphics(Player player)
        {
            this.player = player;

            runfactor = 0f;

            entityParts.Add(new ZFlipEntityPart(player, "entityplayerhead", "entityplayerheadback"));
            entityParts.Add(new EntityPart(player, "entityplayerscarf"));
            entityParts.Add(new EntityPart(player, "entityplayerbody"));
            entityParts.Add(new EntityPart(player, "entityplayerleg1"));
            entityParts.Add(new EntityPart(player, "entityplayerleg1"));
            entityParts.Add(new EntityPart(player, "entityplayerleg2"));
            entityParts.Add(new EntityPart(player, "entityplayerleg2"));
            entityParts.Add(new EntityPart(player, "entityplayerarm"));
            entityParts.Add(new EntityPart(player, "entityplayerarm"));
            entityParts.Add(new EntityPart(player, "entityplayertail"));
            entityParts.Add(new EntityPart(player, null as FAtlasElement));

            GetEntityPart(PartType.Scarf).sortZOffset = 0.6f;
            GetEntityPart(PartType.Body).sortZOffset = -0.5f;
            GetEntityPart(PartType.RLeg).sortZOffset = 0.1f;
            GetEntityPart(PartType.LLeg).sortZOffset = 0.1f;
            GetEntityPart(PartType.RFoot).sortZOffset = 0.5f;
            GetEntityPart(PartType.LFoot).sortZOffset = 0.5f;
            GetEntityPart(PartType.Item).sortZOffset = 0.5f;
            GetEntityPart(PartType.Item).scale = Vector2.one * 0.5f;

            rightArm = new PlayerArm(this, GetEntityPart(PartType.RArm), true);
            leftArm = new PlayerArm(this, GetEntityPart(PartType.LArm), false);
            rightLeg = new PlayerLeg(this, GetEntityPart(PartType.RLeg), GetEntityPart(PartType.RFoot), true);
            leftLeg = new PlayerLeg(this, GetEntityPart(PartType.LLeg), GetEntityPart(PartType.LFoot), false);
            holdItem = new HoldItem(this, GetEntityPart(PartType.Item), GetEntityPart(PartType.RArm));
        }

        public void Update(float deltaTime)
        {
            Vector3 playerPosition = player.worldPosition;

            runfactor += new Vector2(player.velocity.x, player.velocity.z).magnitude * deltaTime * 1.45f;
            moveState = player.velocity.sqrMagnitude > 0.4f ? MoveState.Run : MoveState.Idle;
            useTime += deltaTime;

            Vector3 headPosition = playerPosition + CustomMath.HorizontalRotate(new Vector3((Mathf.Cos(runfactor * 2f) + 0.5f) * 0.1f, 1.91f + (Mathf.Sin(runfactor * -2f) + 1f) * 0.05f, 0f), player.viewAngle);

            GetEntityPart(PartType.Head).worldPosition = headPosition;
            GetEntityPart(PartType.Scarf).worldPosition = playerPosition + new Vector3(0f, 1.41f + Mathf.Sin(runfactor * 2f) * -0.05f, 0f);
            GetEntityPart(PartType.Body).worldPosition = playerPosition + CustomMath.HorizontalRotate(new Vector3(-0.1f, 1.08f + Mathf.Sin(runfactor * 2f) * -0.05f, 0f), player.viewAngle);
            GetEntityPart(PartType.Tail).worldPosition = playerPosition + CustomMath.HorizontalRotate(new Vector3(-0.3f, 0.75f + Mathf.Sin(runfactor * 2f) * -0.05f, Mathf.Sin(runfactor * 2f) * 0.05f), player.viewAngle);

            for (int index = 0; index < entityParts.Count; index++)
                player.entityParts[index].viewAngle = player.viewAngle;

            rightArm.Update(deltaTime);
            leftArm.Update(deltaTime);
            rightLeg.Update(deltaTime);
            leftLeg.Update(deltaTime);
            holdItem.Update(deltaTime);
        }

        private EntityPart GetEntityPart(PartType type)
        {
            return entityParts[(int)type];
        }

        public class AnimationPart
        {
            private PlayerGraphics _playerGraphics;
            public PlayerGraphics playerGraphics
            {
                get
                { return _playerGraphics; }
            }

            public Vector3 playerPosition
            {
                get
                { return playerGraphics.player.worldPosition; }
            }

            public float runFactor
            {
                get
                { return playerGraphics.runfactor; }
            }

            public float viewAngle
            {
                get
                { return playerGraphics.player.viewAngle; }
            }

            public AnimationPart(PlayerGraphics playerGraphics)
            {
                _playerGraphics = playerGraphics;
            }

            public virtual void Update(float deltaTime)
            {

            }
        }

        public class PlayerArm : AnimationPart
        {
            private EntityPart arm;

            private bool right;

            public PlayerArm(PlayerGraphics playerGraphics, EntityPart arm, bool right) : base(playerGraphics)
            {
                this.arm = arm;

                this.right = right;
            }

            public override void Update(float deltaTime)
            {
                switch (playerGraphics.moveState)
                {
                    case MoveState.Idle:
                        arm.worldPosition = playerPosition + CustomMath.HorizontalRotate(
                            new Vector3(0.1f, 0.8f, right ? -0.3f : 0.3f), viewAngle);

                        break;
                    case MoveState.Run:

                        arm.worldPosition = playerPosition + CustomMath.HorizontalRotate(
                            Vector3.Lerp(new Vector3(0.5f, 1.5f, right ? -0.2f : 0.2f), new Vector3(-0.3f, 0.7f, right ? -0.5f : 0.5f), (Mathf.Sin(runFactor + (right ? Mathf.PI : 0f)) + 1f) * 0.5f), viewAngle);
                        break;
                }
                if (right)
                    arm.worldPosition = arm.worldPosition + CustomMath.HorizontalRotate(Vector3.right * Mathf.Lerp(0.5f, 0f, CustomMath.Curve(playerGraphics.useTime * 3f, -2f)), viewAngle);

                arm.viewAngle = viewAngle + (right ? 30f : -30f);

                base.Update(deltaTime);
            }
        }

        public class HoldItem : AnimationPart
        {
            private EntityPart item;
            private EntityPart arm;

            public HoldItem(PlayerGraphics playerGraphics, EntityPart item, EntityPart arm) : base(playerGraphics)
            {
                this.item = item;
                this.arm = arm;
            }

            public override void Update(float deltaTime)
            {
                item.element = null;
                if (!playerGraphics.player.pickedItemContainer.blank)
                    item.element = playerGraphics.player.pickItemStack.item.element;
                item.worldPosition = arm.worldPosition;

                item.viewAngle = viewAngle;

                base.Update(deltaTime);
            }
        }

        public class PlayerLeg : AnimationPart
        {
            private EntityPart leg;
            private EntityPart foot;
            private bool right;

            public PlayerLeg(PlayerGraphics playerGraphics, EntityPart leg, EntityPart foot, bool right) : base(playerGraphics)
            {
                this.leg = leg;
                this.foot = foot;

                this.right = right;
            }

            public override void Update(float deltaTime)
            {
                switch (playerGraphics.moveState)
                {
                    case MoveState.Idle:

                        leg.worldPosition = playerPosition + CustomMath.HorizontalRotate(new Vector3(-0.05f, 0.58f, right ? -0.12f : 0.12f), viewAngle);
                        foot.worldPosition = playerPosition + CustomMath.HorizontalRotate(new Vector3(0f, 0.18f, right ? -0.16f : 0.16f), viewAngle);
                        break;

                    case MoveState.Run:

                        leg.worldPosition = playerPosition + CustomMath.HorizontalRotate(new Vector3(Mathf.Lerp(-0.1f, 0.125f, (Mathf.Sin(runFactor + (right ? Mathf.PI : 0f)) + 1f) * 0.5f), 0.58f, right ? -0.12f : 0.12f), viewAngle);

                        float footFactor = right ? runFactor + Mathf.PI : runFactor;

                        float footX = Mathf.Lerp(-0.4f, 0.5f, (Mathf.Sin(footFactor) + 1f) * 0.5f);
                        float footY;

                        if ((int)((footFactor + Mathf.PI / 2f) / Mathf.PI) % 2 > 0)
                            footY = Mathf.Max(Mathf.Lerp(-0.5f, 0.6f, (Mathf.Sin(-footFactor) + 1f) * 0.5f), 0.18f);
                        else
                            footY = Mathf.Lerp(0.3f, 0.6f, (Mathf.Sin(-footFactor) + 1f) * 0.5f);

                        foot.worldPosition = playerPosition + CustomMath.HorizontalRotate(new Vector3(footX, footY, right ? -0.16f : 0.16f), viewAngle);
                        break;
                }

                leg.viewAngle = viewAngle + (right ? -30f : 30f);
                foot.viewAngle = viewAngle + (right ? -30f : 30f);

                base.Update(deltaTime);
            }
        }
    }
}