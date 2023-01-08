using System.Collections;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] int dmg = 1;
    [SerializeField] float range = 100f;
    [SerializeField] float fireCooldown = .25f;
    [SerializeField] LayerMask ignoreLayers;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] TrailRenderer bulletTrail;
    [SerializeField] Transform firePoint;
    [SerializeField] float trailSpeed;
    [SerializeField] AudioClip shootSound;

    float lastTimeFired;
    Camera mainCamera;
    AudioSource audioSource;

    private void Start()
    {
        mainCamera = GetComponentInParent<Camera>();
        audioSource = GetComponent<AudioSource>();
        //El arma siempre mira al centro de la pantalla
        transform.LookAt(mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 50)));
    }

    public void Shoot()
    {
        if(lastTimeFired + fireCooldown < Time.time)
        {
            audioSource.pitch = Random.Range(1f, 1.3f);
            audioSource.PlayOneShot(shootSound);
            muzzleFlash.Play();
            TrailRenderer trail = Instantiate(bulletTrail, firePoint.position, Quaternion.identity);
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, range))
            {
                ManageCollisions(hit);
                StartCoroutine(SpawnTrail(trail, hit.point));
            }
            else
            {
                Vector3 midScreen = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 50));
                StartCoroutine(SpawnTrail(trail, midScreen)); //Si acertamos, simulamos un hitpoint muy lejos para el trail
            }
            lastTimeFired = Time.time;
        }
    }

    private void ManageCollisions(RaycastHit hit)
    {
        if(hit.transform.tag.Equals("Enemy"))
        {
            HealthComponent healthComponent = hit.transform.GetComponent<HealthComponent>();
            if (healthComponent)
                healthComponent.TakeDamage(dmg);
        }
        else if(hit.transform.tag.Equals("Target"))
        {
            Target target = hit.transform.GetComponentInParent<Target>();
            if(target)
            {
                target.Touch();
                audioSource.PlayOneShot(target.targetHitted);
            }
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint)
    {
        Vector3 startPosition = trail.transform.position;
        float distance = Vector3.Distance(startPosition, hitPoint);
        float startDistance = distance;

        while(distance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (distance / startDistance));
            distance -= Time.deltaTime * trailSpeed;
            yield return null;
        }

        trail.transform.position = hitPoint;
    }
}
