using UnityEngine;
using Photon.Bolt;

public class BasicCharacter : EntityBehaviour<ICustomStatePlayer>
{

    public static BasicCharacter Local;

    public Vector3 lightTeamSpawnCenter = new Vector3(0.0f, 0.0f, 6.0f);
    public float teamSpawnRadius = 1.0f;
    public Vector3 darkTeamSpawnCenter = new Vector3(0.0f, 0.0f, -6.0f);

    //private static readonly int WALK_PROPERTY = Animator.StringToHash("Walk");

    public Camera Camera;

    [SerializeField]
    private float speed = 1f;

    [Header("Relations")]
    [SerializeField]
    private Animator animator = null;

    [SerializeField]
    private Rigidbody physicsBody = null;

    [SerializeField]
    private GameObject ghost = null;

    private Vector3 _movement;

    public bool Entered { get; set; }
    public bool Left { get; set; }

    //similar to Start
    public override void Attached()
    {
        state.SetTransforms(state.CustomCubeTransform, transform);

        state.SetAnimator(animator);

        if(entity.IsOwner == false)
        {
            //Debug.LogWarning("Destroy!!!");
            //GetComponentInChildren<AudioListener>().enabled = false;
            //GetComponentInChildren<Camera>().enabled = false;
        }
        else
        {
            Local = this;
            if(!Entered && GameState.Instance != null && GameState.Instance.IsReady())
            {
                Enter();
            }
        }

        //Debug.LogWarning("transform: " + transform.position + " is Owner " + entity.IsOwner);
    }

    private bool dark = false;

    public void Enter()
    {
        if (Entered) return;
        Entered = true;
        dark = GameState.Instance.IsNextPlayerDark();
        state.Dark = dark;
        state.Nickname = "Ghost #" + (GameState.Instance.state.NextPlayerId+1);
        //Debug.LogWarning("Enter " + GameState.Instance.state.NextPlayerId  + " Dark " + state.Dark);
        if (state.Dark)
        {
            transform.position = darkTeamSpawnCenter;
        }
        else
        {
            transform.position = lightTeamSpawnCenter;
        }
        transform.position += new Vector3(UnityEngine.Random.Range(-teamSpawnRadius, teamSpawnRadius), 0.0f, UnityEngine.Random.Range(-teamSpawnRadius, teamSpawnRadius));

        PlayerEnter enter = PlayerEnter.Create(GlobalTargets.Everyone);
        enter.Player = entity;
        enter.Dark = state.Dark;
        enter.Nickname = state.Nickname;
        enter.Send();
    }


    public void Leave()
    {
        if (Left) return;
        PlayerLeave left = PlayerLeave.Create(GlobalTargets.Everyone);
        left.player = entity;
        left.Send();
    }

    //similar to Update
    public override void SimulateOwner()
    {
        // Vertical
        float inputY = 0;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            inputY = 1;
            ghost.transform.eulerAngles = new Vector3(ghost.transform.rotation.x, 325, ghost.transform.rotation.z);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            inputY = -1;
            ghost.transform.eulerAngles = new Vector3(ghost.transform.rotation.x, 150, ghost.transform.rotation.z);
        }

        // Horizontal
        float inputX = 0;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            inputX = 1;
            ghost.transform.eulerAngles = new Vector3(ghost.transform.rotation.x, 55, ghost.transform.rotation.z);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            inputX = -1;
            ghost.transform.eulerAngles = new Vector3(ghost.transform.rotation.x, 235, ghost.transform.rotation.z);
        }

        // Normalize
        _movement = new Vector3(inputX, 0, inputY).normalized;
        physicsBody.velocity = (_movement * speed) * 4;
    }

    public void Update()
    {
        if (entity.IsOwner && !Camera.gameObject.activeInHierarchy)
        {
            Camera.gameObject.SetActive(true);
        }

        if (entity.IsAttached)
        {
            //state.Animator.SetBool(WALK_PROPERTY, state.IsMoving);
        }
    }

    private void OnApplicationQuit()
    {
        Leave();
    }


}
