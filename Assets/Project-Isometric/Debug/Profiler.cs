using System;
using UnityEngine;

public class Profiler
{

}

public class WorldProfiler : Profiler
{
    private World world;

    public UpdateProfiler updateProfiler { get; private set; }

    public WorldProfiler(World world)
    {
        this.world = world;

        updateProfiler = new UpdateProfiler();
    }

    public class UpdateProfiler
    {
        private string[] updateDebugNames;

        private FContainer debuggerContainer;
        private FSprite[] graph;
        private FLabel[] nameLabel;
        private FLabel[] infoLabel;
        private float[] peak;

        private float lastTime;

        public UpdateProfiler()
        {
            updateDebugNames = Enum.GetNames(typeof(UpdateProfilerType));

            debuggerContainer = new FContainer();

            graph = new FSprite[updateDebugNames.Length];
            nameLabel = new FLabel[updateDebugNames.Length];
            infoLabel = new FLabel[updateDebugNames.Length];
            peak = new float[updateDebugNames.Length];

            for (int index = 0; index < updateDebugNames.Length; index++)
            {
                graph[index] = new FSprite("Futile_White");
                nameLabel[index] = new FLabel("font", updateDebugNames[index] + ": ");
                infoLabel[index] = new FLabel("font", "ms");

                graph[index].SetAnchor(new Vector2(0f, 0.5f));
                graph[index].SetPosition(new Vector2(-Futile.screen.halfWidth + 80f, Futile.screen.halfHeight - 10f + index * -15f));
                graph[index].scaleX = 0f;
                graph[index].scaleY = 0.5f;

                nameLabel[index].SetPosition(new Vector2(-Futile.screen.halfWidth + 5f, Futile.screen.halfHeight - 10f + index * -15f));
                nameLabel[index].scale = 0.8f;
                nameLabel[index].alignment = FLabelAlignment.Left;

                infoLabel[index].SetPosition(graph[index].GetPosition() + Vector2.right);
                infoLabel[index].scale = 0.6f;
                infoLabel[index].alignment = FLabelAlignment.Left;

                debuggerContainer.AddChild(graph[index]);
                debuggerContainer.AddChild(nameLabel[index]);
                debuggerContainer.AddChild(infoLabel[index]);

                peak[index] = 0f;
            }
        }

        public void SwitchProfiler()
        {
            if (debuggerContainer.container == null)
                Futile.stage.AddChild(debuggerContainer);
            else
                debuggerContainer.RemoveFromContainer();
        }

        public void StartMeasureTime()
        {
            lastTime = Time.realtimeSinceStartup;
        }

        public void MeasureTime(UpdateProfilerType type)
        {
            float ms = (Time.realtimeSinceStartup - lastTime) * 1000f;
            int index = (int)type;

            peak[index] = peak[index] < ms ? ms : Mathf.Lerp(peak[index], ms, 0.1f);

            graph[index].scaleX = peak[index];
            graph[index].color = Color.Lerp(Color.green, Color.red, peak[index] * 0.1f);

            infoLabel[index].text = string.Concat(ms.ToString("0.##"), "ms");

            StartMeasureTime();
        }

        public void CleanUp()
        {
            debuggerContainer.RemoveFromContainer();
        }
    }
}

public enum UpdateProfilerType
{
    ChunkUpdate,
    RenderUpdate,
}