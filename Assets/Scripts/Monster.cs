/* NAME:            Monster.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     This is a place holder so that the Game Manager can compile.
 * REQUIREMENTS:    Base class MovingObject.cs must be present.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MovingObject {

	//[SerializeField]
	public List<Stats> EnemyParty = new List<Stats> ();
	public Canvas BattleCanvas;

    //public Stats MonsterStats;
    [Tooltip("Set whether this AI searches for the best path")]
    public bool Pathfinding;
    [Tooltip("How many seconds between attemping to move again.")]
    public float MoveRate = 0.5f;
    [Tooltip("The maximum distance between player to chase her")]
    public int Radius = 5;

    private float NextActionTime = 0f;
	private Animator AnimatorMonster;
    private Transform Target;
    private bool Paused = false;
    //AStar algorithm variables
    private MyPathNode NextGridNode;
    private MyPathNode[,] grid;
    private MyPathNode currentGridPosition;
    private MyPathNode startGridPosition;
    private MyPathNode endGridPosition;
    private Transform MySpot;
    private AudioClip MonsterSound;
    private AudioSource MyAudioSource;

    /// <summary>
    /// Pauses the monster when the player flees to give him a chance to escape.
    /// </summary>
    public void Pause(float pause_time)
    {
        Paused = true;
        StartCoroutine(PauseCoroutine(pause_time));
    }

    /// <summary>
    /// The coroutine that counts the time for pausing. Then unpauses.
    /// </summary>
    private IEnumerator PauseCoroutine(float pause_time)
    {
        yield return new WaitForSeconds(pause_time);
        Paused = false;
    }

	// Use this for initialization
	protected override void Awake () {
        AnimatorMonster = GetComponent<Animator>();
        MyAudioSource = GetComponent<AudioSource>();
		//MonsterStats = new Stats ("Monster", 1, 50, 0, 5, 5);
        base.Awake();
    }

    private void Start()
    {
        Target = Player.Instance.transform;
        MonsterSound = ResourceManager.Instance.GetSound("MonsterGrunt");
        MySpot = transform.GetChild(0);
        MySpot.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsState(GameStates.IdleState)) {
            if (!GameManager.Instance.IsState(GameStates.PlayerMovingState))
                NextActionTime = Time.time;
        }
        else {
            if (!IAmMoving && MySpot.position != transform.position)
                MySpot.position = transform.position;
            if (!Paused) { //only move when game is in idle mode and monster is not paused from fleeing battle
                if (Time.time > NextActionTime) { //see if it is time to move again
                    NextActionTime += MoveRate; //set the next time to move
                    float sqr_magnitude = Vector3.SqrMagnitude(new Vector3((Target.position.x - transform.position.x), (Target.position.y - transform.position.y)));
                    if (sqr_magnitude <= 1) {
                        //BattleManager.Instance.Encounter(GetComponent<Stats>());
						BattleCanvas.GetComponent<BattleManager>().Encounter(this);
						//BattleManager.Instance.Encounter(this);
						//OneVOneManager.Instance.Encounter(this);
                    }
                    else if (sqr_magnitude <= (Radius * Radius) && !IAmMoving) {
                        if (!Pathfinding)
                            DetectPlayer();
                        else
                            DetectPlayerAStar();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Attempts to move in the direction (x_dir, y_dir)
    /// </summary>
    protected override void AttemptMove<T>(int x_dir, int y_dir)
    {
        MySpot.position = (transform.position + new Vector3(x_dir, y_dir));
        base.AttemptMove<T>(x_dir, y_dir, this);
    }

    /* ################################################################### NOT WORKING :((
    /// <summary>
    /// Called on when the monster enters a collision
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("collide with " + collision.gameObject.name);
        if (collision.gameObject.tag == "Player")
        {
            //start battle screen, get component of the battle game object, or maybe the game manager will handle this
            Debug.Log("Monster wants to battle!");
            BattleManager.Instance.Encounter(EnemyParty);
        }
    }
    */

    /// <summary>
    /// Called if the Monster cannot move
    /// </summary>
    protected override void OnCantMove<T>(T component)
    {
        //TODO: define what the monster does when run into a blocking object

        //Declare hitPlayer and set it to equal the encountered componenet.
        Player hitPlayer = component as Player;
    }

    /// <summary>
    /// if player reaches the monster's line of sight, the monster chases after the player
    /// </summary>
    private void DetectPlayer()
    {
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Radius, 1 << LayerMask.NameToLayer(Target.tag));
        int x_dir = 0;
        int y_dir = 0;
        //If the difference in positions is approximately zero (Epsilon) do the following:
        if (Mathf.Abs(Target.position.x - transform.position.x) < float.Epsilon) {
            //If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
            y_dir = Target.position.y > transform.position.y ? 1 : -1;
        }
        //If the difference in positions is not approximately zero (Epsilon) do the following:
        else {
            //Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
            x_dir = Target.position.x > transform.position.x ? 1 : -1;
        }
        if (!MyAudioSource.isPlaying)
            SoundManager.Instance.RandomizeSfx(MonsterSound, MyAudioSource);
        AttemptMove<Player>(x_dir, y_dir);
        /*
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject.tag == Target.tag) {
                AttemptMove<Player>(x_dir, y_dir);
            }
        }
        */
    }

    /// <summary>
    /// Uses an AStar algorithm to find the best path to get to the Target.dd
    /// </summary>
    private void DetectPlayerAStar()
    {
        //Calculate if the player is within Radius
        int x_dir = 0;
        int y_dir = 0;

        grid = new MyPathNode[3*Radius, 3*Radius];
        Vector2 GridOrigin = new Vector2(transform.position.x - Radius, transform.position.y - Radius);
        RaycastHit2D hit;
        //define the grid of size 2Radiusx2Radius around the monster, record if each square is a Wall
        for (int x = 0; x < 3*Radius; x++) {
            for (int y = 0; y < 3*Radius; y++) {
                //mini diagonal linecast in grid square
                hit = Physics2D.Linecast(
                    new Vector2(GridOrigin.x+x-0.4f, GridOrigin.y+y-0.4f), new Vector2(GridOrigin.x+x+0.4f, GridOrigin.y+y+0.4f), BlockingLayer);
                grid[x, y] = new MyPathNode(x, y, hit.transform != null);
            }
        }
        startGridPosition = new MyPathNode(Radius, Radius, false);
        currentGridPosition = startGridPosition;
        endGridPosition = new MyPathNode(
            (int)(Target.transform.position.x-GridOrigin.x), (int)(Target.transform.position.y-GridOrigin.y), false);
        findUpdatedPath(currentGridPosition.x, currentGridPosition.y);
        if (NextGridNode.x != currentGridPosition.x)
            x_dir = NextGridNode.x > currentGridPosition.x ? 1 : -1;
        else if (NextGridNode.y != currentGridPosition.y)
            y_dir = NextGridNode.y > currentGridPosition.y ? 1 : -1;
        if (!MyAudioSource.isPlaying)
            SoundManager.Instance.RandomizeSfx(MonsterSound, MyAudioSource);
        AttemptMove<Player>(x_dir, y_dir);
    }
    
    /// <summary>
    /// Sets NextNode to be the next best tile to take for the AStar algorithm.
    /// </summary>
    private void findUpdatedPath(int currentX, int currentY)
    {
        MySolver<MyPathNode, System.Object> aStar = new MySolver<MyPathNode, System.Object>(grid);
        IEnumerable<MyPathNode> path = aStar.Search(new Vector2(currentX, currentY), new Vector2(endGridPosition.x, endGridPosition.y), null);
        int x = 0;

        if (path != null) {
            foreach (MyPathNode node in path) {
                if (x == 1) {
                    NextGridNode = node;
                    break;
                }
                x++;
            }
        }
    }

    /// <summary>
    /// AStar heuristic
    /// </summary>
    public class MySolver<TPathNode, TUserContext> : SettlersEngine.SpatialAStar<TPathNode,
    TUserContext> where TPathNode : SettlersEngine.IPathNode<TUserContext>
    {
        protected override Double Heuristic(PathNode inStart, PathNode inEnd)
        {
            int formula = AStar.distance;
            int dx = Math.Abs(inStart.X - inEnd.X);
            int dy = Math.Abs(inStart.Y - inEnd.Y);

            if (formula == 0)
                return Math.Sqrt(dx * dx + dy * dy); //Euclidean distance
            else if (formula == 1)
                return (dx * dx + dy * dy); //Euclidean distance squared
            else if (formula == 2)
                return Math.Min(dx, dy); //Diagonal distance
            else if (formula == 3)
                return (dx * dy) + (dx + dy); //Manhatten distance
            else
                return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
        }

        protected override Double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            return Heuristic(inStart, inEnd);
        }

        public MySolver(TPathNode[,] inGrid)
            : base(inGrid)
        {
        }
    }

    /// <summary>
    /// Class to represent each grid spot in the AStar algorithm used in DetectPlayerAStar();
    /// </summary>
    public class MyPathNode : SettlersEngine.IPathNode<System.Object>
    {
        public int x;
        public int y;
        bool IsWall;

        public MyPathNode() { }

        public MyPathNode(int x, int y, bool is_wall)
        {
            this.x = x;
            this.y = y;
            IsWall = is_wall;
        }

        public bool IsWalkable(System.Object unused)
        {
            return !IsWall;
        }
    }
}
