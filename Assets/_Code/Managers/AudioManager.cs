using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private static Dictionary<string, Sound> soundMap = new Dictionary<string, Sound>();

    void Awake()
    {
        Singleton.AssertSingletonState<AudioManager>(gameObject, true);

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.pitch = sound.pitch;
            sound.source.volume = sound.volume;

            soundMap[sound.name] = sound;
        }
    }

    /// <summary>
    /// Play sound with the given name in the Audio Manager. If no sound was found
    /// it will be ignored and a warning logged.
    /// </summary>
    /// <param name="name">Sound name as set up in audio manager</param>
    public static void Play(string name)
    {
        if (soundMap.ContainsKey(name))
        {
            Sound soundToPlay = soundMap[name];
            soundToPlay.source.Play();
        }
        else
        {
            Debug.LogWarning("The sound " + name + " does not exist in the Audio Manager Sound Mapping");
        }
    }

    /// <summary>
    /// Play sound with some variance in its original pitch.
    /// </summary>
    /// <param name="name">Name of the sound to play</param>
    /// <param name="maxPitchVariance">Allowed range for pitch to vary</param>
    public static void Play(string name, float maxPitchVariance)
    {
        if (soundMap.ContainsKey(name))
        {
            Sound soundToPlay = soundMap[name];
            float pitch = soundToPlay.source.pitch;

            soundToPlay.source.pitch = soundToPlay.pitch + Random.Range(-maxPitchVariance, maxPitchVariance);
            soundToPlay.source.Play();
        }
        else
        {
            Debug.LogWarning("The sound " + name + " does not exist in the Audio Manager Sound Mapping");
        }
    }
}

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0, 1)]
    public float volume;

    [Range(0.1f, 3)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;
}