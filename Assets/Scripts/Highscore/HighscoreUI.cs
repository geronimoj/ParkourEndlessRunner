using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreUI : MonoBehaviour
{
    /// <summary>
    /// The highscore that should be displayed
    /// </summary>
    [Tooltip("The highscore that will be displayed")]
    public Highscore m_hScore = null;
    /// <summary>
    /// The RawImage for the camera to display the head of the model the player was using
    /// </summary>
    [Tooltip("The RawImage to be used for displaying the head")]
    public RawImage m_modelHead = null;

    public TMPro.TextMeshProUGUI m_scoreText = null;

    public TMPro.TextMeshProUGUI m_durationText = null;

    public Button m_deleteButton = null;

    [SerializeField]
    private GameObject m_modelCameraPrefab = null;

    private GameObject _modelCamera = null;

    public GameObject ModelCamera => _modelCamera;

    private void Start()
    {
        SetValues();
    }

    public void SetValues()
    {   //Only render if the highscore is assigned
        if (m_hScore)
        {   //Draw the score text if its assigned
            if (m_scoreText)
                m_scoreText.text = m_hScore.Score.ToString();
            //Write the duration text if its assigned
            if (m_durationText)
                m_durationText.text = m_hScore.Duration.ToString("n2");
            //If we have a delete button
            if (m_deleteButton)
            {   //Remove all listeners to avoid listeners being carried from prevsious uses of this UI component
                m_deleteButton.onClick.RemoveAllListeners();
                //Add our highscore to be deleted
                m_deleteButton.onClick.AddListener(() => { Highscore.RemoveScore(m_hScore); gameObject.SetActive(false); });
            }
            //Render the models head if its assigned
            if (m_modelHead)
            {
                if (m_modelCameraPrefab)
                {   //If model camera is not assigned to, assign it now
                    if (!_modelCamera)
                        _modelCamera = Instantiate(m_modelCameraPrefab, Vector3.zero, Quaternion.LookRotation(-Vector3.forward, Vector3.up));
                    //Set the information about the camera
                    Camera c = _modelCamera.GetComponent<Camera>();
                    //If there is no camera, complain
                    if (!c)
                    {
                        Debug.LogError("m_modelCameraPrefab does not have a camera component!");
                        return;
                    }
                    //Make sure the camera is setup correctly
                    c.clearFlags = CameraClearFlags.SolidColor;
                    c.backgroundColor = Color.clear;
                    c.cullingMask = LayerMask.GetMask("Model");
                    RenderTexture tex = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32)
                    {
                        dimension = UnityEngine.Rendering.TextureDimension.Tex2D
                    };
                    c.targetTexture = tex;
                    m_modelHead.texture = tex;

                    MenuModelCamera modCam = _modelCamera.GetComponent<MenuModelCamera>();
                    //Cycle to the correct model
                    if (modCam)
                        modCam.ModelIndex = (int)m_hScore.Model;
                    //Load the player models if they haven't already been loaded
                    if (!MainMenuManager.s_modelsLoaded)
                        //This is sort of expected to be a menu thing so we just one line it and hope they don't try to use this elsewhere
                        GameObject.FindGameObjectWithTag("GameManager").GetComponent<MainMenuManager>().LoadPlayerModels();
                }
                //If they want a model head to appear but don't have a camera, complain
                else
                    Debug.LogWarning("Camera Prefab for highscore not assigned. Cannot display model heads");
            }
        }
    }
}
