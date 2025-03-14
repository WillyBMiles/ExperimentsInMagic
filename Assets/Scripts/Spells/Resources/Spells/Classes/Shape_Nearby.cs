using System.Collections.Generic;
using UnityEngine;

public class Shape_Nearby : Shape
{

    public List<float> ranges = new();
    protected override void InternalCreate()
    {
        float r = ranges[CastInformation.spellValueHolder.PopNext()];
        foreach (var obj in Physics.OverlapSphere(transform.position, r))
        {
            if (obj.TryGetComponent<Affected>(out _))
            {
                Apply(CastInformation, obj.transform.position, obj.transform.rotation,
                    obj.attachedRigidbody.gameObject);
            }
            
        }

        
    }
}
