using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
//here because reticle is locked
public class Rainstorm : MonoBehaviour
{   

    public float rainstorm_time=5.0f;
    public float delta = 50f;
    
   
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Destroy(this.gameObject, rainstorm_time);
        float radius = 1f;
        
        cell_controller.instance.growTrees(transform.position, radius, delta*Time.deltaTime);
        
        
      
        
        
   
    }
    
}
