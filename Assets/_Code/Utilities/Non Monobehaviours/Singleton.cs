using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton
{
    /// <summary>
    /// This method will take the given Type and make sure that if others of its type already exist, then the
    /// object passed as a parameter destroys itself.
    /// </summary>
    /// <typeparam name="T">A component to make a singleton(Monobehaviour)</typeparam>
    /// <param name="obj">The gameobject that is calling this method</param>
    /// <param name="dontDestroyOnLoad">Do you want this object to be destroyed on scene switch?</param>
    public static void AssertSingletonState<T>(GameObject obj, bool dontDestroyOnLoad = false) where T : MonoBehaviour
    {
        int numberOfMe = GameObject.FindObjectsOfType<T>().Length;
        if (numberOfMe > 1)
        {
            GameObject.DestroyImmediate(obj);
            Debug.Log("Singleton: " + typeof(T).Name + " was destroyed because an instance of this type already exists");
        }

        if (dontDestroyOnLoad)
            GameObject.DontDestroyOnLoad(obj);
    }
}
