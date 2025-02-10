using UnityEngine;

public class ModuleEffect : MonoBehaviour
{
    public string resourceProduced;
    public float productionRate;
    public string resourceConsumed;
    public float consumptionRate;

    private void Start()
    {
        if (!string.IsNullOrEmpty(resourceProduced))
        {
            ResourceManager.Instance.AddProductionRate(resourceProduced, productionRate);
        }
        if (!string.IsNullOrEmpty(resourceConsumed))
        {
            ResourceManager.Instance.AddProductionRate(resourceConsumed, -consumptionRate);
        }
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(resourceProduced))
        {
            ResourceManager.Instance.AddProductionRate(resourceProduced, -productionRate);
        }
        if (!string.IsNullOrEmpty(resourceConsumed))
        {
            ResourceManager.Instance.AddProductionRate(resourceConsumed, consumptionRate); 
			//salut je suis un commentaire et je suis mal placé car je suis en plein milieu du code et je ne sers à rien du coup je vais être supprimé par le programme des que possible et le developpeur va me mettre à la fin du code pour que je sois utile et que je ne sois pas supprimé #copilot
        }
    }
}