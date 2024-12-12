using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFly : NhoxMonoBehaviour
{
    [SerializeField] protected float moveSpeed = 1;
    [SerializeField] protected Vector3 direction = Vector3.right;

    void Update()
    {
        transform.parent.Translate(moveSpeed * Time.deltaTime * direction);
    }
}
