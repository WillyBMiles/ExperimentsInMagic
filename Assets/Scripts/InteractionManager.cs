using System;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore;

public class InteractionManager : MonoBehaviour
{
    public float interactionDistance;
    public GameObject holdPosition;
    public Transform readyHoldPosition;
    Rigidbody holdingObject;
    IInteractiveObject holdingObjectInteractive;
    Rigidbody holdingObjectInteractiveRb;

    public Collider playerCollider;

    public static Action<GameObject> DropObj;

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GlyphManager.Instance.OnGlyphAdded += ClickedGlyph;
        DropObj += (obj) =>
        {
            if (holdingObject == obj)
                Drop();
            if (holdingObjectInteractive != null && holdingObjectInteractiveRb.gameObject == obj)
                DropOffset();

        };
    }

    // Update is called once per frame
    void Update()
    {
        ManagePickups();
        ManageInteractions();
    }

    bool dropped = false;
    void ManagePickups()
    {
        if (holdingObjectInteractive != null)
        {
            ControlsDisplay.RegisterString("E - Drop book");
            if (Input.GetKeyDown(KeyCode.E))
            {
                DropOffset();
                dropped = true;
            }


        }

        if (Physics.Raycast(transform.position, transform.forward, interactionDistance, LayerMask.GetMask("SpellDisplay")))
        {

            ControlsDisplay.RegisterString("Left Click - Move Glyph");
            ControlsDisplay.RegisterString("Right Click - Remove Glyph");
            SpellInstructions.Instance.Show();
        }
        else
        {
            SpellInstructions.Instance.Hide();
        }



        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionDistance, LayerMask.GetMask("Pickup")))
        {
            ControlsDisplay.RegisterString("Left Click (hold) - Move Object");
            if (Input.GetMouseButtonDown(0))
            {
                
                holdingObject = hit.rigidbody;
                holdingObject.useGravity = false;
                
                foreach (var collider in holdingObject.gameObject.GetComponentsInChildren<Collider>(true))
                {
                    Physics.IgnoreCollision(collider, playerCollider, true);
                }

               

            }
            var inter = hit.rigidbody.GetComponent<IInteractiveObject>();
            if (inter != null)
            {
                ControlsDisplay.RegisterString("Right Click - " + inter.GetInteraction());
                if (inter.ShouldOffset())
                {
                    ControlsDisplay.RegisterString("E - " + inter.GetHoldInteraction());
                    if (Input.GetKeyDown(KeyCode.E) && !dropped)
                    {

                        holdingObjectInteractive = inter;
                        holdingObjectInteractiveRb = hit.rigidbody;
                        foreach (var collider in holdingObjectInteractiveRb.gameObject.GetComponentsInChildren<Collider>(true))
                        {
                            collider.enabled = false;
                        }
                        holdingObjectInteractiveRb.useGravity = false;

                    }
                }

            }

            
        }

        if (Input.GetMouseButtonDown(0))
        {
            GlyphClickDown(false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            GlyphClickDown(true);
        }
        if (Input.GetMouseButton(0))
        {
            GlyphDragClickHeld();
        }
        if (Input.GetMouseButtonUp(0))
        {
            GlyphDragClickRelease();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Drop();
        }

        if (holdingObject != null)
        {
            var holdPos = holdPosition.transform.position;

            holdingObject.useGravity = false;
            float distance = Vector3.Distance(holdingObject.transform.position, holdPos);
            Vector3 targetVelocity = (holdPos - holdingObject.transform.position).normalized *
                 Mathf.Pow(distance * 5f, 2f);
            holdingObject.linearVelocity = Vector3.Lerp(holdingObject.linearVelocity, targetVelocity, Time.deltaTime * 7.5f);
        }


        dropped = false;
    }

    void DropOffset()
    {
        holdingObjectInteractiveRb.useGravity = true;
        holdingObjectInteractiveRb.ResetInertiaTensor();
        holdingObjectInteractiveRb.linearVelocity = new();
        holdingObjectInteractiveRb.transform.position = holdPosition.transform.position;

        foreach (var collider in holdingObjectInteractiveRb.gameObject.GetComponentsInChildren<Collider>(true))
        {
            collider.enabled = true;
        }
        holdingObjectInteractive = null;
    }
    private void LateUpdate()
    {
        if (holdingObjectInteractive != null)
        {
            holdingObjectInteractive.Hold();
            holdingObjectInteractiveRb.transform
                .SetPositionAndRotation(readyHoldPosition.transform.position, readyHoldPosition.transform.rotation);
        }
    }


    void Drop()
    {
        if (holdingObject != null)
        {
            holdingObject.useGravity = true;

            foreach (var collider in holdingObject.gameObject.GetComponentsInChildren<Collider>(true))
            {
                Physics.IgnoreCollision(collider, playerCollider, false);
            }
            holdingObject = null;
        }
    }
    void ManageInteractions()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionDistance, LayerMask.GetMask("Pickup")))
        {
            if (hit.collider.attachedRigidbody.gameObject.TryGetComponent(out IInteractiveObject obj))
            {
                obj.Hover();
                if (Input.GetMouseButtonDown(1))
                {
                    obj.Interact();
                }
                
            }
        }


    }

    void ClickedGlyph(Glyph glyph)
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionDistance, LayerMask.GetMask("SpellDisplay")))
        {
            if (hit.collider.gameObject.TryGetComponent(out GlyphDisplay glyphDisplay))
            {
                glyphDisplay.AddToDisplay(glyph, hit.point);
            }
        }
    }

    SingleGlyph movingGlyph = null;
    GlyphDisplay movingGlyphDisplay = null;
    const float MAX_CLICK_DISTANCE = .1f;
    void GlyphClickDown(bool delete)
    {
        
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionDistance, LayerMask.GetMask("SpellDisplay")))
        {
            if (hit.collider.gameObject.TryGetComponent(out GlyphDisplay glyphDisplay))
            {
                if (glyphDisplay.AllGlyphs.Count == 0)
                    return;
                SingleGlyph closestGlyph = 
                    glyphDisplay.AllGlyphs.OrderBy(glyph => Vector3.Distance(glyph.transform.position, hit.point)).First();
                
                if (Vector3.Distance(closestGlyph.transform.position, hit.point) < MAX_CLICK_DISTANCE)
                {
                    if (delete)
                    {
                        closestGlyph.DestroyThis();
                    }
                    else
                    {
                        movingGlyph = closestGlyph;
                        movingGlyphDisplay = glyphDisplay;
                    }
                }
            }
        }
    }
    void GlyphDragClickHeld()
    {
        if (movingGlyph == null)
            return;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionDistance, LayerMask.GetMask("SpellDisplay")))
        {
            if (hit.collider.gameObject.TryGetComponent(out GlyphDisplay glyphDisplay))
            { 
                if (glyphDisplay == movingGlyphDisplay)
                {
                    movingGlyph.transform.position = hit.point;
                    return;
                }
            }
        }
        movingGlyph = null;
        movingGlyphDisplay = null;
    }
    void GlyphDragClickRelease()
    {
        movingGlyph = null;
    }
}
