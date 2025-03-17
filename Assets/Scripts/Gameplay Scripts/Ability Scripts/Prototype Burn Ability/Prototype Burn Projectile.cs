using UnityEngine;

public class ProtopyeBurnProjectile : MonoBehaviour
{

    private float tickDuration = 1;
    private float burnDamage = 3;
    private int numTicks = 5;
    private float aoeRadius = 10;

    public float selfDestructTime = 4f;

    public float TickDuration { get => tickDuration; set => tickDuration = value; }
    public float BurnDamage { get => burnDamage; set => burnDamage = value; }
    public int NumTicks { get => numTicks; set => numTicks = value; }

    public void Start() 
    {
        Destroy(this.gameObject, selfDestructTime);
    }

    private void OnCollisionEnter(Collision other) 
    {
        //non AOE
        /*if (other.gameObject.tag == "Enemy")
        {
            Destroy(this.gameObject);
            other.gameObject.GetComponent<Enemy>().ApplyBurn(tickDuration, burnDamage, numTicks);
        } else if (other.gameObject.tag == "Ground" ||
                   other.gameObject.tag == "Townhall")
        {
            Destroy(this.gameObject);
        }*/

        //AOE
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Ground" || other.gameObject.tag == "Lighthouse")
        {
            Vector3 pos = transform.position;
            Destroy(this.gameObject);

            Collider[] hitColliders = Physics.OverlapSphere(pos, aoeRadius);
            foreach (var hitCollider in hitColliders)
            {
                if(hitCollider.gameObject.tag == "Enemy"){
                    hitCollider.gameObject.GetComponent<Enemy>().ApplyBurn(tickDuration, burnDamage, numTicks);
                }
            }
        }
    }
}
