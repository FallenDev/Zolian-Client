﻿using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia
{
    [ExecuteAlways]
    public class PW_VFX_Clouds : MonoBehaviour
    {
        #region Variables

        public static PW_VFX_Clouds Instance
        {
            get { return m_instance; }
        }
        [SerializeField]
        private static PW_VFX_Clouds m_instance;

        [Header("Lighting")]
        public Light SunLight;

        [Header("Settings")] 
        public bool m_trackPlayer = true;
        public float m_yOffset = 50f;
        public bool m_followCamOnYAxis = true;
        public int UpdateTicks = 30;

        public Color AmbColor = new Color(0.12f, 0.12f, 0.22f);
        public Vector3 SunDirection;
        public Vector4 PW_Wind_Cloud_Dir;
        public Vector4 PW_SkyDome_Opacity;
        public Cubemap PW_Clouds_HDRI = null;
        public float PW_Cloud_Brightness = 1f;
        public float PW_Clouds_HDRI_Blur_Level = 1f;
        public float PW_Clouds_Clouds_Blur_Level = 1f;
        public float PW_Clouds_Clouds_Distortion_Level = 0.5f;
        public float PW_Clouds_Height_Mask = 2.5f;
        public AnimationCurve PW_SkyDome_Brightness;
        public float PW_Clouds_Fade = 128f;
        public Camera GameCam;
        public Transform CloudsPrefab;
        public float m_seaLevel = 50f;
        public float m_scale = 1f;
        public float m_cloudRotationSpeedLow = 0.3f;
        public float m_cloudRotationSpeedMiddle = 0.2f;
        public float m_cloudRotationSpeedFar = 0.1f;

        public Material CloudParticleMateiral;
        public List<Material> CloudLayerMaterials = new List<Material>();
        public Material SpaceMaterial;

        [SerializeField]
        private GameObject SkyDomeLow;
        [SerializeField]
        private GameObject SkyDomeMiddle;
        [SerializeField]
        private GameObject SkyDomeFar;
        [SerializeField]
        private GameObject SkyDome;
        private int currentTick = 0;

        [SerializeField]
        private bool m_cloudLowExists;
        [SerializeField]
        private bool m_cloudMidExists;
        [SerializeField]
        private bool m_cloudFarExists;
        [SerializeField]
        private bool m_cloudMaterialExists;
        [SerializeField]
        private bool m_gradientOrCurvesExists;
        [SerializeField]
        private bool m_sunExists;
        [SerializeField]
        private bool m_cameraExists;
        [SerializeField]
        private Camera m_editorSceneViewCamera;
        private bool m_editorCameraExists;
        [SerializeField]
        private ProceduralWorldsGlobalWeather WeatherSystem;

        #endregion

        #region Unity Functions

        private void Start()
        {
            Initialize();

            if (GameCam != null)
            {
                m_cameraExists = true;
            }

            if (CloudsPrefab == null)
            {
                CloudsPrefab = gameObject.transform;
            }

            if (CloudParticleMateiral == null)
            {
                CloudParticleMateiral = GaiaUtils.GetParticleMaterial(gameObject);
            }

            if (CloudParticleMateiral != null)
            {
                m_cloudMaterialExists = true;
            }

            m_gradientOrCurvesExists = CheckIfGradientsAndAnimationCruvesExist();

            m_instance = this;
            currentTick = 0;
            CloudsUpdate();
        }
        private void OnEnable()
        {
            m_instance = this;
            if (CloudsPrefab == null)
            {
                CloudsPrefab = gameObject.transform;
            }
            if (CloudParticleMateiral == null)
            {
                CloudParticleMateiral = GaiaUtils.GetParticleMaterial(gameObject);
            }
            if (CloudLayerMaterials.Count <= 0)
            {
                CloudLayerMaterials = GaiaUtils.GetCloudLayerMaterials("PW_VFX_SkyDome", "PW_VFX_SkyDome_Space");
            }

            if (SpaceMaterial == null)
            {
                GameObject cloudObject = GameObject.Find("PW_VFX_SkyDome_Space");
                if (cloudObject != null)
                {
                    MeshRenderer renderer = cloudObject.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        SpaceMaterial = renderer.sharedMaterial;
                    }
                }
            }

            Initialize();
        }
        private void LateUpdate()
        {
            if (m_cloudLowExists)
            {
                SkyDomeLow.transform.Rotate(Vector3.up * m_cloudRotationSpeedLow * Time.deltaTime);
            }
            if (m_cloudMidExists)
            {
                SkyDomeMiddle.transform.Rotate(Vector3.up * m_cloudRotationSpeedMiddle * Time.deltaTime);
            }
            if(m_cloudFarExists)
            {
                SkyDomeFar.transform.Rotate(Vector3.up * m_cloudRotationSpeedFar * Time.deltaTime);
            }

            if (currentTick > UpdateTicks)
            {
                CloudsUpdate();
            }
            currentTick++;
        }
        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdateCloudPosition;
#endif

        }
        private void OnDestroy()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdateCloudPosition;
#endif
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Applies updates
        /// </summary>
        public void CloudsUpdate()
        {
            if (WeatherSystem == null)
            {
                WeatherSystem = ProceduralWorldsGlobalWeather.Instance;
                return;
            }

            m_sunExists = SetSunLight(WeatherSystem.CheckIsNight());
            if (m_sunExists)
            {
                SunDirection = SunLight.transform.forward;
                Shader.SetGlobalVector(GaiaShaderID.m_SunDirection, SunDirection);
                Shader.SetGlobalColor(GaiaShaderID.m_SunColor, SunLight.color);
                Shader.SetGlobalVector(GaiaShaderID.m_cloudSunDirection, SunDirection);
                if (SpaceMaterial)
                {
                    if (WeatherSystem.CheckIsNight())
                    {
                        Shader.SetGlobalVector(GaiaShaderID.m_skySunDirection, -SunDirection);
                    }
                    else
                    {
                        Shader.SetGlobalVector(GaiaShaderID.m_skySunDirection, SunDirection);
                    }
                }
            }

            AmbColor = RenderSettings.fogColor;
            if (m_cloudMaterialExists)
            {
                Shader.SetGlobalFloat(GaiaShaderID.m_cloudFade, PW_Clouds_Fade);
                Shader.SetGlobalColor(GaiaShaderID.m_cloudAmbientColor, AmbColor);
            }

            bool updateDomeBrightness = !WeatherSystem.IsRaining;
            if (WeatherSystem.IsSnowing)
            {
                updateDomeBrightness = false;
            }
            if (updateDomeBrightness)
            {
                if (CloudLayerMaterials.Count > 0)
                {
                    if (m_gradientOrCurvesExists)
                    {
                        if (WeatherSystem.m_renderPipeline == GaiaConstants.EnvironmentRenderer.HighDefinition)
                        {
#if HDPipeline
                            Shader.SetGlobalFloat(GaiaShaderID.m_cloudDomeBrightness,PW_SkyDome_Brightness.Evaluate(PWSkyStandalone.GetTimeOfDayMainValue()) * 8f);
#endif
                        }
                        else
                        {
                            Shader.SetGlobalFloat(GaiaShaderID.m_cloudDomeBrightness, PW_SkyDome_Brightness.Evaluate(PWSkyStandalone.GetTimeOfDayMainValue()) * 1.35f);
                        }
                    }
                }
            }

            if (m_trackPlayer)
            {
                if (Application.isPlaying)
                {
                    if (m_cameraExists)
                    {
                        UpdateCloudPosition(GameCam.transform.position, Quaternion.identity);
                    }
                }
            }

            gameObject.transform.localScale = new Vector3(m_scale, m_scale, m_scale);
            currentTick = 0;
        }
        /// <summary>
        /// Setup
        /// </summary>
        public void Initialize()
        {
            if (SkyDomeLow == null)
            {
                SkyDomeLow = GameObject.Find("PW_VFX_SkyDome_Clouds_Low");
                if (SkyDomeLow != null)
                {
                    m_cloudLowExists = true;
                }
                else
                {
                    m_cloudLowExists = false;
                }
            }
            else
            {
                m_cloudLowExists = true;
            }

            if (SkyDomeMiddle == null)
            {
                SkyDomeMiddle = GameObject.Find("PW_VFX_SkyDome_Clouds_Middle");
                if (SkyDomeMiddle != null)
                {
                    m_cloudMidExists = true;
                }
                else
                {
                    m_cloudMidExists = false;
                }
            }
            else
            {
                m_cloudMidExists = true;
            }

            if (SkyDomeFar == null)
            {
                SkyDomeFar = GameObject.Find("PW_VFX_SkyDome_FarClouds");
                if (SkyDomeFar != null)
                {
                    m_cloudFarExists = true;
                }
                else
                {
                    m_cloudFarExists = false;
                }
            }
            else
            {
                m_cloudFarExists = true;
            }

            if (SkyDome == null)
            {
                SkyDome = GameObject.Find("PW_VFX_SkyDome");
            }

            if (GameCam == null)
            {
                GameCam = Camera.main;
            }

            if (SunLight == null)
            {
                SunLight = GaiaUtils.GetMainDirectionalLight(false);
            }

#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdateCloudPosition;
            if (!Application.isPlaying)
            {
                EditorApplication.update += EditorUpdateCloudPosition;
            }
#endif

            CloudsUpdate();
        }
        /// <summary>
        /// Sets the render status of the cloud domes in the scene
        /// </summary>
        /// <param name="enabled"></param>
        public void SetCloudRenderer(bool enabled)
        {
            if (SkyDomeLow != null && SkyDomeMiddle != null && SkyDomeFar != null)
            {
                SkyDomeLow.SetActive(enabled);
                SkyDomeMiddle.SetActive(enabled);
                SkyDomeFar.SetActive(enabled);
            }
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Updates the posotion and rotation of the root of the clouds gameobject
        /// Used to follow the player around the world
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        private void UpdateCloudPosition(Vector3 position, Quaternion rotation)
        {
            //Offset
            float yPos = m_yOffset;
            if (m_followCamOnYAxis)
            {
                //Update y offset
                yPos = Mathf.Max(m_seaLevel + m_yOffset, position.y + m_yOffset);
            }
            //Apply position to clouds
            CloudsPrefab.SetPositionAndRotation(new Vector3(position.x, yPos, position.z), rotation);
        }
        /// <summary>
        /// Checks to make sure all gradients and animation curves are present
        /// </summary>
        /// <returns></returns>
        private bool CheckIfGradientsAndAnimationCruvesExist()
        {
            if (PW_SkyDome_Brightness == null)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Sets the active sun light
        /// </summary>
        /// <param name="isNight"></param>
        /// <returns></returns>
        private bool SetSunLight(bool isNight)
        {
            bool hasSun = false;
            if (WeatherSystem == null)
            {
                WeatherSystem = ProceduralWorldsGlobalWeather.Instance;
                if (WeatherSystem == null)
                {
                    Debug.LogError("Global Weather system was not found!");
                }

                return false;
            }

            if (isNight)
            {
                SunLight = WeatherSystem.m_moonLight;
            }
            else
            {
                SunLight = WeatherSystem.m_sunLight;
            }

            if (SunLight != null)
            {
                hasSun = true;
            }

            return hasSun;
        }

        #endregion

        #region Editor Utils

#if UNITY_EDITOR
        /// <summary>
        /// Gets the editor scene view camera
        /// </summary>
        /// <returns></returns>
        private Camera GetEditorSceneViewCamera()
        {
            Camera sceneCamera = null;
            if (SceneView.lastActiveSceneView != null)
            {
                sceneCamera = SceneView.lastActiveSceneView.camera;
            }

            return sceneCamera;
        }
        /// <summary>
        /// Editor update function that is called when the application is not playing
        /// </summary>
        private void EditorUpdateCloudPosition()
        {
            if (!m_trackPlayer)
            {
                return;
            }
            //Get camera
            if (m_editorSceneViewCamera == null)
            {
                m_editorSceneViewCamera = GetEditorSceneViewCamera();
                if (m_editorSceneViewCamera)
                {
                    m_editorCameraExists = true;
                }
                else
                {
                    m_editorCameraExists = false;
                }
                return;
            }
            //Position
            Vector3 position = m_editorSceneViewCamera.transform.position;
            //Offset from sceneview camera
            position += -m_editorSceneViewCamera.transform.forward * 1.5f;
            if (m_editorCameraExists)
            {
                //Apply position
                UpdateCloudPosition(position, Quaternion.identity);
            }
        }
#endif

        #endregion
    }
}