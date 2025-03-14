using System.Collections.Generic;
using UnityEngine;

public class Effect_Levitate : Effect
{
    protected override void InternalCreate()
    {
        float duration = GetDuration();
        PersistentEffect newEffect = new(duration,
            new()
            {
                OnApply = affected => 
                    affected.Rigidbody.AddForceAtPosition(Vector3.up *GetPower(),transform.position, ForceMode.VelocityChange),
                OnUpdate = affected => affected.Rigidbody.useGravity = false,
                OnRemove = affected => affected.Rigidbody.useGravity = true
            });

        AddPersistenteEffectToAffected(newEffect);
    }


}
