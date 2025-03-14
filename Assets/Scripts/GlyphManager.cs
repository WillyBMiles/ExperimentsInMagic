using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static UnityEngine.EventSystems.StandaloneInputModule;

[DefaultExecutionOrder(100)]
public class GlyphManager : MonoBehaviour
{
    static GlyphManager instance;
    public static GlyphManager Instance => instance;

    public int hiddenLayers;

    List<GlyphInputNode> glyphInputNodes = new();
    public List<Glyph> glyphs = new();
    public List<KeyCode> keyCodes = new();

    public int startingGlyphs;
    [HideInInspector]
    public List<Glyph> currentGlyphs = new();

    public event Action<Glyph> OnGlyphAdded;
    
    NeuralNet net;


    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateWeights();
        for (int i =0;  i < startingGlyphs; i++)
        {
            AddNewGlyph();
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i=0; i < currentGlyphs.Count; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                OnGlyphAdded(currentGlyphs[i]);
            }
        }

    }

    public Glyph AddNewGlyph()
    {
        var available = glyphs.Where(g => !currentGlyphs.Contains(g));
        if (!available.Any())
            return null;
        var glyph = available.Skip(UnityEngine.Random.Range(0, available.Count())).First();
        currentGlyphs.Add(glyph);
        return glyph;
    }


    public void GenerateWeights()
    {
        net = new();
        net.CreateNet(GenerateInputNodes(), GenerateOutputNodes(), hiddenLayers);
    }

    List<InputNode> GenerateInputNodes()
    {
        
        List<InputNode> inputNodes = new();
        glyphs = glyphs.OrderBy((_) => UnityEngine.Random.value).ToList();
        foreach (var glyph in glyphs)
        {
            var node = new GlyphExistenceInputNode() { assignedGlyph = glyph };
            var nodeX = new GlyphPositionInputNode()
            { direction = GlyphPositionInputNode.Direction.X, assignedGlyph = glyph };
            var nodeY = new GlyphPositionInputNode()
            { direction = GlyphPositionInputNode.Direction.Y, assignedGlyph = glyph };
            var nodeA = new GlyphAreaInputNode() { assignedGlyph = glyph };
            inputNodes.Add(node);
            glyphInputNodes.Add(node);
            inputNodes.Add(nodeX);
            glyphInputNodes.Add(nodeX);
            inputNodes.Add(nodeY);
            glyphInputNodes.Add(nodeY);
            inputNodes.Add(nodeA);
            glyphInputNodes.Add(nodeA);
        }

        Debug.Log($"Created {inputNodes.Count} input nodes.");
        return inputNodes;
    }
    List<OutputNode> GenerateOutputNodes()
    {
        //For each effect, modifier, target, value, generate an output
        var outputNodes = SpellManager.Instance.GetOutputNodes();
        Debug.Log($"Created {outputNodes.Count} output nodes.");
        return outputNodes;
    }


    public Spell CalculateSpell(SpellBook spellBook)
    {
        if (spellBook.glyphDisplay.UniqueGlyphs == 0)
        {
            return null;
        }
        foreach (var node in glyphInputNodes)
        {
            node.SetUp(spellBook.glyphDisplay);
        }
        var output = net.GenerateOutput();
        Debug.Log($"Generated {output.Count} output node values");
        return SpellManager.Instance
            .CalculateSpell(output, spellBook.glyphDisplay.UniqueGlyphs);
    }

}

[Serializable]
public class Glyph
{
    public string name;
    public Sprite sprite;
}
