using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("UI Stuff")]
    public GameObject inventoryPanel;
    public GameObject[] itemCards; 
    
    [Header("Item Sprites")]
    public Sprite unknownSprite; 
    public Sprite[] obtainedSprites; 

    private bool[] hasItem = new bool[3];

    void Awake()
    {
        // Automatically wipes your saves every time you hit Play in the Editor!
        #if UNITY_EDITOR
        PlayerPrefs.DeleteAll();
        #endif

        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else 
        {
            Destroy(gameObject); 
            return;
        }

        LoadInventory();
        inventoryPanel.SetActive(false); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory(!inventoryPanel.activeSelf);
        }
    }

    public void ToggleInventory(bool show)
    {
        // Safe check in case the UI panel doesn't exist in the current scene
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(show);
        }

        if (show)
        {
            UpdateUI();
        }
    }

    public void GiveItem(int index)
    {
        if (index >= 0 && index < 3)
        {
            hasItem[index] = true;
            SaveInventory();
            
            // Fixed the ghost object error by checking if the panel actually exists first!
            if (inventoryPanel != null && inventoryPanel.activeSelf) 
            {
                UpdateUI();
            }
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < 3; i++)
        {
            // Safeguard to make sure you assigned the UI slots in the inspector
            if (itemCards[i] != null)
            {
                itemCards[i].GetComponent<Image>().sprite = hasItem[i] ? obtainedSprites[i] : unknownSprite;
            }
        }
    }

    private void SaveInventory()
    {
        PlayerPrefs.SetInt("Item0", hasItem[0] ? 1 : 0);
        PlayerPrefs.SetInt("Item1", hasItem[1] ? 1 : 0);
        PlayerPrefs.SetInt("Item2", hasItem[2] ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        hasItem[0] = PlayerPrefs.GetInt("Item0", 0) == 1;
        hasItem[1] = PlayerPrefs.GetInt("Item1", 0) == 1;
        hasItem[2] = PlayerPrefs.GetInt("Item2", 0) == 1;
    }

    public void CheckEnding()
    {
        Debug.Log("YOO! CheckEnding was clicked!");
        int winCount = 0;
        
        for (int i = 0; i < 3; i++)
        {
            if (hasItem[i]) winCount++;
        }

        Debug.Log("Total wins counted: " + winCount);

        if (winCount == 0)
        {
            Debug.Log("Trying to load: LoseAllEnding");
            SceneManager.LoadScene("LoseAllEnding"); 
        }
        else if (winCount == 1 || winCount == 2)
        {
            Debug.Log("Trying to load: MixedEnding");
            SceneManager.LoadScene("MixedEnding"); 
        }
        else if (winCount == 3)
        {
            Debug.Log("Trying to load: WinAllEnding");
            SceneManager.LoadScene("WinAllEnding");
        }
    }
    
}