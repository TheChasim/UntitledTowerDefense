using UnityEngine;
using TMPro;
using System.Collections.Generic;

[ExecuteAlways]
public class TowerSelection : MonoBehaviour
{
    [SerializeField] private int indexTower;
    private TowerSpawning spawning;
    private TMP_Dropdown dropdown;

    private void Awake()
    {
        spawning = FindAnyObjectByType<TowerSpawning>();
        dropdown = GetComponent<TMP_Dropdown>();
    }

    private void OnValidate()
    {
        // En mode éditeur, OnValidate peut être appelé avant Awake, donc il faut sécuriser :
        if (dropdown == null)
            dropdown = GetComponent<TMP_Dropdown>();

        if (dropdown == null) return;

        // Si TowerSpawning n’est pas encore dans la scène, on évite les null refs
        spawning = FindAnyObjectByType<TowerSpawning>();
        if (spawning == null || spawning.towers == null) return;

        // Vide les anciennes options
        dropdown.ClearOptions();

        // Crée une liste d’OptionData à partir de ta liste "allTower"
        List<TMP_Dropdown.OptionData> options = new();

        for (int i = 0; i < spawning.towers.Count; i++)
        {
            var towerPrefab = spawning.towers[i];
            string name = towerPrefab != null ? towerPrefab.name : $"Tower {i}";
            options.Add(new TMP_Dropdown.OptionData(name));
        }

        dropdown.AddOptions(options);
        dropdown.RefreshShownValue();
    }

    public void OnClick()
    {
        if (spawning == null) return;
        spawning.OnSelectIndex(indexTower);
    }

    public void OnChange(int index)
    {
        if (spawning == null) return;
        spawning.OnSelectIndex(index);
    }
}
