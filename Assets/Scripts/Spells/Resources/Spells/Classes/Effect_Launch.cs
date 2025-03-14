using UnityEngine;

public class Effect_Launch : Effect
{
    protected override void InternalCreate()
    {
        float power = GetPower();
        int dir = 
            CastInformation.spellValueHolder.PopNext();

        AffectAllAffected(affected =>
        {
            Vector3 direction = dir switch
            {
                0 or 1 => Vector3.up,
                _ => CastInformation.rotation * Vector3.forward,
            };

            InteractionManager.DropObj(affected.gameObject);

            affected.Rigidbody.AddForceAtPosition(direction * power,
                affected.Rigidbody.worldCenterOfMass, ForceMode.VelocityChange);
        });
    }
}
