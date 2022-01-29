using UnityEngine;
using Photon.Bolt;

public class BasicCharacter : EntityBehaviour<ICustomStatePlayer>
{
    private static readonly int WALK_PROPERTY = Animator.StringToHash("Walk");

    public Camera Camera;

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

    //similar to Start
    public override void Attached()
    {
        state.SetTransforms(state.CustomCubeTransform, transform);

        state.SetAnimator(animator);
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

        //if (state.IsMoving)
        //{
        //    //state.Animator.SetBool(WALK_PROPERTY, Math.Abs(_movement.sqrMagnitude) > Mathf.Epsilon);
        //}

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
            state.Animator.SetBool(WALK_PROPERTY, state.IsMoving);
        }
    }
}
