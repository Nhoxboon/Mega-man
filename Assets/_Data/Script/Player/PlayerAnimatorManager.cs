using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : NhoxMonoBehaviour
{
    [SerializeField] protected PlayerCtrl playerCtrl;
    [SerializeField] protected Animator anim;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        LoadPlayerCtrl();
        LoadAnimator();
    }

    protected virtual void FixedUpdate()
    {
        this.UpdateAnimator();
    }

    protected virtual void UpdateAnimator()
    {
       
        switch(playerCtrl.PlayerMovement.StateManager)
        {
            case PlayerState.Idle:
                anim.SetInteger("state", 0);
                break;
            case PlayerState.Run:
                anim.SetInteger("state", 1);
                break;
            case PlayerState.Jump:
                anim.SetInteger("state", 2);
                break;
            case PlayerState.Fall:
                anim.SetInteger("state", 3);
                break;
            case PlayerState.WallSliding:
                //
                break;
            case PlayerState.WallJumping:
                //
                break;
            case PlayerState.Dash:
                anim.SetInteger("state", 4);
                break;
        }
    }

    void LoadPlayerCtrl()
    {
        if (playerCtrl != null) return;
        playerCtrl = GetComponentInParent<PlayerCtrl>();
        Debug.Log(transform.name + " loaded PlayerCtrl", gameObject);
    }

    void LoadAnimator()
    {
        if (anim != null) return;
        anim = GetComponent<Animator>();
        Debug.Log(transform.name + " loaded Animator", gameObject);
    }
}
