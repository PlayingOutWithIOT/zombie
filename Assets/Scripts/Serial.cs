using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class Serial : MonoBehaviour {

    private SerialPort stream = new SerialPort("\\\\.\\COM12", 9600);

    // Use this for initialization
    void Start () {
        stream.Open();
        stream.BaseStream.Flush();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (stream.IsOpen )
        {
            string value = stream.ReadLine();
            Debug.Log("Was line" + value);
        }
    }
}
