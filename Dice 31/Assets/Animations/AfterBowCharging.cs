using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterBowCharging : MonoBehaviour
{
    public GameObject arrowPrefab;

    public void ShootArrow() {
        StartCoroutine(ShootArrowAnimation());
    }

    public IEnumerator ShootArrowAnimation() {
        GameObject arrow = Instantiate(arrowPrefab, new Vector3(0.5f, 1f, 1.14f), Quaternion.identity);
        arrow.transform.rotation = Quaternion.Euler(90, 0, 0);
        var runTime = 0f;
        while (runTime < 0.5f) {
            runTime += Time.deltaTime;
            Vector3 start = arrow.transform.position;
            Vector3 target = GameManager.Inst.um.arrowTarget;

            arrow.transform.position = ((runTime * target) + (0.5f - runTime) * start) / 0.5f;
            yield return null;
        }
        Destroy(arrow);
        GameManager.Inst.um.arrowShootDone = true;
    }
}
