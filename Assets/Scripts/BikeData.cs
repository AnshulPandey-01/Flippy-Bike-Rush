using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BikeData{
    private int bikeType;
    private bool unlocked;
    private float speed;
    private float acceleration;
    private float handling;
    private float brakes;
    private int speedLevel;
    private int accelerationLevel;
    private int handlingLevel;
    private int brakesLevel;

    public BikeData(Movement bike, int type){
        BikeType = type;
        Unlocked = false;
        Speed = bike.maxSpeed;
        Acceleration = bike.acceleration;
        Handling = bike.handling;
        Brakes = bike.brakes;
    }

    public BikeData(BikeData bike){
        BikeType = bike.BikeType;
        Unlocked = bike.Unlocked;
        Speed = bike.Speed;
        Acceleration = bike.Acceleration;
        Handling = bike.Handling;
        Brakes = bike.Brakes;
        SpeedLevel = bike.SpeedLevel;
        AccelerationLevel = bike.AccelerationLevel;
        HandlingLevel = bike.HandlingLevel;
        BrakesLevel = bike.BrakesLevel;
    }

    public void UnlockBike(){
        if(bikeType==0) UnlockMotorBike();
        else if(bikeType==1) UnlockSportsBike();
        else if(bikeType==2) UnlockSuperBike();
    }

    private void UnlockMotorBike(){
        UnlockNow();
    }

    private void UnlockSportsBike(){
        UnlockNow();
        Speed = 85f;
        Acceleration = 29f;
        Handling = 34f;
        Brakes = 35f;
        
    }

    private void UnlockSuperBike(){
        UnlockNow();
        Speed = 88f;
        Acceleration = 32f;
        Handling = 42f;
        Brakes = 45f;
    }

    private void UnlockNow(){
        Unlocked = true;
        SpeedLevel = 0;
        AccelerationLevel = 0;
        HandlingLevel = 0;
        BrakesLevel = 0;
    }

    public float Brakes { get => brakes; set => brakes = value; }
    public float Handling { get => handling; set => handling = value; }
    public float Acceleration { get => acceleration; set => acceleration = value; }
    public float Speed { get => speed; set => speed = value; }
    public bool Unlocked { get => unlocked; private set => unlocked = value; }
    public int BikeType { get => bikeType; private set => bikeType = value; }
    public int SpeedLevel { get => speedLevel; set => speedLevel = value; }
    public int AccelerationLevel { get => accelerationLevel; set => accelerationLevel = value; }
    public int HandlingLevel { get => handlingLevel; set => handlingLevel = value; }
    public int BrakesLevel { get => brakesLevel; set => brakesLevel = value; }
}
