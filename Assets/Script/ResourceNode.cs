using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public int totalResources = 100;

    public bool Gather(int amount)
    {
        if (totalResources > 0)
        {
            totalResources -= amount;
            if (totalResources <= 0)
            {
                Destroy(gameObject); // Node depleted
            }
            return true;
        }
        return false;
    }
}