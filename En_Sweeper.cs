using System.Collections;
using UnityEngine;

public class En_Sweeper : EnemyController
{
    private enum State { Idle, Move }

    private readonly int HashIdle = Animator.StringToHash("Idle");
    private readonly int HashBlink = Animator.StringToHash("Blink");

    [Header("Statemachine")]
    [SerializeField] private State currentState = State.Move;
    [SerializeField] private float idleStateDuration = 1.5f;

    [Header("Patrol Settings")]
    [SerializeField] private float moveMaxDistance = 10f;
    [SerializeField] private bool moveLeft = true;

    private void Start()
    {
        IsShielding = true;
        ChangeState(State.Idle);
    }

    private void ChangeState(State state)
    {
        if (currentState != state)
            OnStateEnter();

        switch (state)
        {
            case State.Idle:
                currentState = State.Idle;
                StartCoroutine(IdleCoroutine());
                break;

            case State.Move:
                currentState = State.Move;
                StartCoroutine(MoveCoroutine());
                break;
        }
    }

    private void OnStateEnter()
    {
        MyRigidbody.velocity = Vector2.zero;
    }

    private IEnumerator IdleCoroutine()
    {
        if (Player != null)
        {
            MyAnimator.Play(HashBlink);
        }
        else
        {
            MyAnimator.Play(HashIdle);
        }

        yield return new WaitForSeconds(idleStateDuration);

        ChangeState(State.Move);
    }

    private IEnumerator MoveCoroutine()
    {
        Vector3 startPosition = transform.position;
        Vector3 newPosition = new Vector3(moveLeft ? startPosition.x - moveMaxDistance : startPosition.x + moveMaxDistance, startPosition.y);

        while ((newPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            float desiredMoveSpeed;

            if (Player != null)
            {
                MyAnimator.Play(HashBlink);
                desiredMoveSpeed = moveSpeed * 2;
            }
            else
            {
                MyAnimator.Play(HashIdle);
                desiredMoveSpeed = moveSpeed;
            }

            transform.position = Vector3.MoveTowards(transform.position, newPosition, desiredMoveSpeed * Time.deltaTime);

            yield return null;
        }

        moveLeft = !moveLeft;
        ChangeState(State.Idle);
    }
}
