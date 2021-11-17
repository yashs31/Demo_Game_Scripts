using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    Player player;
    Inventory inventory;
    SettingsMenu settings;
    LevelLoader levelLoader;

    [Header("LEVEL CONFIG")]
    public string nextLevel = "Level02";
    public int levelToUnlock = 2;


    //Scriptable objects
    //public IntValue currHealth;


    /*private void Awake()
    {
        int musicPlayerCount = FindObjectsOfType<GameManager>().Length;
        if (musicPlayerCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    */
    private void Start()
    {
        player = FindObjectOfType<Player>();
        settings = FindObjectOfType<SettingsMenu>();
        levelLoader = FindObjectOfType<LevelLoader>();
        inventory = FindObjectOfType<Inventory>();
        //DontDestroyOnLoad(this);
        /*if(levelLoader.currentSceneIndex==1|| levelLoader.currentSceneIndex==2)
        {
            Debug.Log("setting loaded");
            LoadMusicVolume();
        }
        */
        
    }
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(player);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        //player.health = data.health;

        player.SetHealth(data.health);
        //player.health = data.health;
        //levelToUnlock = data.levelReached+1;

        /*
        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];

        player.transform.position = position;
        */
    }

    public void SaveSettings()
    {
        SaveSystem.SaveSettingsData(settings);
    }

    public void LoadMusicVolume()
    {
        SettingsData data = SaveSystem.LoadSettings();
        settings.SetMusicVolume(data.musicVolume);
    }
    
    public void WinLevel()
    {
        if (PlayerPrefsController.GetLevelReached() >=levelToUnlock)                          
        {
            //if saved levelReached >= LevelToUnlock ie already played level
            //dont load next level as the levelReached var will be set again to the current level
            SavePlayerInventoryAndLoadNextLevel();
            return;
        }
        else
        {
            //Didnt play this level
            PlayerPrefsController.SetLevelReached(levelToUnlock);
            //PlayerPrefs.SetInt("levelReached", levelToUnlock);
            SavePlayerInventoryAndLoadNextLevel();
        }

    }
    public void RestartLevel()
    {
        levelLoader.RestartLevel();
    }

    //SAVE ALL PLAYER INVENTORY USED IN CURRENT LEVEL
    void SavePlayerInventoryAndLoadNextLevel()
    {
        inventory.SaveCoins();
        inventory.SaveBullets();
        levelLoader.LoadLevel(nextLevel);
    }
}
