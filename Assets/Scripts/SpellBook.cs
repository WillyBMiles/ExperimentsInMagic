using DG.Tweening;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

public class SpellBook : MonoBehaviour, IInteractiveObject
{
    public GlyphDisplay glyphDisplay;

    public GameObject openBook;
    public GameObject closedBook;
    public GameObject castParticle;

    public List<Renderer> materials = new();
    Rigidbody rb;


    float currentCooldown = 0f;
    Spell spell;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        glyphDisplay.gameObject.SetActive(false);
        rb = GetComponent<Rigidbody>();
    }

    int holding = 0;
    // Update is called once per frame
    void Update()
    {

        if (currentCooldown >= 0f)
            currentCooldown -= Time.deltaTime;

        if (spell != null)
        {
            foreach (var mat in materials)
            {
                mat.material.SetInt("_Glow", 1);
            }

        }
        else
        {
            foreach (var mat in materials)
            {
                mat.material.SetInt("_Glow", 0);
            }
        }


        if (holding < 0)
        {
            if (glyphDisplay.gameObject.activeSelf)
            {
                openBook.SetActive(true);
                closedBook.SetActive(false);
            }
            else
            {
                closedBook.SetActive(true);
                openBook.SetActive(false);
            }
        }
        holding--;
    }

    public void Interact(InteractionInfo interactionInfo = default)
    {
        glyphDisplay.gameObject.SetActive(!glyphDisplay.gameObject.activeSelf);
        if (!glyphDisplay.gameObject.activeSelf && glyphDisplay.AllGlyphs.Count > 0)
        {
            spell = GlyphManager.Instance.CalculateSpell(this);
            if (spell == null)
                return;
            StringBuilder builder = new();
            builder.AppendLine("Spell calculated! Part overview:");
            foreach (var part in spell.spellParts)
            {
                builder.AppendLine(part.name);
            }
            Debug.Log(builder.ToString());
            
           
        }

        if (glyphDisplay.gameObject.activeSelf)
        {
            AudioManager.Instance.PlaySound(.5f, transform.position, AudioManager.Sound.Open);
            openBook.SetActive(true);
            closedBook.SetActive(false);
            if (Vector3.Dot(openBook.transform.up, Vector3.up) < 0)
            {
                openBook.transform.rotation *= Quaternion.Euler(180f, 0f, 0f);
            }
        }
        else
        {
            AudioManager.Instance.PlaySound(.5f, transform.position, AudioManager.Sound.Close);
            closedBook.SetActive(true);
            openBook.SetActive(false);
        }
        rb.AddForceAtPosition(Vector3.up, rb.centerOfMass, ForceMode.Force);
    }

    public void Hover(InteractionInfo interactionInfo = default)
    {
        if (Input.GetKeyDown(KeyCode.Q) && !glyphDisplay.gameObject.activeSelf)
        {
            CastSpell();
        }
    }

    public void Hold()
    {
        
        if (!glyphDisplay.gameObject.activeSelf && spell != null)
        {
            if (currentCooldown > 0f)
            {
                ControlsDisplay.RegisterString("Spell on cooldown");
                ControlsDisplay.RemoveString("Q - Cast Spell");
            }
            else
            {
                ControlsDisplay.RegisterString("Q - Cast Spell");
                ControlsDisplay.RemoveString("Spell on cooldown");
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                CastSpell();
            }
        }

        if (ShouldOffset())
        {
            openBook.transform.localRotation = Quaternion.identity;
            closedBook.SetActive(false);
            openBook.SetActive(true);
            holding = 1;
        }
    }


    void CastSpell()
    {
        if (currentCooldown > 0f)
            return;
        if (spell != null)
        {
            AudioManager.Instance.PlaySound(.5f, transform.position, AudioManager.Sound.Cast);
            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() => Instantiate(castParticle, transform));
            sequence.AppendInterval(.25f);
            sequence.AppendCallback(() => SpellManager.Instance.CastSpell(spell, this));
            currentCooldown = 2.5f;
        }
        
    }

    public bool ShouldOffset()
    {
        return spell != null && !glyphDisplay.gameObject.activeSelf;
    }

    public string GetInteraction()
    {
        return glyphDisplay.gameObject.activeSelf ? "Close Book" : "Open Book";
    }

    public string GetHoldInteraction()
    {
        return "Pickup Book";
    }
}
