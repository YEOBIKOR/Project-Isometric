using UnityEngine;

public class TutorialNPC : EntityCreature
{
    private const string String = "W A S D : Move the character\nSpace : Jump the character\nQ, E : Move the camera\nEsc : Exit the game";

    public TutorialNPC() : base(1f, 2f, 100f)
    {
        _entityParts.Add(new EntityPart(this, "bordercollie"));
    }

    public override void Update(float deltaTime)
    {
        _entityParts[0].worldPosition = worldPosition + Vector3.up * 0.5f;
        _entityParts[0].viewAngle = viewAngle;

        base.Update(deltaTime);
    }

    public override void OnSpawn()
    {
        base.OnSpawn();

        HearAdvice();
    }

    public void HearAdvice()
    {
        world.cameraHUD.Speech(this, String);
    }
}
