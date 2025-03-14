using UnityEngine;
using System.Collections.Generic;

public class Effect_Fireball : Effect
{
    public List<float> persistentDamages = new();
    protected override void InternalCreate()
    {
        float power = GetPower();
        float duration = GetDuration();
        float persistent = persistentDamages[CastInformation.spellValueHolder.PopNext()];

        AddPersistenteEffectToAffected(new PersistentEffect(
            duration,
            new PersistentEffectInfo()
            {
                OnApply = affected => affected.TakeDamage(power /
                 Mathf.Max(1f, Vector3.Distance(affected.transform.position, transform.position) / 2f), "Explosion"
                 ),
                OnUpdate = affected => affected.TakeDamage(
                    ( persistent / duration) * Time.deltaTime,
                    "Being on Fire")
            }));
    }
}
