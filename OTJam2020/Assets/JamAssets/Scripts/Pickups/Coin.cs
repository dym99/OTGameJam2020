using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public GameObject m_coinEffect;


    private void OnTriggerEnter2D(Collider2D other) {
        Player player = null;
        if (other.TryGetComponent<Player>(out player)) {
            GameObject go = Instantiate(m_coinEffect);
            go.transform.position = this.transform.position;
            Destroy(go, 1.0f);
            Destroy(this.gameObject);
            LevelManager.currentLevel.ScorePoints(1000);
        }
    }
}
