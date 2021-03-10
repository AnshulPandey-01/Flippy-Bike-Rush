using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour{

    private int option = 0, soundEnable;
    public GameObject[] bikes;
    public Text startTxt, bikesTxt, storeTxt, settingsTxt, quitTxt, coinsTxt, difficultyTxt;
    private Text[] txtFields = new Text[5];
    private Color activeColor = new Color(255/255f, 35/255f, 0/255f), passiveColor = Color.white;
    public AudioSource upDownSound, selectSound;
    public GameObject music;

    void Awake() {
        int musicEnable = PlayerPrefs.GetInt("Music", 1);
        soundEnable = PlayerPrefs.GetInt("Sound", 1);
        if(GameObject.Find("Music(Clone)")==null && musicEnable==1){
            GameObject musicObject = Instantiate(music, transform.position, Quaternion.identity);
            DontDestroyOnLoad(musicObject);
        }
        CreateBikeData();
    }

    void Start(){
        int bikeNumber = PlayerPrefs.GetInt("BikeNumber", 1) - 1;
        GameObject pos = GameObject.Find("SpawnPoint");
        Instantiate(bikes[bikeNumber], pos.transform.position, Quaternion.identity);

        Time.timeScale = 1.0f;
        Cursor.visible = true;
        
        txtFields[0] = startTxt;
        txtFields[1] = bikesTxt;
        txtFields[2] = storeTxt;
        txtFields[3] = settingsTxt;
        txtFields[4] = quitTxt;

        int coinAmount = PlayerPrefs.GetInt("Coins", 5000);
        coinsTxt.text = coinAmount.ToString();

        int difficulty = PlayerPrefs.GetInt("difficulty_level", 0);
        if(difficulty==0) difficultyTxt.text = "EASY";
        else if(difficulty==1) difficultyTxt.text = "MEDIUM";
        else if(difficulty==2) difficultyTxt.text = "HARD";
        else if(difficulty==3) difficultyTxt.text = "EXTREME";
    }

    void Update(){
        if(Input.GetKeyDown("w") || Input.GetKeyDown(KeyCode.UpArrow)){
            PassiveButton();
            option = option==0 ? 4 : option - 1;
            ActiveButton();
        }else if(Input.GetKeyDown("s") || Input.GetKeyDown(KeyCode.DownArrow)){
            PassiveButton();
            option++;
            option %= 5;
            ActiveButton();
        }else if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
            if(option==0){
                StartButton();
            }else if(option==1){
                BikesButton();
            }else if(option==2){
                StoreButton();
            }else if(option==3){
                SettingsButton();
            }else if(option==4){
                QuitButton();
            }
        }
    }

    void PassiveButton(){
        if(soundEnable==1) upDownSound.Play();
        txtFields[option].color = passiveColor;
    }

    void ActiveButton(){    
        txtFields[option].color = activeColor;
    }

    private void CreateBikeData(){
        for(int i = 1; i<=11; i++){
            string fileName = "Bike" + i;
            if(!Progress.check(fileName)){
                if(i==1){
                    Movement bikeScript = bikes[i-1].GetComponent<Movement>();
                    BikeData bikeData = new BikeData(bikeScript, 0);
                    bikeData.UnlockBike();
                    Progress.SaveBike(fileName, bikeData);
                }else{
                    int type = 0;
                    if(i>=6 && i<=10) type = 1;
                    else if(i==11) type = 2;
                    Movement bikeScript = bikes[i-1].GetComponent<Movement>();
                    Progress.SaveBike(fileName, bikeScript, type);
                }
            }
        }
    }

    public void StartButton(){
        Destroy(GameObject.Find("Music(Clone)"));
        if(soundEnable==1) selectSound.Play();
        int trackNo = Random.Range(0, 2);
        SceneManager.LoadScene(6 + trackNo);
    }

    public void BikesButton(){
        if(soundEnable==1) selectSound.Play();
        SceneManager.LoadScene(1);
    }

    public void StoreButton(){
        if(soundEnable==1) selectSound.Play();
        SceneManager.LoadScene(2);
    }

    public void SettingsButton(){
        if(soundEnable==1) selectSound.Play();
        SceneManager.LoadScene(3);
    }

    public void QuitButton(){
        if(soundEnable==1) selectSound.Play();
        Application.Quit();
    }

    public void GetCoins(){
        int coinAmount = PlayerPrefs.GetInt("Coins", 5000);
        coinAmount += 10000;
        PlayerPrefs.SetInt("Coins", coinAmount);
        coinsTxt.text = coinAmount.ToString();
    }
}
