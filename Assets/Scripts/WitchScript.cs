using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;

public class WitchScript : MonoBehaviour {

    public GameObject m_playAnim;
    public GameObject m_witchSkin;
    public GameObject m_textGameObject;
    private Text m_text;
    private Vector3 m_position;
    Animator m_anim;

    private SerialPort stream = new SerialPort("\\\\.\\COM12", 9600);
    private float animStartTime = 0;
    private bool isWalking = false;
    private List<int> list = new List<int>();

    // Use this for initialization
    void Start ()
    {
        Button btn1 = m_playAnim.GetComponent<Button>();
        m_anim = m_witchSkin.GetComponent<Animator>();
        m_text = m_textGameObject.GetComponent<Text>();

        btn1.onClick.AddListener(TaskOnClick);

        stream.Open();
        stream.BaseStream.Flush();

        animStartTime = 0;
        isWalking = false;
    }

    // Update is called once per frame
    void Update () {
        // Get the transform
        m_position = m_witchSkin.transform.position;

        if (Input.GetKey(KeyCode.Return ))
        {
            m_anim.SetTrigger("Retreat");
        }

        int valueNum = 0;
        float elapsedTime = Time.time - animStartTime;

        if (stream.IsOpen)
        {
            string value = stream.ReadLine();

            try
            {
                valueNum = int.Parse(value);

                if (valueNum > 0)
                {
                    list.Add(valueNum);
                }

                if (list.Count > 10)
                {
                    list.RemoveAt(0);

                    // Average a list
                    int avNum = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        avNum += list[i];
                    }
                    avNum /= list.Count;

                    // Average between 10 and 100
                    if( valueNum < 100 )
                    {
                        if (!isWalking)
                        {
                            isWalking = true;

                            animStartTime = Time.time;
                            m_anim.SetTrigger("Walk");

                            Debug.Log("Set animation to walk: " + animStartTime);
                        }
                    }

                    // Average between 10 and 100
                    if( valueNum < 50 )
                    {
                        if (elapsedTime > 5.0f)
                        {
                            m_anim.SetTrigger("Retreat");
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        m_text.text = "Vars: " +
        m_position.z.ToString("0.00") + "\n" +
        valueNum.ToString("0.00") + "\n" +
        elapsedTime.ToString("0.00");

        m_anim.SetFloat("Position", m_position.z);
    }

    void TaskOnClick()
    {
        Debug.Log("Clicked on start witch anim: " );

        //m_anim.Play("idle");
        m_anim.SetTrigger("Walk");
    }
}
