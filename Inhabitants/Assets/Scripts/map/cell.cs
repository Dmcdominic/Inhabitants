using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cell : MonoBehaviour
{
    public Sprite noTrees;
    public Sprite withTrees;

    private float _state;
    private SpriteRenderer _rend;

    public SpriteRenderer rend
    {
        get { return _rend; }
        private set { _rend = value; }
    }

    public float state
    {
        get { return _state; }
        set
        {
            _state = value;
            if (state < 0.3)
            {
                rend.sprite = noTrees;
                Vector3 scale = new Vector3(0.0f, 0.0f, 1.0f);
                transform.localScale = scale;
            }
            else
            {
                rend.sprite = withTrees;
                Vector3 scale = new Vector3((state - 0.3f) / 0.7f, (state - 0.3f) / 0.7f, 1.0f);
                transform.localScale = scale;
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        rend = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}