using System;
using UnityEngine;
using Photon.Bolt;

public class BasicCharacter : EntityBehaviour<ICustomStatePlayer>
{

    public static BasicCharacter Local;


    private static readonly int WALK_PROPERTY = Animator.StringToHash("Walk");

    [SerializeField]
    private float speed = 1f;

    [Header("Relations")]
    [SerializeField]
    private Animator animator = null;

    [SerializeField]
    private Rigidbody physicsBody = null;

    [SerializeField]
    private SpriteRenderer spriteRenderer = null;

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
            GetComponentInChildren<Camera>().enabled = false;
        }
        else
        {
            Local = this;
            if(!Entered && GameState.Instance != null)
            {
                Enter();
            }
        }

        //Debug.LogWarning("transform: " + transform.position + " is Owner " + entity.IsOwner);
    }

    public void Enter()
    {
        if (Entered) return;
        Entered = true;
        state.Dark = GameState.Instance.IsNextPlayerDark();
        state.Nickname = "Ghost #" + (GameState.Instance.state.NextPlayerId+1);
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
            inputY = 1;
        else if (Input.GetKey(KeyCode.DownArrow))
            inputY = -1;

        // Horizontal
        float inputX = 0;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            inputX = 1;
            spriteRenderer.flipX = false;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            inputX = -1;
            spriteRenderer.flipX = true;
        }


        if (inputX != 0 || inputY != 0)
        {
            state.IsMoving = true;
        }
        else
        {
            state.IsMoving = false;
        }

        // Normalize
        _movement = new Vector3(inputX, 0, inputY).normalized;

        physicsBody.velocity = (_movement * speed) * 4;
    }

    public void Update()
    {
        if (entity.IsAttached)
        {
            state.Animator.SetBool(WALK_PROPERTY, state.IsMoving);
        }
    }

    private void OnApplicationQuit()
    {
        Leave();
    }


}
