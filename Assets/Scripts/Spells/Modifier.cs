using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Modifier : SpellPart
{
    public abstract void Create(CastInformation castInformation, Action onComplete);

    public abstract void OnEffectOccur(CastInformation castInformation, Effect effect);
}
