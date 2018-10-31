using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainScript : MonoBehaviour {
    private UnityEvent m_MyEvent = null;

    // Use this for initialization
    void Start () {

        if (m_MyEvent == null)
            m_MyEvent = new UnityEvent();

        m_MyEvent.AddListener(WasReset);
    }

    void WasReset()
    {
        Debug.Log("Was reset!");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            m_MyEvent.Invoke();

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
