using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class ByteDataHelper
{

    public static byte[] GameDataToBytes(GameData gameData)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryFormatter.Serialize(memoryStream, gameData);

            return memoryStream.ToArray();
        }
    }

    public static GameData BytesToGameData(byte[] bytes)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (MemoryStream memoryStream = new MemoryStream(bytes))
        {
            return (GameData) binaryFormatter.Deserialize(memoryStream);
        }
    }
}
