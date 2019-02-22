using System;

public class AnimationRig <Rig> where Rig : AnimationRig <Rig>
{
    private AnimationState<Rig> _currentState;

    public AnimationRig()
    {

    }

    public virtual void Update(float deltaTime)
    {
        if (_currentState != null)
            _currentState.Update(this as Rig, deltaTime);
    }

    protected void ChangeState(AnimationState<Rig> newState)
    {
        if (_currentState == newState)
            return;

        if (_currentState != null)
            _currentState.End(this as Rig);
        _currentState = newState;

        if (_currentState != null)
            _currentState.Start(this as Rig);
    }
}

public abstract class AnimationState <T> where T : AnimationRig <T>
{
    public abstract void Start(T rig);

    public abstract void Update(T rig, float deltaTime);

    public abstract void End(T rig);
}
