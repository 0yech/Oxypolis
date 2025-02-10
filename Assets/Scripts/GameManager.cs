using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
	public TextMeshProUGUI oxygenText;
	public TextMeshProUGUI foodText;
	public TextMeshProUGUI energyText;
	public TextMeshProUGUI fortyniumText;
	public TextMeshProUGUI humanText;
	public TextMeshProUGUI availableHumansText;

	public TextMeshProUGUI oxygenRateText;
	public TextMeshProUGUI foodRateText;
	public TextMeshProUGUI energyRateText;
	public TextMeshProUGUI fortyniumRateText;
	public TextMeshProUGUI humanRateText;
	public TextMeshProUGUI stepDurationText;
	public TextMeshProUGUI stepCooldownText;

	public float baseStepDuration = 10f;

	public GameObject BuildMenu;
	public GameObject MainUI;
	public GameObject modules;
	public GameObject ui;
	public GameObject pauseMenu;
	public string type;

	public static GameManager Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Debug.LogError("Une autre instance de GameManager existe déjà !");
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		Application.targetFrameRate = 60;
		if (ui == null)
		{
			Debug.LogError("La référence 'ui' est nulle. Assurez-vous qu'elle est assignée.");
			return;
		}

		var gameManager = ui.GetComponent<GameManager>();
		if (gameManager == null)
		{
			Debug.LogError("GameManager n'est pas trouvé sur l'objet référencé par 'ui'.");
			return;
		}
		Debug.Log($"Valeur de 'type' : {type}");
		switch (type)
		{
			case "rocket":
				gameManager.Consume_human(1);
				gameManager.Add_rocket(1);
				break;
			case "food_prod":
				gameManager.Add_wheat(1);
				break;
			case "oxygen_prod":
				gameManager.Add_tree(1);
				break;
			case "oxygen_tank":
				gameManager.Add_tank(1);
				break;
			default:
				Debug.LogWarning($"Type de module inconnu : {type}");
				break;
		}

		if (type == "food_prod" || type == "oxygen_prod" || type == "oxygen_tank")
		{
			gameManager.Consume_human(2);
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			if (Time.timeScale == 0f)
				ResumeGame();
			else
				PauseGame();
		}
	}

	private void Update()
	{
		ResourceManager.Instance.Update();
		UpdateUi();
	}

	private void UpdateUi()
	{
		oxygenText.text = $"{ResourceManager.Instance.GetResourceAmount("Oxygen"):F1}";
		foodText.text = $"{ResourceManager.Instance.GetResourceAmount("Food"):F1}";
		energyText.text = $"{ResourceManager.Instance.GetResourceAmount("Energy"):F1}";
		fortyniumText.text = $"{ResourceManager.Instance.GetResourceAmount("Fortynium"):F1}";
		humanText.text = $"{ResourceManager.Instance.GetResourceAmount("Humans"):F0}";
		availableHumansText.text = $"{ResourceManager.Instance.GetResourceAmount("AvailableHumans"):F0}";
		stepDurationText.text = $"Step duration : {GetStepDuration()}s";
		stepCooldownText.text = $"Step cooldown : {GetStepDuration() - (Time.time - ResourceManager.Instance.lastUpdateTimestamp):F0}s";


		float oxygenRate = ResourceManager.Instance.GetResource("Oxygen").productionRate - ResourceManager.Instance.GetResource("Oxygen").consumptionRate;
		float foodRate = ResourceManager.Instance.GetResource("Food").productionRate - ResourceManager.Instance.GetResource("Food").consumptionRate;
		float energyRate = ResourceManager.Instance.GetResource("Energy").productionRate - ResourceManager.Instance.GetResource("Energy").consumptionRate;
		float fortyniumRate = ResourceManager.Instance.GetResource("Fortynium").productionRate - ResourceManager.Instance.GetResource("Fortynium").consumptionRate;
		float humanRate = ResourceManager.Instance.GetResource("Humans").productionRate - ResourceManager.Instance.GetResource("Humans").consumptionRate;

		UpdateRateText(oxygenRateText, oxygenRate);
		UpdateRateText(foodRateText, foodRate);
		UpdateRateText(energyRateText, energyRate);
		UpdateRateText(fortyniumRateText, fortyniumRate);
		UpdateRateText(humanRateText, humanRate);
	}

	private void UpdateRateText(TextMeshProUGUI textElement, float rate)
	{
		if (GetStepDuration() == 0f)
		{
			textElement.text = ("0 per step");
		}
		else
		{
			textElement.text = $"{(rate >= 0 ? "+" : "")}{rate * Instance.GetStepDuration():F1} per step";

		}
		textElement.color = rate >= 0 ? Color.green : Color.red;
	}

	public void OnBuildModuleButtonClicked()
	{
		MainUI.SetActive(false);
		BuildMenu.SetActive(true);
		ModuleManager.Instance.HighlightAvailablePositions();
	}

	public void OnCancelBuild()
	{
		MainUI.SetActive(true);
		BuildMenu.SetActive(false);
		ModuleManager.Instance.ClearHighlights();
	}

	public void Consume_human(int amount)
	{
		ResourceManager.Instance.ConsumeResource("Humans", amount);
	}

	public void Add_rocket(int amount)
	{
		ResourceManager.Instance.AddResource("Rocket", amount);
	}

	public void Add_wheat(int amount)
	{
		ResourceManager.Instance.AddResource("Food", amount);
	}

	public void Add_tree(int amount)
	{
		ResourceManager.Instance.AddResource("Oxygen", amount);
	}

	public void Add_tank(int amount)
	{
		ResourceManager.Instance.AddResource("OxygenCapacity", amount);
	}

	public float GetStepDuration()
	{
		if (ResourceManager.Instance != null)
		{
			return ResourceManager.Instance.StepDuration;
		}

		Debug.LogWarning("ResourceManager non trouvé !");
		return 0;
	}

	public void SetGameSpeed(float multiplier)
	{
		if (ResourceManager.Instance != null)
		{
			float newStepDuration = baseStepDuration * multiplier;
			ResourceManager.Instance.StepDuration = newStepDuration;
			Debug.Log($"Vitesse du jeu mise à jour : stepDuration = {newStepDuration}");
		}
		else
		{
			Debug.LogWarning("ResourceManager non trouvé !");
		}
	}

	public void OnSpeedUpButtonClick()
	{
		GameManager.Instance.SetGameSpeed(2f);
	}

	public void OnSlowDownButtonClick()
	{
		GameManager.Instance.SetGameSpeed(1f);
	}

	public void OnStopButtonClick()
	{
		if (Time.timeScale == 0f)
		{
			ResumeGame();
			pauseMenu.SetActive(false);

		}
		else
		{
			PauseGame();
			pauseMenu.SetActive(true);
		}

	}

	public void PauseGame()
	{
		Time.timeScale = 0f;
	}

	public void ResumeGame()
	{
		Time.timeScale = 1f;
	}

}