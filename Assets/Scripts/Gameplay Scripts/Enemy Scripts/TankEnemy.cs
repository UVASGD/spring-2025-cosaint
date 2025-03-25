using System;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Rendering;

public class TankEnemy : Enemy
{

    private void Awake()
    {
        maxHealth = 500f;
        health = 500f;
        speed = DEFAULT_SPEED/4;
    }

}