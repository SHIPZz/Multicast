namespace CodeBase.UI.Game.Services
{
    public interface IClusterPlacementService
    {
        bool TryPlaceCluster(string clusterText, WordSlotHolder wordSlotHolder, int startIndex);
        void ResetCluster(string clusterText);
    }
}