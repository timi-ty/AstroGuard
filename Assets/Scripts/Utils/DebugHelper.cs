//In Progress
using UnityEngine;
using UnityEditor;

public class DebugHelper
{

    public static void LogWarning(string message, GameObject gameObject)
    {
        Debug.LogWarning("Warning logged by " + gameObject.name + " in level " + GameManager.currentLevel + ": " + message);
    }

    public static void LogError(string message, GameObject gameObject)
    {
        Debug.LogError("Error logged by " + gameObject.name + " in level " + GameManager.currentLevel + ": " + message);
    }

    public static void Log(string message, GameObject gameObject)
    {
        Debug.Log("Messaged logged by " + gameObject.name + " in level " + GameManager.currentLevel + ": " + message);
    }

    public static UnityException ThrowException(string message, GameObject gameObject) 
    {
        throw new UnityException("Exception thrown by " + gameObject.name + " in level " + GameManager.currentLevel + ": " + message);
    }
}