using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMe : MonoBehaviour
{
    public float delay = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, delay);
    }

}
