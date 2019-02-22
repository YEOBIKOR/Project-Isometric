using System;
using UnityEngine;
using Custom;

public class AnimationRigBipedal : AnimationRig <AnimationRigBipedal>
{
    private EntityPart _body;
    private EntityPart _head;
    private EntityPart _rArm;
    private EntityPart _lArm;
    private EntityPart _rLeg;
    private EntityPart _lLeg;
    private EntityPart _rFoot;
    private EntityPart _lFoot;

    public Vector3 worldPosition { get; set; }
    public float viewAngle { get; set; }
    public float moveSpeed { get; set; }
    public bool landed { get; set; }

    public AnimationRigBipedal(EntityPart body, EntityPart head, EntityPart rArm, EntityPart lArm, EntityPart rLeg, EntityPart lLeg, EntityPart rFoot, EntityPart lFoot) : base()
    {
        _body = body;
        _head = head;
        _rArm = rArm;
        _lArm = lArm;
        _rLeg = rLeg;
        _lLeg = lLeg;
        _rFoot = rFoot;
        _lFoot = lFoot;

        ChangeState(new StateIdle());
    }

    public override void Update(float deltaTime)
    {
        _body.viewAngle = viewAngle;
        _head.viewAngle = viewAngle;
        _rArm.viewAngle = viewAngle + 30f;
        _lArm.viewAngle = viewAngle - 30f;
        _rLeg.viewAngle = viewAngle - 30f;
        _lLeg.viewAngle = viewAngle + 30f;
        _rFoot.viewAngle = viewAngle - 30f;
        _lFoot.viewAngle = viewAngle + 30f;

        base.Update(deltaTime);
    }

    private class StateIdle : AnimationState <AnimationRigBipedal>
    {
        public StateIdle()
        {
            
        }

        public override void Start(AnimationRigBipedal rig)
        {

        }

        public override void Update(AnimationRigBipedal rig, float deltaTime)
        {
            rig._head.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(0f, 1.91f, 0f), rig.viewAngle);
            rig._body.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(-0.1f, 1.08f, 0f), rig.viewAngle);
            rig._rArm.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(0.1f, 0.8f, -0.3f), rig.viewAngle);
            rig._lArm.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(0.1f, 0.8f, 0.3f), rig.viewAngle);
            rig._rLeg.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(-0.05f, 0.58f, -0.12f), rig.viewAngle);
            rig._lLeg.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(-0.05f, 0.58f, 0.12f), rig.viewAngle);
            rig._rFoot.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(0f, 0.18f, -0.16f), rig.viewAngle);
            rig._lFoot.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(0f, 0.18f, 0.16f), rig.viewAngle);

            if (rig.moveSpeed > 0.01f)
                rig.ChangeState(new StateRun());
        }

        public override void End(AnimationRigBipedal rig)
        {

        }
    }

    private class StateRun : AnimationState <AnimationRigBipedal>
    {
        private float _runFactor;

        public StateRun()
        {
            _runFactor = 0f;
        }

        public override void Start(AnimationRigBipedal rig)
        {

        }

        public override void Update(AnimationRigBipedal rig, float deltaTime)
        {
            _runFactor += rig.moveSpeed;

            Vector3 headPosition = CustomMath.HorizontalRotate(new Vector3((Mathf.Cos(_runFactor * 2f) + 0.5f) * 0.1f, 1.91f + (Mathf.Sin(_runFactor * -2f) + 1f) * 0.05f, 0f), rig.viewAngle);

            rig._head.worldPosition = rig.worldPosition + headPosition;
            rig._body.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(-0.1f, 1.08f + Mathf.Sin(_runFactor * 2f) * -0.05f, 0f), rig.viewAngle);
            
            rig._rArm.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(Vector3.Lerp(new Vector3(0.5f, 1.5f, -0.2f), new Vector3(-0.3f, 0.7f, -0.5f), (Mathf.Sin(_runFactor + Mathf.PI) + 1f) * 0.5f), rig.viewAngle);
            rig._lArm.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(Vector3.Lerp(new Vector3(0.5f, 1.5f, 0.2f), new Vector3(-0.3f, 0.7f, 0.5f), (Mathf.Sin(_runFactor) + 1f) * 0.5f), rig.viewAngle);

            rig._rLeg.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(Mathf.Lerp(-0.1f, 0.125f, (Mathf.Sin(_runFactor + Mathf.PI) + 1f) * 0.5f), 0.58f, -0.12f), rig.viewAngle);
            rig._lLeg.worldPosition = rig.worldPosition + CustomMath.HorizontalRotate(new Vector3(Mathf.Lerp(-0.1f, 0.125f, (Mathf.Sin(_runFactor) + 1f) * 0.5f), 0.58f, 0.12f), rig.viewAngle);

            rig._rFoot.worldPosition = rig.worldPosition + GetFootPosition(rig, true);
            rig._lFoot.worldPosition = rig.worldPosition + GetFootPosition(rig, false);
            
            if (rig.moveSpeed < 0.01f)
                rig.ChangeState(new StateIdle());
        }

        public override void End(AnimationRigBipedal rig)
        {

        }

        private Vector3 GetFootPosition(AnimationRigBipedal rig, bool right)
        {
            float footFactor = right ? _runFactor + Mathf.PI : _runFactor;

            float footX = Mathf.Lerp(-0.4f, 0.5f, (Mathf.Sin(footFactor) + 1f) * 0.5f);
            float footY;

            if ((int)((footFactor + Mathf.PI / 2f) / Mathf.PI) % 2 > 0)
                footY = Mathf.Max(Mathf.Lerp(-0.5f, 0.6f, (Mathf.Sin(-footFactor) + 1f) * 0.5f), 0.18f);
            else
                footY = Mathf.Lerp(0.3f, 0.6f, (Mathf.Sin(-footFactor) + 1f) * 0.5f);

            return CustomMath.HorizontalRotate(new Vector3(footX, footY, right ? -0.16f : 0.16f), rig.viewAngle);
        }
    }
}