//In Progress
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class GameDataManager
{
    private static string SubPath { get; set; } = "/SaveFile.bin";
    private static string FilePath { get; set; } = Application.persistentDataPath + SubPath;

    public static GameData GameData 
    { 
        get => GameData.Instance;
        set 
        { 
            GameData.Instance = value; 
        } 
    }

    public static void SaveGameData()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        FileStream saveStream = new FileStream(FilePath, FileMode.Create);

        binaryFormatter.Serialize(saveStream, GameData);

        saveStream.Close();
    }

    public static void LoadGameData()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        try
        {
            FileStream loadStream = new FileStream(FilePath, FileMode.Open);

            GameData = (GameData) binaryFormatter.Deserialize(loadStream);

            loadStream.Close();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message + ". Error loading save file at: " + FilePath);

            SaveGameData();
        }
    }

    #region Only Called When Authenticated
    public static GameData MoreRecent(GameData incomingGameData)
    {
        System.DateTime dtLocal = GameData.TimeStamp;
        System.DateTime dtIncoming = incomingGameData.TimeStamp;

        int comparison = System.DateTime.Compare(dtLocal, dtIncoming);

        if (comparison < 0) return incomingGameData;
        else return GameData;
    }

    public static bool IsCompatibleWith(GameData incomingGameData)
    {
        if (incomingGameData == null || incomingGameData.UserID == null) return false;

        bool isValidUID = incomingGameData.UserID.Equals(GameData.UserID) && GameData.UserID == FirebaseUtility.CurrentUser.UserId;
        bool isValidDeviceID = GameData.DeviceID.Equals(incomingGameData.DeviceID);

        return isValidUID && isValidDeviceID;
    }

    public static bool IsUploadable()
    {
        return GameData.UserID == FirebaseUtility.CurrentUser.UserId || GameData.UserID == null;
    }
    #endregion

    public static void ResetProgress(){
        GameData.Clear();
        SaveGameData();
    }
}
