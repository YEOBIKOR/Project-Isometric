using System.Collections.Generic;
using UnityEngine;
using Custom;
using Isometric.Items;
using Isometric.Interface;

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

        _inventory = new ItemContainer[36];

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

            inventory[index].SetItem(new ItemStack(items[index], 30));
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

        private AnimationRigBipedal _rig;

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

            _rig = new AnimationRigBipedal(body, head, rArm, lArm, rLeg, lLeg, rFoot, lFoot);

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
            entityParts.Add(new EntityPart(player, null as FAtlasElement));

            GetEntityPart(PartType.Scarf).sortZOffset = 0.6f;
            GetEntityPart(PartType.Body).sortZOffset = -0.5f;
            GetEntityPart(PartType.RLeg).sortZOffset = 0.1f;
            GetEntityPart(PartType.LLeg).sortZOffset = 0.1f;
            GetEntityPart(PartType.RFoot).sortZOffset = 0.5f;
            GetEntityPart(PartType.LFoot).sortZOffset = 0.5f;
            GetEntityPart(PartType.Item).sortZOffset = 0.5f;
            GetEntityPart(PartType.Item).scale = new Vector2(-1f, 1f);
        }

        public void Update(float deltaTime)
        {
            Vector3 playerPosition = player.worldPosition;

            _rig.worldPosition = playerPosition;
            _rig.viewAngle = player.viewAngle;
            _rig.moveSpeed = new Vector2(player.velocity.x, player.velocity.z).magnitude * deltaTime * 1.45f;
            _rig.landed = player.landed;

            GetEntityPart(PartType.Scarf).worldPosition = Vector3.Lerp(GetEntityPart(PartType.Body).worldPosition, GetEntityPart(PartType.Head).worldPosition, 0.3f);
            GetEntityPart(PartType.Tail).worldPosition = playerPosition + CustomMath.HorizontalRotate(new Vector3(-0.3f, 0.75f + Mathf.Sin(runfactor * 2f) * -0.05f, Mathf.Sin(runfactor * 2f) * 0.05f), player.viewAngle);

            for (int index = 0; index < entityParts.Count; index++)
                player.entityParts[index].viewAngle = player.viewAngle;
            
            _rig.Update(deltaTime);

            EntityPart item = GetEntityPart(PartType.Item);

            item.element = null;
            if (!player.pickedItemContainer.blank)
                item.element = player.pickItemStack.item.element;
            item.worldPosition = GetEntityPart(PartType.RArm).worldPosition;
        }

        private EntityPart GetEntityPart(PartType type)
        {
            return entityParts[(int)type];
        }
    }
}