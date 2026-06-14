using UnityEngine;

public class EnemyIndicator3D : MonoBehaviour
{
    public Animator animator;

    // Type "Hover" and "Leave" in the Inspector!
    public string hoverTrigger = "Hover";
    public string leaveTrigger = "Leave";

    public void PlayHover() => animator.SetTrigger(hoverTrigger);
    public void PlayLeave() => animator.SetTrigger(leaveTrigger);
}