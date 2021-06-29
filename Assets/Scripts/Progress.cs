using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Progress{

    public static void SaveBike(string fileName, Movement bike, int type){
        BinaryFormatter formatter = new BinaryFormatter();

        if(!Directory.Exists(Directory.GetCurrentDirectory()+"/FBR_Data")){
            Directory.CreateDirectory(Directory.GetCurrentDirectory()+"/FBR_Data");
        }
        string path = Directory.GetCurrentDirectory() + "/FBR_Data/" + fileName + ".data";

        FileStream file = File.Create(path);

        BikeData data = new BikeData(bike, type);

        formatter.Serialize(file, data);

        file.Close();
    }

    public static void SaveBike(string fileName, BikeData bike){
        BinaryFormatter formatter = new BinaryFormatter();

        if(!Directory.Exists(Directory.GetCurrentDirectory()+"/FBR_Data")){
            Directory.CreateDirectory(Directory.GetCurrentDirectory()+"/FBR_Data");
        }
        string path = Directory.GetCurrentDirectory() + "/FBR_Data/" + fileName + ".data";

        FileStream file = File.Create(path);

        BikeData data = new BikeData(bike);

        formatter.Serialize(file, data);

        file.Close();
    }

    public static bool check(string fileName){
        if(Directory.Exists(Directory.GetCurrentDirectory()+"/FBR_Data")){
            string path = Directory.GetCurrentDirectory() + "/FBR_Data/" + fileName + ".data";
            if(File.Exists(path)){
                return true;
            }else{
                return false;
            }
        }else{
            return false;
        }
    }

    public static BikeData LoadBike(string fileName){
        string path = Directory.GetCurrentDirectory() + "/FBR_Data/" + fileName + ".data";
        if(File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);

            BikeData data = formatter.Deserialize(file) as BikeData;

            file.Close();

            return data;
        }else{
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void DeleteDirectory(){
        if(Directory.Exists(Directory.GetCurrentDirectory()+"/FBR_Data")){
            Directory.Delete(Directory.GetCurrentDirectory()+"/FBR_Data", true);
        }
    }
}
