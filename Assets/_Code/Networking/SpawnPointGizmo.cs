using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpawnPointGizmo : MonoBehaviour
{
    Color baseColor;

    [SerializeField] Vector3 boxSize = Vector3.one;

    private void OnDrawGizmos()
    {
        Team team = GetComponent<SpawnPoint>().team;

        if (team == Team.Green) baseColor = Color.green; else baseColor = Color.red;

        Gizmos.color = baseColor;
        Gizmos.DrawWireCube(transform.position, boxSize);

        Color insideColor = baseColor;
        insideColor.a = 0.15f;

        Gizmos.color = insideColor;
        Gizmos.DrawCube(transform.position, boxSize);
    }
}
