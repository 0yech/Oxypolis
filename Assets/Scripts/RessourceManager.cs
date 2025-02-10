using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
	public static ResourceManager Instance;
	public float lastUpdateTimestamp = 0f;

	[SerializeField] private float stepDuration = 3f; // step time

	[System.Serializable]
	public class GameResource
	{
		public string name;
		public float amount;
		public float productionRate;
		public float consumptionRate;

		public GameResource(string name, float amount, float productionRate, float consumptionRate)
		{
			this.name = name;
			this.amount = amount;
			this.productionRate = productionRate;
			this.consumptionRate = consumptionRate;
		}
	}

	private Dictionary<string, GameResource> resources = new Dictionary<string, GameResource>();

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			InitializeResources();
		}
		else
		{
			Debug.LogError("Une autre instance de ResourceManager existe déjà");
			Destroy(gameObject);
		}
	}

	public void Update()
	{
		if (Time.time - lastUpdateTimestamp >= stepDuration)
		{
			lastUpdateTimestamp = Time.time;
			UpdateResources();
		}
	}


	public void InitializeResources()
	{
		AddResource("Oxygen", 100, 2, 1);
		AddResource("Food", 50, 1, 0.5f);
		AddResource("Energy", 10, 0, 0);
		AddResource("Fortynium", 0, 0, 0);
		AddResource("Humans", 10, 0, 0);
		AddResource("AvailableHumans", 10, 0, 0);
	}

	public void AddResource(string name, float initialAmount, float productionRate = 0, float consumptionRate = 0)
	{
		if (!resources.ContainsKey(name))
		{
			resources[name] = new GameResource(name, initialAmount, productionRate, consumptionRate);
		}
		else
		{
			Debug.LogWarning($"Resource {name} already exists.");
		}
	}

	public GameResource GetResource(string name)
	{
		if (resources.ContainsKey(name))
		{
			return resources[name];
		}
		Debug.LogError($"Resource {name} doesn't exists.");
		return null;
	}

	public float GetResourceAmount(string name)
	{
		var resource = GetResource(name);
		return resource != null ? resource.amount : 0;
	}

	public void ModifyResource(string name, float amount)
	{
		if (amount > 0)
		{
			AddResourceAmount(name, amount);
		}
		else
		{
			ConsumeResource(name, -amount);
		}
	}

	public bool ConsumeResource(string name, float amount)
	{
		var resource = GetResource(name);
		if (resource != null && resource.amount >= amount)
		{
			resource.amount -= amount;
			return true;
		}

		Debug.LogWarning($"Pas assez de {name} pour consommer {amount}. Disponible : {resource?.amount ?? 0}"); //TODO - create a popup to inform the player
		return false;
	}

	public void AddResourceAmount(string name, float amount)
	{
		var resource = GetResource(name);
		if (resource != null)
		{
			resource.amount += amount;
		}
	}

	public void UpdateResources()
	{
		foreach (var resource in resources.Values)
		{
			if (resource.productionRate != 0 || resource.consumptionRate != 0)
			{
				resource.amount += (resource.productionRate - resource.consumptionRate) * stepDuration;
				resource.amount = Mathf.Max(resource.amount, 0); // Pas de ressources négatives
			}
		}

		Debug.Log("Ressources updated !");
	}

	public void AddProductionRate(string name, float amount)
	{
		var resource = GetResource(name);
		if (resource != null)
		{
			resource.productionRate += amount;
		}
	}

	public void SetStepDuration(float newDuration)
	{
		if (newDuration >= 0)
		{
			stepDuration = newDuration;
			Debug.Log($"step : {stepDuration} sec."); //TODO - create a popup to inform the player of the time before the next step / time passed
		}
		else
		{
			Debug.LogWarning("step should be positive !");
		}
	}

	public float StepDuration
	{
		get => stepDuration;
		set
		{
			if (value >= 0)
			{
				stepDuration = value;
				Debug.Log($"stepDuration Updated : {stepDuration}");
			}
			else
			{
				Debug.LogWarning("stepDuration not positive !");
			}
		}
	}

	public void AdjustRatesForModule(string resourceProduced, float production, string resourceConsumed, float consumption)
	{
		AddProductionRate(resourceProduced, production);
		AddProductionRate(resourceConsumed, -consumption);
	}
}