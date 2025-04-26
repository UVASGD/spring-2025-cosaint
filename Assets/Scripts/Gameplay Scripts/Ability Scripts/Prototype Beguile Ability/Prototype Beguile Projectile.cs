using UnityEngine;
using UnityEngine.Rendering;

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

    public void setBeguileDamge(float beguileDamge)
    {
        this.beguileDamage = beguileDamge;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (EnemyTags.IsEnemyTag(other.gameObject.tag))
        {
            other.gameObject.GetComponentInChildren<Enemy>().ApplyBeguile(20f, beguileDamage);
            Destroy(this.gameObject);
        }
        else if (other.gameObject.tag == "Ground" ||
                other.gameObject.tag == "Lighthouse")
        {
            Destroy(this.gameObject);
        }
    }
}
