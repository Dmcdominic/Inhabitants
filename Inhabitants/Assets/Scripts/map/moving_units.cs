using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moving_units : MonoBehaviour
{

    public region target_region;

    public player start_owner;

    public Vector3 start_position;

    public int units;

    private float movespeed = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float ds = Time.deltaTime * movespeed;
        


        //checks if the # of units moving has gone down to zero. Terminate if so.
        if (units <= 0)
        {

            Destroy(this.gameObject);
        }
        else
        {
            //moves the units
            if (Vector3.Distance(transform.position, target_region.transform.position) > ds)
            {

                Vector3 direction = target_region.transform.position - transform.position;
                transform.Translate(direction.normalized * ds);

            }
            else
            {

                //check if the target region is friendly, if yes
                if (target_region.Owner == start_owner)
                {

                    target_region.units += units;


                }
                //if no, check if the target region has more or less units
                else
                {

                    if (target_region.units < units)
                    {

                        target_region.Owner = start_owner;
                        target_region.units = units - target_region.units;

                    }
                    else if (target_region.units == units)
                    {//turn the region neutral

                        target_region.Owner = player.none;
                        target_region.units = 0;

                    }
                    else
                    {
                        target_region.units -= units;


                    }


                }

                Destroy(this.gameObject);


            }
        }
    }
}
