using Unity.VisualScripting;
using UnityEngine;

public class RunnerEnemy : Enemy
{   
    protected override void Start()
    {
        base.Start();
        setMaxHealth(1);
        SetSpeed(5f);
        setDamage(0f);
    }

    public override void SetDoingDamage(bool doingDamage)
    {
        base.SetDoingDamage(doingDamage);
        if (doingDamage)
        {
            lighthouse.TakeDamage(5f / Time.deltaTime);
            Debug.Log("Contact Damage: " + lighthouse.GetHealth());
            Die();
        }
    }

    protected override void Die()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "Lighthouse")
            {
                lighthouse.TakeDamage(5f / Time.deltaTime);
                Debug.Log("Explosion Damage: " + lighthouse.GetHealth());
            }
        }

        base.Die();
    }
}
