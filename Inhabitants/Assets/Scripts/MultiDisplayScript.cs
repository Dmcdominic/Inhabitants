using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDisplayScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // (Theoretically) activates all connected displays
        // IMPORTANT! You cannot deactivate a display, and this must be called *once* during startup.
        foreach (Display d in Display.displays)
            d.Activate();
    }
}
