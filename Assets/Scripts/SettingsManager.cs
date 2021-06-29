using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour{

    private int selected, musicEnable, soundEnable;
    private Color activeColor = new Color(50/255f, 50/255f, 50/255f);
    public GameObject[] difficultyBtns;
    public GameObject canvas, confirmBox, music;
    public Text musicTxt, soundTxt;
    public AudioSource difficultySound, selectSound;
    
    void Awake(){
        selected = PlayerPrefs.GetInt("difficulty_level", 0);
        for(int i = 0; i<difficultyBtns.Length; i++){
            if(i!=selected){
                difficultyBtns[i].GetComponent<Animation>().Play("buttonAnimation");
            }else{
                difficultyBtns[i].GetComponentInChildren<Text>().color = activeColor;
            }
        }

        musicEnable = PlayerPrefs.GetInt("Music", 1);
        if(musicEnable==0) musicTxt.text = "MUSIC   ON";

        soundEnable = PlayerPrefs.GetInt("Sound", 1);
        if(soundEnable==0) soundTxt.text = "SOUND   ON";
    }
    
    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Back();
        }
    }

    public void Back(){
        if(soundEnable==1) selectSound.Play();
        SceneManager.LoadScene(0);
    }

    public void EasyBtn(){
        Deactivate();
        selected = 0;
        PlayerPrefs.SetInt("difficulty_level", 0);
        Activate();
    }

    public void MediumBtn(){
        Deactivate();
        selected = 1;
        PlayerPrefs.SetInt("difficulty_level", 1);
        Activate();
    }

    public void HardBtn(){
        Deactivate();
        selected = 2;
        PlayerPrefs.SetInt("difficulty_level", 2);
        Activate();
    }

    public void ExtremeBtn(){
        Deactivate();
        selected = 3;
        PlayerPrefs.SetInt("difficulty_level", 3);
        Activate();
    }

    private void Activate(){
        if(soundEnable==1) difficultySound.Play();
        difficultyBtns[selected].GetComponent<Animation>().Play("buttonAnimationEnable");
        difficultyBtns[selected].GetComponentInChildren<Text>().color = activeColor;
    }

    private void Deactivate(){
        difficultyBtns[selected].GetComponent<Animation>().Play("buttonAnimation");
        difficultyBtns[selected].GetComponentInChildren<Text>().color = Color.white;
    }

    public void ResetProgress(){
        if(soundEnable==1) selectSound.Play();
        confirmBox.SetActive(true);
        canvas.SetActive(false);
    }

    public void Controls(){
        if(soundEnable==1) selectSound.Play();
        SceneManager.LoadScene(4);
    }

    public void About(){
        if(soundEnable==1) selectSound.Play();
        SceneManager.LoadScene(5);
    }

    public void YesBtn(){
        Progress.DeleteDirectory();
        PlayerPrefs.DeleteAll();
        Back();
    }

    public void NoBtn(){
        if(soundEnable==1) selectSound.Play();
        canvas.SetActive(true);
        confirmBox.SetActive(false);
    }

    public void Music(){
        if(soundEnable==1) selectSound.Play();
        if(musicEnable==0){
            GameObject musicObject = Instantiate(music, transform.position, Quaternion.identity);
            DontDestroyOnLoad(musicObject);
            musicTxt.text = "MUSIC   OFF";
            musicEnable = 1;
            PlayerPrefs.SetInt("Music", musicEnable);
        }else{
            Destroy(GameObject.Find("Music(Clone)"));
            musicTxt.text = "MUSIC   ON";
            musicEnable = 0;
            PlayerPrefs.SetInt("Music", musicEnable);
        }
    }

    public void Sound(){
        if(soundEnable==0){
            soundTxt.text = "SOUND   OFF";
            soundEnable = 1;
            PlayerPrefs.SetInt("Sound", soundEnable);
        }else{
            soundTxt.text = "SOUND   ON";
            soundEnable = 0;
            PlayerPrefs.SetInt("Sound", soundEnable);
        }
        if(soundEnable==1) selectSound.Play();
    }
}
