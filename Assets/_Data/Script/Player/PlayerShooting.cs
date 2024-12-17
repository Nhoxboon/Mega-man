using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : NhoxMonoBehaviour
{
    [SerializeField] protected PlayerCtrl playerCtrl;

    [SerializeField] protected bool isShooting = false;
    public bool IssShooting => isShooting;
    [SerializeField] protected float shootDelay = 1f;
    [SerializeField] protected float shootTimer = 0f;
    [SerializeField] protected Transform bulletPrefab;

    void Update()
    {
        this.IsShooting();
    }

    private void FixedUpdate()
    {
        this.Shooting();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadPlayerCtrl();
    }

    protected virtual void Shooting()
    {
        if (!this.isShooting) return;

        this.shootTimer += Time.fixedDeltaTime;
        if (this.shootTimer < this.shootDelay) return;
        this.shootTimer = 0;

        Vector3 spawnPos = transform.position;
        Quaternion rotation = transform.parent.rotation;
        Instantiate(this.bulletPrefab, spawnPos, rotation);
        //Debug.Log("Shooting");
    }

    protected virtual bool IsShooting()
    {
        this.isShooting = InputManager.Instance.OnFiring == 1;
        if (!this.isShooting) 
            playerCtrl.PlayerAnimatorManager.Reset();
        return this.isShooting;
    }

    protected virtual void LoadPlayerCtrl()
    {
        if (this.playerCtrl != null) return;
        this.playerCtrl = GetComponentInParent<PlayerCtrl>();
        Debug.Log(transform.name + " Load PlayerCtrl", gameObject);
    }
}
