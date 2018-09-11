using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameConsole : MonoBehaviour
{
    TextMeshProUGUI consoleText;

    static List<string> textEntries = new List<string>();

    static int maxLines = 6;

    void Start()
    {
        Singleton.AssertSingletonState<GameConsole>(gameObject);

        consoleText = GetComponentInChildren<TextMeshProUGUI>();
        InvokeRepeating("UpdateText", 0, 0.16f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) AddLine("Player Has Joined The Server");
    }

    private void UpdateText()
    {
        consoleText.text = "";

        foreach (string entry in textEntries)
        {
            consoleText.text += entry;
        }
    }

    public static void AddLine(string text)
    {
        textEntries.Add("\n" + text);

        if (textEntries.Count > maxLines)
        {
            textEntries.RemoveAt(0);
        }
    }

    public static void DebugLog(string text)
    {
        textEntries.Add("\n" + text);

        if (textEntries.Count > maxLines)
        {
            textEntries.RemoveAt(0);
        }

        Debug.Log(text);
    }
}
