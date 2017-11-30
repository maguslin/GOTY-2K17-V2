﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkForDeathAbility : Ability
{
    public ParticleSystem particleEffect;
    public float duration = 20f;
    public float damageMultiplier = 2f;
    public float range = 40f;
    public float playerHeight = 2f;

    private LineRenderer line;
    private Vector3 shootPoint;

    protected override void Start()
    {
        base.Start();
        applyOnFrame = true;
        this.line = GetComponent<LineRenderer>();
    }

    public override void applyEffect(GameObject player)
    {
        base.applyEffect(player);

        player.transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        StartCoroutine(ShotEffect());
        shootPoint = new Vector3(0, playerHeight,0) + player.transform.position;

        RaycastHit hit;
        this.line.SetPosition(0, shootPoint);
        if (Physics.Raycast(shootPoint, Camera.main.transform.forward, out hit, range))
        {
            // show point of hit with particle effect
            particleEffect.transform.position = hit.point;
            particleEffect.Play();

            this.line.SetPosition(1, hit.point);
            RigCollider rigCollider = hit.transform.GetComponent<RigCollider>();
            if (rigCollider == null)
            {
                return;
            }

            Unit unit = rigCollider.RootUnit;
            if (unit is AggressiveUnit)
            {
                AggressiveUnit monster = (AggressiveUnit)unit;
                monster.ApplyStatus(new StatusVulnerable(monster, duration, damageMultiplier));
            }
        }
        else
        {
            this.line.SetPosition(1, shootPoint + (Camera.main.transform.forward * range));
        }
    }

    private IEnumerator ShotEffect()
    {
        line.enabled = true;
        yield return new WaitForSeconds(.5f);
        line.enabled = false;
    }
}
