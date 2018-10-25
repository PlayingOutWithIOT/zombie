using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Basic Learning:
// Play animations
// Play sound
// Add 2d interface
// Mouse control
// Mouse buttons
//
// Advanced Learning:
// Add a rag-doll dynamically
// Create a shader
//
// Done:
// Keyboard entry
// Transforms
// Applying force to a rigid body

public class MainScript : MonoBehaviour {
    public GameObject Ball = null;

    private Rigidbody rb = null;
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
