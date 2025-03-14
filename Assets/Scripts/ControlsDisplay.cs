using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ControlsDisplay : MonoBehaviour
{
    static ControlsDisplay instance;
    TextMeshProUGUI text;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        instance = this;
    }

    public Dictionary<string, int> strings = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var key in strings.Keys.ToArray())
        {
            strings[key]--;
            if (strings[key] < 0)
            {
                strings.Remove(key);
            }
                
        }
        
        text.text = string.Join("\n", strings.Keys.OrderBy(s => s));
    }

    void LocalRegisterString(string s)
    {
        if (!strings.ContainsKey(s))
        {
            strings.Add(s,2);
        }
        else
            strings[s] = 2;

    }
    public static void RegisterString(string s)
    {
        instance.LocalRegisterString(s);
    }
    public static void RemoveString(string s)
    {
        if (instance.strings.ContainsKey(s))
        {
            instance.strings.Remove(s);
        }
    }
}
