using UnityEngine;
using TMPro; // Ensure TextMeshPro is used

public class ResourceManager : MonoBehaviour
{
    [Header("Resource Counts")]
    public int wood = 0;
    public int stone = 0;
    public int gold = 0;

    [Header("UI References")]
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI goldText;

    private void Start(){
        UpdateUI(); // Initialize the UI
    }

    public void AddResource(string resourceType, int amount){
        switch (resourceType)
        {
            case "Wood":
                wood += amount;
                break;
            case "Stone":
                stone += amount;
                break;
            case "Gold":
                gold += amount;
                break;
            default:
                Debug.LogWarning($"Unknown resource type: {resourceType}");
                break;
        }

        UpdateUI(); // Update the display after adding resources
    }

    private void UpdateUI(){
        woodText.text = $"Wood: {wood}";
        stoneText.text = $"Stone: {stone}";
        goldText.text = $"Gold: {gold}";
    }
}