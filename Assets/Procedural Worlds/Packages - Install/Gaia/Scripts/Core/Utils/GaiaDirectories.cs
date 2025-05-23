﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gaia
{

    public static class GaiaDirectories
    {
        public const string SESSION_OPERATIONS_DIRECTORY = "/Session Operations";
        public const string SESSION_SCENE_PROFILE_DIRECTORY = "/Scene Profiles";
        public const string TERRAIN_SCENES_DIRECTORY = "/Terrain Scenes";
        public const string MASK_EXPORTS_DIRECTORY = "/Mask Exports";
        public const string HDRP_LIGHTING_PROFILE_DIRECTORY = "/Lighting Profiles/HDRP Lighting Profiles";
        public const string URP_LIGHTING_PROFILE_DIRECTORY = "/Lighting Profiles/URP Lighting Profiles";
        public const string SRP_LIGHTING_PROFILE_DIRECTORY = "/Lighting Profiles/SRP Lighting Profiles";
        public const string IMPOSTOR_SCENES_DIRECTORY = "/Impostor Scenes";
        public const string COLLIDER_SCENES_DIRECTORY = "/Collider Scenes";
        public const string BACKUP_SCENES_DIRECTORY = "/Backup Scenes";
        public const string BACKUP_HEIGHTMAPS_DIRECTORY = "/Terrain Heightmap Backup";
        public const string BACKUP_SPECIALSTAMPER_DIRECTORY = "/Stamper Backup";
        public const string TERRAIN_DATA_DIRECTORY = "/Terrain Data";
        public const string SETTINGS_DIRECTORY = "/Settings";
        public const string STAMP_DIRECTORY = "/Stamps";
        public const string USER_STAMP_DIRECTORY = "/My Saved Stamps";
        public const string TERRAIN_LAYERS_DIRECTORY = "/Terrain Layers";
        public const string FLORA_DATA_DIRECTORY = "/Flora Data";
        public const string EXPORT_DIRECTORY = "/Exports";
        
        public const string FLORA_SHADER_DIRECTORY = "/Content Resources/Shaders";
        public const string GAIA_SHADER_DIRECTORY = "/Shaders/PW_General";
        public const string STAMPER_EXPORT_DIRECTORY = "/Stamper Exports";
        public const string STAMPER_TERRAIN_EXPORT_DIRECTORY = "/Stamper Terrain Exports";
        public const string WORLD_DESIGNER_EXPORT_DIRECTORY = "/World Designer Exports";
        public const string SCANNER_EXPORT_DIRECTORY = "/Scanner Exports";
        public const string MASK_MAP_EXPORT_DIRECTORY = "/Mask Maps";
        public const string COLLISION_DATA_DIRECTORY = "/TerrainCollisionData";
        public const string GAIA_PRO = "/Gaia Pro";
        public const string GAIA_MESH_SIMPLIFIER_DIRECTORY = "/Scripts/Core/Mesh Simplifier";
        public const string TERRAIN_MESH_EXPORT_DIRECTORY = "/Mesh Terrains";
        public const string GWS_DIRECTORY = "/Gaia Wizard Settings";
        public const string GRC_DIRECTORY = "/Gaia Runtime Settings";
        public const string GAIA_WATER_MATERIAL_DIRECTORY = "/Gaia Lighting and Water/Gaia Water/Ocean Water/Resources/Material";
        public const string TEMP_EXPORT_PATH = "Assets/Temp Export";

        /// <summary>
        /// Returns the Gaia Pro folder exists in the project
        /// </summary>
        /// <returns></returns>
        public static bool GetGaiaProDirectory()
        {
            bool isPro = false;
            string dataPath = GetFullFileSystemPath(GetGaiaDirectory() + GAIA_PRO);

            if (Directory.Exists(dataPath))
            {
                isPro = true;
            }
            else
            {
                isPro = false;
            }

            return isPro;
        }

        /// <summary>
        /// Returns if the Gaia Mesh Simplifier folder exists in the project
        /// </summary>
        /// <returns></returns>
        public static bool GetGaiaMeshSimplifierDirectory()
        {
            bool isPro = false;
            string dataPath = GetFullFileSystemPath(GetGaiaDirectory() + GAIA_MESH_SIMPLIFIER_DIRECTORY);

            if (Directory.Exists(dataPath))
            {
                isPro = true;
            }
            else
            {
                isPro = false;
            }

            return isPro;
        }

        /// <summary>
        /// Return the Gaia directory in the project
        /// </summary>
        /// <returns>String containing the Gaia directory</returns>
        public static string GetGaiaDirectory()
        {
            //Default Directory, will be returned if not in Editor
            string gaiaDirectory = "Assets/Procedural Worlds/Gaia/";
#if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets("Gaia_ReadMe", null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (Path.GetFileName(path) == "Gaia_ReadMe.txt")
                {
                    gaiaDirectory = path.Replace("/Gaia_ReadMe.txt","");
                }
            }
#endif
            return gaiaDirectory;
        }

        /// <summary>
        /// Return the Flora directory in the project (if flora exists, otherwise returns null)
        /// </summary>
        /// <returns>String containing the Gaia directory</returns>
        public static string GetFloraDirectory()
        {
            //Default Directory, will be returned if not in Editor
            string floraDirectory = "";
#if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets("Flora_ReadMe", null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (Path.GetFileName(path) == "Flora_ReadMe.txt")
                {
                    floraDirectory = path.Replace("/Flora_ReadMe.txt", "");
                }
            }
#endif
            return floraDirectory;
        }

        /// <summary>
        /// Returns the Gaia Session Directory. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Session directory</returns>
        public static string GetSessionDirectory()
        {
            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
            Directory.CreateDirectory(gaiaSettings.m_sessionsDirectory);
            return gaiaSettings.m_sessionsDirectory;
        }

        /// <summary>
        /// Returns the Gaia Data Directory. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Data directory</returns>
        public static string GetSettingsDirectory()
        {
            return GetGaiaSubDirectory(SETTINGS_DIRECTORY);
        }

        /// <summary>
        /// Returns the path to store terrain scenes in, according to the session filename
        /// </summary>
        /// <param name="gaiaSession">The session to get / create this path for</param>
        /// <returns></returns>
        public static string GetTerrainScenePathForSession(GaiaSession gaiaSession=null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            return CreatePathIfDoesNotExist(GetScenePath(gaiaSession) + TERRAIN_SCENES_DIRECTORY);
        }
        /// <summary>
        /// Returns the path to store the SRP lighting profiles in, according to the session filename
        /// </summary>
        /// <param name="gaiaSession">The session to get / create this path for</param>
        /// <returns></returns>
        public static string GetSRPLightingProfilePathForSession(GaiaSession gaiaSession = null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            return CreatePathIfDoesNotExist(GetScenePath(gaiaSession) + SRP_LIGHTING_PROFILE_DIRECTORY);
            
        }
        /// <summary>
        /// Returns the path to store the HDRP lighting profiles in, according to the session filename
        /// </summary>
        /// <param name="gaiaSession">The session to get / create this path for</param>
        /// <returns></returns>
        public static string GetHDRPLightingProfilePathForSession(GaiaSession gaiaSession = null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            return CreatePathIfDoesNotExist(GetScenePath(gaiaSession) + HDRP_LIGHTING_PROFILE_DIRECTORY);

        }
        /// <summary>
        /// Returns the path to store the URP lighting profiles in, according to the session filename
        /// </summary>
        /// <param name="gaiaSession">The session to get / create this path for</param>
        /// <returns></returns>
        public static string GetURPLightingProfilePathForSession(GaiaSession gaiaSession = null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            return CreatePathIfDoesNotExist(GetScenePath(gaiaSession) + URP_LIGHTING_PROFILE_DIRECTORY);

        }
        /// <summary>
        /// Returns the path to store terrain scenes in, according to the Terrain Scene Storage scriptable object
        /// </summary>
        /// <param name="gaiaSession">The session to get / create this path for</param>
        /// <returns></returns>
        public static string GetTerrainScenePathForStorageFile(TerrainSceneStorage terrainSceneStorage = null)
        {
#if UNITY_EDITOR
            if (terrainSceneStorage == null)
            {
                terrainSceneStorage = TerrainLoaderManager.Instance.TerrainSceneStorage;
            }
            return CreatePathIfDoesNotExist(AssetDatabase.GetAssetPath(terrainSceneStorage).Replace("/" + terrainSceneStorage.name + ".asset","") + TERRAIN_SCENES_DIRECTORY);
#else
            return "";
#endif
        }

        /// <summary>
        /// Returns the path to store impostor scenes in, according to the session filename
        /// </summary>
        /// <param name="gaiaSession">The session to get / create this path for</param>
        /// <returns></returns>
        public static string GetImpostorScenePath(GaiaSession gaiaSession = null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            return CreatePathIfDoesNotExist(GetScenePath(gaiaSession) + IMPOSTOR_SCENES_DIRECTORY);
        }

        /// <summary>
        /// Returns the path to store collider scenes in, according to the session filename
        /// </summary>
        /// <param name="gaiaSession">The session to get / create this path for</param>
        /// <returns></returns>
        public static string GetColliderScenePath(GaiaSession gaiaSession = null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            return CreatePathIfDoesNotExist(GetScenePath(gaiaSession) + COLLIDER_SCENES_DIRECTORY);
        }


        /// <summary>
        /// Returns the path to store impostor scenes in, according to the session filename
        /// </summary>
        /// <param name="gaiaSession">The session to get / create this path for</param>
        /// <returns></returns>
        public static string GetBackupScenePath(GaiaSession gaiaSession = null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            return CreatePathIfDoesNotExist(GetScenePath(gaiaSession) + BACKUP_SCENES_DIRECTORY);
        }

        /// <summary>
        /// Returns the path to store heightmap backups in, according to the session filename
        /// </summary>
        /// <param name="gaiaSession">The session to get / create this path for</param>
        /// <returns></returns>
        public static string GetBackupHeightmapsPath(bool create = false, GaiaSession gaiaSession = null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            string path = GetScenePath(gaiaSession) + BACKUP_HEIGHTMAPS_DIRECTORY;

            if (create)
            {
                return CreatePathIfDoesNotExist(path);
            }
            else
            {
                return path;
            }
        }

        public static string GetStamperBackupsPath(bool create = false, GaiaSession gaiaSession = null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            if (create)
            {
                return CreatePathIfDoesNotExist(GetBackupHeightmapsPath(true, gaiaSession) + "/" + BACKUP_SPECIALSTAMPER_DIRECTORY);
            }
            else
            {
                return GetBackupHeightmapsPath(false, gaiaSession) + "/" + BACKUP_SPECIALSTAMPER_DIRECTORY;
            }
        }

        /// <summary>
        /// Returns a path to store scriptable object data for the session operations.
        /// </summary>
        /// <param name="gaiaSession">The session to create the directory for</param>
        /// <returns></returns>
        public static string GetSessionOperationPath(GaiaSession gaiaSession, bool create = true)
        {
#if UNITY_EDITOR
            return CreatePathIfDoesNotExist(GetSessionSubFolderPath(gaiaSession, create) + SESSION_OPERATIONS_DIRECTORY);
#else
            return "";
#endif
        }

        /// <summary>
        /// Returns a folder that is named as the session asset, to store session related data into.
        /// </summary>
        /// <param name="gaiaSession">The session to create the directory for</param>
        /// <returns></returns>
        public static string GetSessionSubFolderPath(GaiaSession gaiaSession, bool create = true)
        {
#if UNITY_EDITOR
            string masterScenePath = AssetDatabase.GetAssetPath(gaiaSession).Replace(".asset", "");
            if (String.IsNullOrEmpty(masterScenePath))
            {
                GaiaSessionManager gsm = GaiaSessionManager.GetSessionManager(false);
                gsm.SaveSession();
                masterScenePath = AssetDatabase.GetAssetPath(gsm.m_session).Replace(".asset", "");
            }
            if (create)
                return CreatePathIfDoesNotExist(masterScenePath);
            else
                return masterScenePath;
#else
            return "";
#endif
        }
        public static string GetSceneProfilesFolderPath(GaiaSession gaiaSession, bool create = true)
        {
#if UNITY_EDITOR
            return CreatePathIfDoesNotExist(GetSessionSubFolderPath(gaiaSession, create) + SESSION_SCENE_PROFILE_DIRECTORY);
#else
            return "";
#endif
        }

        /// <summary>
        /// Returns the path to store scene files in, according to the Gaia Session path
        /// </summary>
        /// <param name="gaiaSession">The session to get / create the directory for</param>
        /// <returns></returns>
        public static string GetScenePath(GaiaSession gaiaSession = null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            return GetSessionSubFolderPath(gaiaSession);
        }

        /// <summary>
        /// Returns the path to store terrain scenes in, according to the session filename        
        /// /// </summary>
        /// <param name="gaiaSession">The current session to get / create this directory for</param>
        /// <returns></returns>
        public static string GetTerrainDataScenePath(GaiaSession gaiaSession = null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            return CreatePathIfDoesNotExist(GetScenePath(gaiaSession) + TERRAIN_DATA_DIRECTORY);
        }


        /// <summary>
        /// Returns a path for temporary exports (before they are copied to the final destination outside the "Assets" folder)
        /// </summary>
        /// <returns></returns>
        public static string GetTempExportPath()
        {
            return TEMP_EXPORT_PATH;
        }

        /// <summary>
        /// Returns the path to store terrain layer files in
        /// </summary>
        /// <param name="gaiaSession">The current session to get / create this directory for</param>
        /// <returns></returns>
        public static string GetTerrainLayerPath(GaiaSession gaiaSession = null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            return CreatePathIfDoesNotExist(GetScenePath(gaiaSession) + TERRAIN_LAYERS_DIRECTORY);
        }

        /// <summary>
        /// Returns the path to store the scriptable object files for the flora configuration
        /// </summary>
        /// <param name="gaiaSession">The current session to get / create this directory for</param>
        /// <returns></returns>
        public static string GetFloraDataPath(GaiaSession gaiaSession = null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            return CreatePathIfDoesNotExist(GetScenePath(gaiaSession) + FLORA_DATA_DIRECTORY);
        }

        /// <summary>
        /// Returns the older Gaia Legacy Stamps Directory. 
        /// </summary>
        /// <returns>The path to the Gaia Stamps directory</returns>
        public static string GetLegacyStampDirectory()
        {
#if UNITY_EDITOR
            foreach (var path in AssetDatabase.GetAllAssetPaths())
            {
                if (path.EndsWith("/Procedural Worlds/Gaia/Stamps"))
                {
                    return path;

                }
            }
            //return default if not found otherwise
            return "Assets/Procedural Worlds/Gaia/Stamps";
#else
            return "Assets/Procedural Worlds/Gaia/Stamps";
#endif

        }

        /// <summary>
        /// Returns the Gaia Stamps Directory. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Stamps directory</returns>
        public static string GetStampDirectory()
        {
            return GetPackageInstallDirectory() + STAMP_DIRECTORY;
        }

        /// <summary>
        /// Returns the Gaia Mask Export Directory. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Stamps directory</returns>
        public static string GetExportDirectory(GaiaSession gaiaSession = null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            return CreatePathIfDoesNotExist(GetScenePath(gaiaSession) + EXPORT_DIRECTORY);
        }

        public static string GetUserBiomeDirectory(string biomeName ="")
        {
            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
            //If no biome name was passed in, return the path to the root biome user data folder
            if (biomeName != "")
            {
                return CreatePathIfDoesNotExist(gaiaSettings.m_biomesDirectory + "/" + biomeName.Replace(" Biome", ""));
            }
            else
            {
                return CreatePathIfDoesNotExist(gaiaSettings.m_biomesDirectory);
            }
        }

        public static string GetUserSettingsDirectory()
        {
            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
            return CreatePathIfDoesNotExist(gaiaSettings.m_userSettingsDirectory);
        }

        /// <summary>
        /// Returns the path to store terrain scenes in, according to the session filename
        /// </summary>
        /// <param name="gaiaSession">The session to get / create this path for</param>
        /// <returns></returns>
        public static string GetMaskExportPathForSession(GaiaSession gaiaSession = null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            return CreatePathIfDoesNotExist(GetScenePath(gaiaSession) + MASK_EXPORTS_DIRECTORY);
        }

        public static string GetGWSettingsDirectory()
        {
            return CreatePathIfDoesNotExist(GetUserSettingsDirectory() + GWS_DIRECTORY);
        }

        public static string GetGaiaRuntimeComponentsDirectory()
        {
            return CreatePathIfDoesNotExist(GetUserSettingsDirectory() + GRC_DIRECTORY);
        }


        public static string GetMaskMapExportDirectory(GaiaSession gaiaSession = null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            return CreatePathIfDoesNotExist(GetExportDirectory(gaiaSession) + MASK_MAP_EXPORT_DIRECTORY);
        }

        /// <summary>
        /// Returns the Gaia User Stamp directory. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Stamps directory</returns>
        public static string GetUserStampDirectory()
        {
            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
            return CreatePathIfDoesNotExist(gaiaSettings.m_userStampsDirectory);
        }

        /// <summary>
        /// Returns the default stamper export directory within the user folder. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Stamps directory</returns>
        public static string GetStamperExportsDirectory()
        {
            return CreatePathIfDoesNotExist(GetUserStampDirectory() + STAMPER_EXPORT_DIRECTORY);
        }

        /// <summary>
        /// Returns the default stamper terrain export directory within the user folder. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Stamps directory</returns>
        public static string GetStamperTerrainExportsDirectory()
        {
            return CreatePathIfDoesNotExist(GetUserStampDirectory() + STAMPER_TERRAIN_EXPORT_DIRECTORY);
        }

        /// <summary>
        /// Returns the default directory for exporting the world designer preview within the user folder. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Stamps directory</returns>
        public static string GetWorldDesignerExportsDirectory()
        {
            return CreatePathIfDoesNotExist(GetUserStampDirectory() + WORLD_DESIGNER_EXPORT_DIRECTORY);
        }

        /// <summary>
        /// Returns the default scanner export directory within the user folder. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Stamps directory</returns>
        public static string GetScannerExportDirectory()
        {
            return CreatePathIfDoesNotExist(GetUserStampDirectory() + SCANNER_EXPORT_DIRECTORY);
        }

        /// <summary>
        /// Returns the Gaia Collision Data Directory. Will create it if it does not exist.
        /// </summary>
        /// <returns>The path to the Gaia Stamps directory</returns>
        public static string GetCollisionDataDirectory()
        {
            return GetGaiaSubDirectory(COLLISION_DATA_DIRECTORY, false);
        }

        public static string GetTerrainMeshExportDirectory(GaiaSession gaiaSession=null)
        {
            if (gaiaSession == null)
            {
                gaiaSession = GaiaSessionManager.GetSessionManager().m_session;
            }
            return CreatePathIfDoesNotExist(GetExportDirectory(gaiaSession) + TERRAIN_MESH_EXPORT_DIRECTORY);
        }

        /// <summary>
        /// Creates and returns the path to a certain stamp feature type.
        /// </summary>
        /// <param name="featureType">The feature type which we want to create / get the folder for.</param>
        /// <returns>The path to the specific stamp feature insisde the stamps folder.</returns>
        public static string GetStampFeatureDirectory(GaiaConstants.FeatureType featureType)
        {
            return CreatePathIfDoesNotExist(GetStampDirectory() + "/" + featureType.ToString());
        }

        /// <summary>
        /// Creates and returns the path to a certain terrain's baked collision data.
        /// </summary>
        /// <param name="terrain">The terrain for which we want to create / get the folder for.</param>
        /// <returns>The path to the collision data folder for this terrain.</returns>
        public static string GetTerrainCollisionDirectory(Terrain terrain)
        {
            return GetCollisionDataDirectory() + "/" + terrain.name;
        }



        /// <summary>
        /// Gets the path for a specific stamp instance within the stamp / feature structure.
        /// </summary>
        /// <param name="m_featureType">The stamp feature type</param>
        /// <param name="m_featureName">The stamp name</param>
        /// <returns>The path to the specific stamp instance directory.</returns>
        public static string GetStampInstanceDirectory(GaiaConstants.FeatureType m_featureType, string m_featureName)
        {
            return CreatePathIfDoesNotExist(GetStampFeatureDirectory(m_featureType)) + "/" + m_featureName;
        }


        /// <summary>
        /// Returns a path to a Gaia subdirectory. The subdirectory can be optionally created if it does not exist already.
        /// </summary>
        /// <param name="subDir">The Subdir to create.</param>
        /// <param name="create">Should the subdir be created if it does not exist?</param>
        /// <returns>The complete path to the subdir.</returns>
        private static string GetGaiaSubDirectory(string subDir, bool create = true)
        {
            string path = GetGaiaDirectory() + subDir;
            if (create)
            {
                return CreatePathIfDoesNotExist(path);
            }
            else
            {
                return path;
            }
        }

        /// <summary>
        /// Checks if a path exists, if not it will be created.
        /// </summary>
        /// <param name="path"></param>
        public static string CreatePathIfDoesNotExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }


        /// <summary>
        /// Expects a full file system path starting at a drive letter, and will return a path starting at the projects / games asset folder
        /// </summary>
        /// <param name="inputPath">Full file system path starting at a drive letter</param>
        /// <returns></returns>
        public static string GetPathStartingAtAssetsFolder(string inputPath)
        {
            return inputPath.Substring(Application.dataPath.Length - "Assets".Length);
        }
        /// <summary>
        /// Expects a path starting at the asset folder, and will return a full file system path starting at the Drive Letter
        /// </summary>
        /// <param name="inputPath">Unity path starting at the asset folder</param>
        /// <returns></returns>
        public static string GetFullFileSystemPath(string inputPath)
        {
            return Application.dataPath.Substring(0,Application.dataPath.Length - "Assets".Length) + inputPath;
        }

        /// <summary>
        /// Returns the path to the Gaia-specific shader directory
        /// </summary>
        /// <returns></returns>
        public static string GetGaiaShaderPath()
        {
            return GetGaiaDirectory() + GAIA_SHADER_DIRECTORY;
        }

        /// <summary>
        /// Returns the path to the Flora-specific shader directory. Will return empty string if flora folder is not present in the project.
        /// </summary>
        /// <returns></returns>
        public static string GetFloraShaderPath()
        {
            string floraDir = GetFloraDirectory();

            if (!string.IsNullOrEmpty(floraDir))
            {
                floraDir += FLORA_SHADER_DIRECTORY;
            }

            return floraDir;
        }

        /// <summary>
        /// Returns the path to the install directory of the PW packages
        /// </summary>
        /// <returns></returns>
        public static string GetPackageInstallDirectory()
        {
            //Default Directory, will be returned if not in Editor
            string rootDir = "Assets/Procedural Worlds/Packages - Install/";
#if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets("Packages_Install_Readme", null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (Path.GetFileName(path) == "Packages_Install_Readme.txt")
                {
                    rootDir = path.Replace("/Packages_Install_Readme.txt", "");
                }
            }
#endif
            return rootDir;
        }
    }
}
