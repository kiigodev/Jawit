using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    // Call this from your NPC when they die or when you click "Finish"
    public void FireEnding()
    {
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.CheckEnding();
        }
        else
        {
            Debug.Log("Bro where is the InventoryManager?! 💀");
        }
    }
}