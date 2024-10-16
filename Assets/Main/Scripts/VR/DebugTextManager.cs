using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class DebugTextManager : MonoBehaviour
{
    public static DebugTextManager Instance;

    private TMP_Text debugText;

    private Dictionary<string, object> debugTexts = new();
    private Dictionary<string, float> messageTexts = new();

    private void Awake()
    {
        Instance = this;

        debugText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(UpdateTexts), 0f, 0.05f);
    }

    //I want to get the variable name from the object
    //public static void AddText<T>(System.Func<T> value)
    //{
    //    string name = "No name found";
    //    FieldInfo[] fields = typeof(Program).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    //    foreach (FieldInfo field in fields)
    //    {
    //        if (field.GetValue(value).Equals(value))
    //        {
    //            name = field.Name;
    //        }
    //    }
    //    AddText(name, value);
    //}

    public static void AddMessage(string text, float duration = 0)
    {
        if (!Instance) return;
        if (!Instance.messageTexts.ContainsKey(text))
        {
            Instance.messageTexts.Add(text, duration);
        }
        else
        {
            //Reset timer
            Instance.messageTexts[text] = duration;
        }
    }

    public static void AddText(string text, object value)
    {
        if (!Instance) return;
        if (!Instance.debugTexts.ContainsKey(text))
        {
            Instance.debugTexts.Add(text, value);
        }
        else
        {
            if (value is float floatValue)
            {
                value = Mathf.Round(floatValue * 100) / 100;
            }
            //Update value
            Instance.debugTexts[text] = value;
        }
    }

    private void UpdateTexts()
    {
        debugText.SetText(string.Empty);

        if (debugTexts.Count <= 0 && messageTexts.Count <= 0) return;

        for (int i = 0; i < messageTexts.Count; i++)
        {
            debugText.text += $"*{messageTexts.ElementAt(i).Key}*\n";
        }

        //Count down message timers
        for (int i = 0; i < messageTexts.Count; i++)
        {
            string key = messageTexts.ElementAt(i).Key;

            if (messageTexts.TryGetValue(key, out float value))
            {
                value -= 0.05f;

                if (value <= 0)
                {
                    messageTexts.Remove(key);
                }
                else
                {
                    messageTexts[key] = value;
                }
            }
        }

        for (int i = 0; i < debugTexts.Count; i++)
        {
            debugText.text += debugTexts.ElementAt(i) + "\n";
        }
    }
}