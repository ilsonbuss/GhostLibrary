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
    public GameObject ghost = null;

    private GameObject nearCrystal = null;

    private Vector3 _movement;

    public bool Entered { get; set; }
    public bool Left { get; set; }

    //similar to Start
    public override void Attached()
    {
        state.SetTransforms(state.CustomCubeTransform, transform);
        state.SetAnimator(animator);


        if (entity.IsOwner == false)
        {
            //Debug.LogWarning("Destroy!!!");
            GetComponentInChildren<AudioListener>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
        }
        else
        {
            //GetComponentInChildren<AudioListener>().enabled = true;
            //GetComponentInChildren<Camera>().enabled = false;

            Local = this;
            if (!Entered && GameState.Instance != null && GameState.Instance.IsReady())
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
        state.Nickname = "Ghost #" + (GameState.Instance.state.NextPlayerId + 1);
        //Debug.LogWarning("Enter " + GameState.Instance.state.NextPlayerId  + " Dark " + state.Dark);
        Respawn();

        PlayerEnter enter = PlayerEnter.Create(GlobalTargets.Everyone);
        enter.Player = entity;
        enter.Dark = state.Dark;
        enter.Nickname = state.Nickname;
        enter.Send();

        //if(dark)
        //{
        //    ghost = gameObject.transform.Find("DarkGhost").gameObject;
        //    //Debug.LogWarning("Enter " + ghost.tag);
        //}
        //else
        //{
        //    ghost = gameObject.transform.Find("LightGhost").gameObject;
        //    //Debug.LogWarning("Enter " + ghost.tag);
        //}
    }


    public void Leave()
    {
        //if (Left) return;
        //PlayerLeave left = PlayerLeave.Create(GlobalTargets.Everyone);
        //left.Dark = dark;
        //left.Send();
    }

    public void Respawn()
    {
        if (state.Dark)
        {
            transform.position = darkTeamSpawnCenter;
        }
        else
        {
            transform.position = lightTeamSpawnCenter;
        }
        transform.position += new Vector3(UnityEngine.Random.Range(-teamSpawnRadius, teamSpawnRadius), 0.0f, UnityEngine.Random.Range(-teamSpawnRadius, teamSpawnRadius));
    }

    float targetMoveAngle;
    Quaternion targetQuat;
    float targetMoveTime;
    float targetMove;

    //similar to Update
    public override void SimulateOwner()
    {
        //block input after end game
        if (GameState.Instance.state.GameFinished)
        {
            return;
        }


        // Vertical
        float inputY = 0;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            inputY = 1;
            targetMoveAngle = 325;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            inputY = -1;
            targetMoveAngle = 150;
        }

        // Horizontal
        float inputX = 0;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            inputX = 1;
            targetMoveAngle = 55;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            inputX = -1;
            targetMoveAngle = 235;
        }
        
        if (inputX != 0.0 || inputY != 0.0)
        {
            var old = targetMoveAngle;
            targetMoveAngle = Mathf.Atan2(inputX, inputY) * Mathf.Rad2Deg - 35;
            //Debug.Log("Diff: " + (old - targetMoveAngle));
        }


        //ghost.transform.eulerAngles = new Vector3(ghost.transform.rotation.x, Mathf.LerpAngle(ghost.transform.rotation.y, targetMoveAngle, 0.1f), ghost.transform.rotation.z);
        //ghost.transform.eulerAngles = new Vector3(ghost.transform.rotation.x, targetMoveAngle, ghost.transform.rotation.z);

        targetQuat = Quaternion.Euler(new Vector3(ghost.transform.rotation.x, targetMoveAngle, ghost.transform.rotation.z));


        if (Input.GetKey(KeyCode.Space))
        {
            if (state.IsNearCrystal && nearCrystal != null)
            {
                //ativa/desativa o cristal
                if (nearCrystal.TryGetComponent(out LightManager lightManager))
                {
                    //se for time DARK, desativa
                    //se forma time Light, ativa
                    lightManager.Activate(state.Dark ? false : true);
                }
            }
        }



        // Normalize
        _movement = new Vector3(inputX, 0, inputY).normalized;
        physicsBody.velocity = (_movement * speed) * 2;
    }

    public void Update()
    {
        if (entity.IsOwner && !Camera.gameObject.activeInHierarchy)
        {
            Camera.gameObject.SetActive(true);
        }



        // if (entity.IsOwner == false)
        //{
        //ghost.transform.eulerAngles = new Vector3(ghost.transform.rotation.x, state.Rotation.eulerAngles.y, ghost.transform.rotation.z);
        ///state.Rotation.eulerAngles.z;
        //}

        if (entity.IsOwner)
        {
            ghost.transform.rotation = Quaternion.Slerp(ghost.transform.rotation, targetQuat, Time.deltaTime * 5.75f);
            state.MoveAngle = ghost.transform.rotation.eulerAngles.y;
            //state.Rotation = ghost.transform.rotation;
        }
        else
        {
            //ghost.transform.rotation = state.Rotation;
            ghost.transform.eulerAngles = new Vector3(ghost.transform.rotation.x, state.MoveAngle, ghost.transform.rotation.z);
        }

        if (entity.IsAttached)
        {
            //state.Animator.SetBool(WALK_PROPERTY, state.IsMoving);
        }
    }

    public void OnTriggerEnter(Collider other)
    {

        //check if is crystal collision
        if (other.TryGetComponent(out LightManager component))
        {
            //set player state to near crystal
            state.IsNearCrystal = true;
            nearCrystal = other.gameObject;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        //check if is crystal collision
        if (other.TryGetComponent(typeof(LightManager), out Component component))
        {
            state.IsNearCrystal = false;
            nearCrystal = null;
        }
    }

    private void OnApplicationQuit()
    {
        Leave();
    }


}
