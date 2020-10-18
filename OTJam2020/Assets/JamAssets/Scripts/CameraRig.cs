using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    public static CameraRig current;

    public Transform player;

    public float m_followSpeed = 3.0f;

    public float minX, minY, maxX, maxY;



    public void Start() {
        current = this;
    }

    public void Update() {
        if (player) {
            Vector3 pos = Vector2.Lerp(transform.position, player.position, Time.deltaTime * m_followSpeed);
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            pos.z = -10.0f;
            transform.position = pos;
            
        }
    }



}
