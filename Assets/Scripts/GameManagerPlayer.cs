using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameManagerPlayer
{
    public int Id { get; set; }

    public string Nickname { get; set; }

    public bool Dark { get; set; }

    [SerializeField]
    private float TimeLastAttack;

    public bool Attack(GameManager gameManager)
    {
        if (TimeLastAttack + gameManager.AttackCooldown > gameManager.CurrentTime)
        {
            return false;
        }
        TimeLastAttack = gameManager.CurrentTime;
        return true;
    }




}
