using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{

    public Transform m_spawnPoint;
    public GameObject m_playerPrefab;

    public void SpawnPlayer() {
        GameObject go = Instantiate(m_playerPrefab);
        go.transform.position = m_spawnPoint.position;
    }
}
