using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public SpriteRenderer m_playerSprite;
    public SpriteRenderer m_thisSprite;

    private float xOffset = 0;

    public bool wallSliding = false;

    public void Start() {
        xOffset = transform.localPosition.x;
    }

    public void Update() {
        m_thisSprite.flipX = m_playerSprite.flipX;
        Vector3 pos = transform.localPosition;
        
        if (m_playerSprite.flipX) {
            pos.x = -xOffset;
        } else {
            pos.x = xOffset;
        }
        if (wallSliding) {
            pos.x *=0.4f;
        }
        transform.localPosition = pos;
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
