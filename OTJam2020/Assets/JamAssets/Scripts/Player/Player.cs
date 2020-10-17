using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D m_rigidBody;


    public float m_maxSpeed = 6.0f;
    public float m_acceleration = 24.0f;
    public float m_jumpSpeed = 9.5f;

    [Range(0, 1)]
    public float m_airControl = 0.5f;

    public float m_wallSlideSpeed = 1.0f;

    public bool m_hasDoubleJump = true;
    public bool m_hasDash = true;

    private float m_xSpeedTarget = 0.0f;

    private bool m_grounded = false;

    private bool m_slideOnCooldown = false;

    Collider2D m_otherL = null;
    Collider2D m_otherR = null;

    private enum PlayerState { 
        NORMAL,
        DASHING,
        WALLSLIDE,
        WALLDASHING,
    }
    private PlayerState m_state;

    public void Awake() {
        m_xSpeedTarget = 0.0f;
        m_state = PlayerState.NORMAL;
    }

    public void Update() {
        m_xSpeedTarget = Input.GetAxis("Horizontal") * m_maxSpeed;

        switch (m_state) {
            case PlayerState.NORMAL:
                if (Input.GetButtonDown("Jump")) {
                    
                    Vector2 vel = m_rigidBody.velocity;
                    if (m_grounded) {
                        vel.y = m_jumpSpeed;
                    } else {
                        if (m_hasDoubleJump) {
                            vel.y = m_jumpSpeed;
                            m_hasDoubleJump = false;
                        }
                    }
                    m_rigidBody.velocity = vel;
                }
                if (Input.GetButtonDown("Dash")) {
                    if (m_hasDash && m_xSpeedTarget != 0.0f) {
                        m_hasDash = false;
                        Vector2 vel = m_rigidBody.velocity;
                        vel.y = 0;
                        vel.x = m_xSpeedTarget > 0 ? m_maxSpeed * 4.0f : -m_maxSpeed * 4.0f;
                        m_rigidBody.gravityScale = 0.0f;
                        m_rigidBody.velocity = vel;
                        m_state = PlayerState.DASHING;
                        StartCoroutine(DashRoutine());
                    }
                }
                break;
            case PlayerState.WALLSLIDE:
                if (Input.GetButtonDown("Jump")) {
                    Vector2 vel = m_rigidBody.velocity;
                    if (m_otherR) {
                        //Jump Left
                        vel.x = -m_maxSpeed;
                    } else if (m_otherL) {
                        //Jump Right
                        vel.x = m_maxSpeed;
                    }
                    vel.y = m_jumpSpeed * 0.75f;
                    m_rigidBody.velocity = vel;
                    m_state = PlayerState.NORMAL;
                    m_slideOnCooldown = true;
                    StartCoroutine(WallHopRoutine());
                }
                if (Input.GetButtonDown("Dash") && m_hasDash) {
                    m_hasDash = false;
                    Vector2 vel = m_rigidBody.velocity;
                    vel.x = 0;
                    vel.y = m_maxSpeed;
                    m_rigidBody.gravityScale = 0.0f;
                    m_rigidBody.velocity = vel;
                    m_state = PlayerState.WALLDASHING;
                    StartCoroutine(DashRoutine());
                }
                break;
        }
    }

    public void FixedUpdate() {
        Collider2D other = Physics2D.OverlapBox(new Vector2(this.transform.position.x, this.transform.position.y) + Vector2.down * 0.55f, new Vector2(0.9f, 0.05f), 0);
        if (other) {
            m_grounded = true;
        } else {
            m_grounded = false;
        }

        m_otherR = Physics2D.OverlapBox(new Vector2(this.transform.position.x, this.transform.position.y) + Vector2.right * 0.53f, new Vector2(0.01f, 0.9f), 0);
        m_otherL = Physics2D.OverlapBox(new Vector2(this.transform.position.x, this.transform.position.y) + Vector2.right * -0.53f, new Vector2(0.01f, 0.9f), 0);

        Vector2 vel = m_rigidBody.velocity;
        switch (m_state) {
            case PlayerState.NORMAL:
                NormalPhysicsUpdate();
                break;
            case PlayerState.DASHING:
                vel.y = 0;
                m_rigidBody.velocity = vel;
                break;
            case PlayerState.WALLSLIDE:
                WallSlideUpdate();
                break;
            case PlayerState.WALLDASHING:
                vel.x = 0;
                m_rigidBody.velocity = vel;
                break;
        }
    }

    private IEnumerator DashRoutine() {

        yield return new WaitForSeconds(0.1f);

        m_rigidBody.gravityScale = 1.0f;
        m_state = PlayerState.NORMAL;

    }
    private IEnumerator WallDashRoutine() {

        yield return new WaitForSeconds(0.1f);

        m_rigidBody.gravityScale = 1.0f;
        m_state = PlayerState.NORMAL;

        Vector2 vel = m_rigidBody.velocity;
        vel.y = m_maxSpeed;
        m_rigidBody.velocity = vel;

    }

    private IEnumerator WallHopRoutine() {

        yield return new WaitForSeconds(0.2f);

        m_slideOnCooldown = false;
    }

    private void WallSlideUpdate() {
        Vector3 vel = m_rigidBody.velocity;

        vel.x = m_xSpeedTarget*0.02f;

        if (vel.y < -m_wallSlideSpeed) {
            vel.y = -m_wallSlideSpeed;
        }
        
        if ((!m_otherL && !m_otherR)||m_grounded) {
            m_state = PlayerState.NORMAL;
            vel.x = m_xSpeedTarget * 0.5f;
        }

        m_rigidBody.velocity = vel;
    }


    private void NormalPhysicsUpdate() {
        Vector3 vel = m_rigidBody.velocity;

        float control = 1.0f;
        if (!m_grounded) {
            control = m_airControl;
        }

        if (vel.x > m_xSpeedTarget) {
            vel.x -= m_acceleration * control * Time.deltaTime;
            if (vel.x < m_xSpeedTarget) {
                vel.x = m_xSpeedTarget;
            }
        } else if (vel.x < m_xSpeedTarget) {
            vel.x += m_acceleration * control * Time.deltaTime;
            if (vel.x > m_xSpeedTarget) {
                vel.x = m_xSpeedTarget;
            }
        }

        vel.x = Mathf.Clamp(vel.x, -m_maxSpeed, m_maxSpeed);

        if ((m_otherR||m_otherL)&&!m_grounded && !m_slideOnCooldown) {
            m_state = PlayerState.WALLSLIDE;
        }

        m_rigidBody.velocity = vel;
    }
}
