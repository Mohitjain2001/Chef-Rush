using YesChef.Player;

namespace YesChef.Stations
{
    public interface IInteractable
    {
        void Interact(PlayerInteraction player);
        string GetInteractPrompt(PlayerInteraction player);
    }
}
