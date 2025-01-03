using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : NhoxMonoBehaviour
{
    [SerializeField] protected PlayerMovement playerMovement;
    public PlayerMovement PlayerMovement => playerMovement;
    [SerializeField] protected PlayerAnimatorManager playerAnimatorManager;
    public PlayerAnimatorManager PlayerAnimatorManager => playerAnimatorManager;

    [SerializeField] protected PlayerShooting playerShooting;
    public PlayerShooting PlayerShooting => playerShooting;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        LoadPlayerMovement();
        LoadPlayerAnimatorManager();
        LoadPlayerShooting();
    }

    void LoadPlayerMovement()
    {
        if (playerMovement != null) return;
        playerMovement = GetComponentInChildren<PlayerMovement>();
        Debug.Log(transform.name + " loaded PlayerMovement", gameObject);
    }

    void LoadPlayerAnimatorManager()
    {
        if (playerAnimatorManager != null) return;
        playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();
        Debug.Log(transform.name + " loaded PlayerAnimatorManager", gameObject);
    }

    protected virtual void LoadPlayerShooting()
    {
        if (playerShooting != null) return;
        playerShooting = GetComponentInChildren<PlayerShooting>();
        Debug.Log(transform.name + " loaded PlayerShooting", gameObject);
    }
}
