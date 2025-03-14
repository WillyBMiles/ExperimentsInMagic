using UnityEngine;

public class Shape_Default : Shape
{
    protected override void InternalCreate()
    {
        Apply(CastInformation, CastInformation.position, CastInformation.rotation, CastInformation.objectHit);
    }
}
