using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsManager : MonoBehaviour{

    public AudioSource selectSound;
    

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Back();
        }else if(Input.GetKeyDown("k")){
            int coinAmount = PlayerPrefs.GetInt("Coins", 5000);
            coinAmount += 10000;
            PlayerPrefs.SetInt("Coins", coinAmount);
        }
    }

    public void Back(){
        if(PlayerPrefs.GetInt("Sound", 1)==1) selectSound.Play();
        SceneManager.LoadScene(3);
    }
}
