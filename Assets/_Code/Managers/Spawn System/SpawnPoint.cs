using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] public Team team;
}

public enum Team { None = -2, All = -1, Red, Green, Count }
