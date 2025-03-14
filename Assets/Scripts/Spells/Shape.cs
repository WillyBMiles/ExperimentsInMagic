using System.Collections.Generic;
using UnityEngine;

public abstract class Shape : SpellPart
{
    CastInformation castInformation;
    protected CastInformation CastInformation => castInformation;

    public void Create(CastInformation castInformation)
    {
        this.castInformation = castInformation;
        InternalCreate();
    }

    protected abstract void InternalCreate();

    protected List<Effect> instantiatedEffects = new();

    //Please call this
    protected void Apply(CastInformation castInformation, 
        Vector3 position, Quaternion rotation, GameObject objectHit)
    {
        castInformation.effectValueHolder = castInformation.effectValueHolder.Duplicate();
        castInformation.spellValueHolder = castInformation.spellValueHolder.Duplicate();

        castInformation.position = position;
        castInformation.rotation = rotation;
        castInformation.objectHit = objectHit;
        foreach (var part in castInformation.casting.parts)
        {
            if (part is Effect effect)
            {
                var instantiatedEffect = GameObject.Instantiate(effect, position, rotation);
                instantiatedEffects.Add(instantiatedEffect);
                foreach (Modifier m in castInformation.casting.InstantiatedModifiers)
                {
                    m.OnEffectOccur(castInformation, effect);
                }
                instantiatedEffect.Create(castInformation);
            }
        }

        if (castInformation.casting.index < castInformation.casting.allCastings.Count - 1)
        {
            Casting nextCasting = castInformation.casting.allCastings[castInformation.casting.index + 1];
            SpellManager.Instance.StartCastCoroutine(nextCasting, castInformation);
        }
    }

    readonly List<MiniObject> minis = new();

    
    protected void InstantiateMiniObjects()
    {
        bool any = false;
        foreach (var part in castInformation.casting.parts)
        {
            if (part is Effect e)
            {
                MiniObject mini = 
                Instantiate(e.miniObject, transform);
                if (e.miniObjectGradient != null)
                {
                    mini.ApplyColor(e.miniObjectGradient);
                }
                minis.Add(mini);
                any = true;
            }
        }
        if (!any)
        {
            MiniObject mini = Instantiate(SpellManager.defaultMiniObjectPrefab, transform);
            minis.Add(mini);
        }
    }

    protected void DestroyThis()
    {
        foreach (var mini in minis)
        {
            mini.DestroyThis();
        }
        Destroy(gameObject);
    }
}

