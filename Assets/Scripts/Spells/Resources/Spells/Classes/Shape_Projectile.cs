using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Shape_Projectile : Shape
{
    [SerializeField]
    List<float> speeds = new();
    [SerializeField]
    bool overrideSpeed = false;
    [SerializeField]
    float overrideSpeedValue;

    [SerializeField]
    bool pierce = false;

    float speed; 
    protected override void InternalCreate()
    {
        InstantiateMiniObjects();
        if (overrideSpeed)
            speed = overrideSpeedValue;
        else 
            speed = speeds[CastInformation.spellValueHolder.PopNext()];
        transform.rotation = CastInformation.casterView.transform.rotation;
        
    }

    private void Update()
    {
        transform.position += speed * Time.deltaTime * transform.forward;
    }
    bool destroyed = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (destroyed)
            return;
        var other = collision.collider;

        if (other.attachedRigidbody != null)
        {
            Debug.Log("________");
            Debug.Log(other.attachedRigidbody.gameObject);
            Debug.Log(CastInformation.book.gameObject);
            Debug.Log(CastInformation.book.gameObject == other.attachedRigidbody.gameObject);
        }

        if (other.attachedRigidbody == null || (
                other.attachedRigidbody.gameObject != CastInformation.book.gameObject
                && other.attachedRigidbody.gameObject != CastInformation.caster))
        {
            GameObject go = other.attachedRigidbody == null ? other.gameObject :
                other.attachedRigidbody.gameObject;
            Apply(CastInformation, transform.position, transform.rotation, go);
            if (!pierce)
            {
                destroyed = true;
                DestroyThis();
            }
                
        }
    }
}
