using System.Collections;
using UnityEngine;

public class En_SniperJill : EnemyController
{
    private enum State { Defend, Shoot }

    private readonly int HashShoot = Animator.StringToHash("Shoot");
    private readonly int HashDefend = Animator.StringToHash("Defend");

    [Header("Statemachine")]
    [SerializeField] private State currentState = State.Defend;
    [SerializeField] private float defendStateDuration = 2f;
    [SerializeField] private float shootStateDuration = 1.5f;

    [Header("Weapon")]
    [SerializeField] private GameObject bulletPrefab = null;
    [SerializeField] private float speed = 8f;
    [SerializeField] private int shootDamage = 2;
    [SerializeField] private int bulletPerShot = 3;
    [SerializeField] private Transform shootExit = null;
    [SerializeField] private LayerMask targetLayer = default;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Player = GameManager.Instance.Player;
        ChangeState(State.Defend);
    }

    private void ChangeState(State state)
    {
        if (currentState != state)
            OnStateEnter();

        switch (state)
        {
            case State.Defend:
                currentState = State.Defend;

                MyAnimator.Play(HashDefend);
                IsShielding = true;

                StartCoroutine(DefendState());

                break;

            case State.Shoot:
                if (IsVisible)
                {
                    currentState = State.Shoot;

                    MyAnimator.Play(HashShoot);
                    IsShielding = false;

                    StartCoroutine(ShootState());
                }
                else
                {
                    ChangeState(State.Defend);
                }
                   
                break;
        }
    }

    private void OnStateEnter()
    {
        if (Player != null)
            physicMover.LookAtTarget(Player.transform);
    }

    private IEnumerator DefendState()
    {
        yield return new WaitForSeconds(defendStateDuration);

        ChangeState(State.Shoot);
    }

    private IEnumerator ShootState()
    {
        yield return new WaitForSeconds(0.2f);
        yield return CreateProjectiles();
        yield return new WaitForSeconds(shootStateDuration);

        ChangeState(State.Defend);
    }

    private IEnumerator CreateProjectiles()
    {
        for (int i = 0; i < bulletPerShot; i++)
        {
            SFXManager.Instance.PlayOneShot(shootSfx, 2f);
            Vector2 direction = physicMover.CurrentDirection;

            EnemyBullet bullet;
            bullet = ObjectPoolManager.Instance.SpawnFromPool(bulletPrefab.name, shootExit.position, Quaternion.identity).GetComponent<EnemyBullet>();
            bullet.Initialized(speed, shootDamage, direction, targetLayer);

            yield return new WaitForSeconds(0.35f);
        }
    }
}
