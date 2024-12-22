using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NhoxMonoBehaviour : MonoBehaviour
{

    protected virtual void Awake()
    {
        this.LoadComponents();
    }

    protected virtual void Start()
    {
        //For override
    }


    protected virtual void LoadComponents()
    {
        //For override
    }

    public virtual void Reset()
    {
        this.LoadComponents();
    }


    protected virtual void OnEnable()
    {
        //For override
    }

    protected virtual void OnDisable()
    {
        //For override
    }
}
