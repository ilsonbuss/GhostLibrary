using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAtackAnimator : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        //if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("AtackTrigger");
        }
    }
}