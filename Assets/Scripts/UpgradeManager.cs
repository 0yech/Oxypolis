using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public List<Upgrade> upgrades = new List<Upgrade>();

    void Start()
    {
        LoadUpgrades("Assets/Data/upgrades.json");
    }

    public void LoadUpgrades(string filePath)
    {
        string jsonData = File.ReadAllText(filePath);
        upgrades = JsonUtility.FromJson<UpgradeList>(jsonData).upgrades;
    }
}

[System.Serializable]
public class Upgrade
{
    public string title;
    public string description;
    public int cost;
    public string effect;
}

[System.Serializable]
public class UpgradeList
{
    public List<Upgrade> upgrades;
}