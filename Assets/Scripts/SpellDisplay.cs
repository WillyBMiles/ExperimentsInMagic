using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellDisplay : MonoBehaviour
{
    HashSet<string> earnedSpellParts = new();
    public int SpellsEarned => earnedSpellParts.Count;

    public static SpellDisplay Instance { get; private set; }
    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    TextMeshProUGUI newText;
    [SerializeField]
    Image xpBar;

    [SerializeField]
    CanvasGroup xpCanvas;

    [SerializeField]
    CanvasGroup newGlyphCanvas;

    [SerializeField]
    Image newGlpyhImage;
    [SerializeField]
    TextMeshProUGUI newGlyphText;


    public int xp;
    public int nextXp;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        text.DOFade(0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Sequence sequence;
    public void SpellCreated(List<SpellPart> parts)
    {
        HashSet<SpellPart> newParts = new();
        foreach (var part in parts)
        {
            if (!earnedSpellParts
                .Any(n => part.Name == n))
            {
                earnedSpellParts.Add(part.Name);
                newParts.Add(part);
            }
        }
       

        text.text = string.Join(", ", parts.Select(part => part.Name));
        if (newParts.Count > 0)
        {
            newText.text = "New parts:\n";
            newText.text += string.Join(", ", newParts.Select(part => part.Name));
            xp += newParts.Count();
            Sequence newSequence = DOTween.Sequence();
            newSequence.Append(xpCanvas.DOFade(1f, .2f));
            newSequence.Append(DOTween.To(() => xpBar.fillAmount, (f) => xpBar.fillAmount = f,
                (float) xp / (float) nextXp, 2f));
            newSequence.AppendCallback(() =>
            {
                if (xp >= nextXp)
                {
                    GetNewGlyph();
                }
            });
            newSequence.AppendInterval(3f);
            newSequence.Append(xpCanvas.DOFade(0f, 1f));


        }
        sequence?.Kill();
        sequence = DOTween.Sequence();
        sequence.Append(text.DOFade(1f, 1f));
        sequence.AppendInterval(5f);
        sequence.Append(text.DOFade(0f, 1f));
    }

    void GetNewGlyph()
    {
        xp -= nextXp;
        nextXp += 2;
        Glyph glyph= GlyphManager.Instance.AddNewGlyph();
        newGlpyhImage.sprite = glyph.sprite;
        newGlyphText.text = glyph.name;
        Sequence newSequence = DOTween.Sequence();
        newSequence.Append(newGlyphCanvas.DOFade(1f, .25f));
        newSequence.AppendInterval(3f);
        newSequence.Append(newGlyphCanvas.DOFade(0f, .25f));
        newSequence.AppendCallback(() => xpBar.fillAmount = 0f);
        
    }
}
