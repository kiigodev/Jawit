using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void KeluarGame()
    {
        Debug.Log("Game ditutup!"); // Biar keliatan di console pas di editor
        Application.Quit();
    }
}