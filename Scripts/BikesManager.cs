using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BikesManager : MonoBehaviour{

    public GameObject LeftButton, RightButton;
    public GameObject[] bikes;
    public Image speedImg, accelerationImg, handlingImg, brakesImg;
    public Image speedExtra, accelerationExtra, handlingExtra, brakesExtra;
    public Text speedTxt, accelerationTxt, handlingTxt, brakesTxt;
    public Text speedAddTxt, accelerationAddTxt, handlingAddTxt, brakesAddTxt;
    public Text speedUpPrice, accelerationUpPrice, handlingUpPrice, brakesUpPrice;
    public GameObject speedBtn, accelerationBtn, handlingBtn, brakesBtn;
    public Text coinFieldTxt;
    public Light[] spotLight = new Light[3];
    public AudioSource upgradeSound, notEnoughCoins, selectSound;
    private GameObject[] bikesAvailable;
    private int totalBikesAvailable = 0, currentIndex = 0, bikeNumber = 0, bikeType, soundEnable;
    private GameObject currentBike;
    private string fileName;
    BikeData bikeData;
    private Dictionary<int, int> map = new Dictionary<int, int>();
    private Upgrade[] upgradePrice = new Upgrade[3];
    private UpgradeStates[] upgradeData = new UpgradeStates[3];

    void Awake(){
        currentIndex = PlayerPrefs.GetInt("BikeNumber", 1) - 1;

        soundEnable = PlayerPrefs.GetInt("Sound", 1);

        CheckAvailableBikes();

        SetUpgradePricing();

        SetUpgradeStates();
    }
    
    void Start(){
        currentBike = Instantiate(bikesAvailable[currentIndex], transform.position, Quaternion.identity);

        SetCoins();

        SetStatesPanel();

        ArrowCheck();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown("a")){
            LeftArrow();
        }else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown("d")){
            RightArrow();
        }else if(Input.GetKeyDown(KeyCode.Escape)){
            Back();
        }else if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
            Select();
        }
    }

    private void SetCoins(){
        coinFieldTxt.text = PlayerPrefs.GetInt("Coins", 5000).ToString();
    }

    public void LeftArrow(){
        if(LeftButton.activeSelf){
            if(soundEnable==1) selectSound.Play();
            LeftButton.transform.GetComponent<Animation>().Play("arrowLeft");
            Destroy(currentBike);
            currentBike = Instantiate(bikesAvailable[--currentIndex], transform.position, Quaternion.identity);

            SetStatesPanel();

            ArrowCheck();
        }
    }

    public void RightArrow(){
        if(RightButton.activeSelf){
            if(soundEnable==1) selectSound.Play();
            RightButton.transform.GetComponent<Animation>().Play("arrowRight");
            Destroy(currentBike);
            currentBike = Instantiate(bikesAvailable[++currentIndex], transform.position, Quaternion.identity);
            
            SetStatesPanel();

            ArrowCheck();
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
        }
        if(currentIndex==totalBikesAvailable-1){
            RightButton.SetActive(false);
        }
    }

    public void Select(){
        bikeNumber = map[currentIndex];
        PlayerPrefs.SetInt("BikeNumber", bikeNumber+1);
        Back();
    }

    public void Back(){
        if(soundEnable==1) selectSound.Play();
        SceneManager.LoadScene(0);
    }

    private void CheckAvailableBikes(){
        List<GameObject> bikesUnlocked = new List<GameObject>();
        for(int i = 1; i<=11; i++){
            fileName = "Bike" + i;
            BikeData bikeData = Progress.LoadBike(fileName);
            if(bikeData.Unlocked){
                map.Add(totalBikesAvailable, i-1);
                if((i-1)==currentIndex) currentIndex = totalBikesAvailable;
                bikesUnlocked.Add(bikes[i-1]);
                totalBikesAvailable++;
            }
        }
        
        bikesAvailable = new GameObject[totalBikesAvailable];
        bikesAvailable = bikesUnlocked.ToArray();
    }

    private void SetUpgradePricing(){
        for(int i = 0; i<3; i++){
            upgradePrice[i] = new Upgrade();
        }

        upgradePrice[0].setSpeedPrice(3500, 5800, 8400);
        upgradePrice[1].setSpeedPrice(6000, 8500, 12500);
        upgradePrice[2].setSpeedPrice(8000, 12500, 20000);

        upgradePrice[0].setAccelerationPrice(3000, 4500, 6600);
        upgradePrice[1].setAccelerationPrice(4500, 6700, 9600);
        upgradePrice[2].setAccelerationPrice(6000, 8200, 12000);

        upgradePrice[0].setHandlingPrice(2500, 3800, 6000);
        upgradePrice[1].setHandlingPrice(3500, 5500, 8000);
        upgradePrice[2].setHandlingPrice(4500, 6900, 10000);

        upgradePrice[0].setBrakesPrice(1500, 2800, 4200);
        upgradePrice[1].setBrakesPrice(3000, 4500, 7000);
        upgradePrice[2].setBrakesPrice(4000, 6000, 8500);
    }

    private void SetUpgradeStates(){
        for(int i = 0; i<3; i++){
            upgradeData[i] = new UpgradeStates();
        }

        upgradeData[0].setSpeed(82);
        upgradeData[1].setSpeed(87);
        upgradeData[2].setSpeed(90);

        upgradeData[0].setAcceleration(27);
        upgradeData[1].setAcceleration(31);
        upgradeData[2].setAcceleration(34);

        upgradeData[0].setHandling(31);
        upgradeData[1].setHandling(37);
        upgradeData[2].setHandling(45);

        upgradeData[0].setBrakes(28);
        upgradeData[1].setBrakes(38);
        upgradeData[2].setBrakes(48);
    }

    private void SetStatesPanel(){
        bikeNumber = map[currentIndex];
        fileName = "Bike" + (bikeNumber+1);
        bikeData = Progress.LoadBike(fileName);
        bikeType = bikeData.BikeType;

        SetSpeedBlock();
        SetAccelerationBlock();
        SetHandlingBlock();
        SetBrakesBlock();
    }

    private void SetSpeedBlock(){
        if(bikeData.SpeedLevel>=3) speedBtn.SetActive(false);
        else speedBtn.SetActive(true);

        float maxSpeed = 95f;

        speedImg.fillAmount = bikeData.Speed/maxSpeed;
        speedTxt.text = (bikeData.Speed * 1.21).ToString("0");

        if(bikeData.SpeedLevel<3){
            speedExtra.fillAmount = (float)upgradeData[bikeType].Speed[bikeData.SpeedLevel]/maxSpeed;
            float upgradeSpeed = (float)(upgradeData[bikeType].Speed[bikeData.SpeedLevel]*1.21);
            float currentSpedd = (float)(bikeData.Speed*1.21);
            float addSpeed = Mathf.Round(upgradeSpeed) - Mathf.Round(currentSpedd);
            speedAddTxt.text = "+  " + addSpeed.ToString("0");
            speedUpPrice.text = upgradePrice[bikeType].Speed[bikeData.SpeedLevel].ToString();
        }else{
            speedExtra.fillAmount = 0;
            speedAddTxt.text = "";
            speedUpPrice.text = "";
        }
    }

    private void SetAccelerationBlock(){
        if(bikeData.AccelerationLevel>=3) accelerationBtn.SetActive(false);
        else accelerationBtn.SetActive(true);

        float maxAcceleration = 37f;
        
        accelerationImg.fillAmount = bikeData.Acceleration/maxAcceleration;
        accelerationTxt.text = bikeData.Acceleration.ToString();

        if(bikeData.AccelerationLevel<3){
            accelerationExtra.fillAmount = (float)upgradeData[bikeType].Acceleration[bikeData.AccelerationLevel]/maxAcceleration;
            accelerationAddTxt.text = "+  " + (upgradeData[bikeType].Acceleration[bikeData.AccelerationLevel] - bikeData.Acceleration).ToString();
            accelerationUpPrice.text = upgradePrice[bikeType].Acceleration[bikeData.AccelerationLevel].ToString();
        }else{
            accelerationExtra.fillAmount = 0;
            accelerationAddTxt.text = "";
            accelerationUpPrice.text = "";
        }
    }

    private void SetHandlingBlock(){
        if(bikeData.HandlingLevel>=3) handlingBtn.SetActive(false);
        else handlingBtn.SetActive(true);

        float maxHandling = 50f;
        
        handlingImg.fillAmount = bikeData.Handling/maxHandling;
        handlingTxt.text = bikeData.Handling.ToString();

        if(bikeData.HandlingLevel<3){
            handlingExtra.fillAmount = (float)upgradeData[bikeType].Handling[bikeData.HandlingLevel]/maxHandling;
            handlingAddTxt.text = "+  " + (upgradeData[bikeType].Handling[bikeData.HandlingLevel] - bikeData.Handling).ToString();
            handlingUpPrice.text = upgradePrice[bikeType].Handling[bikeData.HandlingLevel].ToString();
        }else{
            handlingExtra.fillAmount = 0;
            handlingAddTxt.text = "";
            handlingUpPrice.text = "";
        }
    }

    private void SetBrakesBlock(){
        if(bikeData.BrakesLevel>=3) brakesBtn.SetActive(false);
        else brakesBtn.SetActive(true);

        float maxBrakes = 56f;

        brakesImg.fillAmount = bikeData.Brakes/maxBrakes;
        brakesTxt.text = bikeData.Brakes.ToString();

        if(bikeData.BrakesLevel<3){
            brakesExtra.fillAmount = (float)upgradeData[bikeType].Brakes[bikeData.BrakesLevel]/maxBrakes;
            brakesAddTxt.text = "+  " + (upgradeData[bikeType].Brakes[bikeData.BrakesLevel] - bikeData.Brakes).ToString();
            brakesUpPrice.text = upgradePrice[bikeType].Brakes[bikeData.BrakesLevel].ToString();
        }else{
            brakesExtra.fillAmount = 0;
            brakesAddTxt.text = "";
            brakesUpPrice.text = "";
        }
    }

    public void UpgradeSpeed(){
        int speedPrice = upgradePrice[bikeType].Speed[bikeData.SpeedLevel];

        int coinAmount = PlayerPrefs.GetInt("Coins", 5000);

        if(coinAmount>=speedPrice && bikeData.SpeedLevel<3){
            bikeData.Speed = upgradeData[bikeType].Speed[bikeData.SpeedLevel];
            bikeData.SpeedLevel++;
            coinAmount -= speedPrice;
            PlayerPrefs.SetInt("Coins", coinAmount);
            SetCoins();
            Progress.SaveBike(fileName, bikeData);
            if(soundEnable==1) upgradeSound.Play();
            foreach(Light spLight in spotLight){
                spLight.color = Color.red;
                spLight.transform.GetComponent<Animation>().Play("upgradeAnimation");
            }
            SetSpeedBlock();
        }else{
            if(soundEnable==1) notEnoughCoins.Play();
        }
    }

    public void UpgradeAcceleration(){
        int accelerationPrice = upgradePrice[bikeType].Acceleration[bikeData.AccelerationLevel];

        int coinAmount = PlayerPrefs.GetInt("Coins", 5000);

        if(coinAmount>=accelerationPrice && bikeData.AccelerationLevel<3){
            bikeData.Acceleration = upgradeData[bikeType].Acceleration[bikeData.AccelerationLevel];
            bikeData.AccelerationLevel++;
            coinAmount -= accelerationPrice;
            PlayerPrefs.SetInt("Coins", coinAmount);
            SetCoins();
            Progress.SaveBike(fileName, bikeData);
            if(soundEnable==1) upgradeSound.Play();
            foreach(Light spLight in spotLight){
                spLight.color = Color.yellow;
                spLight.transform.GetComponent<Animation>().Play("upgradeAnimation");
            }
            SetAccelerationBlock();
        }else{
            if(soundEnable==1) notEnoughCoins.Play();
        }
    }

    public void UpgradeHandling(){
        int handlingPrice = upgradePrice[bikeType].Handling[bikeData.HandlingLevel];

        int coinAmount = PlayerPrefs.GetInt("Coins", 5000);

        if(coinAmount>=handlingPrice && bikeData.HandlingLevel<3){
            bikeData.Handling = upgradeData[bikeType].Handling[bikeData.HandlingLevel];
            bikeData.HandlingLevel++;
            coinAmount -= handlingPrice;
            PlayerPrefs.SetInt("Coins", coinAmount);
            SetCoins();
            Progress.SaveBike(fileName, bikeData);
            if(soundEnable==1) upgradeSound.Play();
            foreach(Light spLight in spotLight){
                spLight.color = Color.green;
                spLight.transform.GetComponent<Animation>().Play("upgradeAnimation");
            }
            SetHandlingBlock();
        }else{
            if(soundEnable==1) notEnoughCoins.Play();
        }
    }

    public void UpgradeBrakes(){
        int BrakesPrice = upgradePrice[bikeType].Brakes[bikeData.BrakesLevel];

        int coinAmount = PlayerPrefs.GetInt("Coins", 5000);

        if(coinAmount>=BrakesPrice && bikeData.BrakesLevel<3){
            bikeData.Brakes = upgradeData[bikeType].Brakes[bikeData.BrakesLevel];
            bikeData.BrakesLevel++;
            coinAmount -= BrakesPrice;
            PlayerPrefs.SetInt("Coins", coinAmount);
            SetCoins();
            Progress.SaveBike(fileName, bikeData);
            if(soundEnable==1) upgradeSound.Play();
            foreach(Light spLight in spotLight){
                spLight.color = Color.magenta;
                spLight.transform.GetComponent<Animation>().Play("upgradeAnimation");
            }
            SetBrakesBlock();
        }else{
            if(soundEnable==1) notEnoughCoins.Play();
        }
    }
}
