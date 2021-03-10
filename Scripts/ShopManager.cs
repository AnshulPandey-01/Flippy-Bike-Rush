using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour{

    public GameObject LeftButton, RightButton, purchaseButton;
    public GameObject[] bikes;
    private GameObject currentBike;
    public Image speed, acceleration, handling, brakes;
    public Text speedTxt, accelerationTxt, handlingTxt, brakesTxt, coinFieldTxt;
    public AudioSource notEnoughCoins, purchaceSound, selectSound;
    private Text price;
    private int currentIndex, soundEnable;
    private string fileName;

    void Awake() {
        coinFieldTxt.text = PlayerPrefs.GetInt("Coins", 5000).ToString();

        price = purchaseButton.GetComponentInChildren<Text>();

        currentIndex = PlayerPrefs.GetInt("BikeNumber", 1) - 1;
        currentBike = Instantiate(bikes[currentIndex], transform.position, Quaternion.identity);

        soundEnable = PlayerPrefs.GetInt("Sound", 1);

        ArrowCheck();
        BikeUnlocked();
        StatesBar();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown("a")){
            LeftArrow();
        }else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown("d")){
            RightArrow();
        }else if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
            Purchase();
        }else if(Input.GetKeyDown(KeyCode.Escape)){
            Back();
        }
    }

    public void LeftArrow(){
        if(LeftButton.activeSelf){
            if(soundEnable==1) selectSound.Play();
            LeftButton.transform.GetComponent<Animation>().Play("arrowLeft");
            Destroy(currentBike);
            currentBike = Instantiate(bikes[--currentIndex], transform.position, Quaternion.identity);
        
            ArrowCheck();
            BikeUnlocked();
            StatesBar();
        }
    }

    public void RightArrow(){
        if(RightButton.activeSelf){
            if(soundEnable==1) selectSound.Play();
            RightButton.transform.GetComponent<Animation>().Play("arrowRight");
            Destroy(currentBike);
            currentBike = Instantiate(bikes[++currentIndex], transform.position, Quaternion.identity);

            ArrowCheck();
            BikeUnlocked();
            StatesBar();
        }
    }

    private void ArrowCheck(){
        if(!LeftButton.activeSelf){
            LeftButton.SetActive(true);
        }
        if(!RightButton.activeSelf){
            RightButton.SetActive(true);
        }

        if(currentIndex==0){
            LeftButton.SetActive(false);
        }else if(currentIndex==10){
            RightButton.SetActive(false);
        }
    }

    private void BikeUnlocked(){
        if(currentIndex<=4){
            fileName = "Bike" + (currentIndex + 1);
            price.text = 25000.ToString();
        }else if(currentIndex>4 && currentIndex<10){
            fileName = "Bike" + (currentIndex + 1);
            price.text = 50000.ToString();
        }else if(currentIndex==10){
            fileName = "Bike" + (currentIndex + 1);
            price.text = 100000.ToString();
        }

        BikeData bikeData = Progress.LoadBike(fileName);
        if(bikeData.Unlocked){
            purchaseButton.SetActive(false);
        }else{
            purchaseButton.SetActive(true);
        }
    }

    private void StatesBar(){
        float maxSpeed = 96f;
        float maxAcceleration = 37f;
        float maxHandling = 50f;
        float maxBrakes = 56f;

        if(currentIndex<=4){
            speed.fillAmount = 80f/maxSpeed;
            acceleration.fillAmount = 25f/maxAcceleration;
            handling.fillAmount = 28f/maxHandling;
            brakes.fillAmount = 25f/maxBrakes;

            speedTxt.text = (80*1.21).ToString("0");
            accelerationTxt.text = 25.ToString();
            handlingTxt.text = 28.ToString();
            brakesTxt.text = 25.ToString();
        }else if(currentIndex>4 && currentIndex<10){
            speed.fillAmount = 85f/maxSpeed;
            acceleration.fillAmount = 29f/maxAcceleration;
            handling.fillAmount = 34f/maxHandling;
            brakes.fillAmount = 35f/maxBrakes;

            speedTxt.text = (85*1.21).ToString("0");
            accelerationTxt.text = 29.ToString();
            handlingTxt.text = 34.ToString();
            brakesTxt.text = 35.ToString();
        }else if(currentIndex==10){
            speed.fillAmount = 88f/maxSpeed;
            acceleration.fillAmount = 32f/maxAcceleration;
            handling.fillAmount = 42f/maxHandling;
            brakes.fillAmount = 45f/maxBrakes;

            speedTxt.text = (88*1.21).ToString("0");
            accelerationTxt.text = 32.ToString();
            handlingTxt.text = 42.ToString();
            brakesTxt.text = 45.ToString();
        }
    }

    public void Purchase(){
        int coinAmount = PlayerPrefs.GetInt("Coins", 5000);
        if(currentIndex<=4){
            if(coinAmount>=25000){
                UnlockCurrentBike();
                coinAmount -= 25000;
                purchaseButton.SetActive(false);
                PlayerPrefs.SetInt("BikeNumber", currentIndex+1);
            }else{
                if(soundEnable==1) notEnoughCoins.Play();
            }
        }else if(currentIndex>4 && currentIndex<10){
            if(coinAmount>=50000){
                UnlockCurrentBike();
                coinAmount -= 50000;
                purchaseButton.SetActive(false);
                PlayerPrefs.SetInt("BikeNumber", currentIndex+1);
            }else{
                if(soundEnable==1) notEnoughCoins.Play();
            }
        }else if(currentIndex==10){
            if(coinAmount>=100000){
                UnlockCurrentBike();
                coinAmount -= 100000;
                purchaseButton.SetActive(false);
                PlayerPrefs.SetInt("BikeNumber", currentIndex+1);
            }else{
                if(soundEnable==1) notEnoughCoins.Play();
            }
        }
        coinFieldTxt.text = coinAmount.ToString();
        PlayerPrefs.SetInt("Coins", coinAmount);
    }

    private void UnlockCurrentBike(){
        fileName = "Bike" + (currentIndex + 1);
        BikeData bikeData = Progress.LoadBike(fileName);
        bikeData.UnlockBike();
        Progress.SaveBike(fileName, bikeData);
        if(soundEnable==1) purchaceSound.Play();
    }

    public void Back(){
        if(soundEnable==1) selectSound.Play();
        SceneManager.LoadScene(0);
    }
}
