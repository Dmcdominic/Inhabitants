using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wolf_reps : MonoBehaviour
{
    public float radius;
    public Vector2 basestartpoint;
    public Vector2 target;
    public float speed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {

        target = NextDestination();

    }

    // Update is called once per frame
    void Update()
    {
        float ds = Time.deltaTime * speed;
        if (Vector2.Distance(transform.position, target) > ds)
        {
            Vector2 direction = target - (Vector2)transform.position;
            transform.Translate(direction.normalized * ds);
            return;
        }
        target = NextDestination();

    }

    Vector2 NextDestination()
    {
        return basestartpoint + (Random.insideUnitCircle * radius);


    }
}
