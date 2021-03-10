using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class UpgradeStates{
    private int[] speed;
    private int[] acceleration;
    private int[] handling;
    private int[] brakes;

    public UpgradeStates(){
        speed = new int[3];
        acceleration = new int[3];
        handling = new int[3];
        brakes = new int[3];
    }

    public int[] Speed{ get {return speed; } }
    public int[] Acceleration{ get {return acceleration; } }
    public int[] Handling{ get {return handling; } }
    public int[] Brakes{ get {return brakes; } }

    public void setSpeed(int up1){
        speed[0] = up1;
        speed[1] = up1 + 3;
        speed[2] = speed[1] + 3;
    }

    public void setAcceleration(int up1){
        acceleration[0] = up1;
        acceleration[1] = up1 + 1;
        acceleration[2] = acceleration[1] + 2;
    }

    public void setHandling(int up1){
        handling[0] = up1;
        handling[1] = up1 + 3;
        handling[2] = handling[1] + 2;
    }

    public void setBrakes(int up1){
        brakes[0] = up1;
        brakes[1] = up1 + 4;
        brakes[2] = brakes[1] + 4;
    }
}
