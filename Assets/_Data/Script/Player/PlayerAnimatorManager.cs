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
        bool isShooting = playerCtrl.PlayerShooting.IssShooting;

        switch (playerCtrl.PlayerMovement.StateManager)
        {
            case PlayerState.Idle:
                anim.SetInteger("state", 0);
                anim.SetBool("isShooting", isShooting);
                break;
            case PlayerState.Run:
                anim.SetInteger("state", 1);
                anim.SetBool("isShooting", isShooting);
                break;
            case PlayerState.Jump:
                anim.SetInteger("state", 2);
                anim.SetBool("isShooting", isShooting);
                break;
            case PlayerState.Fall:
                anim.SetInteger("state", 3);
                anim.SetBool("isShooting", isShooting);
                break;
            case PlayerState.WallSliding:
                anim.SetInteger("state", 5);
                //anim.SetBool("isShooting", isShooting);
                break;
            case PlayerState.WallJumping:
                anim.SetInteger("state", 6);
                break;
            case PlayerState.Dash:
                anim.SetInteger("state", 4);
                anim.SetBool("isShooting", isShooting);
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

    public override void Reset()
    {
        anim.SetBool("isShooting", false);
    }
}
