using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaundryBasket : MonoBehaviour
{
    public Animator m_animator;

    public AudioSource m_audioSource;

    private void OnTriggerEnter2D(Collider2D other) {
        Player player = null;
        if (other.gameObject.TryGetComponent<Player>(out player)) {
            Destroy(other.gameObject);
            m_animator.SetTrigger("Kerchunk");
            LevelManager.currentLevel.Victory();
        }
    }

    public void PlaySound() {
        m_audioSource.Play();
    }
}
