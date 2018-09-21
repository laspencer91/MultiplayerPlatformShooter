using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpawnPointGizmo))]
public class SpawnPoint : NetworkStartPosition
{
    [SerializeField] public Team team;
}
