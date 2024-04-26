using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickcheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
    if (Input.GetMouseButtonDown(0)) {  // Check for left mouse button click
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            // You can add conditions to check if the hit object is part of Vuforia's AR
            Debug.Log("Hit " + hit.transform.name);
        }
    }
}
}

