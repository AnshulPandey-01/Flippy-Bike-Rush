using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour{

    [HideInInspector]
    public int caller = 0;
    public GameObject rotationTxt, pauseBtn, comSc, speedometer;
    public Image positionImage;
    public GameObject[] playerBikes;
    private List<Transform> racers;
    private GameObject player;
    private Rigidbody playerRigiedBidy;
    [HideInInspector]
    public bool playerAlive = true;
    public Text displayText, speedText, positionText, finishPositionText, prizeText;
    public GameObject coinImage;
    public AudioSource explosionSound;
    private Vector3 initialPosImg;
    private bool racing = true;
    private int currentPosition, difficultyMultiplyer;
    private float maxSpeed;
    private int[] difficultyRewardMultiplyer = {1, 2, 4, 5}, aiSpeed = {75, 79, 85, 90}, aiAcceleration = {30, 33, 37, 37};

    void Awake(){
        int bikeNumber = PlayerPrefs.GetInt("BikeNumber", 1) - 1;
        string fileName = "Bike" + (bikeNumber+1);
        BikeData bikeData = Progress.LoadBike(fileName);

        player = Instantiate(playerBikes[bikeNumber], transform.position, Quaternion.identity);

        maxSpeed = bikeData.Speed;

        player.GetComponent<Movement>().maxSpeed = bikeData.Speed;
        player.GetComponent<Movement>().acceleration = bikeData.Acceleration;
        player.GetComponent<Movement>().handling = bikeData.Handling;
        player.GetComponent<Movement>().brakes = bikeData.Brakes;

        playerRigiedBidy = player.GetComponent<Rigidbody>();

        GameObject AIs = GameObject.Find("AI_Agents");

        GameObject[] ai = new GameObject[AIs.transform.childCount];

        racers = new List<Transform>();
        racers.Add(player.transform);

        int difficultyLevel = PlayerPrefs.GetInt("difficulty_level", 0);


        for(int i = 0; i<AIs.transform.childCount; i++){
            ai[i] = AIs.transform.GetChild(i).gameObject;
            ai[i].GetComponent<AI_Controls>().maxVelocity = aiSpeed[difficultyLevel];
            ai[i].GetComponent<AI_Controls>().acceleration = aiAcceleration[difficultyLevel];
            racers.Add(ai[i].transform);
        }
        
        if(difficultyLevel==0){
            ai[0].GetComponent<AI_Controls>().maxVelocity += 3;
            ai[1].GetComponent<AI_Controls>().maxVelocity += 3;
        }else if(difficultyLevel==1){
            ai[0].GetComponent<AI_Controls>().maxVelocity += 4;
            ai[1].GetComponent<AI_Controls>().maxVelocity += 4;
        }else if(difficultyLevel==2){
            ai[0].GetComponent<AI_Controls>().maxVelocity += 5;
            ai[1].GetComponent<AI_Controls>().maxVelocity += 5;
            ai[2].GetComponent<AI_Controls>().maxVelocity += 5;
        }else if(difficultyLevel==3){
            ai[0].GetComponent<AI_Controls>().maxVelocity += 7;
            ai[0].GetComponent<AI_Controls>().acceleration += 1;
            ai[1].GetComponent<AI_Controls>().maxVelocity += 7;
            ai[1].GetComponent<AI_Controls>().acceleration += 1;
            ai[2].GetComponent<AI_Controls>().maxVelocity += 7;
            ai[2].GetComponent<AI_Controls>().acceleration += 1;
            ai[3].GetComponent<AI_Controls>().maxVelocity += 7;
            ai[4].GetComponent<AI_Controls>().maxVelocity += 7;
        }

        difficultyMultiplyer = difficultyRewardMultiplyer[difficultyLevel];

        initialPosImg = positionImage.transform.localPosition;

        if(PlayerPrefs.GetInt("Sound", 1)==0){
            GameObject.Find("AudioManager").SetActive(false);
            GameObject.Find("Audio").SetActive(false);
        }
    }

    void Update(){
        if(playerAlive){

            for(int i = 0; i<racers.Count-1; i++){
                bool swapped = false;
                for(int j = 0; j<racers.Count-i-1; j++){
                    if(racers[j].position.z<racers[j+1].position.z){
                        Transform temp = racers[j];
                        racers[j] = racers[j+1];
                        racers[j+1] = temp;
                        swapped = true;
                    }
                }
                if(!swapped) break;
            }

            for(int i = 0; i<racers.Count; i++){
                if(racers[i].position.z==player.transform.position.z){
                    currentPosition = i + 1;
                    positionText.text = currentPosition.ToString();
                    break;
                }
            }

            if(playerRigiedBidy.velocity.z>maxSpeed)
                speedText.text = (maxSpeed*1.21).ToString("0");
            else
                speedText.text = (playerRigiedBidy.velocity.z * 1.21f).ToString("0");

            Vector3 imgPos = initialPosImg;
            imgPos.y += player.transform.position.z/94f;
            positionImage.transform.localPosition = imgPos;

            if(player.transform.position.z>11001 && racing){
                racing = false;
                FindObjectOfType<PauseMenu>().enabled = false;
                pauseBtn.SetActive(false);
                speedometer.SetActive(false);
                comSc.SetActive(true);
                int prize;
                switch(currentPosition){
                    case 1: finishPositionText.text = currentPosition.ToString() + "st";
                            prize = 5000 * difficultyMultiplyer;
                            Win(prize);
                    break;
                    case 2: finishPositionText.text = currentPosition.ToString() + "nd";
                            prize = 2500 * difficultyMultiplyer;
                            Win(prize);
                    break;
                    case 3: finishPositionText.text = currentPosition.ToString() + "rd";
                            prize = 1000 * difficultyMultiplyer;
                            Win(prize);
                    break;
                    default: finishPositionText.text = currentPosition.ToString() + "th";
                            prizeText.text = "";
                            coinImage.SetActive(false);
                    break;
                }
                player.GetComponent<Movement>().enabled = false;
                player.GetComponent<AudioListener>().enabled = false;
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FollowPlayer>().enabled = false;
            }
        }
    }

    public void GameOver(){
        explosionSound.Play();
        Invoke("gameEnds", 0.75f);
    }

    private void gameEnds(){
        GameObject[] ai = GameObject.FindGameObjectsWithTag("AI");
        for(int i = 0; i<ai.Length; i++){
            ai[i].GetComponent<AudioSource>().enabled = false;
        }
        Time.timeScale = 0f;
        speedometer.SetActive(false);
        pauseBtn.SetActive(false);
        comSc.SetActive(true);
        displayText.text = "Game Over";
        finishPositionText.text = "";
        prizeText.text = "";
        coinImage.SetActive(false);
        Destroy(player);
    }

    public void Rotation(int reward){
        rotationTxt.GetComponent<Text>().text = reward.ToString();
        int coinAmount = PlayerPrefs.GetInt("Coins", 5000);
        coinAmount += reward;
        PlayerPrefs.SetInt("Coins", coinAmount);
        rotationTxt.SetActive(true);
        rotationTxt.GetComponent<Animator>().enabled = true;
        Invoke("EndRotation", 0.7f);
    }

    private void EndRotation(){
        rotationTxt.GetComponent<Animator>().enabled = false;
        rotationTxt.SetActive(false);
    }

    private void Win(int prize){
        int coinAmount = PlayerPrefs.GetInt("Coins", 5000);
        coinAmount += prize;
        PlayerPrefs.SetInt("Coins", coinAmount);
        prizeText.text = prize.ToString();
    }
}
