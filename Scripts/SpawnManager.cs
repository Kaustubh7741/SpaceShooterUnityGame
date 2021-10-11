using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    //Enemy prefab
    [SerializeField]
    private GameObject _enemyPrefab;

    //Enemy container (folder)
    [SerializeField]
    private GameObject _enemyContainer;

    /*//Triple shot power up prefab
    [SerializeField]
    private GameObject _tripleShotPowerUpPrefab;*/

    //Power-up prefab array
    [SerializeField]
    private GameObject[] _powerUpPrefabs;

    //Power up container
    [SerializeField]
    private GameObject _powerUpContainer;

    //Spawn control
    private bool _spawnEnabled = false;

    // Start is called before the first frame update
    public void StartSpawning()
    {
        SetSpawning(true);
        StartCoroutine(EnemySpawnRoutine());
        StartCoroutine(PowerUpSpawnRoutine());
    }

    IEnumerator EnemySpawnRoutine()
    {
        yield return new WaitForSeconds(1f);
        while(_spawnEnabled)
        {
            //Get instance in a variable
            GameObject newEnemy = Instantiate(_enemyPrefab);//, new Vector3(0, 8f, 0), Quaternion.identity);

            //Move new enemy instance to enemy container (folder)
            newEnemy.transform.parent = _enemyContainer.transform;

            //Equivalent to Thread.sleep(1.5sec)
            yield return new WaitForSeconds(1.5f);
        }

    }

    public void SetSpawning(bool state)
    {

        //_spawnEnabled = !_spawnEnabled;
        _spawnEnabled = state;
    }

    IEnumerator PowerUpSpawnRoutine()
    {
        yield return new WaitForSeconds(5f);
        while (_spawnEnabled)
        {
            //Instantiate a power up in next 6-10 sec

            //int powerupID = Random.Range(0, 2);     //min inclusive max exclusive
            GameObject powerUp;         //Store instance of power up created
            /*switch (powerupID)
            {
                case 0:
                    powerUp = Instantiate(_tripleShotPowerUpPrefab);
                    break;
                case 1:
                    powerUp = Instantiate(_speedBoostPowerUpPrefab);
                    break;
                case 2:
                    //instantiate shield power up
                    break;
            }*/

            powerUp = Instantiate(_powerUpPrefabs[Random.Range(0, _powerUpPrefabs.Length)]);

            if (powerUp == null)
                Debug.LogError("Power up instantiation failed in spawn manager");
            else
                powerUp.transform.parent = _powerUpContainer.transform;

            yield return new WaitForSeconds(Random.Range(6f, 10f));

        }
    }

    public bool SpawnEnabled()
    {
        return _spawnEnabled;
    }


}
