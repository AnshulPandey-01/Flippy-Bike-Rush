using UnityEngine;

public class AI_Controls : MonoBehaviour{

    System.Random rnd = new System.Random();
    private Rigidbody rb;
    private GameObject sets;
    private Transform[][] pointSet;
    private Transform nextLocation;
    private Animator animFront, animBack;
    private int layeMask, indexNO = 0, hitCounter = 0;
    public float acceleration = 30.0f, maxVelocity = 75.0f;
    private float handlingSpeed = 16.0f, rightLoc = 0.0f, leftLoc = 0.0f;
    private bool check = false, locationAssigned = false, isFinnished = false, lands = false, soundEnable;

    void Start(){
        soundEnable = PlayerPrefs.GetInt("Sound", 1)==1;
        rb = gameObject.GetComponent<Rigidbody>();
        animFront = gameObject.transform.GetChild(0).GetComponent<Animator>();
        animBack = gameObject.transform.GetChild(1).GetComponent<Animator>();

        layeMask = 1 << 8;
        layeMask = ~layeMask;

        sets = GameObject.Find("PointSets");
        pointSet = new Transform[sets.transform.childCount][];
        for(int i = 0; i<sets.transform.childCount; i++){
            GameObject point = sets.transform.GetChild(i).gameObject;
            Transform[] set = new Transform[point.transform.childCount];
            for(int j = 0; j<point.transform.childCount; j++){
                set[j] = point.transform.GetChild(j);
            }
            pointSet[i] = set;
        }

        animFront.enabled = true;
        animBack.enabled = true;
    }

    void FixedUpdate(){
        functionAssignLocation();

        bool grounded = IsGrounded();

        if(nextLocation.position.z>transform.position.z){
            if(grounded){
                rb.AddForce(0, 0, acceleration);
            }
        }else{
            locationAssigned = false;
        }

        RaycastHit hit;
        bool obstacleHit = Physics.Raycast(transform.position, Vector3.forward, out hit, 35.0f, layeMask) 
        && hit.transform.tag=="Obstacle";
        if(obstacleHit){
            hitCounter++;
            if(rb.velocity.z>20.0f){
                rb.AddForce(0, 0, -30);
            }
            handlingSpeed = 40.0f;
        }else{
            hitCounter = 0;
            handlingSpeed = 28.0f;
        }

        if(hitCounter>300){
            resetPosition();
        }

        if(rb.velocity.magnitude > maxVelocity){
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }

        if(soundEnable){
            if(transform.position.y>1.0f){
                lands = true;
            }else if(grounded && lands){
                gameObject.transform.GetChild(0).GetComponent<AudioSource>().Play();
                lands = false;
            }
        }

        if(grounded && check){
            rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            check = false;
        }else if(!grounded){
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            check = true;
        }

        float zRotation = getAngle(transform.eulerAngles.z);
        bool tilt = false;
        
        if(grounded && !(leftLoc<=transform.position.x && rightLoc>=transform.position.x)){
            tilt = true;
            Vector3 v = rb.velocity;
            if(nextLocation.position.x>transform.position.x){
                if(zRotation>-25f){
                    transform.Rotate(0, 0, -1f);
                }
                if(v.x<0){
                    rb.AddForce(51.0f, 0, 0);
                }
                rb.AddForce(handlingSpeed, 0, 0);
            }else if(nextLocation.position.x<transform.position.x){
                if(zRotation<25f){
                    transform.Rotate(0, 0, 1f);
                }
                if(v.x>0){
                    rb.AddForce(-51.0f, 0, 0);
                }
                rb.AddForce(-handlingSpeed, 0, 0);
            }
        }

        if(transform.position.x>12.00f){
            Vector3 v = rb.velocity;
            rb.velocity = new Vector3(-2.00f, v.y, v.z);
        }else if(transform.position.x<-12.00f){
            Vector3 v = rb.velocity;
            rb.velocity = new Vector3(2.00f, v.y, v.z);
        }

        if(!tilt && grounded){
            Vector3 v = rb.velocity;
            zRotation = getAngle(transform.eulerAngles.z);
            if(zRotation>=-1.0f && zRotation<=1.0f){
                Vector3 myRotation = transform.eulerAngles;
                myRotation.z = 0.0f;
                transform.rotation = Quaternion.Euler(myRotation);
                rb.velocity = new Vector3(0.0f, v.y, v.z);
            }
            if(zRotation<-1.0f){
                transform.Rotate(0, 0, 0.8f);
                rb.velocity = new Vector3(v.x-(v.x/3), v.y, v.z);
            }
            if(zRotation>1.0f){
                transform.Rotate(0, 0, -0.8f);
                rb.velocity = new Vector3(v.x-(v.x/3), v.y, v.z);
            }
        }

        if(grounded){
            setRotationZ();
            setRotationY();
            setRotationX();
        }
    }

    private void functionAssignLocation(){
        if(!locationAssigned && !isFinnished){
            if((pointSet[indexNO][0].position.z - transform.position.z)>100){
                int randomIndex;
                if(FindObjectOfType<GameManager>().caller%2==0){
                    randomIndex = rnd.Next(0, pointSet[indexNO].Length);
                }else{
                    randomIndex = UnityEngine.Random.Range(0, pointSet[indexNO].Length);
                }
                nextLocation = pointSet[indexNO][randomIndex];
            }else{
                float minimum = 101;
                float min = 101;
                Transform next = pointSet[indexNO][0];
                foreach(Transform loc in pointSet[indexNO]){
                    min = Mathf.Abs(loc.position.x - transform.position.x);
                    if(minimum>min){
                        minimum = min;
                        next = loc;
                    }
                }
                nextLocation = next;
            }
            FindObjectOfType<GameManager>().caller++;
            locationAssigned = true;
            if(indexNO+1<sets.transform.childCount){
                indexNO++;
            }else{
                isFinnished = true;
            }
            leftLoc = nextLocation.position.x - 0.7f;
            rightLoc = nextLocation.position.x + 0.7f;
        }
    }

    private bool IsGrounded(){
        Vector3 pos = transform.position;
        pos.y += 0.5f;
        return Physics.Raycast(pos, Vector3.down, 1.8f, layeMask);
    }

    private float getAngle(float Angle){
        if (Angle > 180) Angle -= 360;
        return Angle;
    }

    private void resetPosition(){
        Vector3 pos = transform.position;
        pos.z -= 8;
        transform.position = pos;
        hitCounter = 0;
    }

    private void setRotationZ(){
        float zRotation = getAngle(transform.eulerAngles.z);
        if(!(zRotation<30f && zRotation>-30f)){
            Vector3 myRotation = transform.eulerAngles;
            myRotation.z = 0.0f;
            transform.rotation = Quaternion.Euler(myRotation);
        }
    }

    private void setRotationY(){
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
