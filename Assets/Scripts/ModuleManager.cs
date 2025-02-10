using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModuleManager : MonoBehaviour
{
	public static ModuleManager Instance;

	public GameObject modulesParent; 
	public GameObject placementIndicatorPrefab; 
	private List<GameObject> activeIndicators = new List<GameObject>();

	private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
    }
    else
    {
        Debug.LogError("Une autre instance de ModuleManager existe déjà");
        Destroy(gameObject);
        return;
    }

    if (transform.parent != null)
    {
        transform.SetParent(null);
    }
    DontDestroyOnLoad(gameObject);
}

private void Start()
{
	if (modulesParent.transform.childCount > 0)
    {
        GameObject firstModule = modulesParent.transform.GetChild(0).gameObject;
        // Correction de position forcée
        firstModule.transform.position = Vector3.zero;
    }
}
	// Place un nouveau module à côté d'un module existant
	public GameObject PlaceModule(Vector3 position, GameObject newModulePrefab)
{
    if (newModulePrefab == null)
    {
        Debug.LogError("Impossible de placer un module : prefab==null");
        return null;
    }

    Collider2D hit = Physics2D.OverlapPoint(position);
    if (hit != null && hit.GetComponent<ModuleType>() != null)
    {
        return null;
    }

    GameObject newModule = Instantiate(newModulePrefab, position, Quaternion.identity, modulesParent.transform);

    ModuleType moduleType = newModule.GetComponent<ModuleType>();
    if (moduleType != null)
    {
        HandleModuleType(moduleType.type);
    }
    
    return newModule;
}

	public void HandleModuleType(string type)
	{
		switch (type)
		{
			case "rocket":
				ResourceManager.Instance.ConsumeResource("Humans", 1);
				ResourceManager.Instance.AddResource("Rocket", 1);
				break;

			case "food_prod":
				ResourceManager.Instance.AddResource("Food", 5);
				break;

			case "oxygen_prod":
				ResourceManager.Instance.AddResource("Oxygen", 10);
				break;

			case "oxygen_tank":
				ResourceManager.Instance.AddResource("OxygenCapacity", 1000);
				break;

			default:
				Debug.LogWarning($"Type : {type} inconnu.");
				break;
		}
	}

	// Pos dispo 
	public List<Vector3> GetAvailablePositions(GameObject referenceModule)
{
    List<Vector3> availablePositions = new List<Vector3>();
    Vector2[] directions = {
        new Vector2(1, 0),  // Est
        new Vector2(-1, 0), // Ouest
        new Vector2(0, 1),  // Nord
        new Vector2(0, -1)  // Sud
    };

    SpriteRenderer referenceSpriteRenderer = referenceModule.GetComponent<SpriteRenderer>();

    if (referenceSpriteRenderer == null)
    {
        return availablePositions;
    }

    Vector2 referenceSize = referenceSpriteRenderer.bounds.size;

    foreach (Vector2 dir in directions)
    {
        Vector3 potentialPosition = new Vector3(
            referenceModule.transform.position.x + referenceSize.x * dir.x,
            referenceModule.transform.position.y + referenceSize.y * dir.y,
            referenceModule.transform.position.z
        );

        Collider2D hit = Physics2D.OverlapPoint(potentialPosition);
        if (hit == null || hit.GetComponent<ModuleManager>() == null)
        {
            availablePositions.Add(potentialPosition);
        }
    }

    return availablePositions;
}

	// affichage des emplacement dispo
	public void HighlightAvailablePositions()
	{
		ClearHighlights(); // Nettoie les indicateurs existants

		foreach (var module in modulesParent.GetComponentsInChildren<Transform>())
		{
			List<Vector3> availablePositions = GetAvailablePositions(module.gameObject);

			foreach (Vector3 position in availablePositions)
			{
				GameObject indicator = Instantiate(placementIndicatorPrefab, position, Quaternion.identity);
				activeIndicators.Add(indicator);
			}
		}
	}

	// clear les emplacement dispo
	public void ClearHighlights()
	{
		foreach (GameObject indicator in activeIndicators)
		{
			Destroy(indicator);
		}
		activeIndicators.Clear();
	}

public bool IsPositionOccupied(Vector3 position)
{
    foreach (Transform child in modulesParent.transform)
    {
        Vector3 modulePosition = child.position;

        if (Vector3.Distance(modulePosition, position) < 0.1f) 
        {
            return true;
        }
    }
    return false;
}
}