using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HandleUI : MonoBehaviour
{
    public GameObject m_resetScene, m_playAnim, m_textPosition, m_textReset;
    public Canvas m_canvas;
    private bool m_hidden = true;

    void Start()
    {
        RenderSettings.ambientLight = Color.black;
        Cursor.visible = false;

        Button resetButton = m_resetScene.GetComponent<Button>();

        resetButton.onClick.AddListener(ResetSceneClick);

        m_resetScene.SetActive(false);
        m_textPosition.SetActive(false);
        m_textReset.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
           ShowUI();
           m_hidden = !m_hidden;
        }
    }

    void ShowUI()
    {
        //Output this to console when the Button is clicked
        Debug.Log("You have clicked the button!");
        m_resetScene.SetActive(m_hidden);
        m_textPosition.SetActive(m_hidden);
        m_textReset.SetActive(m_hidden);
    }

    void ResetSceneClick()
    {
        //Output this to console when the Button is clicked
        Debug.Log("You have clicked the button!");

        // Reset the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}