using UnityEngine;

public class Townhall : MonoBehaviour
{
    private float health;

    private RoundManager roundManager;

    private GameObject damageCollider;

    private void Start()
    {
        health = 1000;
        roundManager = GameObject.Find("Round Manager").GetComponent<RoundManager>();
        damageCollider = GameObject.Find("Damage Collider");
    }


    private void Update() 
    {
        CheckTownhallDeath();
    }
 
 
 
 
    public float getHealth()
    {
        return health;
    }

    public void setHealth(float newHealth)
    {
        this.health = newHealth;
    }

    public void TakeDamage(float amount)
    {
        health -= amount * Time.deltaTime;
    }

  
  
    public void CheckTownhallDeath()
    {
        if (health <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        Destroy(this.gameObject); //Perhaps add a particle effect/more elaborate than just destroy()
        roundManager.setRoundPhase(RoundManager.RoundPhase.GameOver);
        Debug.Log("Game Over");
    }



}
