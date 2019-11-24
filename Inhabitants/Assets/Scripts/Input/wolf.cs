using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wolf : MonoBehaviour
{
    public wolf_reps wolf_rep;
    public float colony_num = 50f;
    private float max_colony_num = 50f;
    private CircleCollider2D cc;
    public float radius = 1f;
    private wolf_reps[] wolf_r = new wolf_reps[5];

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CircleCollider2D>();
        this.gameObject.name = "wolf";
        for (int i = 0; i < 5; i++)
        {
            Vector3 pos = Random.insideUnitCircle * radius;
            wolf_reps r = Instantiate(wolf_rep, transform.position + pos, Quaternion.identity);
            r.radius = radius;
            r.transform.parent = this.transform;
            r.basestartpoint = transform.position;
            wolf_r[i] = r;



        }

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
        float per = colony_num / max_colony_num;
        if (per < 0.2f)
        {
            for (int i = 1; i < 5; i++)
            {
                if (wolf_r[i].transform.gameObject.activeInHierarchy)
                {
                    wolf_r[i].transform.gameObject.SetActive(false);
                }
            }
        }
        else if (per < 0.4f)
        {
            for (int i = 2; i < 5; i++)
            {
                if (wolf_r[i].transform.gameObject.activeInHierarchy)
                {
                    wolf_r[i].transform.gameObject.SetActive(false);
                }
            }
        }

        else if (per < 0.6f)
        {
            for (int i = 3; i < 5; i++)
            {
                if (wolf_r[i].transform.gameObject.activeInHierarchy)
                {
                    wolf_r[i].transform.gameObject.SetActive(false);
                }
            }
        }

        //if there are no wolves left
        if (colony_num <= 0 && this.gameObject != null)
        {
            Destroy(this.gameObject);
            

        }

    }
}
