using UnityEngine;

public class FollowPlayer : MonoBehaviour{

    private GameObject player;
    private Vector3 offset;
    private Vector3 spaceOffset;
    private Quaternion initialRotation;
    [HideInInspector]
    public bool playerAlive = true;
    public GameObject dirLight;
    private bool addAngle = true;

    void Awake(){
        player = GameObject.FindGameObjectWithTag("Player");
        offset = transform.position;
        spaceOffset = offset;
        spaceOffset.z = 3;
        initialRotation = transform.rotation;
    }

    void LateUpdate() {

        if(playerAlive){
            if(Input.GetKey(KeyCode.Space)){
                transform.position = player.transform.position + spaceOffset;
                Vector3 angle = transform.rotation.eulerAngles;
                if(addAngle){
                    angle.y += 180;
                    transform.rotation = Quaternion.Euler(angle);
                    dirLight.SetActive(true);
                    addAngle = false;
                }
            }else{
                transform.position = player.transform.position + offset;
                if(!addAngle){
                    transform.rotation = initialRotation;
                    dirLight.SetActive(false);
                    addAngle = true;
                }
            }
        }

    }
}
