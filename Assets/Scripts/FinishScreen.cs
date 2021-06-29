using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinishScreen : MonoBehaviour{

    public GameObject comSc;
    public Text restartText, menuText;
    public AudioSource upDownSound, selectSound;
    private Text[] uiFields = new Text[2];
    private Color passiveColor = new Color(48/255f, 48/255f, 48/255f);
    private Color activeColor = new Color(0/255f, 96/255f, 255/255f);
    private int selected = 0;

    void Awake(){
        uiFields[0] = restartText;
        uiFields[1] = menuText;
    }

    void Update(){
        if(comSc.activeSelf){
            Cursor.visible = true;
            if(Input.GetKeyDown(KeyCode.R)){
                Restart();
            }
            
            if(Input.GetKeyDown("w") || Input.GetKeyDown(KeyCode.UpArrow)){
                passiveButton();
                selected = selected==0 ? 1 : selected-1;
                ActiveButton();
            }else if(Input.GetKeyDown("s") || Input.GetKeyDown(KeyCode.DownArrow)){
                passiveButton();
                selected++;
                selected %= 2;
                ActiveButton();
            }else if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
                if(selected==0){
                    Restart();
                }else if(selected==1){
                    Menu();
                }
            }
        }else{
            Cursor.visible = false;
        }
    }

    void passiveButton(){
        uiFields[selected].color = passiveColor;
        uiFields[selected].fontSize = 72;
    }

    void ActiveButton(){
        Debug.Log("fi");
        upDownSound.Play();
        uiFields[selected].color = activeColor;
        uiFields[selected].fontSize = 108;
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
