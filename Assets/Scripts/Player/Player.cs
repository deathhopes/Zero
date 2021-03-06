﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    private Animator anim;

    // Movement variables
    public Rigidbody2D rb;
    private Vector3 moveDirection;
    private const float MOVE_SPEED = 7.5f; //5.333f

    //Tumble
    private Vector3 tumbleDirection;
    private float tumbleSpeed;
    private Vector3 lastMoveDirection;
    public Ghost ghost;

    //Combat 
    public int hitpoint = 10;
    public int maxHitpoint = 10;
    public GameObject floatingText;

    //State Machine
    public enum State
    {
        Normal,
        Attacking,
        Tumbling,
        Dead
    }

    public State state;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        state = State.Normal;
    }

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(1);
        }
        switch (state)
        {
            case State.Normal:

                //Movement Detection and activating animation based on the direction pressed.
                float moveX, moveY;
                DetectPlayerMovement(out moveX, out moveY);
                MovementAnimation(moveX, moveY);

                //Tumble Detection
                if (Input.GetKeyDown(KeyCode.Space))
                    {
                        //Create Initial tumble Ghost Effect
                        ghost.makeGhost = true;

                        tumbleDirection = lastMoveDirection;
                        tumbleSpeed = 40f;
                        state = State.Tumbling;
                        //Roll Animation
                        TumblingAnimation();
                    }

                break;

            case State.Tumbling:
                ReduceTumbleSpeedOverTime();
                break;

            case State.Attacking:
                //Stop Movement
                rb.velocity = Vector3.zero;
                anim.SetBool("moving", false);
                break;
        }
    }

    protected virtual void FixedUpdate()
    {
        switch (state) 
            {
                case State.Normal:
                    //Create movement where necessary.
                    rb.velocity = moveDirection * MOVE_SPEED;
                    
                    break;

                case State.Tumbling:
                    //Create Additional Tumble ghosts
                    ghost.makeGhost = true;

                    //Use last direction and tumble that way.
                    rb.velocity = tumbleDirection * tumbleSpeed;
                    break;
            }
    }
    private void DetectPlayerMovement(out float moveX, out float moveY)
    {
        moveX = 0f;
        moveY = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            moveY = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveY = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = +1f;
        }

        moveDirection = new Vector3(moveX, moveY).normalized;
        if (moveX != 0 || moveY != 0)
        {
            //Not Idle
            lastMoveDirection = moveDirection;
        }
    }

    private void MovementAnimation(float moveX, float moveY)
    {
        if (moveDirection != Vector3.zero)
        {
            anim.SetFloat("moveX", moveX);
            anim.SetFloat("moveY", moveY);
            anim.SetBool("moving", true);
        }
        else
        {
            anim.SetBool("moving", false);
        } 
    }

    private void ReduceTumbleSpeedOverTime()
    {
        float tumbleSpeedDropMultiplier = 3.3f;
        tumbleSpeed -= tumbleSpeed * tumbleSpeedDropMultiplier * Time.deltaTime;

        float tumbleSpeedMinimum = 1f;
        if (tumbleSpeed < tumbleSpeedMinimum)
        {
            state = State.Normal;
        }
    }
 
    private void TumblingAnimation()
    {
        anim.SetTrigger("tumbling");
    }

    public void TakeDamage(int damage)
    {
        if (state != State.Tumbling)
        {
            hitpoint -= damage;

            Debug.Log("Player HP: " + hitpoint);

            HUD.instance.OnHitPointChange();

            //Instantiate, change color and text
            Instantiate(floatingText, transform.position, Quaternion.identity, transform);
            floatingText.GetComponent<TextMeshPro>().color = Color.red;
            floatingText.GetComponent<TextMeshPro>().text = damage.ToString();

            //Play hurt animation
            // anim.SetTrigger("Hurt");
        }
    }
}
