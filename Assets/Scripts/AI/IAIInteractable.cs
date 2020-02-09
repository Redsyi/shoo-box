public interface IAIInteractable
{
    float AIInteractTime();
    void AIFinishInteract();
    void AIInteracting(float interactProgress);
    bool NeedsInteraction();
}