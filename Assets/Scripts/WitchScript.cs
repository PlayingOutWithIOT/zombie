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
    public GameObject m_witchSkin;
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
    private List<int> list = new List<int>();
    private String m_comPort;
    private Thread mThread;
    private int m_avValue = 0;
    private Vector3 m_originalPosition;

    enum ZombieState {
        Idle,
        Walking,
        Attack,
        Retreat,
    }

    // Use this for initialization
    void Start ()
    {
        // Setup the buttons
        Button btn1 = m_playAnim.GetComponent<Button>();
        m_anim = m_witchSkin.GetComponent<Animator>();
        m_text = m_textGameObject.GetComponent<Text>();
        m_zombieMoanSource = m_zombieMoan.GetComponent<AudioSource>();

        btn1.onClick.AddListener(TaskOnClick);

        ChangeState(ZombieState.Idle);

        m_originalPosition = m_zombie.transform.position;

        ThreadStart ts = new ThreadStart(AsyncRead);
        mThread = new Thread(ts);
        mThread.Start();
    }

    void AsyncRead()
    {
        XmlDocument xmlDoc = new XmlDocument();

        string xml = File.ReadAllText("setup.xml");

        xmlDoc.LoadXml(xml);

        XmlNode root = xmlDoc.DocumentElement;

        XmlNode firstChildElement = root.FirstChild;

        m_comPort = root["COM"].InnerText;
        Debug.Log("COM port: " + m_comPort);

        // Open the com port e.g. "\\\\.\\COM12"
        //m_comPort = "\\\\.\\COM13";
        try
        {
            if (stream == null)
            {
                stream = new SerialPort(m_comPort, 9600);

                stream.Open();
                stream.BaseStream.Flush();
            }

            if (stream.IsOpen)
            {
                while (true)
                {
                    // Read the stream
                    string value = stream.ReadLine();
                    //Debug.Log("COM value: " + value);

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

                    int avNum = 0;

                    // Check the list
                    if (list.Count > 10)
                    {
                        // Average a list
                        for (int i = 0; i < list.Count; i++)
                        {
                            avNum += list[i];
                        }
                        avNum /= list.Count;
                    }
                    m_avValue = avNum;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Could not open COM port: " + e.Message);
        }
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy");
        mThread.Abort();
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
   
        if (Input.GetKey(KeyCode.W))
        {
            Walk();
        }

        if (Input.GetKey(KeyCode.R))
        {
            Retreat();
        }

        // Average between 10 and 100
        if (state == ZombieState.Idle)
        {
            if (m_avValue < 100)
            {
                Walk();
                Debug.Log("Set animation to walk: " + animStartTime);
            }
        }
        else if (state == ZombieState.Attack)
        {
            // Average between 10 and 100
            if( (m_avValue < 50 ) && (elapsedTime > 5.0f) )
            {
                Retreat();
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
        else if (state == ZombieState.Retreat)
        {
            if (elapsedTime > 10.0f)
            {
                ChangeState(ZombieState.Idle);
                m_anim.Play("idle");
                m_zombie.transform.position = m_originalPosition;
                list.Clear();
                m_avValue = 0;
                //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        m_text.text = "Version 31.13\nVars: " +
        m_position.z.ToString("0.00") + "\n" +
        m_avValue.ToString("0.00") + "\n" +
        elapsedTime.ToString("0.00") + "\n" +
        "List count " + list.Count + "\n" +
        m_comPort;
    }

    void TaskOnClick()
    {
        Debug.Log("Clicked on start witch anim: " );

        //m_anim.Play("idle");
        m_anim.SetTrigger("Walk");
    }
}
