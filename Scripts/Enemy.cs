using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Game environment boundaries
    private float _leftXBound = GlobalVariables.leftXBound;
    private float _rightXBound = GlobalVariables.rightXBound;
    private float _topYBound = GlobalVariables.topYBound;
    private float _bottomYBound = GlobalVariables.bottomYBound;

    private float _deltaTime;

    [SerializeField]
    private float _enemyMoveSpeed = 2.5f;

    private float _enemyScaleFactor;

    //private float _enemyRotateSpeed = 5f;

    /*//Collision counter
    private static int _laserEnemyCollisionCount = 0;*/

    private MyPlayer _player;  //Player handle for score updates

    private Animator _animator;     //Animation handle for explosion sequence on death

    /*[SerializeField]
    private AudioClip _explosionClip;    //to store an audio clip from unity*/
    private AudioSource _explosionSound;    //handle for playing audio clip

    private Vector3 _movementDirection;      //enemy movement direction

    // Start is called before the first frame update
    void Start()
    {
        _deltaTime = Time.deltaTime;
        //_enemyMoveSpeed *= deltaTime;
        //_enemyRotateSpeed = 5f;

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<MyPlayer>();
        if (_player == null)
            Debug.LogError("Player was not instantiated in Enemy class");

        _animator = GetComponent<Animator>();
        if (_animator == null)
            Debug.LogError("Animation reference was not instantiated in Enemy class");

        _enemyScaleFactor = Random.Range(0.5f, 1.6f);

        transform.position = new Vector3(Random.Range((_leftXBound+_enemyScaleFactor), (_rightXBound-_enemyScaleFactor)), _topYBound + 2f, 0);
        transform.localScale = new Vector3(_enemyScaleFactor, _enemyScaleFactor, transform.localScale.z);   //Sca;ing enemies to classify score

        _explosionSound = transform.GetComponent<AudioSource>();
        if (_explosionSound == null)
            Debug.LogError("Explosion sound was not initialized in Explosion class");

        //_explosionSound.clip = _explosionClip;

        _movementDirection = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        //movement direction is 1 unit down and 1 unit towards player
        float xDirection = 0;
        if (_player != null)
            xDirection = (_player.gameObject.transform.position.x - transform.position.x)/Mathf.Abs(_player.gameObject.transform.position.x - transform.position.x);

        //move enemy down towards player
        _movementDirection.Set(xDirection * 0.25f, -1f, 0f);
        transform.Translate(_movementDirection * _deltaTime * _enemyMoveSpeed);
        //transform.Translate(_deltaTime * _enemyMoveSpeed * Vector3.down);
        //transform.Rotate(Vector3.up * _enemyRotateSpeed);

        //if reached bottom, wrap to top
        if (transform.position.y < _bottomYBound - 3f)
        {
            float randomPosX = Random.Range((_leftXBound + _enemyScaleFactor), (_rightXBound - _enemyScaleFactor));
            transform.position = new Vector3(randomPosX, -transform.position.y, transform.position.z);
        }
    }

    //Detect laser collision
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Enemy hits the player
            MyPlayer player = other.transform.GetComponent<MyPlayer>();      //Check if player component exists

            //damage player
            if (player != null)
                player.DamagePlayer();//(_laserEnemyCollisionCount);

            //Trigger enemy death explosion animation sequence and destroy this object
            _animator.SetTrigger("IsEnemyDead");

            //Remove the collider component to prevent damage from explosion sequence
            Destroy(GetComponent<Collider2D>());
            Destroy(GetComponent<Rigidbody2D>());

            //play explosion sound
            _explosionSound.Play();

            StartCoroutine(DestroyEnemy());

        }
        if(other.CompareTag("Laser"))
        {
            //Laser hits the enemy

            /*//Increase collision count
            _laserEnemyCollisionCount++;*/

            //Destroy laser
            Destroy(other.gameObject);
            
            //update score
            if(_player != null)
            {
                if (_enemyScaleFactor < 1.0f)
                    _player.IncreaseScore(20);
                else
                    _player.IncreaseScore(10);
            }

            //Trigger enemy death explosion animation sequence and destroy this object
            _animator.SetTrigger("IsEnemyDead");

            //Remove the collider component to prevent damage from explosion sequence
            Destroy(GetComponent<Collider2D>());
            Destroy(GetComponent<Rigidbody2D>());

            //play explosion sound
            _explosionSound.Play();

            StartCoroutine(DestroyEnemy());

        }
        
        //Since this script can be invoked when colliding with power ups, we cannot write any code beyond conditions
        //Destroy(this.gameObject);       //destroy this script's object at the end

        //Instantiate(this, new Vector3(0, 8, 0), Quaternion.identity);
        //if (other.transform.name.ToString().Equals("Player"))
        //    Instantiate(other, new Vector3(0, 0, 0), Quaternion.identity);
    }

    IEnumerator DestroyEnemy()
    {
        //Destroy all children first
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject, 0.5f);

        //If animation is complete or object top reaches bottom in between animation
        //_animator.IsInTransition(0)
        float destructionTime = Time.time + 3f;
        while((Time.time < destructionTime) && (transform.position.y + 2f > _bottomYBound))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Destroy(this.gameObject);
        
    }
}
