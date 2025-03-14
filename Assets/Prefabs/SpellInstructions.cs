using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using System;

public class SpellInstructions : MonoBehaviour
{
    public static SpellInstructions Instance { get; private set; }
    [SerializeField]
    CanvasGroup cg;

    [SerializeField]
    CanvasGroup extraInfo;

    [SerializeField]
    SingleSpellInstruction prefab;
    [SerializeField]
    Transform parent;
    List<SingleSpellInstruction> instances = new();
    Sequence sequence;


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        SpellManager.OnSpellCast += SpellCast;
    }

    void SpellCast(Spell s)
    {
        SpellManager.OnSpellCast -= SpellCast;
        extraInfo.DOFade(0f, 5f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        Calculate();
        sequence?.Kill();
        sequence = DOTween.Sequence();
        sequence.Append(cg.DOFade(1f, .5f));
    }

    public void Hide()
    {
        sequence?.Kill();
        sequence = DOTween.Sequence();
        sequence.Append(cg.DOFade(0f, .5f));
    }

    void Calculate()
    {
        foreach (var ins in instances)
        {
            Destroy(ins.gameObject);
        }
        instances.Clear();

        int i = 0;
        foreach (var availableGlyph in GlyphManager.Instance.currentGlyphs)
        {
            var single = Instantiate(prefab, parent);
            single.title.text = availableGlyph.name;
            single.key.text = GlyphManager.Instance.keyCodes[i].ToString();
            single.image.sprite = availableGlyph.sprite;

            instances.Add(single);
            i++;
        }
    }
}
