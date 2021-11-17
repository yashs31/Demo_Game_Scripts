using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Text yellowCoinsText;
    [SerializeField] Text silverCoinsText;
    //[SerializeField] Text bulletGreenText;
    //[SerializeField] Text bulletYellowText;
    [SerializeField] public int yellowCoins;
    [SerializeField] public int silverCoins;
    [SerializeField] public int bullet_green = 30;
    [SerializeField] public int bullet_purple = 30;
    //[SerializeField] Text bulletsText;

    private void Awake()
    {
        yellowCoins = PlayerPrefsController.GetYellowCoins();
        DisplayYellowCoins();
        //yellowCoins = PlayerPrefs.GetInt("yellowCoins", yellowCoins);
        //Debug.Log("yellowcoins = " + yellowCoins);
        silverCoins = PlayerPrefsController.GetSilverCoins();
        DisplaySilverCoins();
        //silverCoins = PlayerPrefs.GetInt("silverCoins", silverCoins);
        //Debug.Log("silvercoins = " + silverCoins);
        bullet_green = PlayerPrefsController.GetGreenBullets();
        //bullet_green = PlayerPrefs.GetInt("bullet_green", bullet_green);
        bullet_purple = PlayerPrefsController.GetPurpleBullets();
        //bullet_purple =PlayerPrefs.GetInt("bullet_purple", bullet_purple);
    }

    // Update is called once per frame
    void Update()
    {
        //DisplayYellowCoins();
        //DisplaySilverCoins();
    }
    public void DisplayYellowCoins()
    {
        yellowCoinsText.text = " X  " + yellowCoins;
        //Debug.Log("Yellow coins updated");
    }
    public void DisplaySilverCoins()
    {
        silverCoinsText.text = " X  " + silverCoins;
        //Debug.Log("silver coins updated");
    }
    public void SaveCoins()
    {
        PlayerPrefsController.SaveCoins(yellowCoins, silverCoins);
        
        //PlayerPrefs.SetInt("yellowCoins", yellowCoins);
        //PlayerPrefs.SetInt("silverCoins", silverCoins);
    }
    public void SaveBullets()
    {
        PlayerPrefsController.SaveBullets(bullet_green, bullet_purple);

        //PlayerPrefs.SetInt("bullet_green", bullet_green);
        //PlayerPrefs.SetInt("bullet_purple", bullet_purple);
    }
    void DisplayGreenBullets()
    {
        //bulletGreenText.text = " X " + bullet_green.ToString();
    }

    void DisplayPurpleBullets()
    {
        //bulletPurpleText.text = " X " + bulletPurpleText.ToString();
    }
}
