using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    CarController carController;
    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<CarController>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            carController.forward = true;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            carController.back = true;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            carController.left = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            carController.right = true;
        }
    }
}
