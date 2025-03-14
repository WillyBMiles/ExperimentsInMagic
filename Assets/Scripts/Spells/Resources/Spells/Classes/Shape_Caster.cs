using UnityEngine;

public class Shape_Caster : Shape
{
    protected override void InternalCreate()
    {
        Apply(CastInformation, CastInformation.caster.transform.position,
            CastInformation.caster.transform.rotation, CastInformation.caster);
    }
}
