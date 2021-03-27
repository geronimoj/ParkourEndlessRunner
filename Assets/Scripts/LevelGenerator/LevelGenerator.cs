using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Generates a level
/// </summary>
[DefaultExecutionOrder(200)]
public class LevelGenerator : MonoBehaviour
{       
    /// <summary>
    /// The width of a lane
    /// </summary>
    [Tooltip("The width of a lane")]
    [SerializeField]
    private float m_laneWidth = 2;
    /// <summary>
    /// The width of a lane
    /// </summary>
    public float LaneWidth => m_laneWidth;
    /// <summary>
    /// The height of a tile
    /// </summary>
    [Tooltip("The height of a layer")]
    [SerializeField]
    private float m_layerHeight = 2;
    /// <summary>
    /// The height of a tile
    /// </summary>
    public float LayerHeight => m_layerHeight;
    /// <summary>
    /// The length of a tile
    /// </summary>
    [Tooltip("The length of a tile")]
    [SerializeField]
    private float m_tileLength = 7;
    /// <summary>
    /// The length of a tile
    /// </summary>
    public float TileLength => m_tileLength;
    /// <summary>
    /// The number of lanes
    /// </summary>
    [Tooltip("The number of lanes")]
    [SerializeField]
    private uint m_numberOfLanes = 3;
    /// <summary>
    /// The number of lanes
    /// </summary>
    private static uint numberOfLanes = 0;
    /// <summary>
    /// The number of lanes
    /// </summary>
    public uint NumberOfLanes => m_numberOfLanes;
    /// <summary>
    /// The number of layers
    /// </summary>
    [Tooltip("The number of layers")]
    [SerializeField]
    private uint m_numberOfLayers = 3;
    /// <summary>
    /// The number of layers
    /// </summary>
    private static uint numberOfLayers = 0;
    /// <summary>
    /// The number of layers
    /// </summary>
    public uint NumberOfLayers => m_numberOfLayers;
    /// <summary>
    /// The maximum change in height
    /// </summary>
    [Tooltip("The maximum height both positively and negatively changed when a lane changes height")]
    [SerializeField]
    private uint m_maxHeightChange = 3;
    /// <summary>
    /// The length of the level. Currently used for debugging only
    /// </summary>
    [Tooltip("The length of the level after the flat period. Total length of level is this + initialFlatLength")]
    [SerializeField]
    private int m_levelLength = 20;
    /// <summary>
    /// The length of the flat section at the very beginning of each run
    /// </summary>
    [Tooltip("The length of the inital flat peroid of terrain without obstacles. Total length of level is this + levelLength")]
    [SerializeField]
    private int m_initalFlatLength = 7;
    /// <summary>
    /// The distance until the game moves everything back towards 0,0,0 to avoid funky lighting
    /// </summary>
    [Tooltip("The distance the player travels until the game moves everything back towards the origin of the world to avoid floating point errors")]
    [SerializeField]
    private float m_distanceUntilLoop = 100;
    /// <summary>
    /// The prefab to use for general tiles
    /// </summary>
    [Tooltip("The prefab used for generating tiles. The object should be a perfect cube with no scale. It will be scaled upon initialisation")]
    [SerializeField]
    private GameObject m_tilePrefab = null;
    /// <summary>
    /// The prefab used for slopes
    /// </summary>
    [Tooltip("The prefab used for generating slopes. Angle of slope should be 45 degrees as it is scaled up upon generation")]
    [SerializeField]
    private GameObject m_slopePrefab = null;
    /// <summary>
    /// The positional offset of the level from the origin
    /// </summary>
    [Tooltip("The offset of the level upon generation in global co-ordinates")]
    [SerializeField]
    private Vector3 m_generateOffset = new Vector3();
    /// <summary>
    /// The positional offset of the level from the origin
    /// </summary>
    public Vector3 GenerateOffset => m_generateOffset;
    /// <summary>
    /// A list of all active tiles for debugging purposes
    /// </summary>
    //Show for debugging purposes
    [Tooltip("The tiles generated")]
    [SerializeField]
    private List<TileInfo> m_tiles = new List<TileInfo>();
    /// <summary>
    /// The min number of tiles between obstacle spawns
    /// </summary>
    [Tooltip("The max number of tiles between obstacle spawns")]
    [SerializeField]
    private uint _minSpaceBetweenObstacles = 2;
    /// <summary>
    /// The max number of tiles between obstacle spawns
    /// </summary>
    [Tooltip("The max number of tiles between obstacle spawns")]
    [SerializeField]
    private uint _maxSpaceBetweenObstacles = 4;
    /// <summary>
    /// All of the obstacles you want to be able to spawn
    /// </summary>
    [Tooltip("All of the obstacles you want to be able to spawn")]
    [SerializeField]
    private GameObject[] obstacles = new GameObject[0];
    /// <summary>
    /// The probability for the height of a tile to change when generated
    /// </summary>
    [Tooltip("The probability for the height of a tile to change when generated")]
    [Range(0,1)]
    [SerializeField]
    private float m_probabilityToChangeHeight = 0.1f;
    /// <summary>
    /// The probability for the height of a tile to change when generated
    /// </summary>
    public float ProbabilityToChangeHeight => m_probabilityToChangeHeight;
    /// <summary>
    /// The probability for a ramp to spawn even if its not needed
    /// </summary>
    [Tooltip("The probability for a ramp to spawn on a height change of +1 even if its unnecessary")]
    [Range(0,1)]
    [SerializeField]
    private float m_probabilityForNonRequiredRamps = 0.5f;
    /// <summary>
    /// The probability for a ramp to spawn even if its not needed
    /// </summary>
    public float ProbabilityForNonRequiredRamp => m_probabilityForNonRequiredRamps;
    /// <summary>
    /// An array to store how long until a lane can spawn an obstacle
    /// </summary>
    private uint[] _laneObstacleTimer = new uint[0];
    /// <summary>
    /// The current length of the level. Used when extending the level.
    /// </summary>
    private int _currentLength = 0;
    /// <summary>
    /// The "position" of the front most tile. This is in grid cords not world cords
    /// </summary>
    private int _frontTilePos = 0;
    /// <summary>
    /// Store a reference to the players transform for distance and position calculations
    /// </summary>
    private Transform _player;
    /// <summary>
    /// Initalises some variables
    /// </summary>
    private void Awake()
    {
        _frontTilePos = 0;
        _currentLength = 0;
        //By dividing it into and casting it to an int, we effectively round the value to be a multiple of tile Length
        //Basically becoming how many tiles until a loop occurs. We then cast it to an int to chop off decimal points
        //Before them multiplying it back by tile length to convert it back into a distance
        m_distanceUntilLoop = (int)(m_distanceUntilLoop / m_tileLength) * m_tileLength;
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        //If the static values have been set, set our values to the static values.
        //This is to help debugging if custom values want to be used when debugging
        if (numberOfLanes != 0)
            m_numberOfLanes = numberOfLanes;
        if (numberOfLayers != 0)
            m_numberOfLayers = numberOfLayers;
    }
    /// <summary>
    /// Generates a level upon starting the game
    /// </summary>
    private void Start()
    {
        CreateLevel();
    }
    /// <summary>
    /// Generates a new world
    /// </summary>
    [ContextMenu("Regenerate Level")]
    public void CreateLevel()
    {
        GenerateLevel((uint)m_initalFlatLength, true, true);
        ExtendLevel((uint)m_levelLength);
    }
    /// <summary>
    /// Extends an already existing world otherwise generates a new one if one does not already exist
    /// </summary>
    [ContextMenu("Extend Level")]
    void ExtendTestLevel()
    {
        ExtendLevel(5);
    }
    /// <summary>
    /// Extends the length of the world by the given length
    /// </summary>
    /// <param name="length">The number of rows to generate</param>
    public void ExtendLevel(uint length)
    {
        GenerateLevel(length, false);
    }
    /// <summary>
    /// Generates a section of the world
    /// </summary>
    /// <param name="layersToGenerate">The number of layers to generate</param>
    /// <param name="regenerate">Wether or not the level should be completely rengerated or if new layers should be added</param>
    /// <param name="makeFlat">Should the layer be at layer 1 only. Use at the start of a level to avoid instant kills</param>
    void GenerateLevel(uint layersToGenerate, bool regenerate = true, bool makeFlat = false)
    {   //If we already have tiles and are trying to generate more, delete the previous ones. Primarily for debugging
        if (regenerate && m_tiles.Count != 0)
        {
            _frontTilePos = 0;
            _currentLength = 0;
            DeleteLevel();
        }

        if (regenerate)
        {
            if (_laneObstacleTimer.Length != m_numberOfLanes)
                _laneObstacleTimer = new uint[m_numberOfLanes];

            for (int lane = 0; lane < m_numberOfLanes; lane++)
                _laneObstacleTimer[lane] = (uint)Random.Range((int)_minSpaceBetweenObstacles, (int)_maxSpaceBetweenObstacles);
        }
        GameObject obstacle;
        bool canChangeHeight;
        int[] heights = new int[m_numberOfLanes];
        int[] prevHeights = new int[m_numberOfLanes];
        int tileIndex;

        for (int i = _currentLength; i < layersToGenerate + _currentLength; i++)
        {
            tileIndex = i - _frontTilePos;

            for (int lane = 0; lane < m_numberOfLanes; lane++)
            {
                TileInfo tile = new TileInfo();
                //Initialise it to nothing
                TileInfo prevTile = new TileInfo();
                //Calculate if the height of this tile should be randomized
                float prob = Random.Range(0, (float)1);
                canChangeHeight = prob < m_probabilityToChangeHeight;
                int prevTileHeight = 0;

                if (tileIndex == 0)
                    canChangeHeight = true;
                else
                {   //Now we have passed the first row, we can propperly initialise the tile
                    prevTile = m_tiles[((tileIndex - 1) * (int)m_numberOfLanes) + lane];
                    prevTileHeight = (int)prevTile.Height;
                }


                prevHeights[lane] = prevTileHeight;

                int min, max;
                min = prevTileHeight - (int)m_maxHeightChange < 0 ? 0 : prevTileHeight - (int)m_maxHeightChange;
                max = prevTileHeight + (int)m_maxHeightChange >= m_numberOfLayers ? (int)m_numberOfLayers - 1 : prevTileHeight + (int)m_maxHeightChange;

                if (prevTile.IsRamp)
                    canChangeHeight = false;

                if (makeFlat)
                {
                    canChangeHeight = false;
                    prevTileHeight = 1;
                }

                tile.Initialise((uint)lane, canChangeHeight ? (uint)Random.Range(min, max + 1) : (uint)prevTileHeight, (uint)i, false);
                heights[lane] = (int)tile.Height;
                m_tiles.Add(tile);
            }
            //We don't need to solve for the very first tiles since they have no previous tiles
            if (tileIndex != 0)
                //Solve to make sure there are no death walls
                for (int lane = 0; lane < m_numberOfLanes; lane++)
                {
                    int validLanes = 0;
                    int heightChange = heights[lane] - prevHeights[lane];
                    //If there is no height change or the height change went downwards, we don't need to solve for anything
                    if (heightChange <= 0)
                        continue;
                    //We know the rest now are height changes upwards.
                    //Check if there is a valid path in the left or right lane
                    //Make sure we aren't reading invalid memory
                                     //Check if the left lane has a height equal to or less than our current height
                                                                                    //Check that the change in height of the left lane is pathable
                    if (lane - 1 >= 0 && prevHeights[lane - 1] <= prevHeights[lane] && heights[lane - 1] - prevHeights[lane - 1] <= 0)
                        validLanes++;
                    //Repeat for the right side
                    if (lane + 1 < m_numberOfLanes && prevHeights[lane + 1] <= prevHeights[lane] && heights[lane + 1] - prevHeights[lane + 1] <= 0)
                        validLanes++;

                    TileInfo current = m_tiles[tileIndex * (int)m_numberOfLanes + lane];
                    //If not, we make this tile a ramp and reduce its height
                    if (validLanes == 0)
                    {
                        current.IsRamp = true;
                        //Reduce the height
                        current.Height = (uint)prevHeights[lane] + 1;

                        //We also reduce it here sp that, when we are checking for valid lanes, a lane with a ramp 
                        //will be treated as having equal height so it can be counted as a valid lane
                        heights[lane] = prevHeights[lane];
                    }
                    //Even if there is a valid path, if the heightChange is only 1, roll to convert it into a ramp
                    else if (heightChange == 1 && Random.Range(0, (float)1) <= m_probabilityForNonRequiredRamps)
                    {
                        current.IsRamp = true;
                        //We also reduce it here sp that, when we are checking for valid lanes, a lane with a ramp 
                        //will be treated as having equal height so it can be counted as a valid lane
                        heights[lane]--;
                    }
                    m_tiles[tileIndex * (int)m_numberOfLanes + lane] = current;
                }
            //Instantiate any obstacles to slide or vault over
            //Make sure we have obstacles
            if (!makeFlat && obstacles.Length != 0)
                //Decrement the timers if they haven't already reached 0
                for (int lane = 0; lane < m_numberOfLanes; lane++)
                {
                    int currentTile = tileIndex * (int)m_numberOfLanes + lane;
                    if (m_tiles[currentTile].IsRamp)
                        continue;

                    if (_laneObstacleTimer[lane] != 0)
                        _laneObstacleTimer[lane]--;
                    //If they have reached 0, check if we can spawn an obstacle on that tile
                    else if (!m_tiles[currentTile - (int)m_numberOfLanes].IsRamp)
                    {
                        //Select a random obstacle to spawn
                        obstacle = obstacles[Random.Range(0, obstacles.Length)];
                        TileInfo tile = m_tiles[currentTile];
                        tile.AddObstacle(obstacle, m_laneWidth, m_layerHeight, m_tileLength, m_generateOffset);

                        m_tiles[currentTile] = tile;
                        //Reset the timer
                        _laneObstacleTimer[lane] = (uint)Random.Range((int)_minSpaceBetweenObstacles, (int)_maxSpaceBetweenObstacles);
                    }
                }

            //Actually generate the lanes boxes and stuff
            for (int lane = 0; lane < m_numberOfLanes; lane++)
                m_tiles[tileIndex * (int)m_numberOfLanes + lane].GenerateTiles(m_tilePrefab, m_slopePrefab, m_laneWidth, m_layerHeight, m_tileLength, m_generateOffset);
        }

        _currentLength += (int)layersToGenerate;
    }
    /// <summary>
    /// Deletes all tiles and the objects they contain
    /// </summary>
    [ContextMenu("Delete Level")]
    void DeleteLevel()
    {   //Loop through the tiles
        for (int i = 0; i < m_tiles.Count; i++)
            //Loop through the objects on the tile
            for (int objects = 0; objects < m_tiles[i].m_objectsOnTile.Length; objects++)
                //Destroy the object
                DestroyImmediate(m_tiles[i].m_objectsOnTile[objects]);
        m_tiles.Clear();
    }
    /// <summary>
    /// Deletes tiles from the front of the array
    /// </summary>
    /// <param name="tilesToDelete">The number of rows to delete</param>
    void DeleteFrontTiles(uint tilesToDelete)
    {
        for (int i = 0; i < tilesToDelete * m_numberOfLanes; i++)
        {   //Loop through the first tile and delete it. We don't use i because we then remove this tile from the list, making the next tile the first tile
            for (int tileObj = 0; tileObj < m_tiles[0].m_objectsOnTile.Length; tileObj++)
                Destroy(m_tiles[0].m_objectsOnTile[tileObj]);
            //Remove the front tile
            m_tiles.RemoveAt(0);
        }
    }
    /// <summary>
    /// Keeps generating the world and deleting the previous world as well as looping the player back to 0, 0 every once in a while to avoid floating point error
    /// </summary>
    private void FixedUpdate()
    {   //Keep generating the world
        if (_player.position.z > (_frontTilePos + 1) * m_tileLength)
        {
            _frontTilePos++;
            DeleteFrontTiles(1);
            ExtendLevel(1);
        }
        //If the player gets too far away, teleport everything back
        //TLDR: Perform a loop
        if (_player.position.z > m_distanceUntilLoop)
        {   //Teleport the player
            Vector3 p = _player.position;
            p.z -= m_distanceUntilLoop;
            _player.position = p;
            
            //Teleport the world
            for (int i = 0; i < m_tiles.Count; i++)
            {
                for (int tileObj = 0; tileObj < m_tiles[i].m_objectsOnTile.Length; tileObj++)
                {
                    p = m_tiles[i].m_objectsOnTile[tileObj].transform.position;
                    p.z -= m_distanceUntilLoop;
                    m_tiles[i].m_objectsOnTile[tileObj].transform.position = p;
                }
            }
            //Reset the length values so iterators remain valid and we don't generate 100 units in front of the level. That would be bad
            _frontTilePos = 0;
            _currentLength = m_tiles.Count / (int)m_numberOfLanes;

            Physics.SyncTransforms();
        }
    }
    /// <summary>
    /// Set the levels size
    /// </summary>
    /// <param name="layers">Number of layers the level has</param>
    /// <param name="lanes">Number of lanes the level has</param>
    public static void SetLevelSize(uint layers, uint lanes)
    {
        numberOfLayers = layers;
        numberOfLanes = lanes;
    }
}
/// <summary>
/// Stores information about a given tile
/// </summary>
[System.Serializable]
public struct TileInfo
{   /// <summary>
    /// Stores the objects on this tile for ease of access for deleting
    /// </summary>
    public GameObject[] m_objectsOnTile;
    /// <summary>
    /// The lane this tile exists on
    /// </summary>
    private uint _lane;
    /// <summary>
    /// The lane this tile exists on
    /// </summary>
    public uint Lane => _lane;
    /// <summary>
    /// The Z location of the tile in grid co-ordinates
    /// </summary>
    private uint _forwardPoint;
    /// <summary>
    /// The Z location of the tile in grid co-ordinates
    /// </summary>
    public uint ForwardPoint => _forwardPoint;
    /// <summary>
    /// The height of this tile
    /// </summary>
    private uint _height;
    /// <summary>
    /// Gets or sets the height
    /// </summary>
    public uint Height
    {
        get { return _height; }
        set
        {
            _height = value;
        }
    }
    /// <summary>
    /// Is the tile a ramp
    /// </summary>
    private bool _isRamp;
    /// <summary>
    /// Does this tile contain a ramp
    /// </summary>
    public bool IsRamp
    {
        get { return _isRamp; }
        set
        {   //Adjust the size of the objects on this tile based on whether this tile needs to store a ramp
            _isRamp = value;
            GameObject obstacle = m_objectsOnTile[m_objectsOnTile.Length - 1];
            int length = _isRamp ? 2 : 1;
            length += obstacle != null ? 1 : 0;
            m_objectsOnTile = new GameObject[length];
            m_objectsOnTile[length - 1] = obstacle;
        }
    }
    /// <summary>
    /// Initalises the tile
    /// </summary>
    /// <param name="obstacle">The obstacle on this tile</param>
    /// <param name="lane">The lane this tile is in</param>
    /// <param name="height">The height of this tile</param>
    /// <param name="forwardPoint">The z step of this tile</param>
    /// <param name="isRamp">If this tile is a ramp</param>
    public void Initialise(uint lane, uint height, uint forwardPoint, bool isRamp)
    {   //Set the length of objects to store
        int length = isRamp ? 2 : 1;
        m_objectsOnTile = new GameObject[length];
        _lane = lane;
        _height = height;
        _forwardPoint = forwardPoint;
        _isRamp = isRamp;
    }
    /// <summary>
    /// Generates the tile based on the information given in the initialiser
    /// </summary>
    /// <param name="tilePrefab">The prefab for standard ground</param>
    /// <param name="tileSlope">The prefab for slopes</param>
    /// <param name="laneWidth">The width of a tile</param>
    /// <param name="tileHeight">The height of a tile</param>
    /// <param name="tileLength">The length of a tile</param>
    /// <param name="posOffset">The position offset of the tile from the origin</param>
    public void GenerateTiles(GameObject tilePrefab, GameObject tileSlope, float laneWidth, float tileHeight, float tileLength, Vector3 posOffset)
    {
        GameObject obj;
        //Generate the regular cube for the ground
        obj = GameObject.Instantiate(tilePrefab, new Vector3(laneWidth * _lane + posOffset.x, ((float)(_isRamp ? _height - 1 : _height) / 2) * tileHeight + posOffset.y, _forwardPoint * tileLength + posOffset.z), Quaternion.identity);
        //Scale it up so we only have to use 1 object for the ground
        obj.transform.localScale = new Vector3(laneWidth, tileHeight * (_isRamp ? _height : _height + 1), tileLength);

        Renderer r = obj.GetComponent<Renderer>();
        r.material.SetTextureScale("_WallTex", new Vector2(1, _isRamp ? _height : _height + 1));
        r.material.SetTextureScale("_MainTex", new Vector2(1, tileLength));
        //Store it
        m_objectsOnTile[0] = obj;
        //If we have a ramp, create it
        if (_isRamp)
        {
            //Create the ramp
            obj = GameObject.Instantiate(tileSlope, new Vector3(laneWidth * _lane + posOffset.x, _height * tileHeight + posOffset.y, _forwardPoint * tileLength + posOffset.z), Quaternion.identity);
            //Scale the ramp to fit the tile
            obj.transform.localScale = new Vector3(laneWidth, tileHeight, tileLength);
            //Store it
            m_objectsOnTile[1] = obj;
        }
    }
    /// <summary>
    /// Instantiates and stores an obstacle gameObject on this tile
    /// </summary>
    /// <param name="obstacle">The obstacle to create and add</param>
    public void AddObstacle(GameObject obstacle, float laneWidth, float tileHeight, float tileLength, Vector3 posOffset)
    {   //Increase the length of objectsOnTiles to fit the obstacle
        int length = _isRamp ? 2 : 1;
        length += obstacle != null ? 1 : 0;
        m_objectsOnTile = new GameObject[length];
        //Spawn the obstacle
        GameObject obj = GameObject.Instantiate(obstacle, new Vector3(laneWidth * _lane + posOffset.x, (_height + 0.5f) * tileHeight + posOffset.y, _forwardPoint * tileLength + posOffset.z), Quaternion.identity);
        //Store the object
        m_objectsOnTile[length - 1] = obj;
    }
    /// <summary>
    /// Stores an already existing obstacle on this tile
    /// </summary>
    /// <param name="obstacle">The obstacle to add</param>
    public void AddObstacle(ref GameObject obstacle)
    {   //Increase the length of objectsOnTiles to fit the obstacle
        int length = _isRamp ? 2 : 1;
        length += obstacle != null ? 1 : 0;
        m_objectsOnTile = new GameObject[length];
        //Store the object
        m_objectsOnTile[length - 1] = obstacle;
    }
}
