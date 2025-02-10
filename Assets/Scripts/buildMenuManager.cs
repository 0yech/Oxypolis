using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildMenuManager : MonoBehaviour
{
	// b_module_prefab à réactiver quand on aura plus de sprites/modules
	// public Button b_module_prefab;
	public List<Button> b_module_list;
	public Vector2 first_b_module_pos;
	public float space_between_modules;
	public float nbr_of_modules_per_line;

	private ModuleManager moduleManager;
	private GameObject selectedModulePrefab;
	private List<GameObject> placementIndicators = new List<GameObject>();

	public GameObject buildMenu;
	public GameObject mainUI;
	private GameObject referenceModule;


	private void Awake()
	{
		foreach (Button b_module in b_module_list)
		{
			b_module.onClick.AddListener(() => SelectModule(b_module.gameObject));
		}
	}

	private void Start()
	{
		moduleManager = Object.FindFirstObjectByType<ModuleManager>();
		if (moduleManager == null)
		{
			Debug.LogError("ModuleManager introuvable. Assurez-vous qu'il est présent dans la scène.");
			return;
		}

		if (moduleManager.modulesParent.transform.childCount > 0)
		{
			referenceModule = moduleManager.modulesParent.transform.GetChild(0).gameObject;
		}
		else
		{
			Debug.LogWarning("Aucun module de départ trouvé. Assurez-vous que le prefab Oxygen est bien placé dans la scène.");
		}
		// Désactivation de l'instanciation dynamique `b_module_prefab` à remettre quand on aura plus de sprites/modules
		/*
        for (int i = 0; i < b_module_list.Count; i++)
        {
            Button b_module = Instantiate(b_module_prefab, transform);
            b_module.transform.position = new Vector3(
                first_b_module_pos.x + (i % nbr_of_modules_per_line) * space_between_modules,
                first_b_module_pos.y - (i / nbr_of_modules_per_line) * space_between_modules,
                0
            );
            b_module.GetComponent<Image>().sprite = b_module_list[i].GetComponent<Image>().sprite;

            int index = i;
            b_module.onClick.AddListener(() => SelectModule(b_module_list[index].gameObject));
        }
        */
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
		{
			if (!buildMenu.activeSelf)
			{
				OpenBuildMenu();
			}
			else
			{
				CloseBuildMenu();
			}
		}
	}

	public void OpenBuildMenu()
	{
		buildMenu.SetActive(true);
		mainUI.SetActive(false);
	}

	public void CloseBuildMenu()
	{
		buildMenu.SetActive(false);
		mainUI.SetActive(true);
		selectedModulePrefab = null;
		ClearPlacementIndicators();
	}

	public void SelectModule(GameObject buttonObject)
	{
		if (buttonObject == null)
		{
			Debug.LogError("ERREUR : `buttonObject` est NULL !");
			return;
		}


		ModuleButton moduleButton = buttonObject.GetComponent<ModuleButton>();

		if (moduleButton == null)
		{
			return;
		}

		if (moduleButton.modulePrefab == null)
		{
			return;
		}

		selectedModulePrefab = moduleButton.modulePrefab;

		ShowPlacementIndicators();
	}

	private void ShowPlacementIndicators()
	{
		ClearPlacementIndicators();

		// Vérification des références
		if (moduleManager == null)
		{
			return;
		}
		if (moduleManager.placementIndicatorPrefab == null)
		{
			return;
		}

		foreach (Transform module in moduleManager.modulesParent.transform)
		{
			Vector3 referencePosition = module.position;

			// Offsets pour les directions isométriques avec calcul en brut (trouver une autre solution pour faire ca proprement ?)
			Vector3[] directions = {
			new Vector3(3.75f - 1.875f, -1f, -1),  // Sud-Est
            new Vector3(0f - 1.875f, 1f, 1),  // Nord-Ouest
            new Vector3(3.75f - 1.875f, 1f, 1),   // Nord-Est
            new Vector3(0f - 1.875f, -1f, -1)  // Sud-Ouest
        };

			foreach (var offset in directions)
			{
				Vector3 indicatorPosition = RoundPosition(referencePosition + offset);

				if (moduleManager.IsPositionOccupied(indicatorPosition))
				{
					continue; 
				}

				GameObject indicator = Instantiate(moduleManager.placementIndicatorPrefab, indicatorPosition, Quaternion.identity);
				var placementScript = indicator.GetComponent<PlacementIndicator>();
				if (placementScript == null)
				{
					Debug.LogError("ERREUR : `PlacementIndicator` script est absent");
					Destroy(indicator);
					continue;
				}

				placementScript.SetPlacementPosition(indicatorPosition, this);
				placementIndicators.Add(indicator);

			}
		}
	}

	private void ClearPlacementIndicators()
	{
		foreach (GameObject indicator in placementIndicators)
		{
			Destroy(indicator);
		}
		placementIndicators.Clear();
	}

	public void PlaceModule(Vector3 position)
	{
		if (selectedModulePrefab == null)
		{
			Debug.LogError("Aucun module sélectionné !");
			return;
		}

		if (moduleManager.IsPositionOccupied(position))
		{
			return;
		}
		Vector3 finalPosition = RoundPosition(position);
		GameObject newModule = moduleManager.PlaceModule(finalPosition, selectedModulePrefab);

		if (newModule != null)
		{
			referenceModule = newModule;
		}

		ClearPlacementIndicators();
		CloseBuildMenu();
	}

	private Vector3 RoundPosition(Vector3 position, int decimals = 2)
	{
		return new Vector3(
			(float)System.Math.Round(position.x, decimals),
			(float)System.Math.Round(position.y, decimals),
			(float)System.Math.Round(position.z, decimals)
		);
	}
}