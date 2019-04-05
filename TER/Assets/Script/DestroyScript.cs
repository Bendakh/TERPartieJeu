using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyScript : MonoBehaviour
{

    public float cd;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, cd);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
