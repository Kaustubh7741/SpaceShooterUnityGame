using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _asteroidRotationSpeed = 60f;     //Asteroid rotation speed

    private SpawnManager _spawnManager;         //Spawn manager handle

    [SerializeField]
    private GameObject _explosionPrefab;    //Handle to replace object with explosion animation (saved in an empty game object)

    // Start is called before the first frame update
    void Start()
    {
        //_asteroidRotationSpeed *= Time.deltaTime;

        //create spawn manager handle to control spawning on asteroid destruction
        _spawnManager = GameObject.FindGameObjectWithTag("Spawn Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
            Debug.LogError("Spawn Manager not instantiated in Asteroid class");

        /*_explosionPrefab = GameObject.FindWithTag("Explosion");
        if (_explosionPrefab == null)
            Debug.LogError("Explosion prefab not instantiated in Asteroid class");*/

    }

    // Update is called once per frame
    void Update()
    {
        //Rotate asteroid by 3 units per second
        transform.Rotate(_asteroidRotationSpeed * Time.deltaTime * Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //when laser hits laser
        if(collision.CompareTag("Player") || collision.CompareTag("Laser"))
        {
            //enable spawning
            _spawnManager.StartSpawning();

            //replace current object with explosion
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
            //Also destory the explosion instance after 3 seconds >> This is taken care in Explosion class/script
            /*//Destroy explosion object after 3 seconds
            Destroy(explosion, 3f);*/

            //Destory laser
            if (collision.CompareTag("Laser"))
                Destroy(collision.gameObject);
        }
    }
}
