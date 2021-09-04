using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_PlayerTank : S_Tank
{
    public static S_PlayerTank instance;

    public override void Init(TileDescription[][] generatedTiles, TileDescription startTile, TileDescription playerBaseTile, Direction facing, S_WorldManager worldManager)
    {
        base.Init(generatedTiles, startTile, playerBaseTile, facing, worldManager);
        instance = this;
        rotationTime = 0.2f;
        isPlayer = true;
        tankModel.rotation = RotationFromDirection(Direction.Up);
        Facing = Direction.Up;
    }

    protected override void Update()
    {
        base.Update();
        if (!alive)
        {
            return;
        }
        if (S_UI_MovementController.instance != null)
        {
            Vector3 inputDirection = S_UI_MovementController.MovementDirection / S_UI_MovementController.Size;

            float length = inputDirection.magnitude;
            if (length > 1f)
            {
                inputDirection = inputDirection.normalized;
            }

            inputDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * inputDirection.y + transform.right * inputDirection.x;
            if (inputDirection.x > 0.3f)
            {
                FaceDirection(Direction.Right);
            }
            if (inputDirection.x < -0.3f)
            {
                FaceDirection(Direction.Left);
            }
            if (inputDirection.z > 0.3f)
            {
                FaceDirection(Direction.Up);
            }
            if (inputDirection.z < -0.3f)
            {
                FaceDirection(Direction.Down);
            }

            if (S_UI_MovementController.WhantsToMove)
            {
                AddPlayerInput(inputDirection);
            }
            else
            {
                AddPlayerInput(Vector3.zero);
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            FaceDirection(Direction.Left);
        }
        if(Input.GetKey(KeyCode.W))
        {
            FaceDirection(Direction.Up);
        }
        if(Input.GetKey(KeyCode.D))
        {
            FaceDirection(Direction.Right);
        }
        if(Input.GetKey(KeyCode.S))
        {
            FaceDirection(Direction.Down);
        }
    }

    private void AddPlayerInput(Vector3 direction)
    {
        tankRB.velocity = direction * 2;
    }

    protected override void Fire()
    {
        base.Fire();
        S_AudioManager.PlaySound(Sounds.TankShoot);
    }

    public static void FirePlayerStatic() 
    {
        instance.Fire();
    }

}
