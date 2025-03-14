using UnityEngine;

public class Effect_Heal : Effect
{
    protected override void InternalCreate()
    {
        float power = GetPower();
        float duration = GetDuration();
        AddPersistenteEffectToAffected(new(duration,
            new()
            {
                OnUpdate = (a) =>
                {
                    a.Heal(power / duration);
                }
            }));
    }
}
