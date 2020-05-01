/// <summary>
/// interface that defines an object that npcs can interact
/// </summary>
public interface IAIInteractable
{
    /// <summary>
    /// How long should it take for an AI to interact with this?
    /// </summary>
    float AIInteractTime();
    /// <summary>
    /// Called when the AI has finished the interaction
    /// </summary>
    void AIFinishInteract();
    /// <summary>
    /// Called every frame during the interaction with the progress of the interaction
    /// If you see this line, this feature isn't actually implemented yet.
    /// </summary>
    void AIInteracting(float interactProgress);
    /// <summary>
    /// Returns true if this item needs interaction, false if the AI can ignore it
    /// </summary>
    bool NeedsInteraction();
    /// <summary>
    /// Returns a list of what types of AI should care about this object
    /// </summary>
    AIInterest[] InterestingToWhatAI();
}