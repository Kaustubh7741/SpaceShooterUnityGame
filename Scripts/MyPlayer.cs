using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyPlayer : MonoBehaviour
{
    //Game environment boundaries
    private float _leftXBound = GlobalVariables.leftXBound;
    private float _rightXBound = GlobalVariables.rightXBound;
    private float _topYBound = GlobalVariables.topYBound;
    private float _bottomYBound = GlobalVariables.bottomYBound;

    // Start is called before the first frame update
    //factor translation to 1 unit per second
    //private float _perSecondMultiplier;

    //Variable editable in Unity
    [SerializeField]
    private float _playerSpeed = 10f;  //Player translation speed

    [SerializeField]
    private float _speedBoostIntensity = 2f;    //Speed boost multiplier

    [SerializeField]
    private GameObject _laserPrefab;    //Laser prefab - used to create exactly same instances of a 3D object (here, laser). Linked from Unity inspector

    /*[SerializeField]
    private GameObject _tripleShotPrefab;   //Laser prefab after picking up triple shot power up*/

    [SerializeField]
    private float _fireRate = 0.25f;    //Interval between 2 consecutive laser fire

    private float _allowFire = -1f;     //Time.time starts with 0.0f which will allow first fire

    [SerializeField]
    private float _powerUpDuration = 5f;    //Duration of power up in seconds

    [SerializeField]
    private int _playerLife = 3;        //number of times player can collide with enemy

    [SerializeField]
    private GameObject _laserContainer; //Container/folder for new instances of laser. Linked from Unity inspector

    private SpawnManager _spawnManager; //Communicate with SpawnManager class. Instantiated in Start()

    private bool _isTripleShotActive = false;   //If triple shot is picked up
    //private bool _isSpeedBoostActive = false;   //If speed powerup is picked up
    private int _shieldHealth = 0;       //Shield health will increase on Shield powerup pick up

    //private bool _shieldDecayCoroutineInProgress = false;      //check if shield decay couroutine has started

    UIManager _uiManager;      //UI manager handle to update score on screen

    [SerializeField]
    private GameObject _explosionPrefab;    //Handle to trigger explosion animation (saved in object) on death

    private GameObject _thruster;       //Thruster handle to apply transformations based on movement

    private GameObject _damage1, _damage2;        //Handle to enable 2 levels of damage

    private AudioSource _laserShotSound;     //Handle to play laser sound while shooting
    //private AudioSource _explosionSound;    //Handle to play explosion sound on death

    private Animator _animator;     //Animation handle for movement animation

    private AudioSource _powerupLostSound;      //Handle to play inverse of powerup sound when power up is depleted

    //private bool _isGamePaused = false;     //variable to toggle pausing

    private GameManager _gameManager;       //Game manager handle for single player/coop

    [SerializeField]
    private bool _isPlayerOne = true, _isPlayerTwo = false;       //Player identifier for control setup

    private Vector3 _temp;  //temp vector

    //Player scores
    private static int _score = 0;
    private static int _killCount = 0;
    private static float _accuracy = 0;
    private static int _shots = 0;

    void Start()
    {
        //Set to 1 unit per second
        //_perSecondMultiplier = Time.deltaTime;

        //multiply all translations y deltaTime to make it uniform
        //_playerSpeed *= _perSecondMultiplier;

        //Connect with SpawnManager component (script) of Spawn_Manager game object
        _spawnManager = GameObject.FindGameObjectWithTag("Spawn Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager error. It's NULL");
            _spawnManager.SetSpawning(false);
        }

        //Initialize UI manager
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager error. It's NULL");
            _spawnManager.SetSpawning(false);
        }

        _thruster = transform.Find("Player_Thruster").gameObject;
        if (_thruster == null)
            Debug.LogError("Thruster is not initialized in Player class");

        _damage1 = transform.Find("Damage").GetChild(Random.Range(0, 2)).gameObject;    //Minor damange
        _damage2 = transform.Find("Damage").GetChild(Random.Range(2, 4)).gameObject;    //Major damage

        if (_damage1 == null || _damage2 == null)
            Debug.LogError("Damage object was not initialized in Player class");

        _laserShotSound = GameObject.Find("Laser_Shot_sound").GetComponent<AudioSource>();
        if (_laserShotSound == null)
            Debug.LogError("Laser shot sound was not initialized in Player class");

        /*_explosionSound = GameObject.FindGameObjectWithTag("Explosion_sound").GetComponent<AudioSource>();
        if (_explosionSound == null)
            Debug.LogError("Explosion sound was not initialized in Player class");*/

        _animator = GetComponent<Animator>();
        if (_animator == null)
            Debug.LogError("Animation reference was not instantiated in Player class");

        _powerupLostSound = GameObject.Find("PowerUp_Pickup_sound").GetComponent<AudioSource>();
        if (_powerupLostSound == null)
            Debug.LogError("Powerup audio source reference was not instantiated in Player class");

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
            Debug.LogError("Game Manager was not instantiated in Player class");

        //reset Time.time if paused and quit (bug fix)
        //_uiManager.TogglePause(false);    //results in NullReferenceException in UI Manager class

        //set player attributes to zero every time game starts for co-op score consistency
        _score = 0;
        _killCount = 0;
        _accuracy = 0;
        _shots = 0;

        _temp = new Vector3();

        //Override position
        if (_gameManager.IsCoopMode())
        {
            if(_isPlayerOne)
                transform.position = new Vector3(-2.5f, _bottomYBound + 2.5f, 0f);
            if (_isPlayerTwo)
                transform.position = new Vector3(2.5f, _bottomYBound + 2.5f, 0f);
        }
        else
        {
            transform.position = new Vector3(0f, _bottomYBound + 2.5f, 0f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //If Spacebar pressed - fire laser
        //if (Input.GetKeyDown(KeyCode.Space))

        //while spacebar is pressed, fire laser
        if (_isPlayerOne)
        {
            PlayerOneMovement();
            if (Input.GetKey(KeyCode.Space) && _playerLife > 0)// && !_isGamePaused)
            {
                PlayerOneFireLaser();
            }
        }
        else if (_isPlayerTwo)
        {
            PlayerTwoMovement();
            if (Input.GetKey(KeyCode.RightControl) && _playerLife > 0)// && !_isGamePaused)
            {
                PlayerTwoFireLaser();
            }
        }

        /*//if escape is pressed, quit game
        if (Input.GetKeyDown(KeyCode.Escape) && _playerLife > 0)
        {
            //Load main menu scene
            //SceneManager.LoadScene(0);

            //Pause or resume game
            _isGamePaused = !_isGamePaused;
            _uiManager.TogglePause(_isGamePaused);
        }*/

    }

    void PlayerOneMovement()
    {
        float factor = 0.05f;
        //transform.Translate(new Vector3(1, (float)1.5, (float)1.25) * _perSecondMultiplier);

        /* float horizontalInput = Input.GetAxis("Horizontal");
         float verticalInput = Input.GetAxis("Vertical");

         transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * _speed);*/

        /*transform.Translate(Vector3.right * horizontalInput * _speed);
        transform.Translate(Vector3.up * verticalInput * _speed);*/

        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.Translate(_playerSpeed * Time.deltaTime * direction);

        /*if ((transform.position.x > _rightXBound) || (transform.position.x < _leftXBound))
            transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);*/

        //to avoid the flicker effect, give an offset while inverting x position
        if ((transform.position.x > _rightXBound) || (transform.position.x < _leftXBound))
            transform.position = new Vector3(-(transform.position.x - (0.5f * Mathf.Sign(transform.position.x))), transform.position.y, transform.position.z);

        /*if ((transform.position.y > 10f) || (transform.position.y < -10f))
            transform.position = new Vector3(transform.position.x, -transform.position.y, transform.position.z);*/
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _bottomYBound, _topYBound), transform.position.z);

        //Transform thruster gameobject when 'W' is pressed
        float originalScale = 0.5f;//_thruster.transform.localScale.x;
        float originalYOffset = -1.3f;//_thruster.transform.position.y;
        float boostedScale = 0.75f;
        float boostedYOffset = -1.4f;
        if (Input.GetAxis("Vertical") > 0 && _thruster.transform.localScale.x == originalScale)   //if(Input.GetKeyDown(KeyCode.W))
        {
            //scale the object to 1 on all axis and displace the object on Y-axis according to player
            _temp.Set(boostedScale, boostedScale, 1f);
            _thruster.transform.localScale = _temp;
            _temp.Set(0f, boostedYOffset, 0f);
            _thruster.transform.position = transform.position + _temp;
        }

        if (Input.GetAxis("Vertical") <= 0 && _thruster.transform.localScale.x == boostedScale)
        {
            //scale the object to 0.5 on all axis and displace the object on Y-axis according to player
            _temp.Set(originalScale, originalScale, 1f);
            _thruster.transform.localScale = _temp;
            _temp.Set(0f, originalYOffset, 0f);
            _thruster.transform.position = transform.position + _temp;
        }

        //Start movement animation
        if (direction.x < 0)
        {
            //player is moving left
            _animator.SetBool("IsMovingLeft", true);

            //scale down major damage to match wing distance
            MajorDamageAnimationScalar(-1, factor);
        }
        else if (direction.x > 0)
        {
            //player is moving right
            _animator.SetBool("IsMovingRight", true);

            //scale down major damage to match wing distance
            MajorDamageAnimationScalar(1, factor);
        }
        else
        {
            //no movement
            _animator.SetBool("IsMovingLeft", false);
            _animator.SetBool("IsMovingRight", false);

            //scale up/reset scale for major damage to match wing distance
            MajorDamageAnimationScalar(0, factor);
        }
    }

    void PlayerOneFireLaser()
    {
        if (_allowFire < Time.time)     //condition for fire rate
        {
            GameObject newLaser;    //Original top instance
            GameObject newLaserL;   //Left instance
            GameObject newLaserR;   //Right instance

            float playerX = transform.position.x;
            float playerY = transform.position.y;
            float playerZ = transform.position.z;

            //spawn laser
            //Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            
            newLaser = Instantiate(_laserPrefab, new Vector3(playerX, (playerY + 1f), playerZ), Quaternion.identity);

            //Move new instances of laser to Laser_Container
            if (newLaser != null)
                newLaser.transform.parent = _laserContainer.transform;
            else
            {
                Debug.LogError("Laser was not instantiated");
                _spawnManager.SetSpawning(false);
            }

            //Fire rate setup
            _allowFire = Time.time + _fireRate;

            //Shot fired if spawning is enabled to skip asteroid destruction
            if(_spawnManager.SpawnEnabled())
                _shots++;

            //if triple shot is enabled, fire the Triple_Shot prefab
            if (_isTripleShotActive)
            {
                //newLaser = Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);

                //can also be simulated with 2 additional instances of laser with respective offsets
                //newLaser = Instantiate(_laserPrefab, new Vector3(playerX, playerY + 1f, playerZ), Quaternion.identity);
                newLaserL = Instantiate(_laserPrefab, new Vector3((playerX - 0.8f), (playerY - 0.5f), playerZ), Quaternion.Euler(0f, 0f, 45f));
                newLaserR = Instantiate(_laserPrefab, new Vector3((playerX + 0.8f), (playerY - 0.5f), playerZ), Quaternion.Euler(0f, 0f, -45f));

                if (newLaserL != null)
                    newLaserL.transform.parent = _laserContainer.transform;
                else
                {
                    Debug.LogError("LaserL was not instantiated");
                    _spawnManager.SetSpawning(false);
                }
                if (newLaserR != null)
                    newLaserR.transform.parent = _laserContainer.transform;
                else
                {
                    Debug.LogError("LaserR was not instantiated");
                    _spawnManager.SetSpawning(false);
                }

            }
            /*else
            {
                //spawn laser 1.0f above (Y) the cube
                newLaser = Instantiate(_laserPrefab, new Vector3(transform.position.x, (transform.position.y + 1f), transform.position.z), Quaternion.identity);
            }*/

            //Trigger shot audio
            _laserShotSound.Play(0);
            
        }

    }



    void PlayerTwoMovement()
    {
        float factor = 0.05f;
        Vector3 direction = new Vector3();
        //Player 2 movement
        bool down = Input.GetKey(KeyCode.K), left = Input.GetKey(KeyCode.J),
            right = Input.GetKey(KeyCode.L), up = Input.GetKey(KeyCode.I);
        if (up)
            direction.Set(0, 1.0f, 0);
        if (left)
            direction.Set(-1.0f, 0, 0);
        if (down)
            direction.Set(0, -1.0f, 0);
        if (right)
            direction.Set(1.0f, 0, 0);

        transform.Translate(_playerSpeed * Time.deltaTime * direction);
        if ((transform.position.x > _rightXBound) || (transform.position.x < _leftXBound))
            transform.position = new Vector3(-(transform.position.x - (0.5f * Mathf.Sign(transform.position.x))), transform.position.y, transform.position.z);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _bottomYBound, _topYBound), transform.position.z);
        float originalScale = 0.5f;
        float originalYOffset = -1.3f;
        float boostedScale = 0.75f;
        float boostedYOffset = -1.4f;
        if (up && _thruster.transform.localScale.x == originalScale)
        {
            _temp.Set(boostedScale, boostedScale, 1f);
            _thruster.transform.localScale = _temp;
            _temp.Set(0f, boostedYOffset, 0f);
            _thruster.transform.position = transform.position + _temp;
        }

        else if (!up && _thruster.transform.localScale.x == boostedScale)
        {
            _temp.Set(originalScale, originalScale, 1f);
            _thruster.transform.localScale = _temp;
            _temp.Set(0f, originalYOffset, 0f);
            _thruster.transform.position = transform.position + _temp;
        }
        if (direction.x < 0)
        {
            //player is moving left
            _animator.SetBool("IsMovingLeft", true);
            MajorDamageAnimationScalar(-1, factor);
        }
        else if (direction.x > 0)
        {
            _animator.SetBool("IsMovingRight", true);
            MajorDamageAnimationScalar(1, factor);
        }
        else if (direction.x == 0)
        {
            _animator.SetBool("IsMovingLeft", false);
            _animator.SetBool("IsMovingRight", false);
            MajorDamageAnimationScalar(0, factor);
        }
    }

    void PlayerTwoFireLaser()
    {
        if (_allowFire < Time.time)
        {
            GameObject newLaser;
            GameObject newLaserL;
            GameObject newLaserR;
            float playerX = transform.position.x;
            float playerY = transform.position.y;
            float playerZ = transform.position.z;
            newLaser = Instantiate(_laserPrefab, new Vector3(playerX, (playerY + 1f), playerZ), Quaternion.identity);
            if (newLaser != null)
                newLaser.transform.parent = _laserContainer.transform;
            else
            {
                Debug.LogError("Laser was not instantiated");
                _spawnManager.SetSpawning(false);
            }
            _allowFire = Time.time + _fireRate;
            _shots++;
            if (_isTripleShotActive)
            {
                newLaserL = Instantiate(_laserPrefab, new Vector3((playerX - 0.8f), (playerY - 0.5f), playerZ), Quaternion.Euler(0f, 0f, 45f));
                newLaserR = Instantiate(_laserPrefab, new Vector3((playerX + 0.8f), (playerY - 0.5f), playerZ), Quaternion.Euler(0f, 0f, -45f));

                if (newLaserL != null)
                    newLaserL.transform.parent = _laserContainer.transform;
                else
                {
                    Debug.LogError("LaserL was not instantiated");
                    _spawnManager.SetSpawning(false);
                }
                if (newLaserR != null)
                    newLaserR.transform.parent = _laserContainer.transform;
                else
                {
                    Debug.LogError("LaserR was not instantiated");
                    _spawnManager.SetSpawning(false);
                }

            }
            _laserShotSound.Play(0);

        }

    }



    //Public method to update score instead of manually setting
    public void IncreaseScore(int byPoints)
    {
        _score += byPoints;
        _killCount++;

        //Update Score UI text
        _uiManager.UpdateScoreOnScreen(_score);
    }

    public void DamagePlayer()//(int currentHitCount)
    {
        //_score = currentHitCount;

        if (_shieldHealth > 0)
        {
            _shieldHealth -= 1;

            //Disable shield game object
            if (_shieldHealth == 0)
            {
                transform.Find("Player_Shield").gameObject.SetActive(false);

                //play power down sound
                StartCoroutine(PlayPowerupLossSound());
            }
            return;
        }

        _playerLife -= 1;
        //Update life count on UI
        _uiManager.UpdateLivesOnScreen(_playerLife);

        if(_playerLife <= 0)
        {
            //Game over
            _uiManager.InitiateGameOverSequence();

            //Calculate accuracy
            if(_shots != 0)
                _accuracy = ((float)_killCount * 100) / _shots;
            if (_accuracy > 100) _accuracy = 100f;      //will be caused by ideal accuracy with triple shot power up
            _uiManager.DisplayAccuracyOnScreen(_accuracy);
            /*Debug.Log("Game Over!");
            Debug.Log("Score: " + _score + "\tAccuracy: " + _accuracy);*/

            //Stop enemy spawning
            _spawnManager.SetSpawning(false);

            //replace with explosion animation
            GameObject deathExplosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            deathExplosion.transform.localScale.Set(1.5f, 1.5f, 1f);
            deathExplosion.GetComponent<AudioSource>().Stop();

            //Play explosion sound
            //_explosionSound.Play();

            //check if high score
            _uiManager.CheckHighScore(_score, _accuracy);

            Destroy(this.gameObject);

        }

        else
        {
            //Enable damage animations randomly on ship
            if (_playerLife == 2)    //trigger minor (hull) damage
                _damage1.SetActive(true);
            if (_playerLife == 1)    //trigger major (engine) damage
                _damage2.SetActive(true);
        }
    }

    void MajorDamageAnimationScalar(int movementDirection, float animationScalingSpeed)
    {
        if (_damage2.activeSelf)
        {
            //Will only work for 3 conditions-
            //movementDirection = 0     >> No player movement
            //movementDirection = -1    >> Player moving right
            //movementDirection = 1     >> Player moving left

            float scaleDownLimit = 0.75f, scaleUpLimit = 1.1f;

            //scale up or down major damage to match wing distance
            if (movementDirection == -1)
            {
                if (_damage2.transform.position.x < transform.position.x && _damage2.transform.localScale.x > scaleDownLimit)
                {
                    //Condition description-
                    //if damage is LEFT of player position && is active && scale > scaleDownLimit >> scale DOWN by animationScalingSpeed

                    _temp.Set(_damage2.transform.localScale.x - animationScalingSpeed, _damage2.transform.localScale.y - animationScalingSpeed, _damage2.transform.localScale.z);
                    _damage2.transform.localScale = _temp;
                }

                if (_damage2.transform.position.x > transform.position.x && _damage2.transform.localScale.x < scaleUpLimit)
                {
                    //Condition description-
                    //if damage is RIGHT of player position && is active && scale < 1.1 >> scale UP by 0.05f

                    _temp.Set(_damage2.transform.localScale.x + animationScalingSpeed, _damage2.transform.localScale.y + animationScalingSpeed, _damage2.transform.localScale.z);
                    _damage2.transform.localScale = _temp;
                }
            }

            else if (movementDirection == 1)
            {
                if (_damage2.transform.position.x < transform.position.x && _damage2.transform.localScale.x < scaleUpLimit)
                {
                    //Condition description-
                    //if damage is LEFT of player position && is active && scale > scaleDownLimit >> scale DOWN by animationScalingSpeed

                    _temp.Set(_damage2.transform.localScale.x + animationScalingSpeed, _damage2.transform.localScale.y + animationScalingSpeed, _damage2.transform.localScale.z);
                    _damage2.transform.localScale = _temp;
                }

                else if (_damage2.transform.position.x > transform.position.x && _damage2.transform.localScale.x > scaleDownLimit)
                {
                    //Condition description-
                    //if damage is RIGHT of player position && is active && scale < 1.1 >> scale UP by 0.05f

                    _temp.Set(_damage2.transform.localScale.x - animationScalingSpeed, _damage2.transform.localScale.y - animationScalingSpeed, _damage2.transform.localScale.z);
                    _damage2.transform.localScale = _temp;
                }
            }

            else if (movementDirection == 0)
            {
                if (_damage2.transform.localScale.x < 1f)
                {
                    _temp.Set(_damage2.transform.localScale.x + animationScalingSpeed, _damage2.transform.localScale.y + animationScalingSpeed, _damage2.transform.localScale.z);
                    _damage2.transform.localScale = _temp;
                }

                else if (_damage2.transform.localScale.x > 1f)
                {
                    _temp.Set(_damage2.transform.localScale.x - animationScalingSpeed, _damage2.transform.localScale.y - animationScalingSpeed, _damage2.transform.localScale.z);
                    _damage2.transform.localScale = _temp;
                }

                //if scaling goes beyond bounds by less than animationScalingSpeed, it will keep toggling. So reset to 1
                if (_damage2.transform.localScale.x != 1f && (1f - _damage2.transform.localScale.x) < animationScalingSpeed)
                {
                    _temp.Set(1f, 1f, 1f);
                    _damage2.transform.localScale = _temp;
                }
            }
        }

    }

    ///
    /// Power Up Section
    /// 

    public void ActivateTripleShot()
    {
        _isTripleShotActive = true;
        StartCoroutine(EndTripleShot());
    }

    //Co routine for triple shot power up duration
    IEnumerator EndTripleShot()
    {
        yield return new WaitForSeconds(_powerUpDuration);
        _isTripleShotActive = false;

        //play power down sound
        StartCoroutine(PlayPowerupLossSound());
    }

    public void ActivateSpeedBoost()
    {
        //_isSpeedBoostActive = true;
        _playerSpeed *= _speedBoostIntensity;
        StartCoroutine(EndSpeedBoost());
    }

    //Co routine for speed boost power up
    IEnumerator EndSpeedBoost()
    {
        yield return new WaitForSeconds(_powerUpDuration);
        _playerSpeed /= _speedBoostIntensity;
        //_isSpeedBoostActive = false;

        //play power down sound
        StartCoroutine(PlayPowerupLossSound());
    }

    //Shield power-up
    public void IncreaseShieldHealth()
    {
        _shieldHealth += 1;
        if (transform.Find("Player_Shield").gameObject.activeSelf == false)
            transform.Find("Player_Shield").gameObject.SetActive(true);
       /*if (!_shieldDecayCoroutineInProgress)
        {
            //transform.Find("Player_Shield").gameObject.SetActive(true);        //enable child game object - shield
            StartCoroutine(ShieldDecay());
        }*/
    }
    /*IEnumerator ShieldDecay()
    {
        _shieldDecayCoroutineInProgress = true;
        while (_shieldHealth > 0)
        {
            //Debug.Log("Shield health: " + _shieldHealth);
            yield return new WaitForSeconds(10f);
            _shieldHealth--;
        }
        //transform.Find("Player_Shield").gameObject.SetActive(false);       //disable child game object - shield
        _shieldDecayCoroutineInProgress = false;
    }*/

    IEnumerator PlayPowerupLossSound()
    {
        //play power down sound
        _powerupLostSound.pitch = -1;
        _powerupLostSound.time = _powerupLostSound.clip.length - 0.01f;
        _powerupLostSound.Play();
        yield return new WaitForSeconds(1f);
        _powerupLostSound.pitch = 1;
        _powerupLostSound.time = 0;
    }
}
