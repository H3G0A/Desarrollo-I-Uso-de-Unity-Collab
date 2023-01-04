using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] int dmg = 1;
    [SerializeField] float range = 100f;
    [SerializeField] float fireCooldown = .25f;
    [SerializeField] LayerMask ignoreLayers;

    float fireCooldownCounter;
    Transform firePoint;
    Camera mainCamera;

    private void Start()
    {
        firePoint = GetComponentInChildren<Transform>();
        mainCamera = GetComponentInParent<Camera>();

        transform.LookAt(mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 3)));
    }

    public void Shoot()
    {
        if(Time.time >= fireCooldownCounter)
        {
            RaycastHit hit;
            if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, range))
                ManageCollisions(hit);
            fireCooldownCounter = Time.time + fireCooldown;
        }
    }

    private void ManageCollisions(RaycastHit hit)
    {
        Debug.Log(hit.transform.name);
        HealthComponent healthComponent = hit.transform.GetComponent<HealthComponent>();
        if (healthComponent)
            healthComponent.TakeDamage(dmg);
    }
}
