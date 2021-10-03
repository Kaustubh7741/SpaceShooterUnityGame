using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    //Game environment boundaries
    /*private float _leftXBound = -9f;
    private float _rightXBound = 9f;
    private float _topYBound = 5f;
    private float _bottomYBound = -5f;*/

    // Start is called before the first frame update
    void Start()
    {
        //Destory this object after 3 seconds in any case
        Destroy(this.gameObject, 3f);

    }
}
