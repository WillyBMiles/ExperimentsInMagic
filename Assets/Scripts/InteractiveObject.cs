using UnityEngine;

public interface IInteractiveObject 
{
    public void Interact(InteractionInfo interactionInfo = default);

    public void Hover(InteractionInfo interactionInfo = default);

    public void Hold();

    public bool ShouldOffset();

    public string GetInteraction();
    public string GetHoldInteraction();
}

public record InteractionInfo();
