using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disaster : MonoBehaviour
{
    private static bool disaster_queued = false;

    public GameObject disaster_indicator;

    public static float earthquake_radius = 0.7f;
    public static disaster instance;

    //Track all regions in order to delete their units
    public static region[] regions;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        disaster_indicator.SetActive(disaster_queued);
        regions = FindObjectsOfType<region>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Disaster logic, destroys trees and units in a region
    public static void causeDisaster(Vector2 pos, float radius)
    {
        cell_controller.instance.growTrees(pos, radius, -1);
        for(int i = 0; i < regions.Length; i++)
        {
            if(Vector2.Distance(pos, regions[i].centerpoint) <= radius)
            {
                regions[i].units = 20;
                regions[i].Owner = player.none;
            }
        }
    }

    public static bool isDisasterQueued()
    {
        return disaster_queued;
    }

    //Set and update indicator
    public static void setDisaster(bool mode)
    {
        disaster_queued = mode;
        instance.disaster_indicator.SetActive(mode);
    }
}
