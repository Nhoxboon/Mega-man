using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static InputManager Instance { get => instance; }

    protected Vector4 direction;
    public Vector4 Direction => direction;

    protected bool dashInput;
    public bool DashInput => dashInput;


    private void Awake()
    {
        if (InputManager.instance != null)
        {
            Debug.LogError("ONLY 1 instances of InputManager exist");
        }
        InputManager.instance = this;
    }

    private void Update()
    {
        this.GetDirectionByKeyDown();
    }



    protected virtual void GetDirectionByKeyDown()
    {
        this.direction.x = Input.GetKey(KeyCode.A) ? 1 : 0;
        if (this.direction.x == 0) this.direction.x = Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;

        this.direction.y = Input.GetKey(KeyCode.D) ? 1 : 0;
        if (this.direction.y == 0) this.direction.y = Input.GetKey(KeyCode.RightArrow) ? 1 : 0;

        this.direction.z = Input.GetKey(KeyCode.W) ? 1 : 0;
        if (this.direction.z == 0) this.direction.z = Input.GetKey(KeyCode.Space) ? 1 : 0;

        this.dashInput = Input.GetKey(KeyCode.C);


        //if (this.direction.x == 1) Debug.Log("Left");
        //if (this.direction.y == 1) Debug.Log("Right");
        //if (this.direction.z == 1) Debug.Log("Up");
    }


}