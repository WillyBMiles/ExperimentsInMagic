using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Effect : SpellPart
{
    [SerializeField]
    List<float> sizes = new() { 1,2,3,4,5};
    [SerializeField]
    bool useSizeForScale;
    [SerializeField]
    List<float> durations = new() { 1, 2, 3, 4, 5 };
    [SerializeField]
    List<float> powers = new() { 1, 2, 3, 4, 5 };



    [Tooltip("Used for projectiles etc")]
    public MiniObject miniObject;
    [Tooltip("Used for auras")]
    public MiniObject auraObject;

    public Gradient miniObjectGradient = null;
    public bool useGradientForAuraToo = false;

    CastInformation castInformation;
    protected CastInformation CastInformation => castInformation;

    [SerializeField]
    bool JustOneTarget = false;

    public void Create(CastInformation castInformation)
    {
        this.castInformation = castInformation;
        if (useSizeForScale)
        {
            float size = GetSize();
            transform.localScale = new Vector3(size,size,size);
        }

        InternalCreate();
    }
    protected abstract void InternalCreate();



    protected float GetSize()
    {
        return GetFromList(castInformation, sizes, EffectValue.Size);
    }

    protected float GetDuration()
    {
        return GetFromList(castInformation, durations, EffectValue.Duration);
    }

    protected float GetPower()
    {
        return GetFromList(castInformation, powers, EffectValue.Power);
    }

    float GetFromList(CastInformation castInformation, List<float> floats, EffectValue ev)
    {
        return floats[castInformation.effectValueHolder.PopNext(ev)];
    }

    public IEnumerable<Affected> GetAffected()
    {
        HashSet<Affected> affected = new();

        if (JustOneTarget && castInformation.objectHit != null && 
            castInformation.objectHit.TryGetComponent(out Affected aff))
        {
            affected.Add(aff);
            return affected;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.localScale.x);

        foreach (Affected a in colliders.Select(collider => collider.attachedRigidbody)
            .Where(rb => rb != null)
            .Select(rb => Affected.GetAffected(rb))
            .Where(a => a != null))
        {
            affected.Add(a);
        }
        if (affected.Count == 0)
            return affected;
        if (JustOneTarget)
        {
            Affected a = 
                affected.OrderBy(a => Vector3.Distance(a.transform.position, castInformation.position)).First();
            affected.Clear();
            affected.Add(a);
        }

        return affected;
    }

    protected void AffectAllAffected(Action<Affected> action)
    {
        var affected = GetAffected();
        Debug.Log($"Affecting...{affected.Count()} objects");
        foreach (Affected a in affected)
        {
            action?.Invoke(a);
        }
    }

    protected void AddPersistenteEffectToAffected(PersistentEffect persistentEffect, 
        bool useAuraObject = true)
    {
        AffectAllAffected((affected) =>
        {
            var thisPersistentEffect = persistentEffect.Duplicate();
            affected.AddPersistentEffect(persistentEffect,
                transform =>
                {
                    if (!useAuraObject)
                    {
                        return null;
                    }
                    MiniObject mo = Instantiate(auraObject, transform);
                    mo.transform.localScale = new Vector3(1f, 1f, 1f);
                    mo.transform.localPosition = new Vector3(0f, 0f, 0f);
                    if (miniObjectGradient != null && useGradientForAuraToo)
                    {
                        mo.ApplyColor(miniObjectGradient);
                    }
                    return mo;
                }
                
                );
        });
    }
}
