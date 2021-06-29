using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour{
    private bool isPause = false, soundEnable;
    public GameObject pauseMenuUI, pauseBtn;
    public AudioSource upDownSound, selectSound;
    private Text[] uiFields = new Text[3];
    private Color passiveColor = new Color(48/255f, 48/255f, 48/255f);
    private int selected = 0;

    void Awake(){
        soundEnable = PlayerPrefs.GetInt("Sound", 1)==1;

        GameObject[] uiElements = new GameObject[3];
        for(int i = 0; i<uiElements.Length; i++){
            uiElements[i] = pauseMenuUI.transform.GetChild(i).gameObject;
        }

        for(int i = 0; i<uiElements.Length; i++){
            uiFields[i] = uiElements[i].GetComponentInChildren<Text>();
        }
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(isPause){
                Resume();
            }else{
                Pause();
            }
        }else if(Input.GetKeyDown(KeyCode.R)){
            Restart();
        }

        if(isPause){
            Cursor.visible = true;
            if(Input.GetKeyDown("w") || Input.GetKeyDown(KeyCode.UpArrow)){
                passiveButton();
                selected = selected==0 ? 2 : selected-1;
                ActiveButton();
            }else if(Input.GetKeyDown("s") || Input.GetKeyDown(KeyCode.DownArrow)){
                passiveButton();
                selected++;
                selected %= 3;
                ActiveButton();
            }else if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
                if(selected==0){
                    Resume();
                }else if(selected==1){
                    Restart();
                }else if(selected==2){
                    Menu();
                }
            }
        }
    }

    public bool getIsPause(){
        return isPause;
    }

    private void passiveButton(){
        uiFields[selected].color = passiveColor;
        uiFields[selected].fontSize = 72;
    }

    private void ActiveButton(){
        upDownSound.Play();
        uiFields[selected].color = Color.green;
        uiFields[selected].fontSize = 108;
    }

    private void SoundManager(bool condition){
        if(soundEnable){
            GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>().enabled = condition;
            GameObject[] ai = GameObject.FindGameObjectsWithTag("AI");
            for(int i = 0; i<ai.Length; i++){
                ai[i].GetComponent<AudioSource>().enabled = condition;
            }
        }
    }

    public void Resume(){
        selectSound.Play();
        SoundManager(true);
        passiveButton();
        selected = 0;
        ActiveButton();
        pauseMenuUI.SetActive(false);
        pauseBtn.SetActive(true);
        Cursor.visible = false;
        Time.timeScale = 1f;
        isPause = false;
    }

    public void Pause(){
        selectSound.Play();
        SoundManager(false);
        pauseMenuUI.SetActive(true);
        pauseBtn.SetActive(false);
        Cursor.visible = true;
        Time.timeScale = 0f;
        isPause = true;
    }

    public void Restart(){
        selectSound.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu(){
        selectSound.Play();
        SceneManager.LoadScene(0);
    }
}
