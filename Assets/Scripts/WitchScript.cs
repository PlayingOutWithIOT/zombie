using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WitchScript : MonoBehaviour {

    public GameObject m_playAnim;
    public GameObject m_textGameObject;
    public GameObject m_zombieMoan;
    public GameObject m_zombie;

    private Text m_text;
    private Vector3 m_position;
    private AudioSource m_zombieMoanSource;
    Animator m_anim;

    private SerialPort stream = null;
    private ZombieState state;
    private float animStartTime = 0;
    private List<int> m_list = new List<int>();
    private String m_comPort;
    private Thread mThread;
    private float m_avValue = 0;
    private Vector3 m_originalPosition;
    private float m_triggerDistance = 0;
    public bool m_isRunning = true;

    enum ZombieState {
        Idle,
        Walking,
        Attack,
        Retreat,
    }

    // Use this for initialization
    void Start ()
    {
        XmlDocument xmlDoc = new XmlDocument();

        string xml = File.ReadAllText("setup.xml");

        xmlDoc.LoadXml(xml);

        XmlNode root = xmlDoc.DocumentElement;

        XmlNode firstChildElement = root.FirstChild;

        m_comPort = root["COM"].InnerText;
        m_triggerDistance = float.Parse(root["DISTANCE"].InnerText);
        Debug.Log("COM port: " + m_comPort);


        // Setup the buttons
        Button btn1 = m_playAnim.GetComponent<Button>();
        m_anim = m_zombie.GetComponent<Animator>();
        m_text = m_textGameObject.GetComponent<Text>();
        m_zombieMoanSource = m_zombieMoan.GetComponent<AudioSource>();

        btn1.onClick.AddListener(TaskOnClick);

        ChangeState(ZombieState.Idle);

        m_originalPosition = m_zombie.transform.position;

        mThread = new Thread( RunThread );
        mThread.Start( this );
    }

    void RunThread(object data)
    {
        var threadController = (WitchScript)data;

        Debug.Log("Thread argument " + threadController.m_isRunning );

        // Open the com port e.g. "\\.\COM12"
        try
        {
            if (stream == null)
            {
                stream = new SerialPort(m_comPort, 9600);

                stream.Open();
                stream.BaseStream.Flush();
            }
        }
        catch (Exception e)
        {
            Debug.Log("Could not open COM port: " + e.Message);
        }

        int checkCount = 0;

        while (threadController.m_isRunning)
        {
            try
            {
                if (stream.IsOpen)
                {
                    // Read the stream
                    string value = stream.ReadLine();
                   
                    checkCount++;

                    // Get the value
                    int valueNum = int.Parse(value);

                    // Add to the list
                    if (valueNum > 0)
                    {
                        if (m_list.Count > 10)
                        {
                            m_list.RemoveAt(0);
                        }
                        m_list.Add(valueNum);
                    }

                    int avNum = 0;

                    // Check the list
                    if (m_list.Count > 10)
                    {
                        // Average a list
                        for (int i = 0; i < m_list.Count; i++)
                        {
                            avNum += m_list[i];
                        }
                        avNum /= m_list.Count;
                    }
                    m_avValue = (float) avNum;
                    //m_avValue = (float) valueNum;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Could not read from COM port: " + e.Message);
            }
        }
        Debug.Log("My own thread ended with " + checkCount + " iterations.");
    }

    private void OnDestroy()
    {
        m_isRunning = false;
        Debug.Log("OnDestroy");
    }

    void Reset()
    {
        m_zombie.transform.position = m_originalPosition;
        m_list.Clear();
        m_avValue = 0;
        m_anim.ResetTrigger("Attack");
        m_anim.ResetTrigger("Walk");
        m_anim.ResetTrigger("Retreat");
        m_anim.Play("idle");
        ChangeState(ZombieState.Idle);
        m_zombieMoanSource.Play();
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
        m_position = m_zombie.transform.position;

        if (Input.GetKey(KeyCode.Return ))
        {
            Retreat();
        }

        float elapsedTime = Time.time - animStartTime;
   
        if (Input.GetKey(KeyCode.W))
        {
            Walk();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Retreat();
        }

        // Average between 10 and 100
        if (state == ZombieState.Idle)
        {
            if(( m_avValue > 10 ) && (m_avValue < m_triggerDistance))
            {
                Walk();
                Debug.Log("Set animation to walk: " + animStartTime);
            }
        }
        else if (state == ZombieState.Walking)
        {
            if (m_position.z > 8.0f)
            {
                ChangeState(ZombieState.Attack);
                m_anim.SetTrigger("Attack");
                Debug.Log("Set animation to attack: " + animStartTime);
            }
        }
        else if (state == ZombieState.Attack)
        {
            // Make the zombie retreat if we are in close
            if ((m_avValue > 10) && (m_avValue < m_triggerDistance) && (elapsedTime > 5.0f))
            {
                Retreat();
            }
            if (elapsedTime > 20.0f)
            {
                Retreat();
            }
        }
        else if (state == ZombieState.Retreat)
        {
            if (elapsedTime > 10.0f)
            {
                Reset();
                //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        m_text.text = "Version 31.2\n" +
        "Zombie position: " + m_position.z.ToString("0.00") + "\n" +
        "Sensed distance: " + m_avValue.ToString("0.00") + " " +
        "Elapsed: " + elapsedTime.ToString("0.00") + "\n" +
        "Trigger: " + m_triggerDistance.ToString("0.00") + "\n" +
        "COM Port: " + m_comPort;
    }

    void TaskOnClick()
    {
        Debug.Log("Clicked on start zombie anim: " );

        //m_anim.Play("idle");
        m_anim.SetTrigger("Walk");
    }
}
