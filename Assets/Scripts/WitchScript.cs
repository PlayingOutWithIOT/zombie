using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WitchScript : MonoBehaviour {

    public GameObject m_playAnim;
    public GameObject m_witchSkin;
    public GameObject m_textGameObject;
    private Text m_text;
    private Vector3 m_position;
    Animator m_anim;

    // Use this for initialization
    void Start ()
    {
        Button btn1 = m_playAnim.GetComponent<Button>();
        m_anim = m_witchSkin.GetComponent<Animator>();
        m_text = m_textGameObject.GetComponent<Text>();

        btn1.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void Update () {
        //Witch.transform.Translate(new Vector3(0, cameraSpeed, 0));

        m_position = m_witchSkin.transform.position;
        m_text.text = "Position: " + 
            m_position.x.ToString("#.##") + "," +
            m_position.y.ToString("#.##") + "," +
            m_position.z.ToString("#.##");

        m_anim.SetFloat("Position", m_position.z);

        if (Input.GetKey(KeyCode.Return ))
        {
            m_anim.SetTrigger("Retreat");
        }
    }

    void TaskOnClick()
    {
        Debug.Log("Clicked on start witch anim: " );

        //m_anim.Play("idle");
        m_anim.SetTrigger("Walk");
    }
}
