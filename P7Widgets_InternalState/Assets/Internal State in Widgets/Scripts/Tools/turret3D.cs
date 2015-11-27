using UnityEngine;
using System.Collections;

public class turret3D : MonoBehaviour {

    float angle;
    public float speed = 50.0f;
    public Rigidbody bulletPrefab;
	public float minAngle = 60, maxAngle = 120;							//The angles between which the turret rotates

    float bulletSpeed = 10;
    private Transform _transform;

    public float fireRate;
    public GameObject barrel;

    public bool canShoot = false;

    private float nextFire;

    public float levelToEnable = 4;

    AudioSource _audio;
    public AudioClip laserSound;

    void Start()
    {
        _audio = GetComponent<AudioSource>();
		_transform = this.transform;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, minAngle);
    }



    void LateUpdate()
    {
        if(GameVariables.GameManager.CurrentLevel == levelToEnable)
        {
            angle = transform.eulerAngles.z;

            Shoot();

            if (angle <= minAngle && angle <= maxAngle)
            {
                speed = 50;

            }
            if (angle >= maxAngle && angle >= minAngle)
            {
                speed = -50;

            }

            transform.Rotate(0, 0, speed * Time.deltaTime);
        }

    }




    void Shoot()
    {
        if (Time.time > nextFire)
        {
            PlaySound(laserSound);
            nextFire = Time.time + fireRate;
            Rigidbody bullet = (Rigidbody)Instantiate(bulletPrefab, barrel.transform.position, _transform.localRotation);
            bullet.velocity = barrel.transform.up * bulletSpeed;

        }

    }


    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            _audio.clip = clip;
            _audio.Play();
        }
        else
        {
            Debug.LogWarning("No audio clip was found.");
        }
    }
}
