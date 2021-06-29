using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Upgrade{
    private int[] speed;
    private int[] acceleration;
    private int[] handling;
    private int[] brakes;

    public Upgrade(){
        speed = new int[3];
        acceleration = new int[3];
        handling = new int[3];
        brakes = new int[3];
    }

    public int[] Speed{ get {return speed; } }
    public int[] Acceleration{ get {return acceleration; } }
    public int[] Handling{ get {return handling; } }
    public int[] Brakes{ get {return brakes; } }

    public void setSpeedPrice(int up1, int up2, int up3){
        speed[0] = up1;
        speed[1] = up2;
        speed[2] = up3;
    }

    public void setAccelerationPrice(int up1, int up2, int up3){
        acceleration[0] = up1;
        acceleration[1] = up2;
        acceleration[2] = up3;
    }

    public void setHandlingPrice(int up1, int up2, int up3){
        handling[0] = up1;
        handling[1] = up2;
        handling[2] = up3;
    }

    public void setBrakesPrice(int up1, int up2, int up3){
        brakes[0] = up1;
        brakes[1] = up2;
        brakes[2] = up3;
    }
}
