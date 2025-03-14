using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EffectValue
{
    Size,
    Power,
    Duration,
}

public class SpellValue
{
    public int NumberOfLevels { get; protected set; }

    public SpellValue(int numLevels)
    {
        NumberOfLevels = numLevels;
    }

    List<OutputNode> storedOutputNodes = new();
    public IReadOnlyList<OutputNode> StoredOutputNodes => storedOutputNodes;

    public IReadOnlyList<OutputNode> GenerateOutputNodes()
    {
        storedOutputNodes.Clear();
        for (int i = 0; i < NumberOfLevels; i++)
        {
            storedOutputNodes.Add(new OutputNode());
        }
        return storedOutputNodes;

    }

    public int GetLevel(Dictionary<OutputNode, double> outputs)
    {
        var node = outputs
            .Where(kvp => storedOutputNodes.Contains(kvp.Key))
            .OrderByDescending(kvp => kvp.Value)
            .First().Key;
        return storedOutputNodes.IndexOf(node);
    }

}

public class SpellValueHolder
{
    readonly List<int> originalValues = new();
    readonly List<int> currentStack = new();

    public int PopNext()
    {
        if (currentStack.Count == 0)
        {
            currentStack.AddRange(originalValues);
        }

        int nextValue = currentStack[0];
        currentStack.RemoveAt(0);

        return nextValue;
    }

    public SpellValueHolder(IEnumerable<int> inputs)
    {
        originalValues.AddRange(inputs);
        currentStack.AddRange(inputs);
    }
   SpellValueHolder(IEnumerable<int> inputs, IEnumerable<int> currentStack)
    {
        originalValues.AddRange(inputs);
        this.currentStack.AddRange(currentStack);
    }

    public SpellValueHolder Duplicate()
    {
        return new(originalValues, currentStack);
    }
}

public class EffectValueHolder
{
    readonly Dictionary<EffectValue, SpellValueHolder> spellValuesHolders = new();

    public EffectValueHolder(Dictionary<EffectValue, SpellValueHolder> spellValuesHolders)
    {
        foreach (var kvp in spellValuesHolders)
        {
            this.spellValuesHolders.Add(kvp.Key, kvp.Value);
        }
    }

    public int PopNext(EffectValue effectValue)
    {
        return spellValuesHolders[effectValue].PopNext();
    }

    public EffectValueHolder Duplicate()
    {
        return new(spellValuesHolders.Select(
            kvp => new KeyValuePair<EffectValue, SpellValueHolder>(kvp.Key, kvp.Value.Duplicate()))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
    }
}