using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HandleUI : MonoBehaviour {
    public GameObject m_resetScene, m_playAnim, m_textPosition, m_textReset;
    public Canvas m_canvas;

    void Start()
    {
        Button resetButton = m_resetScene.GetComponent<Button>();
        Button playButton = m_playAnim.GetComponent<Button>();
       
        resetButton.onClick.AddListener(ResetSceneClick);
       
        m_resetScene.SetActive(false);
        m_playAnim.SetActive(false);
        m_textPosition.SetActive(false);
        m_textReset.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.H))
        {
            ShowUI();
        }
    }

    void ShowUI()
    {
        //Output this to console when the Button is clicked
        Debug.Log("You have clicked the button!");
        m_resetScene.SetActive(true);
        m_playAnim.SetActive(true);
        m_textPosition.SetActive(true);
        m_textReset.SetActive(true);
    }

    void ResetSceneClick()
    {
        //Output this to console when the Button is clicked
        Debug.Log("You have clicked the button!");

        // Reset the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void TaskWithParameters(string message)
    {
        //Output this to console when the Button is clicked
        //Debug.Log(message);
    }
}