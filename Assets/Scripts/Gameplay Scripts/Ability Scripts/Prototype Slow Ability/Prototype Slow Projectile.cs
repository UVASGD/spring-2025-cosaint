using UnityEngine;

public class ProtopyeSlowProjectile : MonoBehaviour
{

    private float slowTime = 5f;
    private float slowMagnitude = 0.75f;

    public float selfDestructTime = 4f;

    public float SlowTime { get => slowTime; set => slowTime = value; }
    public float SlowMagnitude { get => slowMagnitude; set => slowMagnitude = value; }

    public void Start() 
    {
        Destroy(this.gameObject, selfDestructTime);
    }

    public float getSlowTime()
    {
        return slowTime;
    }

    public void setSlowTime(float slowTime)
    {
        this.slowTime = slowTime;
    }
    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.tag == "Enemy")
        {
            Destroy(this.gameObject);
            other.gameObject.GetComponent<Enemy>().ApplySlow(slowTime, slowMagnitude);
        } else if (other.gameObject.tag == "Ground" ||
                   other.gameObject.tag == "Lighthouse")
        {
            Destroy(this.gameObject);
        }
    }
}
