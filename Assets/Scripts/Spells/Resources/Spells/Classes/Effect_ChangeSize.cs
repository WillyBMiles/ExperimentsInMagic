using DG.Tweening;
using UnityEngine;

public class Effect_ChangeSize : Effect
{
    protected override void InternalCreate()
    {
        float power = GetPower();
        float duration = GetDuration();

        AffectAllAffected(a =>
        {
            a.transform.DOScale(a.transform.localScale * power, duration);
        });
    }
}
