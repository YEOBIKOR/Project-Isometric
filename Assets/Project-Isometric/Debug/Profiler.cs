using System;
using UnityEngine;

public class Profiler
{

}

public class WorldProfiler : Profiler
{
    private World _world;

    public UpdateProfiler updateProfiler { get; private set; }

    public WorldProfiler(World world)
    {
        _world = world;

        updateProfiler = new UpdateProfiler();
    }

    public class UpdateProfiler
    {
        private string[] _updateDebugNames;

        private FContainer _debuggerContainer;
        private FSprite[] _graph;
        private FLabel[] _nameLabel;
        private FLabel[] _infoLabel;
        private float[] _peak;

        private float lastTime;

        public UpdateProfiler()
        {
            _updateDebugNames = Enum.GetNames(typeof(UpdateProfilerType));

            _debuggerContainer = new FContainer();

            _graph = new FSprite[_updateDebugNames.Length];
            _nameLabel = new FLabel[_updateDebugNames.Length];
            _infoLabel = new FLabel[_updateDebugNames.Length];
            _peak = new float[_updateDebugNames.Length];

            for (int index = 0; index < _updateDebugNames.Length; index++)
            {
                _graph[index] = new FSprite("Futile_White");
                _nameLabel[index] = new FLabel("font", _updateDebugNames[index] + ": ");
                _infoLabel[index] = new FLabel("font", "ms");

                _graph[index].SetAnchor(new Vector2(0f, 0.5f));
                _graph[index].SetPosition(new Vector2(-Futile.screen.halfWidth + 80f, Futile.screen.halfHeight - 10f + index * -15f));
                _graph[index].scaleX = 0f;
                _graph[index].scaleY = 0.5f;

                _nameLabel[index].SetPosition(new Vector2(-Futile.screen.halfWidth + 5f, Futile.screen.halfHeight - 10f + index * -15f));
                _nameLabel[index].scale = 0.8f;
                _nameLabel[index].alignment = FLabelAlignment.Left;

                _infoLabel[index].SetPosition(_graph[index].GetPosition() + Vector2.right);
                _infoLabel[index].scale = 0.6f;
                _infoLabel[index].alignment = FLabelAlignment.Left;

                _debuggerContainer.AddChild(_graph[index]);
                _debuggerContainer.AddChild(_nameLabel[index]);
                _debuggerContainer.AddChild(_infoLabel[index]);

                _peak[index] = 0f;
            }
        }

        public void SwitchProfiler()
        {
            if (_debuggerContainer.container == null)
                Futile.stage.AddChild(_debuggerContainer);
            else
                _debuggerContainer.RemoveFromContainer();
        }

        public void StartMeasureTime()
        {
            lastTime = Time.realtimeSinceStartup;
        }

        public void MeasureTime(UpdateProfilerType type)
        {
            float ms = (Time.realtimeSinceStartup - lastTime) * 1000f;
            int index = (int)type;

            _peak[index] = _peak[index] < ms ? ms : Mathf.Lerp(_peak[index], ms, 0.1f);

            _graph[index].scaleX = _peak[index];
            _graph[index].color = Color.Lerp(Color.green, Color.red, _peak[index] * 0.1f);

            _infoLabel[index].text = string.Concat(ms.ToString("0.##"), "ms");

            StartMeasureTime();
        }

        public void CleanUp()
        {
            _debuggerContainer.RemoveFromContainer();
        }
    }
}

public enum UpdateProfilerType
{
    ChunkUpdate,
    RenderUpdate,
}