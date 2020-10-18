using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePlatform : MonoBehaviour
{
    public Animator m_animator;


    private void OnTriggerEnter2D(Collider2D other) {
        Squish();
    }

    private void OnTriggerExit2D(Collider2D other) {
        Pop();
    }

    public void Squish() {
        m_animator.SetTrigger("Squish");
    }

    public void Pop() {
        m_animator.SetTrigger("Pop");
    }
}
