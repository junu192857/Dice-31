using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterBowCharging : MonoBehaviour
{
    public GameObject arrowPrefab;

    private float shootTime = 0.3f;
    public void ShootArrow() {
        StartCoroutine(ShootArrowAnimation());
    }

    public IEnumerator ShootArrowAnimation() {
        GameObject arrow = Instantiate(arrowPrefab, new Vector3(0.51f, 1f, 0.77f), Quaternion.identity);
        Vector3 start = arrow.transform.position;
        Vector3 target = GameManager.Inst.um.arrowTarget;
        float rotation = Vector2.Angle(new Vector2(-1, 0), new Vector2(target.x-start.x,target.z-start.z));
        Debug.Log(rotation);
        arrow.transform.rotation = Quaternion.Euler(90f, 0f, -45 + rotation);

        AudioSource arrowAudio = arrow.GetComponent<AudioSource>();
        arrowAudio.volume = GameManager.Inst.sm.SFXVolume;
        arrowAudio.Play();

        var runTime = 0f;
        while (runTime < shootTime) {
            runTime += Time.deltaTime;
            arrow.transform.position = ((runTime * target) + (shootTime - runTime) * start) / shootTime;
            yield return null;
        }
        Destroy(arrow);
        GameManager.Inst.um.arrowShootDone = true;
    }
}
