using UnityEngine;

public class TestLifeCycle : MonoBehaviour
{
    private void Awake()
    {
        logFrame("Awake");
    }

    private void Main()
    {
        logFrame("Main");
    }

    private void Start()
    {
        logFrame("Start");
    }

    private void myupdateMethod()
    {
        logFrame("MyUpdate method");
    }

    private void OnLevelWasLoaded()
    {
        logFrame("OnLevelWasLoaded");
    }

    private void OnNetworkLoadedLevel()
    {
        logFrame("OnNetworkLoadedLevel");
    }

    private void OnEnable()
    {
        logFrame("OnEnable");
    }

    private void OnDisable()
    {
        logFrame("OnDisable");
    }

    private void OnApplicationQuit()
    {
        logFrame("OnApplicationQuit");
    }

    private void Update()
    {
        logFrame("Update");
    }

    private void LateUpdate()
    {
        logFrame("LateUpdate");
    }

    private void FixedUpdate()
    {
        logFrame("FixedUpdate");
    }

    private void LateFixedUpdate()
    {
        logFrame("LateFixedUpdate");
    }

    private void OnDrawGizmos()
    {
        logFrame("OnDrawGizmos");
    }

    private void OnDrawGizmosSelected()
    {
        logFrame("OnDrawGizmosSelected");
    }

    private static void logFrame(string message)
    {
        Debug.Log(Time.frameCount + " - " + Time.realtimeSinceStartup + " - " + message);
    }
}