using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTank : MonoBehaviour
{
    public Jibbit jibbit;
    public GameObject[] deactivateWhenTank;
    public GameObject[] activateWhenTank;

    public Transform[] tankTurrets;
    public Animator[] turretAnimators;
    public TankShell shellPrefab;
    public GameObject gunBarrel;
    public ParticleSystem muzzleFlashParticles;
    public float fireRate;

    static bool startAsTank;
    public static bool isTank;
    int currKonamiIdx;
    float timeSinceLastFire;
    Vector2 mousePos = Vector2.zero;
    Vector2 controllerAim = Vector2.zero;

    void Start()
    {
        isTank = false;
        if (startAsTank)
        {
            startAsTank = false;
            isTank = true;
            foreach (GameObject obj in deactivateWhenTank)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in activateWhenTank)
            {
                obj.SetActive(true);
            }
            timeSinceLastFire = fireRate;
            CameraScript.current.closeZoomLevel = 14;
            CameraScript.current.farZoomLevel = 20;
            CameraScript.current.camera.orthographicSize = 14;
            JibbitManager.AcquireJibbit(jibbit.id);
            JibbitAcquiredPopup.current.Acquire(jibbit);
        }
    }

    private void Update()
    {
        timeSinceLastFire += Time.deltaTime;

        if (isTank)
        {
            Vector2 aimDirection = Vector2.zero;
            if (Controls.usingController)
            {
                aimDirection = controllerAim;
            } else
            {
                aimDirection = mousePos - (Vector2) CameraScript.current.camera.WorldToScreenPoint(tankTurrets[0].position);
            }

            float angle = -(Utilities.VectorToDegrees(aimDirection) + (270 - CameraScript.current.camera.transform.eulerAngles.y));
            foreach (Transform turret in tankTurrets)
            {
                turret.eulerAngles = new Vector3(0, angle);
            }
        }
    }

    public void OnLook(InputValue val)
    {
        if (isTank)
        {
            if (Controls.usingController)
            {
                Vector2 input = val.Get<Vector2>();
                if (input != Vector2.zero)
                {
                    controllerAim = input;
                }
            }
            else
            {
                mousePos = Input.mousePosition;
            }
        }
    }

    public void OnTankAttack()
    {
        if (isTank && timeSinceLastFire >= fireRate)
        {
            Fire();
        }
    }

    private void Fire()
    {
        TankShell shell = TankShell.SpawnShell(shellPrefab, gunBarrel.transform.position, gunBarrel.transform.rotation);
        shell.firedByPlayer = true;
        shell.Fire();
        timeSinceLastFire = 0;
        muzzleFlashParticles.Stop();
        muzzleFlashParticles.Play();
        foreach (Animator gunAnimator in turretAnimators)
        {
            gunAnimator.SetTrigger("Fire");
        }
    }

    public void OnKonamiUp()
    {
        if (currKonamiIdx == 0 || currKonamiIdx == 1)
            ++currKonamiIdx;
        else
            currKonamiIdx = 0;
    }

    public void OnKonamiDown()
    {
        if (currKonamiIdx == 2 || currKonamiIdx == 3)
            ++currKonamiIdx;
        else
            currKonamiIdx = 0;
    }

    public void OnKonamiLeft()
    {
        if (currKonamiIdx == 4 || currKonamiIdx == 6)
            ++currKonamiIdx;
        else
            currKonamiIdx = 0;
    }

    public void OnKonamiRight()
    {
        if (currKonamiIdx == 5 || currKonamiIdx == 7)
            ++currKonamiIdx;
        else
            currKonamiIdx = 0;
    }

    public void OnKonamiB()
    {
        if (currKonamiIdx == 8)
            ++currKonamiIdx;
        else
            currKonamiIdx = 0;
    }

    public void OnKonamiA()
    {
        if (currKonamiIdx == 9)
            ++currKonamiIdx;
        else
            currKonamiIdx = 0;
    }

    public void OnKonamiStart()
    {
        if (currKonamiIdx == 10)
        {
            if (UIPauseMenu.instance.paused)
                UIPauseMenu.instance.TogglePause();
            startAsTank = true;
            LevelBridge.Reload("With great power...");
        }
        else
            currKonamiIdx = 0;
    }
}
