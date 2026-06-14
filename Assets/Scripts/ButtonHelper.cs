using UnityEngine;

public class ButtonHelper : MonoBehaviour
{
    public void ClaimItem(int slotIndex)
    {
        // This instantly talks to your manager from any scene!
        InventoryManager.instance.GiveItem(slotIndex);
    }

    public void TriggerEndingCheck()
    {
        InventoryManager.instance.CheckEnding();
    }
}