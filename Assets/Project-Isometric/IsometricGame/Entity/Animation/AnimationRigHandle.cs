using UnityEngine;
using System.Collections;
using Custom;

public enum HoldType
{
    None,
    OneHand,
    TwoHands,
    Block
}

public class AnimationRigHandle : AnimationRig <AnimationRigHandle>
{
    private EntityPart _rArm;
    private EntityPart _lArm;
    private EntityPart _item;

    public Vector3 worldPosition { get; set; }
    public float viewAngle { get; set; }
    public float cameraViewAngle { get; set; }

    private AnimationState<AnimationRigHandle>[] _states;

    public AnimationRigHandle(EntityPart rArm, EntityPart lArm, EntityPart item) : base()
    {
        _rArm = rArm;
        _lArm = lArm;
        _item = item;

        _states = new AnimationState<AnimationRigHandle>[]
        {
           null,
           new StateHold1Hand(),
           new StateHold2Hands(),
           new StateHoldBlock()
        };
    }

    public void ChangeHoldState(HoldType type)
    {
        ChangeState(_states[(int)type]);
    }

    private class StateHold1Hand : AnimationState<AnimationRigHandle>
    {
        public override void Start(AnimationRigHandle rig)
        {
            rig._item.doesFlip = false;
        }

        public override void Update(AnimationRigHandle rig, float deltaTime)
        {
            rig._rArm.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(0.5f, 1.2f, -0.2f), rig.viewAngle);
            rig._lArm.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(0.3f, 1.2f, 0.2f), rig.viewAngle);
            rig._item.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(0.7f, 1.3f, 0f), rig.viewAngle);

            rig._item.rotation = 90f - rig.viewAngle + rig.cameraViewAngle;
            rig._item.scale = new Vector2(-1f, rig._item.rotation < 90f ? 1f : -1f);
        }

        public override void End(AnimationRigHandle rig)
        {
            rig._item.rotation = 0f;
            rig._item.scale = Vector2.one;
        }
    }

    private class StateHold2Hands : AnimationState<AnimationRigHandle>
    {
        public override void Start(AnimationRigHandle rig)
        {
            rig._item.doesFlip = false;
        }

        public override void Update(AnimationRigHandle rig, float deltaTime)
        {
            rig._rArm.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(0.5f, 1.0f, -0.2f), rig.viewAngle);
            rig._lArm.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(0.3f, 1.0f, 0.2f), rig.viewAngle);
            rig._item.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(0.7f, 1.3f, 0f), rig.viewAngle);

            rig._item.rotation = 90f - rig.viewAngle + rig.cameraViewAngle;
            rig._item.scale = new Vector2(-1f, rig._item.rotation < 90f ? 1f : -1f);
        }

        public override void End(AnimationRigHandle rig)
        {
            rig._item.rotation = 0f;
            rig._item.scale = Vector2.one;
        }
    }

    private class StateHoldBlock : AnimationState <AnimationRigHandle>
    {
        public override void Start(AnimationRigHandle rig)
        {

        }

        public override void Update(AnimationRigHandle rig, float deltaTime)
        {
            rig._rArm.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(0.3f, 1.0f, -0.5f), rig.viewAngle);
            rig._lArm.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(0.3f, 1.0f, 0.5f), rig.viewAngle);
            rig._item.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(0.7f, 1.3f, 0f), rig.viewAngle);
        }

        public override void End(AnimationRigHandle rig)
        {

        }
    }
}
