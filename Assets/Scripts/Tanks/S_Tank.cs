using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_Tank : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] protected Transform tankModel;
    [SerializeField] protected Transform barrelEnd;
    [SerializeField] protected Rigidbody tankRB;
    [SerializeField] protected GameObject tank;
    [SerializeField] protected GameObject tankDestroyed;
#pragma warning restore 0649

    protected S_WorldManager worldManager;

    protected Direction Facing = Direction.Down;

    protected bool alive;
    protected bool isPlayer = false;
    public bool Alive { get => alive; }
    public bool IsPlayer { get => isPlayer; }

    protected float fireTimer;
    protected float rotationTime;
    protected float rotateTimer;
    protected Quaternion startRotation;
    protected Quaternion desiredRotation;

    protected float moveTime;
    protected float moveTimer;
    protected Vector3 startPosition;
    protected Vector3 desiredPosition;

    protected TileDescription currentTile;
    protected TileDescription tileToMoveTo;

    protected UnityEvent MovedToNewTile;

    public virtual void Init(TileDescription[][] generatedTiles, TileDescription startTile, TileDescription playerBaseTile, Direction facing, S_WorldManager worldManager)
    {
        this.currentTile = startTile;
        this.worldManager = worldManager;

        //this.Facing = facing;
        //FaceDirection(facing);
        rotationTime = 0.5f;
        moveTime = 0.5f;

        alive = true;

        MovedToNewTile = new UnityEvent();
    }

    public void MoveInDirection(Direction direction)
    {
        var newTIle = S_MazeGenerator.GetNeighbourTile(currentTile, direction);

        if (newTIle != null)
        {
            MoveToTile(newTIle);
        }
    }

    public void MoveToTile(TileDescription newTile)
    {
        if(currentTile != newTile && (newTile.Type & (TileType.Clear | TileType.PlayerStart | TileType.EnemyStart)) > 0)
        {
            //... move to tile
            tileToMoveTo = newTile;
            moveTimer = moveTime;
            startPosition = transform.position;
            desiredPosition = newTile.Holder.transform.position;
        }
    }

    protected void FaceDirection(Direction direction)
    {
        if(Facing != direction)
        {
            Facing = direction;
            startRotation = tankModel.rotation;
            desiredRotation = RotationFromDirection(Facing);
            rotateTimer = rotationTime;
        }
    }

    protected virtual void Update()
    {
        if (!alive) 
        {
            return;
        }
        if (rotateTimer > 0f)
        {
            rotateTimer -= Time.deltaTime;
            if(rotateTimer <= 0f)
            {
                rotateTimer = 0f;
            }

            float t = 1f - rotateTimer / rotationTime;
            tankModel.rotation = Quaternion.Lerp(startRotation, desiredRotation, t);
        }
        if (startPosition == Vector3.zero && desiredPosition == Vector3.zero)
        {
            return;
        }
        if (moveTimer > 0f && rotateTimer <= 0)
        {
            moveTimer -= Time.deltaTime;
            if (moveTimer <= 0f)
            {
                moveTimer = 0f;
            }

            float t = 1f - moveTimer / moveTime;
            transform.position = Vector3.Lerp(startPosition, desiredPosition, t);
            if (moveTimer <= 0f)
            {
                currentTile = tileToMoveTo;
                MovedToNewTile.Invoke();
            }
        }
    }

    protected Quaternion RotationFromDirection(Direction direction)
    {
        switch(direction)
        {
            case Direction.Up:
                return Quaternion.Euler(0f, 90f, 0f);
            case Direction.Right:
                return Quaternion.Euler(0f, 180f, 0f);
            case Direction.Down:
                return Quaternion.Euler(0f, 270f, 0f);
            case Direction.Left:
                return Quaternion.Euler(0f, 0f, 0f);
        }

        return Quaternion.identity;
    }

    public bool IsMoving() 
    {
        return moveTimer > 0f;
    }

    public void Destroy()
    {
        StartCoroutine(DestroyIE());
    }

    IEnumerator DestroyIE() 
    {
        alive = false;
        tankDestroyed.transform.rotation = tankModel.rotation;
        tank.gameObject.SetActive(false);
        tankDestroyed.SetActive(true);
        
        S_AudioManager.PlaySound(Sounds.TankDestroy);
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
        S_WorldManager.CheckIfGameIsOverStatic();
    }

    protected virtual void Fire() 
    {
        S_VFX_Manager.SpawnMissile(barrelEnd.position, Facing, this);
    }
}
