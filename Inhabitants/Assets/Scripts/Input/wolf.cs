using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wolf : MonoBehaviour
{
    public float colony_num = 50f;
    private CircleCollider2D cc;
    public float radius = 1f;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CircleCollider2D>();
        this.gameObject.name = "wolf";

    }

    // Update is called once per frame
    void Update()
    {   
        //parameters
        float colony_decrease_rate = 10f * Time.deltaTime;
        bool isDeer = false;

        //checks if there are deers nearby
        Collider2D[] nearObjects = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D obj in nearObjects)
        {   
            if (obj!=null && obj.name == "deer")
            {
                isDeer = true;

            }

        }

        //if there is no deer nearby, decrease colony size
        if (this.gameObject!=null && !isDeer)
        {
            colony_num -= colony_decrease_rate;

        }

        //if there are no wolves left
        if (colony_num <= 0 && this.gameObject != null)
        {
            Destroy(this.gameObject);
            

        }

    }
}
