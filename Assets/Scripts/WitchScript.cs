﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;

public class WitchScript : MonoBehaviour {

    public GameObject m_playAnim;
    public GameObject m_witchSkin;
    public GameObject m_textGameObject;
    public GameObject m_zombieMoan;

    private Text m_text;
    private Vector3 m_position;
    private AudioSource m_zombieMoanSource;
    Animator m_anim;

    private SerialPort stream = new SerialPort("\\\\.\\COM12", 9600);
    private ZombieState state;
    private float animStartTime = 0;
    private List<int> list = new List<int>();

    enum ZombieState {
        Idle,
        Walking,
        Attack,
        Retreat,
    }

    // Use this for initialization
    void Start ()
    {
        Button btn1 = m_playAnim.GetComponent<Button>();
        m_anim = m_witchSkin.GetComponent<Animator>();
        m_text = m_textGameObject.GetComponent<Text>();
        m_zombieMoanSource = m_zombieMoan.GetComponent<AudioSource>();

        btn1.onClick.AddListener(TaskOnClick);

        stream.Open();
        stream.BaseStream.Flush();

        ChangeState(ZombieState.Idle);
    }

    void ChangeState(ZombieState newState)
    {
        state = newState;
        animStartTime = Time.time;
    }

    void Walk()
    {
        m_anim.SetTrigger("Walk");
        ChangeState(ZombieState.Walking);
    }

    void Retreat()
    {
        m_zombieMoanSource.Stop();
        m_anim.SetTrigger("Retreat");
        ChangeState(ZombieState.Retreat);
    }

    // Update is called once per frame
    void Update () {
        // Get the transform
        m_position = m_witchSkin.transform.position;

        if (Input.GetKey(KeyCode.Return ))
        {
            Retreat();
        }

        float elapsedTime = Time.time - animStartTime;
        int avNum = 0;

        if (Input.GetKey(KeyCode.W))
        {
            Walk();
        }
        if (Input.GetKey(KeyCode.R))
        {
            Retreat();
        }

        if (stream.IsOpen)
        {
            // Read the stream
            string value = stream.ReadLine();

            try
            {
                // Get the value
                {
                    int valueNum = int.Parse(value);

                    // Add to the list
                    if (valueNum > 0)
                    {
                        if (list.Count > 10)
                        {
                            list.RemoveAt(0);
                        }
                        list.Add(valueNum);
                    }
                }

                // Check the list
                if (list.Count > 10)
                {
                    // Average a list
                    for (int i = 0; i < list.Count; i++)
                    {
                        avNum += list[i];
                    }
                    avNum /= list.Count;

                    // Average between 10 and 100
                    if (state == ZombieState.Idle)
                    {
                        if (avNum < 100)
                        {
                            Walk();
                            Debug.Log("Set animation to walk: " + animStartTime);
                        }
                    }
                    if (state == ZombieState.Attack)
                    {
                        // Average between 10 and 100
                        if( (avNum < 50 ) && (elapsedTime > 5.0f) )
                        {
                            Retreat();
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
        if (state == ZombieState.Walking)
        {
            if (m_position.z > 8.0f)
            {
                ChangeState(ZombieState.Attack);
                m_anim.SetTrigger("Attack");
                Debug.Log("Set animation to attack: " + animStartTime);
            }
        }
        if (state == ZombieState.Retreat)
        {
        }

        m_text.text = "Vars: " +
        m_position.z.ToString("0.00") + "\n" +
        avNum.ToString("0.00") + "\n" +
        elapsedTime.ToString("0.00") + "\n" +
        "List count " + list.Count;
    }

    void TaskOnClick()
    {
        Debug.Log("Clicked on start witch anim: " );

        //m_anim.Play("idle");
        m_anim.SetTrigger("Walk");
    }
}
