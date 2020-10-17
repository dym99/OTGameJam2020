using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaundryBasket : MonoBehaviour
{
    public Animator m_animator;

    private void OnTriggerEnter2D(Collider2D other) {
        Player player = null;
        if (other.gameObject.TryGetComponent<Player>(out player)) {
            Destroy(other.gameObject);
            m_animator.SetTrigger("Kerchunk");
        }
    }
}
