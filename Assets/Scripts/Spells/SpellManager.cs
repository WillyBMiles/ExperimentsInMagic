using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[DefaultExecutionOrder(0)]
public class SpellManager : MonoBehaviour
{
    static SpellManager instance;
    public static SpellManager Instance => instance;

    public static event System.Action<Spell> OnSpellCast;

    [HideInInspector]
    readonly List<Effect> effects = new();
    [HideInInspector]
    readonly List<Modifier> modifiers = new();
    [HideInInspector]
    readonly List<Shape> shapes = new();
    public Shape DefaultShape;
    readonly Dictionary<EffectValue, List<SpellValue>> effectValues = new();
    readonly List<SpellValue> extraSpellValues = new();

    readonly Dictionary<Effect, OutputNode> effectNodes = new();
    readonly Dictionary<Shape, OutputNode> shapeNodes = new();
    readonly Dictionary<Modifier, OutputNode> modifierNodes = new();

    public List<SpellPart> overrideSpellParts = new();

    [SerializeField]
    MiniObject miniObjectPrefab;
    public static MiniObject defaultMiniObjectPrefab;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultMiniObjectPrefab = miniObjectPrefab;

        var spells = Resources.LoadAll("Spells/");
        foreach (var spellObject in spells)
        {
            if (spellObject is not GameObject)
                continue;
            if ((spellObject as GameObject).TryGetComponent<Effect>(out Effect effect))
            {
                effects.Add(effect);
            }
            if ((spellObject as GameObject).TryGetComponent<Modifier>(out Modifier modifier)) {
                modifiers.Add(modifier);
            }
            if ((spellObject as GameObject).TryGetComponent<Shape>(out Shape shape))
            {
                shapes.Add(shape);
            }
        }
        Debug.Log($"Found {effects.Count} effects, {modifiers.Count} modifiers, {shapes.Count} shapes");
        SetUpEffectValues();
        SetUpSpellValues();
        
    }

    public const int NUM_DIFFERENT_EFFECT_OUTPUTS = 3;


    void SetUpEffectValues()
    {
        foreach (var obj in System.Enum.GetValues(typeof(EffectValue)))
        {
            EffectValue effectValue = (EffectValue) obj;

            effectValues[effectValue] = new();
            for (int i = 0; i < NUM_DIFFERENT_EFFECT_OUTPUTS; i++)
            {
                effectValues[effectValue]
                    .Add(new SpellValue(NUM_SPELL_VALUE_LEVELS));
            }
        }
        
    }

    public const int NUM_EXTRA_SPELL_VALUES = 5;
    public const int NUM_SPELL_VALUE_LEVELS = 5;
    void SetUpSpellValues()
    {
        for (int i =0; i< NUM_EXTRA_SPELL_VALUES; i++)
        {
            extraSpellValues.Add( new SpellValue(NUM_SPELL_VALUE_LEVELS));
        }
    }

    public List<OutputNode> GetOutputNodes()
    {
        List<OutputNode> nodes = new();
        foreach (Effect effect in effects)
        {
            //Create nodes based on the effect
            effectNodes[effect] = new OutputNode();
        }
        foreach (Shape shape in shapes)
        {
            //Create nodes based on the effect
            shapeNodes[shape] = new OutputNode();
        }
        foreach (Modifier modifier in modifiers)
        {
            //Create nodes based on the effect
            modifierNodes[modifier] = new OutputNode();
        }

        foreach (List<SpellValue> valueList in effectValues.Values)
        {
            foreach (SpellValue spellValue in valueList)
            {
                nodes.AddRange(spellValue.GenerateOutputNodes());
            }
        }


        foreach (SpellValue spellValue in extraSpellValues)
        {
            nodes.AddRange(spellValue.GenerateOutputNodes());
        }

        nodes.AddRange(effectNodes.Select(kvp => kvp.Value));
        nodes.AddRange(shapeNodes.Select(kvp => kvp.Value));
        nodes.AddRange(modifierNodes.Select(kvp => kvp.Value));

        return nodes;
    }

    public Spell CalculateSpell(Dictionary<OutputNode, double> allOutputs, 
        int numberOfSpellParts)
    {
        Spell spell = new();

        //First calculate all effect values
        spell.spellValueHolder = new(extraSpellValues.Select(value => value.GetLevel(allOutputs)));
        Dictionary<EffectValue, SpellValueHolder> effectValueDict = new();
        foreach (var kvp in effectValues)
        {
            EffectValue ev = kvp.Key;
            List<SpellValue> spellValues = kvp.Value;

            SpellValueHolder holder = new(spellValues.Select(value => value.GetLevel(allOutputs)));
            effectValueDict[ev] = holder;
        }
        spell.effectValueHolder = new(effectValueDict);

        //FIGURE OUT THE EFFECTS
        //First plan...choose top effect
        IEnumerable<Effect> topEffects = effects.OrderByDescending(e => allOutputs[effectNodes[e]]);

        IEnumerable<OutputNode> 
            currentOutputNodesList = 
            allOutputs
            .Where(kvp => 
                effectNodes.Values.Contains(kvp.Key) 
                || modifierNodes.Values.Contains(kvp.Key) 
                || shapeNodes.Values.Contains(kvp.Key))
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => kvp.Key)
            .Take(numberOfSpellParts);
        

        
        List<SpellPart> finalSpellParts = new()
        {
            topEffects.First()
        };

        foreach (var node in currentOutputNodesList)
        {
            var part = GetFromNode(node);
            if (part == null)
                continue;

            if (finalSpellParts.Contains(part))
            {
                continue;
            }
            finalSpellParts.Add(part);
        }
        while (finalSpellParts.Last() is Shape s && finalSpellParts.Count(part => part is Shape) > 1)
        {
            finalSpellParts.Remove(s);
        }
#if UNITY_EDITOR
        if (overrideSpellParts.Count > 0)
        {
            spell.spellParts.AddRange(overrideSpellParts);
            return spell;
        }
            
        
#endif
        spell.spellParts.AddRange(finalSpellParts.Take(numberOfSpellParts));


        return spell;
        
        SpellPart GetFromNode(OutputNode outputNode)
        {
            if (effectNodes.ContainsValue(outputNode))
            {
                return effectNodes.First(kvp => kvp.Value == outputNode).Key;
            }
            if (shapeNodes.ContainsValue(outputNode))
            {
                return shapeNodes.First(kvp => kvp.Value == outputNode).Key;
            }
            if (modifierNodes.ContainsValue(outputNode))
            {
                return modifierNodes.First(kvp => kvp.Value == outputNode).Key;
            }
            return null;
        }
    }

    public void CastSpell(Spell spell, SpellBook book)
    {
        SpellDisplay.Instance.SpellCreated(spell.spellParts);
        spell.CastSpell(GenerateCastInformation(spell, book));
        OnSpellCast?.Invoke(spell);
    }

    CastInformation GenerateCastInformation(Spell spell, SpellBook book)
    {
        //TODO???
        return new CastInformation()
        {
            book = book,
            caster = FindFirstObjectByType<PlayerMovement>().gameObject,
            casterView = Camera.main.transform,
            position = book.transform.position,
            rotation = Camera.main.transform.rotation,
            objectHit = book.gameObject
        };
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCastCoroutine(Casting casting, CastInformation castInformation)
    {
        StartCoroutine(casting.Cast(castInformation));
    }

}

public class Spell
{
    public List<SpellPart> spellParts = new();
    public SpellValueHolder spellValueHolder;
    public EffectValueHolder effectValueHolder;

    public void CastSpell(CastInformation castInformation)
    {
        castInformation.spellValueHolder = spellValueHolder.Duplicate();
        castInformation.effectValueHolder = effectValueHolder.Duplicate();

        Shape firstshape = null;
        List<Casting> castings = new();
        Casting currentCasting = new();
        castings.Add(currentCasting);

        currentCasting.index = 0;
        
        foreach (var part in spellParts)
        {
            if (part is Shape shape)
            {
                if (firstshape == null)
                {
                    firstshape = shape;
                }
                else
                {
                    currentCasting = new Casting()
                    {
                        shape = shape,
                        index = currentCasting.index + 1
                    };
                }

                currentCasting.shape = shape;
            }
            else
            {
                currentCasting.parts.Add(part);
            }
        }
        
        foreach (Casting c in castings)
        {
            c.allCastings = castings;
        }

        SpellManager.Instance.StartCastCoroutine(castings.First(), castInformation);

    }


}

public class Casting
{
    public Shape shape;
    public List<SpellPart> parts = new();
    public int index;
    public IReadOnlyList<Casting> allCastings;

    readonly List<Modifier> instantiatedModifiers = new();
    public IReadOnlyList<Modifier> InstantiatedModifiers => instantiatedModifiers;
    Shape instantiatedShape;

    public IEnumerator Cast(CastInformation castInformation)
    {
        castInformation.casting = this;
        foreach (var part in parts)
        {
            if (part is Modifier modifierPrefab)
            {
                Modifier m = 
                    GameObject.Instantiate(modifierPrefab);
                bool shouldContinue = false;
                m.Create(castInformation, () => shouldContinue = true);
                yield return new WaitUntil(() => shouldContinue);

                instantiatedModifiers.Add(m);
            }
        }


        if (shape == null)
        {
            shape = SpellManager.Instance.DefaultShape;
        }
        instantiatedShape = GameObject.Instantiate(shape, castInformation.position, castInformation.rotation);
        instantiatedShape.Create(castInformation);
    }

}


public struct CastInformation {
    public SpellBook book;
    public GameObject caster;
    public Transform casterView;
    public Vector3 position;
    public Quaternion rotation;
    public GameObject objectHit;
    public EffectValueHolder effectValueHolder;
    public SpellValueHolder spellValueHolder;
    public Casting casting;
}
