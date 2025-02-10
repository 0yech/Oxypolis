using UnityEngine;

public class PlacementIndicator : MonoBehaviour
{
     private BuildMenuManager buildMenuManager;
    private Vector3 placementPosition;

    public void SetPlacementPosition(Vector3 position, BuildMenuManager manager)
    {
        placementPosition = position;
        buildMenuManager = manager;
    }

    private void OnMouseDown()
    {
        if (buildMenuManager != null)
        {
            buildMenuManager.PlaceModule(placementPosition);
        }
        else
        {
            Debug.LogError("ERREUR : `BuildMenuManager` est absent");
        }
    }
}