using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int hitpoints = 30;

    private void Awake()
    {
        hitpoints = 30;
    }

    public bool TakeHit()
    {
        hitpoints -= 10;
        bool isDead = hitpoints <= 0;
        if (isDead) Destroy(gameObject);
        return isDead;
    }
}
