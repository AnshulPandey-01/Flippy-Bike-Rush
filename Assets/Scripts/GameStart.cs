using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour{

    private int countDownTime = 3;
    private bool soundEnable;
    public Text countDownText;
    public GameObject startPanel, pauseBtn, speedometer;
    public AudioSource[] countDownSound;
    private GameObject player;
    private GameObject[] ai;

    void Start(){
        Cursor.visible = false;
        Time.timeScale = 1.0f;
        soundEnable = PlayerPrefs.GetInt("Sound", 1)==1;

        player = GameObject.FindGameObjectWithTag("Player");

        ai = GameObject.FindGameObjectsWithTag("AI");
        
        Invoke("startGame", 0.3f);
    }

    void startGame(){
        if(countDownTime>=0){
            Invoke("startGame", 0.9f);
            countDownSound[countDownTime].Play();
            if(countDownTime==2){
                countDownText.color = Color.yellow;
            }else if(countDownTime==1){
                countDownText.color = Color.green;
            }
            countDownText.text = countDownTime.ToString();
            countDownTime--;
        }
        if(countDownTime<0){
            Destroy(startPanel);
            pauseBtn.SetActive(true);
            speedometer.SetActive(true);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FollowPlayer>().enabled = true;
            transform.GetComponent<PauseMenu>().enabled = true;
            player.GetComponent<Movement>().enabled = true;
            for(int i = 0; i<ai.Length; i++){
                ai[i].GetComponent<AI_Controls>().enabled = true;
                if(soundEnable) ai[i].GetComponent<AudioSource>().enabled = true;
            }
        }
    }
}
