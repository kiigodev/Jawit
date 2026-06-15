using UnityEngine;

public class SmoothTeleport : MonoBehaviour
{
    // Drag your destination object here in the Unity Inspector
    public Transform teleportTarget; 
    
    // Press Spacebar to blink!
    public KeyCode teleportKey = KeyCode.Space; 

    private CharacterController cc;

    void Start()
    {
        // Automatically grabs the Character Controller on your player
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(teleportKey))
        {
            TeleportPlayer();
        }
    }

    void TeleportPlayer()
    {
        if (teleportTarget == null)
        {
            Debug.LogWarning("Yo! You forgot to assign a Teleport Target in the inspector!");
            return;
        }

        // 1. Tell the Character Controller to look away for a sec
        if (cc != null)
        {
            cc.enabled = false;
        }

        // 2. BAM! Do the actual teleport
        transform.position = teleportTarget.position;
        transform.rotation = teleportTarget.rotation; // Matches the target's direction

        // 3. Turn it back on so you can move again instantly
        if (cc != null)
        {
            cc.enabled = true;
        }

        Debug.Log("Teleported safely! ⚡");
    }
}