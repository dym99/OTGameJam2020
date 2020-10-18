using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D m_rigidBody;
    public SpriteRenderer m_sprite;
    public Animator m_animator;
    public Animator m_wingAnimator;
    public Animator m_lightningAnimator;
    public Indicator m_wingdicator;
    public Indicator m_dashIndicator;

    public AudioSource m_audioSource;

    public AudioClip m_jumpSound;
    public AudioClip m_dashSound;

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
        if (m_hasDoubleJump) {
            m_wingAnimator.gameObject.SetActive(true);
        }
        if (m_hasDash) {
            m_lightningAnimator.gameObject.SetActive(true);
        }

        m_wingdicator.wallSliding = (m_state==PlayerState.WALLSLIDE);
        m_dashIndicator.wallSliding = m_wingdicator.wallSliding;
        switch (m_state) {
            case PlayerState.NORMAL:
                if (Input.GetButtonDown("Jump")) {
                    
                    Vector2 vel = m_rigidBody.velocity;
                    if (m_grounded) {
                        vel.y = m_jumpSpeed;
                        m_animator.SetTrigger("Jump");
                        m_audioSource.clip = m_jumpSound;
                        m_audioSource.Play();
                    } else {
                        if (m_hasDoubleJump) {
                            vel.y = m_jumpSpeed;
                            m_hasDoubleJump = false;
                            m_animator.SetTrigger("Jump");
                            m_audioSource.clip = m_jumpSound;
                            m_audioSource.Play();
                            m_wingAnimator.SetTrigger("Flap");
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
                        m_audioSource.clip = m_dashSound;
                        m_audioSource.Play();
                        m_lightningAnimator.SetTrigger("Dash");
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
                    m_animator.SetTrigger("Jump");
                    m_audioSource.clip = m_jumpSound;
                    m_audioSource.Play();
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
                    m_audioSource.clip = m_dashSound;
                    m_audioSource.Play();
                    m_lightningAnimator.SetTrigger("Dash");
                }
                break;
        }
    }

    public void FixedUpdate() {
        Collider2D other = Physics2D.OverlapBox(new Vector2(this.transform.position.x, this.transform.position.y) + Vector2.down * 0.55f, new Vector2(0.8f, 0.05f), 0, LayerMask.GetMask("World"));
        if (other) {
            m_grounded = true;
        } else {
            m_grounded = false;
        }

        m_otherR = Physics2D.OverlapBox(new Vector2(this.transform.position.x, this.transform.position.y) + Vector2.right * 0.47f, new Vector2(0.05f, 0.7f), 0, LayerMask.GetMask("World"));
        m_otherL = Physics2D.OverlapBox(new Vector2(this.transform.position.x, this.transform.position.y) + Vector2.right * -0.47f, new Vector2(0.05f, 0.7f), 0, LayerMask.GetMask("World"));

        Vector2 vel = m_rigidBody.velocity;
        switch (m_state) {
            case PlayerState.NORMAL:
                NormalPhysicsUpdate();
                
                if (m_xSpeedTarget > 0) {
                    m_sprite.flipX = false;
                } else if (m_xSpeedTarget < 0) {
                    m_sprite.flipX = true;
                }

                m_animator.SetTrigger("Normal");
                break;
            case PlayerState.DASHING:
                vel.y = 0;
                m_rigidBody.velocity = vel;

                m_animator.SetTrigger("Dash");
                break;
            case PlayerState.WALLSLIDE:
                WallSlideUpdate();
                if (m_otherL) {
                    m_sprite.flipX = true;
                } else if (m_otherR) {
                    m_sprite.flipX = false;
                }
                m_animator.SetTrigger("WallSlide");
                break;
            case PlayerState.WALLDASHING:
                vel.x = 0;
                m_rigidBody.velocity = vel;

                m_animator.SetTrigger("WallDash");
                break;
        }

        m_animator.SetBool("Grounded", m_grounded);
        m_animator.SetFloat("xSpeed", Mathf.Abs(m_rigidBody.velocity.x));
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
