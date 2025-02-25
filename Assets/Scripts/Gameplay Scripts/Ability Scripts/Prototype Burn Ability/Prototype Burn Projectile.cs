using UnityEngine;

public class ProtopyeBurnProjectile : MonoBehaviour
{

    private float tickDuration = 1;
    private float burnDamage = 25;
    private int numTicks = 2;
    private float aoeRadius = 10;

    public float selfDestructTime = 4f;

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
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Ground" || other.gameObject.tag == "Townhall")
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
