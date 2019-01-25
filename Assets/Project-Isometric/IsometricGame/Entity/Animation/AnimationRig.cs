using System;

public class AnimationRig <T> where T : AnimationRig <T>
{
    private AnimationState<T> _currentState;

    public AnimationRig()
    {

    }

    public virtual void Update(float deltaTime)
    {
        if (_currentState != null)
            _currentState.Update(this as T, deltaTime);
    }

    protected void ChangeState(AnimationState<T> newState)
    {
        if (_currentState == newState)
            return;

        if (_currentState != null)
            _currentState.End(this as T);
        _currentState = newState;

        _currentState.Start(this as T);
    }
}

public class AnimationState <T> where T : AnimationRig <T>
{
    public virtual void Start(T rig)
    {

    }

    public virtual void Update(T rig, float deltaTime)
    {

    }

    public virtual void End(T rig)
    {

    }
}
