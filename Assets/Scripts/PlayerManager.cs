using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private PlayerStateMachine player;

    public static PlayerManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(instance.gameObject);
            }
        }
        
        instance = this;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("Main Scene"))
        {
            Vector3 startPosition = GameManager.instance.GetPlayerHubStartPosition();
            MovePlayer(startPosition);
        }
    }

    public void ApplyForce(Vector3 force, ForceMode mode, bool zeroGravity, bool cancelJumps)
    {
        player.ApplyForce(force,mode, zeroGravity, cancelJumps);
    }

    public void ApplyForceNoBounce(Vector3 force, ForceMode mode, bool zeroGravity, bool cancelJumps)
    {
        player.ApplyForceNoBounce(force,mode, zeroGravity, cancelJumps);
    }
    
    public void KillPlayer()
    {
        player.Die();
    }

    public void KillPlayerDeathPlane(float height)
    {
        player.DieFromDeathPlane(height);
    }

    public void SetGravity(Vector3 direction)
    {
        player.SetDownDirection(direction);
    }

    public void MovePlayer(Vector3 worldPos)
    {
        player.ForceMovePlayerPosition(worldPos);
    }

    public void MovePlayerDynamically(Vector3 worldPos)
    {
        player.MoveToPositionDynamically(worldPos);
    }

    public void DisableControls(bool disable)
    {
        player.DisableControls(disable);
    }

    public void FadePlayer()
    {
        player.FadePlayer();
    }
}
