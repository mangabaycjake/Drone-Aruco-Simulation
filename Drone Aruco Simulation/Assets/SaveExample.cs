using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveExample : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            SaveGame();
        if (Input.GetKeyDown(KeyCode.L))
            LoadGame();
    }

    private void SaveGame()
    {
        SaveData saveData = new SaveData();
        saveData.marker_side_size = new SaveData().marker_side_size; //dont do like that, im just showing example here
        
        SaveManager.SaveGameState(saveData);
        Debug.Log("Game Saved!");
    }

    private void LoadGame()
    {
        SaveData saveData = SaveManager.LoadGameState();
        if (saveData != null)
        {
            Debug.Log(saveData.marker_side_size);
            Debug.Log("Game Loaded!");
        }
    }
}
