using UnityEngine;

public class ProtopyeBeguileProjectile : MonoBehaviour
{

    private float beguileTime = 10f;
    private float beguileDamage = 5f;
    private const float DEFAULT_TIME = 10f;

    public float selfDestructTime = 4f;

    public void Start() 
    {
        Destroy(this.gameObject, selfDestructTime);
    }

    public float getBeguileTime()
    {
        return beguileTime;
    }

    public void setBeguileTime(float beguileTime)
    {
        // Default beguile time if input is invalid
        if (beguileTime < 0)
        {
            beguileTime = DEFAULT_TIME;
        }

        this.beguileTime = beguileTime;
    }
    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<Enemy>().ApplyBeguile(beguileTime, beguileDamage);
            Destroy(this.gameObject);
        }
        else if (other.gameObject.tag == "Ground" ||
                other.gameObject.tag == "Lighthouse")
        {
            Destroy(this.gameObject);
        }
    }
}
