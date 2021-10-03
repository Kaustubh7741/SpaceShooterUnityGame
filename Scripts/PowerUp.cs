using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    //Game environment boundaries
    private float _leftXBound = GlobalVariables.leftXBound;
    private float _rightXBound = GlobalVariables.rightXBound;
    private float _topYBound = GlobalVariables.topYBound;
    private float _bottomYBound = GlobalVariables.bottomYBound;

    [SerializeField]
    private float _powerUpTranslateSpeed = 8f;

    //[SerializeField]
    private MyPlayer _player;       //Player handle to call appropriate power up function

    [SerializeField]
    private int _powerupID;     //0 - Triple Shot, 1 - Speed Boost, 2 - Shield

    private AudioSource _pickupSound;        //to play sound when player picks up power up

    private int _startXPosition = 0;

    // Start is called before the first frame update
    void Start()
    {
        //_powerUpTranslateSpeed *= Time.deltaTime;
        _startXPosition = Random.Range(0, 2);      //2 is exclusive
        if (_startXPosition == 1)
            transform.position = new Vector3(_leftXBound, Random.Range(_bottomYBound + 1, _topYBound - 1), transform.position.z);
        else
            transform.position = new Vector3(_rightXBound, Random.Range(_bottomYBound + 1, _topYBound - 1), transform.position.z);

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<MyPlayer>();
        if (_player == null)
            Debug.LogError("Player instance error in trigger for powerup");

        _pickupSound = GameObject.Find("PowerUp_Pickup_sound").GetComponent<AudioSource>();
        if (_pickupSound == null)
            Debug.LogError("Powerup Pickup instance error in PowerUp class");

    }

    // Update is called once per frame
    void Update()
    {
        //translate power-up
        if (_startXPosition == 1)    //Left to right
            transform.Translate(_powerUpTranslateSpeed * Time.deltaTime * Vector3.right);
        else if (_startXPosition == 0)  //Right to left
            transform.Translate(_powerUpTranslateSpeed * Time.deltaTime * Vector3.left);

        //If out of bounds, destroy
        if (transform.position.x < _leftXBound || transform.position.x > _rightXBound)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if picked up by player
        if(other.CompareTag("Player"))
        {
            switch(_powerupID)
            {
                case 0:
                    _player.ActivateTripleShot();
                    break;

                case 1:
                    _player.ActivateSpeedBoost();
                    break;

                case 2:
                    _player.IncreaseShieldHealth();
                    break;

                default:
                    Debug.LogError("Power Up ID out of bounds");
                    break;
            }
            //play collected audio
            _pickupSound.Play();

            Destroy(this.gameObject);
        }
        //Debug.Log("Triggered");
    }

}
