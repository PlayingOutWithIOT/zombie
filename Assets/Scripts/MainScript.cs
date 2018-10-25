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
        if (Input.GetKey(KeyCode.R))
        {
            m_MyEvent.Invoke();

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKey(KeyCode.L))
        {
            //   // Ball.transform.Translate(new Vector3(0, cameraSpeed, 0));
            Debug.Log("Apply force");

            if (Ball)
            {
                rb = Ball.GetComponent<Rigidbody>();

                const float thrust = 0.001f;
                rb.AddForce(0, 0, thrust, ForceMode.Impulse);
            }
        }
    }
}
