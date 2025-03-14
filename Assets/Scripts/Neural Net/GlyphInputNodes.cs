using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public abstract class GlyphInputNode : InputNode
{
    public Glyph assignedGlyph;

    protected double value = 0.0;
    public abstract void SetUp(
         GlyphDisplay glyphDisplay);

    public override double GetValue()
    {
        return value;
    }

    protected List<SingleGlyph> GetRelevantGlyphs(GlyphDisplay glyphDisplay)
    {
        return
        glyphDisplay.AllGlyphs
            .Where(sg => sg.Glyph == assignedGlyph).ToList();
    }

}

public class GlyphExistenceInputNode : GlyphInputNode
{
    public override void SetUp
        (GlyphDisplay glyphDisplay)
    {
        value = GetRelevantGlyphs(glyphDisplay).Count
                / 3.0;
    }
}

public class GlyphPositionInputNode : GlyphInputNode
{
    public enum Direction
    {
        X,
        Y
    }
    public Direction direction;
    
    public override void SetUp(GlyphDisplay glyphDisplay)
    {
        var list = GetRelevantGlyphs(glyphDisplay);
        if (list.Count == 0)
        {
            value = 0.0;
            return;
        }
            
        Func<SingleGlyph, float> selector;

        selector = direction switch
        {

            Direction.X =>
                        (SingleGlyph g) => g.currentVector.x,
            Direction.Y =>
                (SingleGlyph g) => g.currentVector.y,
            _ => null
        };
            

        double sum = list.Select(selector).Sum();
        value = sum / list.Count();
    }
}

public class GlyphAreaInputNode : GlyphInputNode
{
    public override void SetUp(GlyphDisplay glyphDisplay)
    {
        var list = GetRelevantGlyphs(glyphDisplay);
        if (list.Count == 2)
        {
            value = Vector2.Distance(list[0].currentVector, list[1].currentVector);
        }
        else if (list.Count == 3)
        {
            Vector2 A = list[0].currentVector;
            Vector2 B = list[1].currentVector;
            Vector2 C = list[2].currentVector;
            //Apparently this is how you find the area of a triangle
            value = Math.Abs(Vector3.Cross(A -B, A - C).z); 
        }
    }
}