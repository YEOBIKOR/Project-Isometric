using UnityEngine;

public class TutorialNPC : EntityCreature
{
    private const string String = "W A S D : Move the character\nSpace : Jump the character\nQ, E : Move the camera\nEsc : Exit the game";

    public TutorialNPC() : base(1f, 2f, 100f)
    {

    }

    public override void OnSpawn(Chunk chunk, Vector3 position)
    {
        base.OnSpawn(chunk, position);

        HearAdvice();
    }

    public void HearAdvice()
    {
        world.cameraHUD.Speech(this, String);
    }
}
