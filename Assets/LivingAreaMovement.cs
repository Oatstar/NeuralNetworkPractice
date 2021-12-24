using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingAreaMovement : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            MoveUp();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveRight();
        if (Input.GetKeyDown(KeyCode.DownArrow))
            MoveDown();
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveLeft();
    }

    void MoveUp()
    {
        this.transform.position = this.transform.position + new Vector3(0, 5,0);
    }

    void MoveRight()
    {
        this.transform.position = this.transform.position + new Vector3(5, 0, 0);
    }

    void MoveDown()
    {
        this.transform.position = this.transform.position + new Vector3(0, -5, 0);
    }

    void MoveLeft()
    {
        this.transform.position = this.transform.position + new Vector3(-5, 0, 0);
    }


}
