using UnityEngine;

public class Movement : MonoBehaviour{

    private Rigidbody rb;
    private Animator[] wheelAnimation;
    public ParticleSystem gameOverEffect;
    private int layeMask, rotationCountFront = 0, rotationCountBack = 0;
    private int[] rotationReward = {250, 500, 1000};
    private bool check = true, velocityCheck = false, rotationCheck = true;
    private bool gameOverCheck = false, lands = false, soundEnable;
    private float rotationSpeed = 4.0f, rotationSum = 0.0f;
    public float acceleration = 25f, maxSpeed = 80f, brakes = 25f, handling = 28f;

    void Start(){
        Time.timeScale = 1.0f;
        soundEnable = PlayerPrefs.GetInt("Sound", 1)==1;

        rb = gameObject.GetComponent<Rigidbody>();

        wheelAnimation = new Animator[2];
        wheelAnimation[0] = gameObject.transform.GetChild(0).GetComponent<Animator>();
        wheelAnimation[1] = gameObject.transform.GetChild(1).GetComponent<Animator>();
 
        layeMask = 1 << 8;
        layeMask = ~layeMask;
    }

    void FixedUpdate(){
        bool grounded = IsGrounded();

        if((Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow)) && grounded){
            rb.AddForce(0, 0, acceleration);
        }else if((Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow)) && !grounded){
            transform.Rotate(rotationSpeed, 0, 0);
            rotationSum += rotationSpeed;
            if(rotationSum>270.0f && rotationCheck){
                FindObjectOfType<GameManager>().Rotation(rotationReward[rotationCountFront]);
                rotationCheck = false;
                rotationCountFront++;
            }
            if(rotationSum>360.0f){
                rotationSum = 0.0f;
                rotationCheck = true;
            }
        }else if((Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow)) && !grounded){
            transform.Rotate(-rotationSpeed, 0, 0);
            rotationSum -= rotationSpeed;
            if(rotationSum<-270.0f && rotationCheck){
                FindObjectOfType<GameManager>().Rotation(rotationReward[rotationCountBack]*2);
                rotationCheck = false;
                rotationCountBack++;
            }
            if(rotationSum<-360.0f){
                rotationSum = 0.0f;
                rotationCheck = true;
            }
        }

        if((Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow)) && grounded && rb.velocity.z>1.0f){
            rb.AddForce(0, 0, -brakes);
        }

        if(grounded){
            rotationSum = 0.0f;
            rotationCountFront = 0;
            rotationCountBack = 0;
            rotationCheck = true;
            velocityCheck = true;
        }

        if(!grounded && velocityCheck){
            Vector3 v = rb.velocity;
            if(v.y>25.0f){
                rb.velocity = new Vector3(0.0f, 25.0f, v.z);
            }else{
                rb.velocity = new Vector3(0.0f, v.y, v.z);
            }
            velocityCheck = false;
        }

        if(rb.velocity.magnitude > maxSpeed){
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        if(transform.position.y>1.0f){
            lands = true;
        }else if(grounded && lands){
            if(soundEnable) gameObject.transform.GetChild(0).GetComponent<AudioSource>().Play();
            lands = false;
        }

        if(rb.velocity.z < 1.0f){
            if(soundEnable) transform.GetComponent<AudioSource>().enabled = false;
            wheelAnimation[0].enabled = false;
            wheelAnimation[1].enabled = false;
        }else if(!wheelAnimation[0].isActiveAndEnabled){
            if(soundEnable) transform.GetComponent<AudioSource>().enabled = true;
            wheelAnimation[0].enabled = true;
            wheelAnimation[1].enabled = true;
        }

        if(grounded && check){
            rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            check = false;
        }else if(!grounded){
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            check = true;
        }

        float zRotation = getAngle(transform.eulerAngles.z);

        if(grounded){
            Vector3 v = rb.velocity;
            if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey("d")){
                if(zRotation>-25f){
                    transform.Rotate(0, 0, -1f);
                }
                if(v.x<0){
                    rb.AddForce(45.0f, 0, 0);
                }
                rb.AddForce(handling, 0, 0);
            }else if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey("a")){
                if(zRotation<25f){
                    transform.Rotate(0, 0, 1f);
                }
                if(v.x>0){
                    rb.AddForce(-45.0f, 0, 0);
                }
                rb.AddForce(-handling, 0, 0);
            }
        }

        if(transform.position.x>12.00f){
            Vector3 v = rb.velocity;
            rb.velocity = new Vector3(-2.00f, v.y, v.z);
        }else if(transform.position.x<-12.00f){
            Vector3 v = rb.velocity;
            rb.velocity = new Vector3(2.00f, v.y, v.z);
        }

        bool noInput = (!Input.GetKey(KeyCode.RightArrow) && 
        !Input.GetKey(KeyCode.LeftArrow) && 
        !Input.GetKey("a") && 
        !Input.GetKey("d"));

        if(grounded && noInput){
            Vector3 v = rb.velocity;
            if(zRotation>=-1.0f && zRotation<=1.0f){
                Vector3 myRotation = transform.eulerAngles;
                myRotation.z = 0.0f;
                transform.rotation = Quaternion.Euler(myRotation);
                rb.velocity = new Vector3(0.0f, v.y, v.z);
            }
            if(zRotation<-1.0f){
                transform.Rotate(0, 0, 0.8f);
                rb.velocity = new Vector3(v.x-(v.x/5), v.y, v.z);
            }
            if(zRotation>1.0f){
                transform.Rotate(0, 0, -0.8f);
                rb.velocity = new Vector3(v.x-(v.x/5), v.y, v.z);
            }
        }
    }

    void Update() {
        if(transform.position.y>1.0f){
            gameOverCheck = true;
        }

        if(gameOverCheck && (transform.rotation.x>0.5f || transform.rotation.x<-0.5f) && IsGrounded()){
            gameOverFunction();
        }else if(IsGrounded() && !gameOverCheck){
            setRotationZ();
            setRotationY();
            setRotationX();
        }

        if(IsGrounded()){
            gameOverCheck = false;
        }
    }

    void gameOverFunction(){
        Instantiate(gameOverEffect, transform.position, Quaternion.identity);
        FindObjectOfType<GameManager>().GameOver();
        FindObjectOfType<FollowPlayer>().playerAlive = false;
        FindObjectOfType<GameManager>().playerAlive = false;
        FindObjectOfType<PauseMenu>().enabled = false;
    }

    bool IsGrounded(){
        Vector3 pos = transform.position;
        pos.y += 0.5f;
        return Physics.Raycast(pos, Vector3.down, 1.8f, layeMask);
    }

    float getAngle(float Angle){
        if (Angle > 180) Angle -= 360;
        return Angle;
    }

    void setRotationZ(){
        float zRotation = getAngle(transform.eulerAngles.z);
        if(!(zRotation<30f && zRotation>-30f)){
            Vector3 myRotation = transform.eulerAngles;
            myRotation.z = 0.0f;
            transform.rotation = Quaternion.Euler(myRotation);
        }
    }

    void setRotationY(){
        double yRotation = getAngle(transform.eulerAngles.y);
        if(yRotation>0.1f || yRotation<-0.1f){
            Vector3 myRotation = transform.eulerAngles;
            myRotation.y = 0.0f;
            transform.rotation = Quaternion.Euler(myRotation);
        }
    }

    void setRotationX(){
        float xRotation = getAngle(transform.eulerAngles.x);
        if(xRotation>1.0f || xRotation<-1.0f){
            Vector3 myRotation = transform.eulerAngles;
            myRotation.x = 0f;
            transform.rotation = Quaternion.Euler(myRotation);
        }
    }
}
