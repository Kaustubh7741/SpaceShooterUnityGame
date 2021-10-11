using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    //Game environment boundaries
    private float _leftXBound = GlobalVariables.leftXBound;
    private float _rightXBound = GlobalVariables.rightXBound;
    private float _topYBound = GlobalVariables.topYBound;
    //private float _bottomYBound = GlobalVariables.bottomYBound;

    //private float _deltaTime;

    [SerializeField]
    private float _laserSpeed = 0.25f;
    //private float _laserSpinSpeed;

    // Start is called before the first frame update
    void Start()
    {
        //_deltaTime = Time.deltaTime;
        //_laserSpeed = 0.2f;// * _deltaTime;
        //_laserSpinSpeed = 20f;// * _deltaTime;
}

    // Update is called once per frame
    void Update()
    {
        //translate laser up and spin
        if(transform.rotation.z == 0f)
            transform.Translate(_laserSpeed * Vector3.up * Time.timeScale);

        //Translate side lasers for triple shot
        if (transform.rotation.eulerAngles.z > 0)       //Left
            transform.Translate(_laserSpeed * new Vector3(0f, 1f, 0f) * Time.timeScale);
        if (transform.rotation.eulerAngles.z < 0)       //Right
            transform.Translate(_laserSpeed * new Vector3(0f, -1f, 0f) * Time.timeScale);

        //transform.Rotate(0, _laserSpinSpeed, 0);
        //transform.Rotate(Vector3.up * _laserSpinSpeed);

        if(transform.position.y > _topYBound || transform.position.x > _rightXBound || transform.position.x < _leftXBound)
        {
            //Destroy parent object for triple shot
            /*if (transform.parent != null)
                Destroy(transform.parent.gameObject);*/
            Destroy(this.gameObject);
        }
    }
}
