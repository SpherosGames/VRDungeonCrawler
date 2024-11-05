using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VRLogSystem : MonoBehaviour
{
    public TextMeshProUGUI logText;  // Assign this in the Inspector
    private string logMessages = ""; // To store all log messages
    private HashSet<string> loggedMessages = new HashSet<string>(); // Track unique messages

    void OnEnable()
    {
        // Subscribe to log event when the script is enabled
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        // Unsubscribe from log event when the script is disabled
        Application.logMessageReceived -= HandleLog;
    }

    // Handle the log messages
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Only handle regular Log messages (ignore Warnings and Errors)
        if (type == LogType.Log && !loggedMessages.Contains(logString))
        {
            // If it's a new log, add it to the HashSet
            loggedMessages.Add(logString);

            // Append the new log message to the log string
            logMessages += logString + "\n";

            // Update the Text UI component with the log messages
            logText.text = logMessages;
        }
    }
}
