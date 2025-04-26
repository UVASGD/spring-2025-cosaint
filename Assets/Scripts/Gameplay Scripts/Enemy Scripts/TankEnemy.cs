using System;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Rendering;

public class TankEnemy : Enemy
{

    private void Awake()
    {
        maxHealth = 200f;
        health = 200f;
        speed = DEFAULT_SPEED/2;
    }

}