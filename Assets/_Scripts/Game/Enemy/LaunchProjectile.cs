using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    [SerializeField]
    int numberOfProjectile;

    [SerializeField]
    GameObject projectile;

    Vector3 startPoint;

    public float radius = 5f;
    public float movespeed = 0.1f;
    public float launchVelocity = 750f;
    public float Timer = 5;

    // Start is called before the first frame update
    void Awake()
    {
        startPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Timer -= Time.deltaTime;
        if(Timer <= 0f)
        {
            SpawnProjectiels(numberOfProjectile);

            Timer = 5f;
        }
        
    }

    void SpawnProjectiels(int numberOfProjectile)
    {
        float angleStep = 360f / numberOfProjectile;
        float angle = 0f;

        for(int i = 0; i <= numberOfProjectile-1; i++)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, transform.forward);

            GameObject proj = Instantiate(projectile, startPoint, rotation * transform.rotation);
            proj.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, launchVelocity, 0));

            angle += angleStep;
        }
    }       
}
