using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public enum PowerupType {
        JUMP,
        DASH,
    }

    public PowerupType m_type;
    public bool m_ready;
    public GameObject spriteObject;
    public float m_rechargeTime = 5.0f;

    public AudioSource m_audioSource;

    private void Start() {
        m_ready = true;
    }

    private void Update() {
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Player player=null;
        if (m_ready) {
            if (other.gameObject.TryGetComponent<Player>(out player)) {
                switch (m_type) {
                    case PowerupType.JUMP:
                        player.m_hasDoubleJump = true;
                        break;
                    case PowerupType.DASH:
                        player.m_hasDash = true;
                        break;
                }

                m_ready = false;
                spriteObject.SetActive(false);
                StartCoroutine(Recharge());
                m_audioSource.Play();
            }
        }
    }

    private IEnumerator Recharge() {
        yield return new WaitForSeconds(m_rechargeTime);

        m_ready = true;
        spriteObject.SetActive(true);
    }
}
