using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField]
    private PlayerStateMachine stateMachine;

    public void PlayerFinishedDying()
    {
        stateMachine.Respawn();
    }

    public void PlayerFinishedRespawning()
    {
        stateMachine.FinishedRespawning();
    }

    public void Footstep()
    {
        AudioManager.instance.PlaySound(Sounds.Footstep);
    }

    public void Crawl()
    {
        AudioManager.instance.PlaySound(Sounds.Crawl);
    }
}
