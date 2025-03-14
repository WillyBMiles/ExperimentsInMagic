using UnityEngine;

public class Effect_Duplicate : Effect
{
    protected override void InternalCreate()
    {
        AffectAllAffected(a =>
        {
            if (!a.isPlayer)
            {
                Instantiate(a, transform.position + Vector3.up, transform.rotation);

            }
        });
    }
}
