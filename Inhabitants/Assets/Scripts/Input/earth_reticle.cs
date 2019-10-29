using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class earth_reticle : MonoBehaviour
{

    // Settings
    private static float rainstorm_rate = 5.0f; //15 seconds
    private static float next_rainstorm = 0.0f;

    private static float tree_growth_radius = 0.3f;
    private static float tree_growth_rate = 0.2f;

    // References
    public Rainstorm rainstorm;
    public deer Deer;
    public wolf Wolf;

    // Components
    private reticle Reticle;


    // Init
    private void Awake()
    {
        Reticle = GetComponent<reticle>();
    }

    // Called every frame
    private void Update()
    {
        // Rainstorm logic
        if (XCI.GetButtonDown(XboxButton.X, Reticle.controller) && Time.time > next_rainstorm)
        {
            if (Physics2D.RaycastAll(transform.position, Vector2.zero).Length >= 2)
            {
                next_rainstorm = Time.time + rainstorm_rate;
                Instantiate(rainstorm, transform.position, Quaternion.identity);
            }
        }

        // Tree-growing logic
        if (XCI.GetButton(XboxButton.A, Reticle.controller) || Input.GetKey(KeyCode.T))
        {
            cell_controller.instance.growTrees(transform.position, tree_growth_radius, Time.deltaTime * tree_growth_rate);
        }
        // Animal-placing (deer) logic:
        if (XCI.GetButtonDown(XboxButton.Y, Reticle.controller))
        {
            if (Physics2D.RaycastAll(transform.position, Vector2.zero).Length >= 2)
            {
                Instantiate(Deer, transform.position, Quaternion.identity);



            }

            
        }

        if (XCI.GetButtonDown(XboxButton.B, Reticle.controller))
            {
                if (Physics2D.RaycastAll(transform.position, Vector2.zero).Length >= 2)
                {
                    Instantiate(Wolf, transform.position, Quaternion.identity);



                }



            }
    }
}
