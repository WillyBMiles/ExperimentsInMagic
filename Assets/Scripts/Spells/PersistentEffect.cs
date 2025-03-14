using UnityEngine;
using System;

public class PersistentEffect 
{
    public readonly float Duration;

    public readonly Func<Affected, bool> ShouldRemoveEarly = null;
    public readonly Action<Affected> OnApply = null;
    public readonly Action<Affected> OnRemove = null;
    public readonly Action<Affected> OnUpdate = null;

    public PersistentEffect(float Duration, PersistentEffectInfo persistentEffectInfo)
    {
        this.Duration = Duration;
        ShouldRemoveEarly = persistentEffectInfo.ShouldRemoveEarly;
        OnApply = persistentEffectInfo.OnApply;
        OnRemove = persistentEffectInfo.OnRemove;
        OnUpdate = persistentEffectInfo.OnUpdate;
    }

    public PersistentEffect Duplicate()
    {
        PersistentEffect effect = new(Duration, new()
        {
            OnApply = OnApply,
            OnRemove = OnRemove,
            OnUpdate = OnUpdate,
            ShouldRemoveEarly = ShouldRemoveEarly
        });

        return effect;
    }
}

public class PersistentEffectInfo
{
    public Func<Affected, bool> ShouldRemoveEarly = null;
    public Action<Affected> OnApply = null;
    public Action<Affected> OnRemove = null;
    public Action<Affected> OnUpdate = null;
}
