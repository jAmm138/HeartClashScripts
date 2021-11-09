using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDropManager : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayerMask = default;
    [SerializeField] private GameObject bombDropPrefab = null;
    [SerializeField] private int damage = 3;

    [SerializeField] private Transform exitPoint = null;

    private PlayerController player;
    private IEnumerator bombCoroutine;
    private bool isStarted = false;

    private void Start()
    {
        bombCoroutine = DropBomb();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (0 != (targetLayerMask.value & 1 << other.gameObject.layer))
        {
            if (player == null)
                player = other.GetComponent<PlayerController>();

            isStarted = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (0 != (targetLayerMask.value & 1 << other.gameObject.layer))
        {
            if (player == null)
                player = other.GetComponent<PlayerController>();

            if (isStarted && player != null)
            {
                StopAllCoroutines();
                StartCoroutine(DropBomb());
            }               
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (0 != (targetLayerMask.value & 1 << other.gameObject.layer))
        {
            StopCoroutine(bombCoroutine);
        }
    }

    private IEnumerator DropBomb()
    {
        isStarted = false;

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 6; i++)
        {
            if (player != null)
            {
                BombDrop bullet;
                Vector2 exit = new Vector2(player.transform.position.x, exitPoint.transform.position.y);

                bullet = ObjectPoolManager.Instance.SpawnFromPool(bombDropPrefab.name, exit, Quaternion.identity).GetComponent<BombDrop>();
                bullet.Initialized(damage, transform, targetLayerMask);
            }
            else
            {
                yield break;
            }

            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(2f);

        isStarted = true;
    }
}
