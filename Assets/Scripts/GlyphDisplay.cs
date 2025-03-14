using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GlyphDisplay : MonoBehaviour
{
    public SingleGlyph glyphPrefab;
    public Canvas canvas;

    List<SingleGlyph> singleGlyphs = new();
    public IReadOnlyList<SingleGlyph> AllGlyphs => singleGlyphs;
    public int UniqueGlyphs => AllGlyphs.GroupBy(sg => sg.Glyph).Count();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        singleGlyphs.RemoveAll(glyph => glyph == null);
    }

    public void AddToDisplay(Glyph glyph, Vector3 point)
    {
        if (singleGlyphs.Count(sg => sg.Glyph == glyph) >= 3) //Can only have 3
            return;

        SingleGlyph singleGlyph = Instantiate(glyphPrefab, canvas.transform);
        singleGlyph.SetGlyph(glyph);
        singleGlyph.transform.position = point;
        var l = singleGlyph.transform.localPosition;
        singleGlyph.transform.localPosition = new Vector3(l.x,l.y,0f);
        singleGlyphs.Add(singleGlyph);
    }

}
