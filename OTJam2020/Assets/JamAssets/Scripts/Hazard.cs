using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        Player player;
        if (other.gameObject.TryGetComponent<Player>(out player)) {
            player.Die();
        }
    }
}
