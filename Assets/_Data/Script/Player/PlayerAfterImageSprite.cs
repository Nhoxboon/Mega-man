using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageSprite : NhoxMonoBehaviour
{
    [SerializeField]
    protected float activeTime = 0.1f;
    protected float timeActivated;
    protected float alpha;
    protected float alphaSet = 0.8f;
    protected float alphaMultiplier = 0.85f;

    [SerializeField] protected Transform player;

    [SerializeField] protected SpriteRenderer sr;
    [SerializeField] protected SpriteRenderer playerSr;

    protected Color color;

    protected override void LoadComponents()
    {
        LoadSprite();
        LoadPlayer();
        LoadPlayerSprite();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        alpha = alphaSet;
        sr.sprite = playerSr.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        timeActivated = Time.time;
    }

    protected void Update()
    {
        alpha *= alphaMultiplier;
        color = new Color(1f, 1f, 1f, alpha);
        sr.color = color;

        if (Time.time >= (timeActivated + activeTime))
        {
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }
    }

    protected void LoadSprite()
    {
        if (sr != null) return;
        sr = GetComponent<SpriteRenderer>();
        //Debug.Log(transform.name + " LoadSprite", gameObject);
    }

    protected void LoadPlayer()
    {
        if (player != null) return;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //Debug.Log(transform.name + " LoadPlayer", gameObject);
    }

    protected void LoadPlayerSprite()
    {
        if (playerSr != null) return;
        playerSr = player.GetComponentInChildren<SpriteRenderer>();
        //Debug.Log(transform.name + " LoadPlayerSprite", gameObject);
    }
}
