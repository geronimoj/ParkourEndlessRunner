using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Generates a level
/// </summary>
[DefaultExecutionOrder(200)]
public class LevelGenerator : MonoBehaviour
{
    [Space]
    [Header("Tile Size Info")]
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

    [Space]
    [Header("Level Size Info")]
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
    /// The positional offset of the level from the origin
    /// </summary>
    [Tooltip("The offset of the level upon generation in global co-ordinates")]
    [SerializeField]
    private Vector3 m_generateOffset = new Vector3();
    /// <summary>
    /// The positional offset of the level from the origin
    /// </summary>
    public Vector3 GenerateOffset => m_generateOffset;
    [Space]
    [Header("Prefabs")]
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
    /// Prefab used for the indoors sections
    /// </summary>
    [Tooltip("The prefab used for generating slopes. Angle of slope should be 45 degrees as it is scaled up upon generation")]
    [SerializeField]
    private GameObject m_indoorPrefab = null;
    /// <summary>
    /// A list of all active tiles for debugging purposes
    /// </summary>
    //Show for debugging purposes
    [Tooltip("The tiles generated")]
    [SerializeField]
    private List<TileInfo> m_tiles = new List<TileInfo>();
    [Space]
    [Header("Obstacles")]
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
    private Obstacle[] _obstacles = new Obstacle[0];
    [Space]
    [Header("Decorations")]
    /// <summary>
    /// The prefab used for curbs
    /// </summary>
    [Tooltip("The prefab used for generating curbs that appear when a layer drop down")]
    [SerializeField]
    private Decoration[] m_decorations = new Decoration[0];
    /// <summary>
    /// The rows that will be enabled +- the players current row to improve performance
    /// </summary>
    [Space]
    [Header("Level Generation")]
    
    [Tooltip("The rows that will be enabled +- the players current row to improve performance")]
    [SerializeField]
    private int m_disableRowFrom = 7;
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
    /// The probability for a door to an indoor section of the level to spawn
    /// </summary>
    [Tooltip("The probability for a door to an indoor section of the level to spawn")]
    [Range(0, 1)]
    [SerializeField]
    private float m_probabilityToSpawnDoor = 0.1f;
    /// <summary>
    /// How many rows the player must pass before that row gets deleted
    /// </summary>
    [Tooltip("How many rows the player must be infront of the front most tile to allow for the front row to be deleted")]
    [SerializeField]
    private uint m_deleteTilesAtRow = 5;
    /// <summary>
    /// The minimum length of an indoor section
    /// </summary>
    [Space]
    [Header("Indoors Info")]
    
    [Tooltip("The minimum length of an indoor section")]
    [SerializeField]
    private uint minIndoorLength = 2;
    /// <summary>
    /// The minimum distance between two indoor obstacles
    /// </summary>
    [SerializeField]
    private int _minDistBetweenIndoorObstacle = 2;
    /// <summary>
    /// The maximum distance between two indoor obstacles
    /// </summary>
    [SerializeField]
    private int _maxDistBetweenIndoorObstacle = 2;
    /// <summary>
    /// An array to store how long until a lane can spawn an obstacle
    /// </summary>
    private uint[] _laneObstacleTimer = new uint[0];
    /// <summary>
    /// An array that stores how long until a lane can spawn an obstacle indoors
    /// </summary>
    private uint[] _indoorObstacleTimer = new uint[0];
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
        Player.player.OnLaneChange.AddListener(ToggleTiles);
        CreateLevel();
    }
    /// <summary>
    /// Generates a new world
    /// </summary>
    [ContextMenu("Regenerate Level")]
    public void CreateLevel()
    {   //Create the level but starting flat for a bit to avoid killing the player with obstacles / walls
        GenerateLevel((uint)m_initalFlatLength, true, true);
        ExtendLevel((uint)m_levelLength);
    }
    /// <summary>
    /// Extends an already existing world otherwise generates a new one if one does not already exist
    /// </summary>
    [ContextMenu("Extend Level")]
    void ExtendTestLevel()
    {   //Extends the level a fixed 5 tiles. This exists only for debugging purposes
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
        //If we are regenerating the level, recalculate the obstacle timers
        if (regenerate)
        {
            if (_laneObstacleTimer.Length != m_numberOfLanes)
                _laneObstacleTimer = new uint[m_numberOfLanes];
            if (_indoorObstacleTimer.Length != m_numberOfLanes)
                _indoorObstacleTimer = new uint[m_numberOfLanes];

            for (int lane = 0; lane < m_numberOfLanes; lane++)
            {
                _laneObstacleTimer[lane] = (uint)Random.Range((int)_minSpaceBetweenObstacles, (int)_maxSpaceBetweenObstacles);
                _indoorObstacleTimer[lane] = (uint)Random.Range(_minDistBetweenIndoorObstacle, _maxDistBetweenIndoorObstacle);
            }
        }
        //Just preparing some variables for later
        GameObject obstacle;
        bool canChangeHeight;
        int[] heights = new int[m_numberOfLanes];
        int[] prevHeights = new int[m_numberOfLanes];
        int tileIndex;
        //Begin looping over the new layers we need to generate
        for (int i = _currentLength; i < layersToGenerate + _currentLength; i++)
        {
            tileIndex = i - _frontTilePos;
            //Loop over each lane and generate a new tile for it
            //The following for loops are kept separate because some of them
            //contain continue statements which would break this loop
            for (int lane = 0; lane < m_numberOfLanes; lane++)
            {
                TileInfo tile = new TileInfo();
                //Initialise it to nothing
                TileInfo prevTile = new TileInfo();
                //Calculate if the height of this tile should be randomized
                float prob = Random.Range(0, (float)1);
                canChangeHeight = prob < m_probabilityToChangeHeight;
                int prevTileHeight = 0;
                //If this is the first row, we can't get the previous tiles
                if (tileIndex == 0)
                    canChangeHeight = true;
                else
                {   //Now we have passed the first row, we can propperly initialise the tile
                    prevTile = m_tiles[((tileIndex - 1) * (int)m_numberOfLanes) + lane];
                    prevTileHeight = (int)prevTile.Height;
                }
                //Store the previous tile height
                prevHeights[lane] = prevTileHeight;
                //Calculate the minimum and maximum height this tile could change to
                int min, max;
                min = prevTileHeight - (int)m_maxHeightChange < 0 ? 0 : prevTileHeight - (int)m_maxHeightChange;
                max = prevTileHeight + (int)m_maxHeightChange >= m_numberOfLayers ? (int)m_numberOfLayers - 1 : prevTileHeight + (int)m_maxHeightChange;
                //If this tile is a ramp, we don't change height
                if (prevTile.IsRamp)
                    canChangeHeight = false;
                //Should this tile not change in height
                if (makeFlat)
                {
                    canChangeHeight = false;
                    prevTileHeight = 1;
                }
                //Store the height
                uint height = canChangeHeight ? (uint)Random.Range(min, max + 1) : (uint)prevTileHeight;
                //Initialise the tile, store its height and itself
                tile.Initialise((uint)lane, height, (uint)i, false);
                //Determine if a tile should be indoors
                //If the previous tile was indoors, we need to update this tile
                //If we haven't met the minimum distance we want to set the indoor height
                //Once we have met the minimum distance, we only need to continue the indoor section until the height drops below the indoor height
                if (prevTile.HasIndoors)
                {
                    if (prevTile.IndoorLength < minIndoorLength || height > prevTile.IndoorHeight)
                        //This works for making a strait line through the environment for an indoor section
                        tile.SetIndoorHeight(prevTile.IndoorHeight, prevTile.IndoorLength + 1);
                    else
                        //Otherwise, this must be an exit so set the height to not be an imminent death drop
                        tile.Height = prevTile.IndoorHeight;
                }
                //Store the height and tile
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
                    //Get the current tile
                    TileInfo current = m_tiles[tileIndex * (int)m_numberOfLanes + lane];

                    float prob = Random.Range(0, (float)1);
                    //If not, we make this tile a ramp and reduce its height
                    if (validLanes == 0)
                    {   //Check if we should spawn a door to an indoor section
                        if (!current.HasIndoors && m_tiles[(tileIndex - 2) * (int)m_numberOfLanes + lane].Height == prevHeights[lane] && prob < m_probabilityToSpawnDoor)
                            //Set the height for the indoor section
                            current.SetIndoorHeight((uint)prevHeights[lane], 0);
                        //Otherwise spawn a ramp
                        else
                        {
                            current.IsRamp = true;
                            //Reduce the height
                            current.Height = (uint)prevHeights[lane] + 1;
                            //We also reduce it here so that, when we are checking for valid lanes, a lane with a ramp 
                            //will be treated as having equal height so it can be counted as a valid lane
                            heights[lane] = prevHeights[lane];
                        }
                    }
                    //Even if there is a valid path, if the heightChange is only 1, roll to convert it into a ramp
                    else if (heightChange == 1 && prob <= m_probabilityForNonRequiredRamps)
                    {
                        current.IsRamp = true;
                        //We also reduce it here sp that, when we are checking for valid lanes, a lane with a ramp 
                        //will be treated as having equal height so it can be counted as a valid lane
                        heights[lane]--;
                    }
                    //Even if there is a valid path, check if we want to spawn a door
                                            //Make sure the previous lane and lane before that have equal heights
                    else if (heightChange > 0 && m_tiles[(tileIndex - 2) * (int)m_numberOfLanes + lane].Height == prevHeights[lane] && !current.HasIndoors && prob <= m_probabilityToSpawnDoor)
                        //Set the height for the indoor section
                        current.SetIndoorHeight((uint)prevHeights[lane], 0);

                    m_tiles[tileIndex * (int)m_numberOfLanes + lane] = current;
                }
            //Generate Obstacles
            //Make sure we have obstacles
            if (!makeFlat && _obstacles.Length != 0)
                //Decrement the timers if they haven't already reached 0
                for (int lane = 0; lane < m_numberOfLanes; lane++)
                {
                    int currentTile = tileIndex * (int)m_numberOfLanes + lane;
                    if (m_tiles[currentTile].IsRamp)
                        continue;
                    int prevTile = currentTile - (int)m_numberOfLanes;
                    //Decrement the lane timers, if they are already 0, attempt to spawn an obstacle
                    if (_laneObstacleTimer[lane] != 0)
                        _laneObstacleTimer[lane]--;
                    //If they have reached 0, check if we can spawn an obstacle on that tile
                    //1. Make sure the previous tile isn' a ramp
                    else if (!m_tiles[prevTile].IsRamp
                        //2. Make sure the tile isn't indoors and, if it is, that out height is different to the height of the indoor tile
                        && (!m_tiles[prevTile].HasIndoors || m_tiles[prevTile].IndoorHeight != m_tiles[currentTile].Height))
                    {
                        //Select a random obstacle to spawn
                        int obstIndex = Random.Range(0, _obstacles.Length);
                        obstacle = _obstacles[obstIndex].m_prefab;
                        //Get a reference to the tile to avoid more indexing
                        TileInfo t = m_tiles[currentTile];
                        //Check if the obstacle is valid
                        if (_obstacles[obstIndex].Spawner != null && obstacle != null)
                            //Spawn the obstacle
                            _obstacles[obstIndex].Spawner.Spawn(obstacle, in m_tiles, (uint)currentTile, 
                                //Since the tile hasn't yet been created, we have to re-calculate the position of the tile. This could be optimised by the position of the tile being calculated in initialisation of the tile
                                //And being stored on the tile. LOOK INTO THIS LATER
                                new Vector3(m_laneWidth * t.Lane + m_generateOffset.x, (t.Height + 0.5f) * m_layerHeight + m_generateOffset.y, t.ForwardPoint * m_tileLength + m_generateOffset.z), 
                                m_laneWidth, m_tileLength, m_layerHeight, m_numberOfLanes);

                        //Reset the timer
                        _laneObstacleTimer[lane] = (uint)Random.Range((int)_minSpaceBetweenObstacles, (int)_maxSpaceBetweenObstacles);
                    }
                }
            //Check for indoor obstacles
            if (!makeFlat && _obstacles.Length != 0)
                //Decrement the timers if they haven't already reached 0
                for (int lane = 0; lane < m_numberOfLanes; lane++)
                {
                    int currentTile = tileIndex * (int)m_numberOfLanes + lane;
                    if (!m_tiles[currentTile].HasIndoors)
                        continue;
                    int prevTile = currentTile - (int)m_numberOfLanes;

                    if (_indoorObstacleTimer[lane] != 0)
                        _indoorObstacleTimer[lane]--;
                    //Make sure the tile 2 back is indoors
                    else if (m_tiles[prevTile - (int)m_numberOfLanes].HasIndoors)
                    {
                        int obstIndex = Random.Range(0, _obstacles.Length);
                        obstacle = _obstacles[obstIndex].m_prefab;
                        //Get a reference to the tile to avoid more indexing
                        TileInfo t = m_tiles[currentTile];
                        //Check if the obstacle is valid
                        if (_obstacles[obstIndex].Spawner != null && obstacle != null)
                            //Spawn the obstacle
                            _obstacles[obstIndex].Spawner.Spawn(obstacle, in m_tiles, (uint)currentTile,
                                //Since the tile hasn't yet been created, we have to re-calculate the position of the tile. This could be optimised by the position of the tile being calculated in initialisation of the tile
                                //And being stored on the tile. LOOK INTO THIS LATER
                                new Vector3(m_laneWidth * t.Lane + m_generateOffset.x, (t.IndoorHeight + 0.5f) * m_layerHeight + m_generateOffset.y, t.ForwardPoint * m_tileLength + m_generateOffset.z),
                                m_laneWidth, m_tileLength, m_layerHeight, m_numberOfLanes);

                        //Reset the timer
                        _indoorObstacleTimer[lane] = (uint)Random.Range((int)_minDistBetweenIndoorObstacle, (int)_maxDistBetweenIndoorObstacle);
                    }
                }

            //Actually generate the lanes boxes and stuff
            for (int lane = 0; lane < m_numberOfLanes; lane++)
            {   //Get the neighbouring tile heights in the specified order
                int[] neighbourHeights = new int[] { -1, -1, -1 };
                neighbourHeights[0] = prevHeights[lane];
                if (lane != 0)
                    neighbourHeights[1] = heights[lane - 1];
                if (lane != m_numberOfLanes - 1)
                    neighbourHeights[2] = heights[lane + 1];
                //Generate the tiles
                m_tiles[tileIndex * (int)m_numberOfLanes + lane].GenerateTiles(m_tilePrefab, m_slopePrefab, m_indoorPrefab, m_laneWidth, m_layerHeight, m_tileLength, m_generateOffset, neighbourHeights);
            }

            //Generate the decorations
            float rand;
            //Make sure this isn't the first row so that the spawners don't have to continually check this
            if (tileIndex != 0)
                //0. Loop over the decorations
                for (int d = 0; d < m_decorations.Length; d++)
                {
                    rand = Random.Range(0, 1);
                    Decoration dec = m_decorations[d];
                    //Only spawn one if the chances are met
                    if (rand > dec.SpawnChance)
                        continue;

                    if (dec.DecorationSpawner == null)
                    {
                        Debug.LogWarning("Decoration Spawner for " + dec.m_prefab.name + " not created yet! Create the spawner plz");
                        continue;
                    }
                    //Spawn the decoration using its spawner
                    dec.DecorationSpawner.Spawn(dec.m_prefab, m_tiles, i, (uint)tileIndex * m_numberOfLanes,
                        m_numberOfLanes, new TileSize(m_tileLength, m_layerHeight, m_laneWidth), m_generateOffset);
                }
            //Loop over the new tiles and disable those that should not be rendered
            for (int lane = 0; lane < m_numberOfLanes; lane++)
            {   //Is the tile too far to the left
                if (lane < Player.player.CurrentLane - m_disableRowFrom
                    //Is the tile too far to the right
                    || lane > Player.player.CurrentLane + m_disableRowFrom)
                    //Toggle the tile off
                    m_tiles[tileIndex * (int)m_numberOfLanes + lane].Toggle(false);
            }
        }

        _currentLength += (int)layersToGenerate;
    }
    /// <summary>
    /// Toggle the tiles on or off depending on the lane the player is in
    /// </summary>
    void ToggleTiles()
    {
        bool toggleOn;
        int numberOfRows = _currentLength - _frontTilePos;
        //Calculate the range of lanes we need to disable or enable
        int startLane = (int)Player.player.CurrentLane - m_disableRowFrom - 2;
        int endLane = (int)Player.player.CurrentLane + m_disableRowFrom + 2;
        int left = startLane + 2;
        int right = endLane - 2;
        if (startLane < 0)
            startLane = 0;
        if (endLane >= m_numberOfLanes)
            endLane = (int)m_numberOfLanes - 1;
        //Loop over the lanes and find the valid lanes
        for (int lane = startLane; lane <= endLane; lane++)
        {   //Is the tile too far to the left
            if (lane < left
                //Is the tile too far to the right
                || lane > right)
                toggleOn = false;
            else
                toggleOn = true;
            //This could be optimised to only update the lanes +2 -2 from the edge of the tiles for even greater performance
            for (int i = 0; i < numberOfRows; i++)
                m_tiles[(int)m_numberOfLanes * i + lane].Toggle(toggleOn);
        }
    }
    /// <summary>
    /// Deletes all tiles and the objects they contain
    /// </summary>
    [ContextMenu("Delete Level")]
    void DeleteLevel()
    {   //Loop through the tiles
        for (int i = 0; i < m_tiles.Count; i++)
            //Loop through the objects on the tile
            for (int objects = 0; objects < m_tiles[i].m_objectsOnTile.Count; objects++)
                //Destroy the object
                DestroyImmediate(m_tiles[i].m_objectsOnTile[objects]);
        //Clear the tiles to avoid reading invalid memory
        m_tiles.Clear();
    }
    /// <summary>
    /// Deletes tiles from the front of the array
    /// </summary>
    /// <param name="tilesToDelete">The number of rows to delete</param>
    void DeleteFrontTiles(uint tilesToDelete)
    {   //Loop over the rows that should be deleted
        for (int i = 0; i < tilesToDelete * m_numberOfLanes; i++)
        {   //Loop through the first tile and delete it. We don't use i because we then remove this tile from the list, making the next tile the first tile
            for (int tileObj = 0; tileObj < m_tiles[0].m_objectsOnTile.Count; tileObj++)
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
        if (_player.position.z > (_frontTilePos + m_deleteTilesAtRow) * m_tileLength)
        {   //Delete the front and generate a new tile
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
                //For each of the objects on the tile, teleport them back
                for (int tileObj = 0; tileObj < m_tiles[i].m_objectsOnTile.Count; tileObj++)
                {
                    p = m_tiles[i].m_objectsOnTile[tileObj].transform.position;
                    p.z -= m_distanceUntilLoop;
                    m_tiles[i].m_objectsOnTile[tileObj].transform.position = p;
                }
            //Reset the length values so iterators remain valid and we don't generate 100 units in front of the level. That would be bad
            _frontTilePos = -((int)m_deleteTilesAtRow - 1);
            _currentLength = (m_tiles.Count / (int)m_numberOfLanes) - ((int)m_deleteTilesAtRow - 1);
            //Re-sync the transforms so the players raycasts do not miss
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
    /// <summary>
    /// Returns a copy of the tile at index.
    /// </summary>
    /// <param name="index">The index of the tile</param>
    /// <returns>Returns a new TileInfo with all values set to false if the index is invalid</returns>
    public TileInfo GetTileCopy(int index)
    {   //Make sure the index is valid
        if (index < 0 || index >= m_tiles.Count)
            return new TileInfo();

        return m_tiles[index];
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
    public List<GameObject> m_objectsOnTile;
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
        {   //If the tile has already been generated, this value cannot be manipulated
            if (_isGenerated)
            {
                Debug.LogError("The tile has already been generated and variable height cannot be manipulated");
                return;
            }

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
        {   //If the tile has already been generated, this value cannot be manipulated
            if (_isGenerated)
            {
                Debug.LogError("The tile has already been generated and variable IsRamp cannot be manipulated");
                return;
            }

            _isRamp = value;
        }
    }
    /// <summary>
    /// Does this tile have an indoor section
    /// </summary>
    private bool _hasIndoors;
    /// <summary>
    /// Does this tile have an indoor section
    /// </summary>
    public bool HasIndoors => _hasIndoors;
    /// <summary>
    /// What is the height of the indoor section
    /// </summary>
    private uint _indoorHeight;
    /// <summary>
    /// What is the height of the indoor section
    /// </summary>
    public uint IndoorHeight => _indoorHeight;
    /// <summary>
    /// How long the indoor section has lasted
    /// </summary>
    private int _indoorLength;
    /// <summary>
    /// How long the indoor section has lasted
    /// </summary>
    public int IndoorLength => _indoorLength;
    /// <summary>
    /// Has this tile been generated yet? Determines which variables can be written to
    /// </summary>
    private bool _isGenerated;
    /// <summary>
    /// Initalises the tile
    /// </summary>
    /// <param name="obstacle">The obstacle on this tile</param>
    /// <param name="lane">The lane this tile is in</param>
    /// <param name="height">The height of this tile</param>
    /// <param name="forwardPoint">The z step of this tile</param>
    /// <param name="isRamp">If this tile is a ramp</param>
    public void Initialise(uint lane, uint height, uint forwardPoint, bool isRamp)
    {   //Make sure the tile has not finished its generation before continuing
        if (_isGenerated)
        {
            Debug.LogError("Cannot Re-Initialise Tile");
            return;
        }

        m_objectsOnTile = new List<GameObject>();
        _lane = lane;
        _height = height;
        _forwardPoint = forwardPoint;
        _isRamp = isRamp;
        _isGenerated = false;
        _hasIndoors = false;
        _indoorHeight = 0;
    }
    /// <summary>
    /// Sets the tiles indoor height. Also sets the tile to be indoors
    /// </summary>
    /// <param name="height">The height of the indoor section of the tile</param>
    public void SetIndoorHeight(uint height, int length)
    {
        _hasIndoors = true;
        _indoorHeight = height;
        _indoorLength = length;

        if (_height <= _indoorHeight)
            _height = _indoorHeight + 1;
    }
    /// <summary>
    /// Generates the tile based on the information given in the initialiser
    /// </summary>
    /// <param name="tilePrefab">The prefab for standard ground</param>
    /// <param name="tileSlope">The prefab for slopes</param>
    /// <param name="tileIndoor">The prefab for indoors</param>
    /// <param name="laneWidth">The width of a tile</param>
    /// <param name="tileHeight">The height of a tile</param>
    /// <param name="tileLength">The length of a tile</param>
    /// <param name="posOffset">The position offset of the tile from the origin</param>
    /// <param name="neighbourHeights">The heights of neighbouring tiles in order, previous, left, right</param>
    public void GenerateTiles(GameObject tilePrefab, GameObject tileSlope, GameObject tileIndoor, float laneWidth, float tileHeight, float tileLength, Vector3 posOffset, int[] neighbourHeights)
    {   //Make sure the tile has not finished its generation before continuing
        if (_isGenerated)
        {
            Debug.LogError("Cannot Re-Generate Tile");
            return;
        }
        //Storage space for the objects to save on memory, we will re-use this.
        GameObject obj;
        //Generate the regular cube for the ground
        obj = GameObject.Instantiate(tilePrefab, new Vector3(laneWidth * _lane + posOffset.x, ((float)(_isRamp ? _height - 1 : _height) / 2) * tileHeight + posOffset.y, _forwardPoint * tileLength + posOffset.z), Quaternion.identity);
        //Scale it up so we only have to use 1 object for the ground
        obj.transform.localScale = new Vector3(laneWidth, tileHeight * (_isRamp ? _height : _height + 1), tileLength);
        //We need to update the texture scaling
        Renderer r = obj.GetComponent<Renderer>();
        r.material.SetTextureScale("_WallTex", new Vector2(1, _isRamp ? _height : _height + 1));
        r.material.SetTextureScale("_MainTex", new Vector2(1, tileLength));
        r.material.SetFloat("_Back", (neighbourHeights[0] + 0.5f) * tileHeight + posOffset.y);
        r.material.SetFloat("_Left", (neighbourHeights[1] + 0.5f) * tileHeight + posOffset.y);
        r.material.SetFloat("_Right", (neighbourHeights[2] + 0.5f) * tileHeight + posOffset.y);
        //Store it
        m_objectsOnTile.Add(obj);
        //If we have a ramp, create it
        if (_isRamp)
        {
            //Create the ramp
            obj = GameObject.Instantiate(tileSlope, new Vector3(laneWidth * _lane + posOffset.x, _height * tileHeight + posOffset.y, _forwardPoint * tileLength + posOffset.z), Quaternion.identity);
            //Scale the ramp to fit the tile
            obj.transform.localScale = new Vector3(laneWidth, tileHeight, tileLength);
            //Store it
            m_objectsOnTile.Add(obj);
        }

        if (_hasIndoors)
        {
            obj = GameObject.Instantiate(tileIndoor, new Vector3(laneWidth * _lane + posOffset.x, _indoorHeight * tileHeight + posOffset.y, _forwardPoint * tileLength + posOffset.z), Quaternion.identity);
            obj.transform.localScale = new Vector3(laneWidth, tileHeight, tileLength);

            m_objectsOnTile.Add(obj);
        }
        //The tile has been generated and cannot be edited
        _isGenerated = true;
    }
    /// <summary>
    /// Instantiates and stores an obstacle gameObject on this tile
    /// </summary>
    /// <param name="obstacle">The obstacle to create and add</param>
    public void AddObstacle(GameObject obstacle, float laneWidth, float tileHeight, float tileLength, Vector3 posOffset)
    {   //Spawn the obstacle
        GameObject obj = GameObject.Instantiate(obstacle, new Vector3(laneWidth * _lane + posOffset.x, (_height + 0.5f) * tileHeight + posOffset.y, _forwardPoint * tileLength + posOffset.z), Quaternion.identity);
        //Store the object
        m_objectsOnTile.Add(obj);
    }
    /// <summary>
    /// Stores an already existing obstacle on this tile
    /// </summary>
    /// <param name="obstacle">The obstacle to add</param>
    public void AddObstacle(ref GameObject obstacle)
    {   //Store the object
        m_objectsOnTile.Add(obstacle);
    }
    /// <summary>
    /// Toggles the objects on the tile
    /// </summary>
    public void Toggle(bool on)
    {
        foreach (GameObject g in m_objectsOnTile)
            g.SetActive(on);
    }
}
/// <summary>
/// Stores information about decorations and where they are positioned
/// </summary>
[System.Serializable]
public struct Decoration
{
    /// <summary>
    /// The prefab for this decoration
    /// </summary>
    [Tooltip("The prefab for the decoration")]
    public GameObject m_prefab;
    /// <summary>
    /// The spawner for the decoration
    /// </summary>
    [Tooltip("The spawner used for this decoration")]
    [SerializeField]
    private DecorationSpawner _decorationSpawner;
    /// <summary>
    /// The spawner for the decoration
    /// </summary>
    public DecorationSpawner DecorationSpawner => _decorationSpawner;
    /// <summary>
    /// The chance of the decoration spawning
    /// </summary>
    [Tooltip("The chance of this decoration spawning")]
    [SerializeField]
    [Range(0, 1)]
    private float m_spawnChance;
    /// <summary>
    /// The chance of this decoration spawning
    /// </summary>
    public float SpawnChance => m_spawnChance;
    /// <summary>
    /// Construct for Decoration struct. Primarily to "value is never assigned to" warnings from the console
    /// </summary>
    /// <param name="prefab">The prefab of the decoration</param>
    /// <param name="spawner">The spawner for this decoration</param>
    /// <param name="spawnChance">The chance of this decoration spawning</param>
    public Decoration(GameObject prefab, DecorationSpawner spawner, float spawnChance)
    {
        m_prefab = prefab;
        _decorationSpawner = spawner;
        m_spawnChance = spawnChance;
    }
}
/// <summary>
/// Stores information about Obstacles
/// </summary>
[System.Serializable]
public struct Obstacle
{
    /// <summary>
    /// The prefab for this obstacle
    /// </summary>
    [Tooltip("The prefab for this obstacle")]
    public GameObject m_prefab;
    /// <summary>
    /// Stores the spawner for this obstacle
    /// </summary>
    [Tooltip("The spawner for this obstacle")]
    [SerializeField]
    private ObstacleSpawner _spawner;
    /// <summary>
    /// Returns the spawner for this obstacle
    /// </summary>
    public ObstacleSpawner Spawner => _spawner;
    /// <summary>
    /// Constructor for Obstacle
    /// </summary>
    /// <param name="prefab">The prefab for the obstacle</param>
    /// <param name="spawner">The spawning behaviour to use for the obstacle</param>
    public Obstacle(GameObject prefab, ObstacleSpawner spawner)
    {
        m_prefab = prefab;
        _spawner = spawner;
    }
}
