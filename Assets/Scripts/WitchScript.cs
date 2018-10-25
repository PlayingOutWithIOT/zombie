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

    // Use this for initialization
    void Start ()
    {
        Button btn1 = m_playAnim.GetComponent<Button>();
        m_anim = m_witchSkin.GetComponent<Animator>();
        m_text = m_textGameObject.GetComponent<Text>();

        btn1.onClick.AddListener(TaskOnClick);

        stream.Open();
        stream.BaseStream.Flush();
    }

    // Update is called once per frame
    void Update () {
        //Witch.transform.Translate(new Vector3(0, cameraSpeed, 0));

        m_position = m_witchSkin.transform.position;

        if (Input.GetKey(KeyCode.Return ))
        {
            m_anim.SetTrigger("Retreat");
        }

        int valueNum = 0;

        if (stream.IsOpen)
        {
            string value = stream.ReadLine();

            valueNum = int.Parse(value);
            if (valueNum > 0)
            {
                Debug.Log("Was line: " + valueNum);

                if (valueNum < 200)
                {
                    if ( !m_anim.GetCurrentAnimatorStateInfo(0).IsName("walk 0") )
                    {
                        m_anim.SetTrigger("Walk");
                    }
                }
            }
        }

        m_text.text = "Position: " +
        m_position.x.ToString("0.00") + "," +
        m_position.y.ToString("0.00") + "," +
        m_position.z.ToString("0.00") + "," +
        valueNum.ToString("0.00");

        m_anim.SetFloat("Position", m_position.z);

    }

    void TaskOnClick()
    {
        Debug.Log("Clicked on start witch anim: " );

        //m_anim.Play("idle");
        m_anim.SetTrigger("Walk");
    }
}
