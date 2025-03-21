using UnityEngine;
using Utils;

/// <summary>
/// Used for throwing objects
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private Vector3 direction = Vector3.zero;
    private Vector3 destination = Vector3.zero;

    [Header("Properties")]
    [SerializeField] float speed = 1f;
    [SerializeField] float maxMovementTime = 1f;
    [SerializeField] float currentMovementTime = 0.0f;
    public float damage = 2.5f;
    [SerializeField] private GameStates gameState = GameStates.Playing;


    private Rigidbody body;
    private LivingEntity player;
    [SerializeField] SphereCollider bodyCollider;

    void Start()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Player");
        player = temp.GetComponent<LivingEntity>();
        bodyCollider = GetComponent<SphereCollider>();
        Setup();
    }

    void Update()
    {
        if (gameState != GameStates.Playing) { return; }

        Vector3 velocity = direction * (Time.deltaTime * speed);
        Vector3 new_position = body.transform.position + velocity;

        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance - 0.4 < bodyCollider.radius)
        {
            player.TakeDamage(damage, transform.position, direction);
            gameObject.SetActive(false);
            return;
        }
        currentMovementTime += Time.deltaTime;

        if(currentMovementTime > maxMovementTime)
        {
            gameObject.SetActive(false);
        }

        body.MovePosition(new_position);
    }

    private void OnEnable()
    {
        body = GetComponent<Rigidbody>();
        GameManager.Instance.Subscribe(OnGameStateChange);
        OnGameStateChange(GameManager.Instance.GameState);
        Setup();
    }

    private void OnDisable()
    {
        GameManager.TryGetInstance()?.Unsubscribe(OnGameStateChange);
    }

    public void Setup()
    {
        currentMovementTime = 0.0f;
    }

    public void setDestination(Vector3 _destination)
    {
        destination = _destination;
        direction = (destination - transform.position).normalized;
    }

    private void OnGameStateChange(GameStates state)
    {
        gameState = state;
        switch (gameState)
        {
            case GameStates.Playing:
                body.isKinematic = true;
                body.constraints = RigidbodyConstraints.None;
                break;
            default:
                body.isKinematic = false;
                body.constraints = RigidbodyConstraints.FreezeAll;
                break;
        }
    }
}
