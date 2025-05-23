﻿using PWCommon5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static Gaia.FbmFractalType;

namespace Gaia
{
    public enum MaskListButtonCommand {None, Delete, Duplicate, Copy, Paste}


    //This class is not a full editor class by itself, but used to collect reusable methods
    //for editing imagemasks in a reorderable list.
    public class ImageMaskListEditor : PWEditor, IPWEditor
    {

        public static float OnElementHeight(int index, ImageMask imageMask)
        {
            float height = 0;

            //header
            height += EditorGUIUtility.singleLineHeight * 1.5f;

            //Early out. if mask is not folded out there is no need to calculate the height
            if (imageMask.m_foldedOut == false)
            {
                return height;
            }

            //2 first basic mask fields if in first position, 3 if in second
            //Exception for smooth / grow&shrink because those never have a blend mode
            if (index == 0 && imageMask.m_operation != ImageMaskOperation.Smooth && imageMask.m_operation != ImageMaskOperation.GrowAndShrink)
            {
                height = EditorGUIUtility.singleLineHeight * 2;
            }
            else
            {
                height = EditorGUIUtility.singleLineHeight * 3;
            }
            switch (imageMask.m_operation)
            {
                case ImageMaskOperation.NoiseMask:
                    //calculate the height according to the foldout display state:
                    if (imageMask.m_ShowNoisePreviewTexture)
                    {
                        height += 256f + 40f;
                    }
                    else
                    {
                        height += EditorGUIUtility.singleLineHeight;
                    }
                    if (imageMask.m_ShowNoiseTransformSettings)
                    {
                        height += EditorGUIUtility.singleLineHeight *5;
                    }
                    else
                    {
                        height += EditorGUIUtility.singleLineHeight;
                    }
                    if (imageMask.m_ShowNoiseTypeSettings)
                    {
                        height += EditorGUIUtility.singleLineHeight * 13;
                    }
                    else
                    {
                        height += EditorGUIUtility.singleLineHeight;
                    }
                    if (imageMask.m_noiseSettings != null && imageMask.m_noiseSettings.domainSettings.noiseTypeName == "Voronoi")
                    { 
                        //Extra height for the Vorornoi noise warning
                        height += EditorGUIUtility.singleLineHeight * 2;
                    }
                    if (Gaia.FbmFractalType.instance != null && imageMask.noiseSettingsGUI!=null && imageMask.noiseSettingsGUI.fractalTypeParams!=null)
                    {
                        FbmFractalInput fbm = (FbmFractalInput)Gaia.FbmFractalType.instance.FromSerializedString(imageMask.noiseSettingsGUI.fractalTypeParams.stringValue);
                        if (fbm.lacunarity > 0.5f && fbm.lacunarity < 1.5f)
                        {
                            //Extra height for the Lacunarity noise warning
                            height += EditorGUIUtility.singleLineHeight * 2;
                        }
                    }


                    height += EditorGUIUtility.singleLineHeight;
                    break;
                case ImageMaskOperation.CollisionMask:
                    //height of the default fields
                    //+ the height required for the tree radius selection dropdowns
                    //+ one line each for each other collision mask entry
                    //+ 4 bonus lines for better accomodating the collision mask list
                    foreach (CollisionMask cm in imageMask.m_collisionMasks)
                    {
                        if (cm.m_type == BakedMaskType.RadiusTree)
                        {
                            height += EditorGUIUtility.singleLineHeight * Mathf.CeilToInt((float)CollisionMask.m_allTreeSpawnRuleNames.Length / 32f);
                        }
                        else
                        {
                            height += EditorGUIUtility.singleLineHeight;
                        }
                    }

                    height += EditorGUIUtility.singleLineHeight * 4; 
                    break;
                case ImageMaskOperation.PolyMask:
                    height += EditorGUIUtility.singleLineHeight * 3;
                    break;
                case ImageMaskOperation.PROHydraulicErosion:
#if GAIA_2023_PRO
                    //this is a complicated one, need to take the open / closed foldouts into account
                    //at least 5 extra lines when everything is closed
                    height += EditorGUIUtility.singleLineHeight * 6;
                    if(imageMask.m_erosionShowAdvancedUI)
                    {
                        height += EditorGUIUtility.singleLineHeight * 4;
                    }
                    if (imageMask.m_erosionShowThermalUI)
                    {
                        height += EditorGUIUtility.singleLineHeight * 3;
                    }
                    if (imageMask.m_erosionShowWaterUI)
                    {
                        height += EditorGUIUtility.singleLineHeight * 3;
                    }
                    if (imageMask.m_erosionShowSedimentUI)
                    {
                        height += EditorGUIUtility.singleLineHeight * 3;
                    }
                    if (imageMask.m_erosionShowRiverBankUI)
                    {
                        height += EditorGUIUtility.singleLineHeight * 4;
                    }
                    height += EditorGUIUtility.singleLineHeight;
#else
                    height = EditorGUIUtility.singleLineHeight * 5;
#endif
                    break;
                case ImageMaskOperation.HeightMask:
                    height += EditorGUIUtility.singleLineHeight * 5;
                    break;
                case ImageMaskOperation.SlopeMask:
                    height += EditorGUIUtility.singleLineHeight * 4.5f;
                    break;
                case ImageMaskOperation.ImageMask:
                    height += EditorGUIUtility.singleLineHeight * 7;
                    if (imageMask.ImageMaskTexture != null && !GaiaConstants.Valid16BitFormats.Contains(imageMask.ImageMaskTexture.format))
                    {
                        height += EditorGUIUtility.singleLineHeight * 2;
                    }
                    if (imageMask.m_imageMaskFilterMode == GaiaConstants.ImageMaskFilterMode.PROColorSelection)
                    {
                        height += EditorGUIUtility.singleLineHeight * 2;
                    }
                    break;
                case ImageMaskOperation.DistanceMask:
                    height += EditorGUIUtility.singleLineHeight * 8;
                    if (imageMask.m_distanceMaskAxes == ImageMaskDistanceMaskAxes.XZSquare)
                    {
                        height += EditorGUIUtility.singleLineHeight;
                    }
                    break;
                case ImageMaskOperation.StrengthTransform:
                    height += EditorGUIUtility.singleLineHeight * 1;
                    break;
                case ImageMaskOperation.Smooth:
                    height += EditorGUIUtility.singleLineHeight * 2;
                    break;
                case ImageMaskOperation.TerrainTexture:
                    height += EditorGUIUtility.singleLineHeight * 2;
                    break;
                case ImageMaskOperation.PROConcaveConvex:
#if GAIA_2023_PRO
                    height += EditorGUIUtility.singleLineHeight * 3;
#else
                    height = EditorGUIUtility.singleLineHeight * 5;
#endif
                    break;
                case ImageMaskOperation.WorldBiomeMask:
                    height += EditorGUIUtility.singleLineHeight * 5;
                    break;
                case ImageMaskOperation.GrowAndShrink:
                    height += EditorGUIUtility.singleLineHeight;
                    break;
                case ImageMaskOperation.GlobalSpawnerMaskStack:
                    height += EditorGUIUtility.singleLineHeight;
                    if (imageMask.m_imageMaskLocation != ImageMaskLocation.SpawnRule)
                    {
                        height += EditorGUIUtility.singleLineHeight * 4;
                    }
                    break;
                case ImageMaskOperation.PROCurvature:
                    height += EditorGUIUtility.singleLineHeight * 5;
                    break;
                //case ImageMaskOperation.ConcaveConvex:

                //    return height + EditorGUIUtility.singleLineHeight * 3;
                default:
                    break;
            }
            return height + EditorGUIUtility.singleLineHeight;
        }

        public static ImageMask[] OnRemoveMaskListEntry(ImageMask[] oldList, int index)
        {
            if (index < 0 || index >= oldList.Length)
                return null;
            ImageMask toRemove = oldList[index];

            if (toRemove != null)
            {
                toRemove.ClearEroder();
            }

            ImageMask[] newList = new ImageMask[oldList.Length - 1];
            for (int i = 0; i < newList.Length; ++i)
            {
                if (i < index)
                {
                    newList[i] = oldList[i];
                }
                else if (i >= index)
                {
                    newList[i] = oldList[i + 1];
                }
            }
            return newList;
        }

        public static ImageMask[] OnAddMaskListEntry(ImageMask[] oldList, float maxCurrentTerrainHeight, float minCurrentTerrainHeight, float seaLevel)
        {
            ImageMask[] newList = new ImageMask[oldList.Length + 1];
            for (int i = 0; i < oldList.Length; ++i)
            {
                newList[i] = oldList[i];
            }
            ImageMask newMask = new ImageMask();
            newMask.m_maxWorldHeight = maxCurrentTerrainHeight;
            newMask.m_minWorldHeight = minCurrentTerrainHeight;
            newMask.m_seaLevel = seaLevel;
            //make sure that the min max for the height mask lies within the min max of the world - otherwise user ends up with a locked slider!
            float spread = newMask.MaxPossiblePercent - newMask.MinPossiblePercent;
            newMask.m_relativeHeightMin = newMask.MinPossiblePercent + spread * 0.25f;
            newMask.m_relativeHeightMax = newMask.MaxPossiblePercent - spread * 0.25f;
            newList[newList.Length - 1] = newMask;
            return newList;
        }

        public static bool DrawFilterListHeader(Rect rect, bool currentFoldOutState, ImageMask[] maskList, EditorUtils editorUtils)
        {
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            rect.xMin += 8f;
            bool newFoldOutState = EditorGUI.Foldout(rect, currentFoldOutState, PropertyCount("MaskSettings", maskList, editorUtils), true);
            EditorGUI.indentLevel = oldIndent;
            return newFoldOutState;
        }

        public static void DrawMaskList(ref bool listExpanded, ReorderableList list, EditorUtils editorUtils, bool helpEnabled)
        {
            if (list == null)
            {
                return;
            }

            Rect maskRect;
            if (listExpanded)
            {
                maskRect = EditorGUILayout.GetControlRect(true, list.GetHeight());
                list.DoList(maskRect);
            }
            else
            {
                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 1;
                listExpanded = EditorGUILayout.Foldout(listExpanded, PropertyCount("MaskSettings", (ImageMask[])list.list, editorUtils), true);
                maskRect = GUILayoutUtility.GetLastRect();
                EditorGUI.indentLevel = oldIndent;
            }
            editorUtils.InlineHelp("MaskSettings",helpEnabled);
            //editorUtils.Panel("MaskBaking", DrawMaskBaking, false);
        }

        public static MaskListButtonCommand DrawMaskListElement(Rect rect, int index, ImageMask[] imageMasks, ref CollisionMask[] collisionMaskListBeingDrawn, EditorUtils m_editorUtils, Terrain currentTerrain, bool isStampOperation, ImageMask copiedMask, Texture2D headerBGTexture = null, Texture2D errorTexture=null, GaiaSettings gaiaSettings = null, GaiaSessionManager gaiaSessionManager = null)
        {
            if (index >= imageMasks.Length)
            {
                return MaskListButtonCommand.None; 
            }

            ImageMask imageMask = imageMasks[index];
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
            //The only image mask operation that displays an error header is currently the texture mask
            if (imageMask.m_hasErrors && imageMask.m_operation == ImageMaskOperation.TerrainTexture)
            {
                headerStyle.normal.background = errorTexture;
            }
            else
            {
                headerStyle.normal.background = headerBGTexture;
            }
            
            Rect headerRect = rect;
            headerRect.height = EditorGUIUtility.singleLineHeight * 1.3f;

            EditorGUI.LabelField(headerRect, "", headerStyle);

            //little bit of indent to fit the foldout in
            rect.x += 15;
            rect.y += 2;
            rect.width -= 15;
            rect.height = EditorGUIUtility.singleLineHeight;


            //Foldout Label Styles
            GUIStyle foldoutLabelStyle = new GUIStyle(GUI.skin.label);
            foldoutLabelStyle.normal.textColor = GUI.skin.label.normal.textColor;
            foldoutLabelStyle.fontStyle = FontStyle.Normal;
            foldoutLabelStyle.normal.background = GUI.skin.label.normal.background;
            foldoutLabelStyle.padding = new RectOffset(3,0,0,8);

            //Button Styles
            GUIStyle smallButtonStyle = new GUIStyle(GUI.skin.button);
            smallButtonStyle.padding = new RectOffset(2, 2, 2, 2);

            Rect foldoutRect = rect;
            //foldoutRect.x = 55;
            foldoutRect.width = 1;
            foldoutRect.height = EditorGUIUtility.singleLineHeight * 1.5f;

            //imageMask.m_foldedOut = EditorGUI.(foldoutRect, imageMask.m_foldedOut ? "-" : "+", imageMask.m_foldedOut, foldoutLabelStyle);
            //imageMask.m_active = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskActive"), imageMask.m_active);
            imageMask.m_foldedOut = EditorGUI.BeginFoldoutHeaderGroup(foldoutRect, imageMask.m_foldedOut, imageMask.m_foldedOut ? "-" : "+", foldoutLabelStyle);
            EditorGUI.EndFoldoutHeaderGroup();

            foldoutRect.x += 1;
            foldoutRect.width = 15;
            
            foldoutRect.y -= 5;
            imageMask.m_active = EditorGUI.Toggle(foldoutRect, "", imageMask.m_active);


            foldoutRect.x += 30;
            foldoutRect.y += 5;
            foldoutRect.width = EditorGUIUtility.labelWidth - 30;

            if (imageMask.m_imageMaskLocation == ImageMaskLocation.SpawnRule)
            {
                imageMask.m_operation = (ImageMaskOperation)EditorGUI.EnumPopup(foldoutRect, "", imageMask.m_operation);
            }
            else
            {
                //Display only a reduced set of operations, but convert the selected index back into the full set
                imageMask.m_operation = (ImageMaskOperation)((ImageMaskOperationReduced)EditorGUI.EnumPopup(foldoutRect, "", (ImageMaskOperationReduced)imageMask.m_operation));
            }

            //EditorGUI.EndFoldoutHeaderGroup();

            foldoutRect.x = rect.width - 15;
            foldoutRect.width = 20;
            foldoutRect.height = 15;
            foldoutRect.y += 2;

            GUIContent GCcopyIcon = GaiaEditorUtils.GetIconGUIContent("IconCopy", gaiaSettings.m_IconCopy, gaiaSettings.m_IconProCopy, m_editorUtils);
            if (GUI.Button(foldoutRect, GCcopyIcon, smallButtonStyle))
            {
                return MaskListButtonCommand.Copy;
            }


            foldoutRect.x = rect.width - 35;
            foldoutRect.width = 20;
            foldoutRect.height = 15;

            GUIContent GCduplicateIcon = GaiaEditorUtils.GetIconGUIContent("IconDuplicate", gaiaSettings.m_IconDuplicate, gaiaSettings.m_IconProDuplicate, m_editorUtils);
            if (GUI.Button(foldoutRect, GCduplicateIcon, smallButtonStyle))
            {
                return MaskListButtonCommand.Duplicate;
            }

            foldoutRect.x = rect.width + 5;
            foldoutRect.width = 20;
            foldoutRect.height = 15;

            bool oldGUIState = GUI.enabled;
            if (copiedMask == null)
            {
                GUI.enabled = false;
            }

            GUIContent GCpasteIcon = GaiaEditorUtils.GetIconGUIContent("IconPaste", gaiaSettings.m_IconPaste, gaiaSettings.m_IconProPaste, m_editorUtils);
            if (GUI.Button(foldoutRect, GCpasteIcon, smallButtonStyle))
            {
                return MaskListButtonCommand.Paste;
            }
            GUI.enabled = oldGUIState;

            foldoutRect.x = rect.width + 35;
            foldoutRect.width = 20;
            foldoutRect.height = 15;

            GUIContent GCremoveIcon = GaiaEditorUtils.GetIconGUIContent("IconRemove", gaiaSettings.m_IconRemove, gaiaSettings.m_IconProRemove, m_editorUtils);
            if (GUI.Button(foldoutRect, GCremoveIcon, smallButtonStyle))
            {
                return MaskListButtonCommand.Delete;
            }

            if (imageMask.m_foldedOut)
            {
                rect.y += EditorGUIUtility.singleLineHeight * 0.5f;


#if !GAIA_2023_PRO
                if (imageMask.m_operation == ImageMaskOperation.PROConcaveConvex || imageMask.m_operation == ImageMaskOperation.PROHydraulicErosion || imageMask.m_operation == ImageMaskOperation.PROCurvature)
                {
                    rect.y += EditorGUIUtility.singleLineHeight;
                    rect.height = EditorGUIUtility.singleLineHeight * 3;
                    EditorGUI.HelpBox(rect, m_editorUtils.GetTextValue("MaskGaiaProInfo"), MessageType.Info);
                }
                else
                {
#endif
                if (imageMask.m_operation == ImageMaskOperation.GlobalSpawnerMaskStack && imageMask.m_imageMaskLocation != ImageMaskLocation.SpawnRule)
                {
                    GUI.enabled = false;
                }

                    //First mask in the stack always needs to be in "Multiply" mode
                    if (index == 0)
                    {
                        //First mask in the stack always needs to be in "Multiply" mode, others can be selected by the user
                        imageMask.m_blendMode = ImageMaskBlendMode.Multiply;
                    }
                    else
                    {
                        //Do not display blend mode for smooth or grow/shrink mask, just makes 0 sense to do that
                        if (imageMask.m_operation != ImageMaskOperation.Smooth && imageMask.m_operation != ImageMaskOperation.GrowAndShrink)
                        {
                            rect.y += EditorGUIUtility.singleLineHeight;
                            imageMask.m_blendMode = (ImageMaskBlendMode)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskBlendMode"), imageMask.m_blendMode);
                        }
                        else
                        {
                            imageMask.m_blendMode = ImageMaskBlendMode.Multiply;
                        }

                    }
                    if (isStampOperation)
                    {
                        rect.y += EditorGUIUtility.singleLineHeight;
                        imageMask.m_influence = (ImageMaskInfluence)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskInfluence"), imageMask.m_influence);
                    }
                    else
                    {
                        imageMask.m_influence = ImageMaskInfluence.Local;
                    }
                    rect.y += EditorGUIUtility.singleLineHeight;
                    imageMask.m_strengthTransformCurve = EditorGUI.CurveField(new Rect(rect.x, rect.y, rect.width - 73, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskStrengthTransformCurve"), imageMask.m_strengthTransformCurve);
                    if (GUI.Button(new Rect(rect.x + rect.width - 70, rect.y, 70, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskInvert")))
                    {
                        GaiaUtils.InvertAnimationCurve(ref imageMask.m_strengthTransformCurve);
                    }
                    rect.y += EditorGUIUtility.singleLineHeight;
#if !GAIA_2023_PRO
                }
#endif
                    GUIStyle alignmentStyleRight = new GUIStyle(GUI.skin.label);
                    alignmentStyleRight.alignment = TextAnchor.MiddleRight;
                    alignmentStyleRight.stretchWidth = true;
                    GUIStyle alignmentStyleLeft = new GUIStyle(GUI.skin.label);
                    alignmentStyleLeft.alignment = TextAnchor.MiddleLeft;
                    alignmentStyleLeft.stretchWidth = true;
                    GUIStyle alignmentStyleCenter = new GUIStyle(GUI.skin.label);
                    alignmentStyleCenter.alignment = TextAnchor.MiddleCenter;
                    alignmentStyleCenter.stretchWidth = true;
                    GUIStyle heightNumberFormat = new GUIStyle();

                GUI.enabled = oldGUIState;

                switch (imageMask.m_operation)
                {
                    case ImageMaskOperation.ImageMask:
                        if (imageMask.ImageMaskTexture != null && !GaiaConstants.Valid16BitFormats.Contains(imageMask.ImageMaskTexture.format))
                        {
                            rect.height = EditorGUIUtility.singleLineHeight * 2;
                            EditorGUI.HelpBox(rect, "Supplied texture is not in 16-bit color format. For optimal quality use 16+ bit color images.", MessageType.Warning);
                            rect.y += EditorGUIUtility.singleLineHeight * 2;
                            rect.height = EditorGUIUtility.singleLineHeight;
                        }
                        imageMask.ImageMaskTexture = (Texture2D)EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width -73, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskImageMask"), imageMask.ImageMaskTexture, typeof(Texture2D), false);
                        if (GUI.Button(new Rect(rect.x + rect.width - 70, rect.y, 70, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskImageOpenStampButton")))
                        {
                            OpenStampBrowser(imageMask);
                        }
                        rect.y += EditorGUIUtility.singleLineHeight;
                        imageMask.m_imageMaskFilterMode = (GaiaConstants.ImageMaskFilterMode)EditorGUI.EnumPopup(rect, m_editorUtils.GetContent("MaskImageFilterMode"), imageMask.m_imageMaskFilterMode);
                        if (imageMask.m_imageMaskFilterMode == GaiaConstants.ImageMaskFilterMode.PROColorSelection)
                        {
#if GAIA_2023_PRO
                            rect.y += EditorGUIUtility.singleLineHeight;
                            imageMask.m_imageMaskColorSelectionColor = EditorGUI.ColorField(rect, m_editorUtils.GetContent("MaskImageColor") ,imageMask.m_imageMaskColorSelectionColor);
                            rect.y += EditorGUIUtility.singleLineHeight;
                            imageMask.m_imageMaskColorSelectionAccuracy = EditorGUI.Slider(rect, m_editorUtils.GetContent("MaskImageColorAccuracy"), imageMask.m_imageMaskColorSelectionAccuracy, 0f,1f);
#else
                            rect.y += EditorGUIUtility.singleLineHeight;
                            EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 2f), m_editorUtils.GetTextValue("MaskImageFilterProInfo"), MessageType.Info);
#endif
                        }

                        DrawOffsetFields(imageMask, rect, m_editorUtils, currentTerrain);


                        break;
                    case ImageMaskOperation.DistanceMask:
                        imageMask.m_distanceMaskCurve = EditorGUI.CurveField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskDistanceMask"), imageMask.m_distanceMaskCurve);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        imageMask.m_distanceMaskAxes = (ImageMaskDistanceMaskAxes)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskDistanceMaskAxes"), imageMask.m_distanceMaskAxes);
                        if (imageMask.m_distanceMaskAxes == ImageMaskDistanceMaskAxes.XZSquare)
                        {
                            rect.y += EditorGUIUtility.singleLineHeight;
                            imageMask.m_distanceMaskSquareRoundness = EditorGUI.Slider(rect, m_editorUtils.GetContent("MaskSquareRoundness"), imageMask.m_distanceMaskSquareRoundness * 100.0f, 0.0f, 100.0f)/100.0f;
                        }
                        DrawOffsetFields(imageMask, rect, m_editorUtils, currentTerrain);
                        
                        break;
                    case ImageMaskOperation.HeightMask:
                        rect.height = EditorGUIUtility.singleLineHeight;
                        imageMask.m_heightMaskCurve = EditorGUI.CurveField(rect, m_editorUtils.GetContent("MaskHeightMaskCurve"), imageMask.m_heightMaskCurve);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        Rect heightMaskTypeRect = new Rect(rect);
                        heightMaskTypeRect.width = EditorGUIUtility.labelWidth + 0.7f * (rect.width - EditorGUIUtility.labelWidth);
                        Rect heightMaskUnitRect = new Rect(rect);
                        heightMaskUnitRect.width = 0.3f * (rect.width - EditorGUIUtility.labelWidth);
                        heightMaskUnitRect.x = rect.x + heightMaskTypeRect.width;

                        imageMask.m_heightMaskType = (HeightMaskType)EditorGUI.EnumPopup(heightMaskTypeRect, m_editorUtils.GetContent("MaskHeightMaskType"), imageMask.m_heightMaskType);
                        imageMask.HeightMaskUnit = (HeightMaskUnit)EditorGUI.EnumPopup(heightMaskUnitRect, imageMask.HeightMaskUnit);
                        rect.y += EditorGUIUtility.singleLineHeight;

                        if (imageMask.HeightMaskUnit == HeightMaskUnit.Meter)
                        {
                            //If min and max world height are switched or the same, the slider cannot display the handles properly. Therefore make sure there is at least 1 unit of height difference
                            if (imageMask.m_minWorldHeight >= imageMask.m_maxWorldHeight)
                            {
                                imageMask.m_maxWorldHeight = imageMask.m_minWorldHeight + 1;
                            }

                            if (imageMask.m_heightMaskType == HeightMaskType.WorldSpace)
                            {
                                //We need to check if the slider can actually acommodate these min max values according to the current world height
                                //If it cannot, we should not display the slider as it would immediately override our min max values without user input.
                                //In this case we deactivate the slider, and the user can still choose to manually edit the text input fields. 
                                //As soon as the world height changes to a point where the slider can fit in the min max values properly, it will be unlocked again as well.

                                if (imageMask.m_absoluteHeightMin >= imageMask.m_minWorldHeight && imageMask.m_absoluteHeightMax <= imageMask.m_maxWorldHeight)
                                {
                                    //The 0.01 adjustments for min max are to make sure the slider range does not exceed the minimum and maximum
                                    //without this it can happen that the user locks themselves out of the slider
                                    EditorGUI.MinMaxSlider(rect, m_editorUtils.GetContent("MaskHeightMinMax"), ref imageMask.m_absoluteHeightMin, ref imageMask.m_absoluteHeightMax, imageMask.m_minWorldHeight + 0.01f, imageMask.m_maxWorldHeight - 0.01f);
                                    imageMask.m_relativeHeightMin = imageMask.m_absoluteHeightMin - imageMask.m_seaLevel;
                                    imageMask.m_relativeHeightMax = imageMask.m_absoluteHeightMax - imageMask.m_seaLevel;
                                }
                                else
                                {
                                    bool previousGUIState = GUI.enabled;
                                    GUI.enabled = false;
                                    float fake1 = imageMask.m_minWorldHeight;
                                    float fake2 = imageMask.m_maxWorldHeight;
                                    EditorGUI.MinMaxSlider(rect, m_editorUtils.GetContent("MaskHeightMinMax"), ref fake1, ref fake2, imageMask.m_minWorldHeight + 0.01f, imageMask.m_maxWorldHeight - 0.01f);
                                    GUI.enabled = previousGUIState;
                                }
                            }
                            else
                            {
                                if (imageMask.m_relativeHeightMin >= imageMask.MinPossibleMeter && imageMask.m_relativeHeightMax <= imageMask.MaxPossibleMeter)
                                {
                                    EditorGUI.MinMaxSlider(rect, m_editorUtils.GetContent("MaskHeightMinMax"), ref imageMask.m_relativeHeightMin, ref imageMask.m_relativeHeightMax, imageMask.m_minWorldHeight - imageMask.m_seaLevel + 0.01f, imageMask.m_maxWorldHeight - imageMask.m_seaLevel - 0.01f);
                                    imageMask.m_absoluteHeightMin = imageMask.m_relativeHeightMin + imageMask.m_seaLevel;
                                    imageMask.m_absoluteHeightMax = imageMask.m_relativeHeightMax + imageMask.m_seaLevel;
                                }
                                else
                                {
                                    bool previousGUIState = GUI.enabled;
                                    GUI.enabled = false;
                                    float fake1 = imageMask.m_minWorldHeight;
                                    float fake2 = imageMask.m_maxWorldHeight;
                                    EditorGUI.MinMaxSlider(rect, m_editorUtils.GetContent("MaskHeightMinMax"), ref fake1, ref fake2, imageMask.m_minWorldHeight + 0.01f, imageMask.m_maxWorldHeight - 0.01f);
                                    GUI.enabled = previousGUIState;
                                }
                            }
                        }
                        else
                        {
                            if (imageMask.m_heightMaskType == HeightMaskType.WorldSpace)
                            {
                                if (imageMask.m_absoluteHeightMin >= 0 && imageMask.m_absoluteHeightMax <= 100)
                                {
                                    EditorGUI.MinMaxSlider(rect, m_editorUtils.GetContent("MaskHeightMinMax"), ref imageMask.m_absoluteHeightMin, ref imageMask.m_absoluteHeightMax, 0, 100f);
                                    imageMask.m_relativeHeightMin = imageMask.m_absoluteHeightMin - imageMask.SeaLevelInPercent;
                                    imageMask.m_relativeHeightMax = imageMask.m_absoluteHeightMax - imageMask.SeaLevelInPercent;
                                }
                                else
                                {
                                    bool previousGUIState = GUI.enabled;
                                    GUI.enabled = false;
                                    float fake1 = 0;
                                    float fake2 = 100;
                                    EditorGUI.MinMaxSlider(rect, m_editorUtils.GetContent("MaskHeightMinMax"), ref fake1, ref fake2, 0, 100f);
                                    GUI.enabled = previousGUIState;
                                }
                            }
                            else
                            {
                                if (imageMask.m_relativeHeightMin >= imageMask.MinPossiblePercent && imageMask.m_relativeHeightMax <= imageMask.MaxPossiblePercent)
                                {
                                    EditorGUI.MinMaxSlider(rect, m_editorUtils.GetContent("MaskHeightMinMax"), ref imageMask.m_relativeHeightMin, ref imageMask.m_relativeHeightMax, imageMask.MinPossiblePercent + 0.01f, imageMask.MaxPossiblePercent - 0.01f);
                                    imageMask.m_absoluteHeightMin = imageMask.m_relativeHeightMin + imageMask.SeaLevelInPercent;
                                    imageMask.m_absoluteHeightMax = imageMask.m_relativeHeightMax + imageMask.SeaLevelInPercent;
                                }
                                else
                                {
                                    bool previousGUIState = GUI.enabled;
                                    GUI.enabled = false;
                                    float fake1 = imageMask.MinPossiblePercent;
                                    float fake2 = imageMask.MaxPossiblePercent;
                                    EditorGUI.MinMaxSlider(rect, m_editorUtils.GetContent("MaskHeightMinMax"), ref fake1, ref fake2, imageMask.MinPossiblePercent, imageMask.MaxPossiblePercent);
                                    GUI.enabled = previousGUIState;
                                }
                            }
                        }

                        float seaLevelMarkerXPos = rect.x + EditorGUIUtility.labelWidth + (rect.width - EditorGUIUtility.labelWidth) * imageMask.ScalarSeaLevel;
                        EditorGUI.DrawRect(new Rect(seaLevelMarkerXPos, rect.y, 3f, EditorGUIUtility.singleLineHeight), Color.red);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        //Draw the input fields for numerical reference / entry
                        //Label
                        EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskHeightNumerical"));
                        //Min Label
                        Rect numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                        GUIContent minContent = m_editorUtils.GetContent("MaskHeightNumericMin");
                        if (imageMask.HeightMaskUnit == HeightMaskUnit.Meter)
                        {
                            if (imageMask.m_heightMaskType == HeightMaskType.WorldSpace)
                            {
                                minContent.text += String.Format(" {0:N0}", imageMask.m_minWorldHeight);
                            }
                            else
                            {
                                minContent.text += String.Format(" {0:N0}", imageMask.MinPossibleMeter);
                            }
                        }
                        else
                        {
                            if (imageMask.m_heightMaskType == HeightMaskType.WorldSpace)
                            {
                                minContent.text += String.Format(" 0%");
                            }
                            else
                            {
                                minContent.text += String.Format(" {0:N0}%", imageMask.MinPossiblePercent);
                            }
                        }
                        EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
                        // Min Entry Field
                        numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                        if (imageMask.HeightMaskUnit == HeightMaskUnit.Meter)
                        {
                            if (imageMask.m_heightMaskType == HeightMaskType.WorldSpace)
                            {
                                imageMask.m_absoluteHeightMin = EditorGUI.FloatField(numFieldRect, (float)Math.Round(imageMask.m_absoluteHeightMin, 2));
                            }
                            else
                            {
                                imageMask.m_relativeHeightMin = EditorGUI.FloatField(numFieldRect, (float)Math.Round(imageMask.m_relativeHeightMin, 2));
                            }
                        }
                        else
                        {
                            if (imageMask.m_heightMaskType == HeightMaskType.WorldSpace)
                            {
                                imageMask.m_absoluteHeightMin = EditorGUI.FloatField(numFieldRect, (float)Math.Round(imageMask.m_absoluteHeightMin, 2));
                            }
                            else
                            {
                                imageMask.m_relativeHeightMin = EditorGUI.FloatField(numFieldRect, (float)Math.Round(imageMask.m_relativeHeightMin, 2));
                            }
                        }
                        //Empty Label Field for Spacing
                        numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                        EditorGUI.LabelField(numFieldRect, " ");
                        //MaxEntryField
                        numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                        if (imageMask.HeightMaskUnit == HeightMaskUnit.Meter)
                        {
                            if (imageMask.m_heightMaskType == HeightMaskType.WorldSpace)
                            {
                                imageMask.m_absoluteHeightMax = EditorGUI.FloatField(numFieldRect, (float)Math.Round(imageMask.m_absoluteHeightMax, 2));
                            }
                            else
                            {
                                imageMask.m_relativeHeightMax = EditorGUI.FloatField(numFieldRect, (float)Math.Round(imageMask.m_relativeHeightMax, 2));
                            }
                        }
                        else
                        {
                            if (imageMask.m_heightMaskType == HeightMaskType.WorldSpace)
                            {
                                imageMask.m_absoluteHeightMax = EditorGUI.FloatField(numFieldRect, (float)Math.Round(imageMask.m_absoluteHeightMax, 2));
                            }
                            else
                            {
                                imageMask.m_relativeHeightMax = EditorGUI.FloatField(numFieldRect, (float)Math.Round(imageMask.m_relativeHeightMax, 2));
                            }
                        }
                        //Max Label
                        numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                        GUIContent maxContent = m_editorUtils.GetContent("MaskHeightNumericMax");
                        if (imageMask.HeightMaskUnit == HeightMaskUnit.Meter)
                        {
                            if (imageMask.m_heightMaskType == HeightMaskType.WorldSpace)
                            {
                                maxContent.text = String.Format("{0:N0} ", imageMask.m_maxWorldHeight) + maxContent.text;
                            }
                            else
                            {
                                maxContent.text += String.Format(" {0:N0}", imageMask.MaxPossibleMeter);
                            }
                        }
                        else
                        {
                            if (imageMask.m_heightMaskType == HeightMaskType.WorldSpace)
                            {
                                maxContent.text += String.Format(" 100%");
                            }
                            else
                            {
                                maxContent.text += String.Format(" {0:N0}%", imageMask.MaxPossiblePercent);
                            }
                        }

                        EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);


                        break;
                    case ImageMaskOperation.SlopeMask:
                        //height masks are always in multiply mode
                        //if (index > 0)
                        //{
                        //    imageMask.m_blendMode = ImageMaskBlendMode.Multiply;
                        //}

                        imageMask.m_slopeMaskCurve = EditorGUI.CurveField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskSlopeMaskCurve"), imageMask.m_slopeMaskCurve);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskSlopeMinMax"), ref imageMask.m_slopeMin, ref imageMask.m_slopeMax, 0.0f, 0.5f);
                        rect.y += EditorGUIUtility.singleLineHeight * 0.5f;
                        Rect slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                        EditorGUI.LabelField(slopeLabelRect, "0°", alignmentStyleLeft);
                        slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth + (rect.width - EditorGUIUtility.labelWidth) * 0.2f, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.6f, EditorGUIUtility.singleLineHeight);
                        EditorGUI.LabelField(slopeLabelRect, "45°", alignmentStyleCenter);
                        slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth + (rect.width - EditorGUIUtility.labelWidth) * 0.8f, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                        EditorGUI.LabelField(slopeLabelRect, "90°", alignmentStyleRight);

                        rect.y += EditorGUIUtility.singleLineHeight;

                        //Label
                        EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskHeightNumerical"));
                        //Min Label
                        numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                        minContent = m_editorUtils.GetContent("MaskHeightNumericMin");
                        EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
                        // Min Entry Field
                        numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                        //slope is stored as scalar value from 0 to 0.5, so we multiply / divide by 180
                        imageMask.m_slopeMin = EditorGUI.FloatField(numFieldRect, (float)Math.Round(imageMask.m_slopeMin * 180f, 2)) / 180f;
                        //Empty Label Field for Spacing
                        numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                        EditorGUI.LabelField(numFieldRect, " ");
                        //MaxEntryField
                        numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                        //slope is stored as scalar value from 0 to 0.5, so we multiply / divide by 180
                        imageMask.m_slopeMax = EditorGUI.FloatField(numFieldRect, (float)Math.Round(imageMask.m_slopeMax * 180f, 2)) / 180f;
                        //Max Label
                        numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                        maxContent = m_editorUtils.GetContent("MaskHeightNumericMax");
                        EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);


                        break;
                    case ImageMaskOperation.NoiseMask:

                        ImageMask.InitNoiseMask(imageMask);

                        imageMask.noiseSettingsGUI.rect = rect;
                        imageMask.m_ShowNoisePreviewTexture = EditorGUI.Foldout(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), imageMask.m_ShowNoisePreviewTexture, m_editorUtils.GetContent("MaskNoisePreviewTexture"));
                        if (imageMask.m_ShowNoisePreviewTexture)
                        {
                            imageMask.noiseSettingsGUI.DrawPreviewTexture(256f, ref imageMask.m_noisePreviewTextureLocked);
                            rect.y += 256f + 24f;
                        }
                        imageMask.noiseSettingsGUI.serializedNoise.Update();
                        //Without function at the moment
                        //imageMask.m_noiseToolSettings.coordSpace = (Gaia.CoordinateSpace)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),m_editorUtils.GetContent("MaskNoiseCoordinateSpace"), imageMask.m_noiseToolSettings.coordSpace);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        //Transform settings
                        imageMask.m_ShowNoiseTransformSettings = EditorGUI.Foldout(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), imageMask.m_ShowNoiseTransformSettings, m_editorUtils.GetContent("MaskNoiseTransformSettings"));
                        if (imageMask.m_ShowNoiseTransformSettings)
                        {
                            EditorGUI.indentLevel++;
                            rect.y += EditorGUIUtility.singleLineHeight;
                            imageMask.noiseSettingsGUI.translation.vector3Value = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskNoiseTranslation"), imageMask.noiseSettingsGUI.translation.vector3Value);
                            rect.y += EditorGUIUtility.singleLineHeight;
                            imageMask.noiseSettingsGUI.rotation.vector3Value = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskNoiseRotation"), imageMask.noiseSettingsGUI.rotation.vector3Value);
                            rect.y += EditorGUIUtility.singleLineHeight;
                            imageMask.noiseSettingsGUI.scale.vector3Value = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskNoiseScale"), imageMask.noiseSettingsGUI.scale.vector3Value);
                            rect.y += EditorGUIUtility.singleLineHeight;
                            imageMask.m_scaleNoiseToTerrainSize = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskNoiseScaleToTerrainSize"), imageMask.m_scaleNoiseToTerrainSize);
                            EditorGUI.indentLevel--;
                        }
                        rect.y += EditorGUIUtility.singleLineHeight;
                        imageMask.m_ShowNoiseTypeSettings = EditorGUI.Foldout(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), imageMask.m_ShowNoiseTypeSettings, m_editorUtils.GetContent("MaskNoiseTypeSettings"));
                        if (imageMask.m_ShowNoiseTypeSettings)
                        {
                            EditorGUI.indentLevel++;
                            rect.y += EditorGUIUtility.singleLineHeight;
                            imageMask.noiseSettingsGUI.noiseTypeName.stringValue = NoiseLib.NoiseTypePopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskNoiseType"), imageMask.noiseSettingsGUI.noiseTypeName.stringValue);
                            rect.y += EditorGUIUtility.singleLineHeight;
                            if (imageMask.noiseSettingsGUI.noiseTypeName.stringValue == "Voronoi")
                            {
                                rect.height = EditorGUIUtility.singleLineHeight * 2;
                                EditorGUI.HelpBox(rect, "A very small fraction of Gaia users reported crashes when using the Voronoi noise type - please make sure to save your data before using this noise type!", MessageType.Warning);
                                rect.y += EditorGUIUtility.singleLineHeight * 2;
                                rect.height = EditorGUIUtility.singleLineHeight;
                            }

                            INoiseType noiseType = NoiseLib.GetNoiseTypeInstance(imageMask.noiseSettingsGUI.noiseTypeName.stringValue);
                            //Currently not really implemented for any noise type
                            //noiseSettingsGUI.noiseTypeParams.stringValue = noiseType?.DoGUI(noiseSettingsGUI.noiseTypeParams.stringValue);

                            imageMask.noiseSettingsGUI.fractalTypeName.stringValue = NoiseLib.FractalTypePopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskNoiseFractalType"), imageMask.noiseSettingsGUI.fractalTypeName.stringValue);

                            rect.y += EditorGUIUtility.singleLineHeight;
                            IFractalType fractalType = NoiseLib.GetFractalTypeInstance(imageMask.noiseSettingsGUI.fractalTypeName.stringValue);
                            imageMask.noiseSettingsGUI.fractalTypeParams.stringValue = fractalType?.DoGUI(rect, imageMask.noiseSettingsGUI.fractalTypeParams.stringValue);
                            EditorGUI.indentLevel--;
                        }

                        imageMask.noiseSettingsGUI.serializedNoise.ApplyModifiedProperties();
                        imageMask.m_gaiaNoiseSettings.m_translation = imageMask.noiseSettingsGUI.translation.vector3Value;
                        imageMask.m_gaiaNoiseSettings.m_rotation = imageMask.noiseSettingsGUI.rotation.vector3Value;
                        imageMask.m_gaiaNoiseSettings.m_scale = imageMask.noiseSettingsGUI.scale.vector3Value;
                        imageMask.m_gaiaNoiseSettings.m_noiseTypeName = imageMask.noiseSettingsGUI.noiseTypeName.stringValue;
                        imageMask.m_gaiaNoiseSettings.m_noiseTypeParams = imageMask.noiseSettingsGUI.noiseTypeParams.stringValue;
                        imageMask.m_gaiaNoiseSettings.m_fractalTypeName = imageMask.noiseSettingsGUI.fractalTypeName.stringValue;
                        imageMask.m_gaiaNoiseSettings.m_fractalTypeParams = imageMask.noiseSettingsGUI.fractalTypeParams.stringValue;
                        //imageMask.m_serializedNoiseObject = imageMask.noiseSettingsGUI.serializedNoise;

                        break;
                    case ImageMaskOperation.CollisionMask:
                        collisionMaskListBeingDrawn = imageMask.m_collisionMasks;
                        CollisionMaskListEditor.DrawMaskList(ref imageMask.m_collisionMaskExpanded, imageMask.m_reorderableCollisionMaskList, m_editorUtils, rect);
                        break;
                    case ImageMaskOperation.PolyMask:
                        PolyMask oldMask = imageMask.m_selectedPolyMaskObject;
                        EditorGUI.BeginChangeCheck();
                        imageMask.m_selectedPolyMaskObject = (PolyMask)EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width - 73, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("MaskPolyMaskObject"), imageMask.m_selectedPolyMaskObject, typeof(PolyMask), true);
                        if (GUI.Button(new Rect(rect.x + rect.width - 70, rect.y, 70, EditorGUIUtility.singleLineHeight), m_editorUtils.GetContent("CreatePolyMask")))
                        {
                            imageMask.m_selectedPolyMaskObject = GaiaUtils.CreatePolyMask();
                            EditorGUIUtility.PingObject(imageMask.m_selectedPolyMaskObject);

                            if (currentTerrain != null)
                            {
                                imageMask.m_selectedPolyMaskObject.transform.position = currentTerrain.transform.position + new Vector3(currentTerrain.terrainData.size.x * 0.5f, 0f, currentTerrain.terrainData.size.z * 0.5f);
                            }
                            else
                            { 
                            
                            }
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (oldMask != null && gaiaSessionManager != null && gaiaSessionManager.m_bakedMaskCache != null)
                            { 
                                gaiaSessionManager.m_bakedMaskCache.OnPolyMaskOutOfScope(oldMask);
                            }
                        }
                        rect.y += EditorGUIUtility.singleLineHeight;
                        if (imageMask.m_selectedPolyMaskObject == null)
                        {
                            GUI.enabled = false;
                        }

                        if (GUI.Button(new Rect(new Vector2(rect.position.x + EditorGUIUtility.labelWidth, rect.position.y), new Vector2(rect.width - EditorGUIUtility.labelWidth, rect.height)), m_editorUtils.GetContent("SelectEditPolyMask")))
                        {
                            EditorGUIUtility.PingObject(imageMask.m_selectedPolyMaskObject);
                            Selection.activeObject = imageMask.m_selectedPolyMaskObject;
                        }
                        //rect.y += EditorGUIUtility.singleLineHeight;
                        //EditorGUI.BeginChangeCheck();
                        //imageMask.m_selectedPolyMaskObject.AlwaysShowVisualization = EditorGUI.Toggle(rect, m_editorUtils.GetContent("ShowPolyMaskNativeVisualization"), imageMask.m_selectedPolyMaskObject.AlwaysShowVisualization);
                        //if (EditorGUI.EndChangeCheck())
                        //{
                        //    imageMask.m_selectedPolyMaskObject.UpdateVisualization();
                        //}
                        GUI.enabled = oldGUIState;

                        break;
                    case ImageMaskOperation.WorldBiomeMask:
                        if (gaiaSessionManager != null && gaiaSessionManager.m_session.m_worldBiomeMaskSettings!=null)
                        {
                            rect.height = EditorGUIUtility.singleLineHeight *3f;
                            EditorGUI.HelpBox(rect, m_editorUtils.GetTextValue("MaskWorldBiomeMaskNotSupported"), MessageType.Error);

                            rect.y += EditorGUIUtility.singleLineHeight * 3f;
                            List<SpawnRule> sessionSpawnRules = gaiaSessionManager.m_session.m_worldBiomeMaskSettings.m_spawnerRules;
                            List<SpawnRule> allBiomeMaskSpawnRules = sessionSpawnRules.Where(x => x.m_resourceType == GaiaConstants.SpawnerResourceType.WorldBiomeMask).ToList();
                            if (allBiomeMaskSpawnRules.Count > 0)
                            {
                                string[] worldBiomeMaskNames = new string[allBiomeMaskSpawnRules.Count];
                                int[] worldBiomeMaskIndices = new int[allBiomeMaskSpawnRules.Count];
                                for (int i = 0; i < allBiomeMaskSpawnRules.Count; i++)
                                {
                                    if (allBiomeMaskSpawnRules[i].m_resourceType == GaiaConstants.SpawnerResourceType.WorldBiomeMask)
                                    {
                                        worldBiomeMaskNames[i] = allBiomeMaskSpawnRules[i].m_name;
                                        worldBiomeMaskIndices[i] = i;
                                    }
                                }
                                int selectedWorldBiomeMaskIndex = 0;
                                if (!String.IsNullOrEmpty(imageMask.m_selectedWorldBiomeMaskGUID))
                                {
                                    selectedWorldBiomeMaskIndex = allBiomeMaskSpawnRules.FindIndex(x => x.GUID == imageMask.m_selectedWorldBiomeMaskGUID);
                                }
                                selectedWorldBiomeMaskIndex = EditorGUI.IntPopup(rect, m_editorUtils.GetTextValue("MaskWorldBiomeMask"), selectedWorldBiomeMaskIndex, worldBiomeMaskNames, worldBiomeMaskIndices);
                                if (selectedWorldBiomeMaskIndex != -1)
                                {
                                    imageMask.m_selectedWorldBiomeMaskGUID = allBiomeMaskSpawnRules[selectedWorldBiomeMaskIndex].GUID;
                                }
                                else
                                {
                                    if (allBiomeMaskSpawnRules.Count > 0)
                                    {
                                        imageMask.m_selectedWorldBiomeMaskGUID = allBiomeMaskSpawnRules[0].GUID;
                                    }
                                }
                            }
                            else
                            {
                                rect.height = EditorGUIUtility.singleLineHeight;
                                EditorGUI.HelpBox(rect, m_editorUtils.GetTextValue("MaskNoWorldBiomeMask"), MessageType.Warning);
                            }
                        }
                        break;
                    case ImageMaskOperation.StrengthTransform:
                        //No extra fields to draw here, all included in the default fields already
                        break;
                    case ImageMaskOperation.PROHydraulicErosion:
#if GAIA_2023_PRO
                        rect.y += EditorGUIUtility.singleLineHeight;
                        imageMask.m_erosionMaskOutput = (GaiaConstants.ErosionMaskOutput)EditorGUI.EnumPopup(rect, m_editorUtils.GetContent("ErosionMaskOutput"), imageMask.m_erosionMaskOutput);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        imageMask.m_erosionSimScale = EditorGUI.Slider(rect, m_editorUtils.GetContent("ErosionSimScale"), imageMask.m_erosionSimScale, 0.0f, 100f);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        imageMask.m_erosionHydroTimeDelta = EditorGUI.Slider(rect, m_editorUtils.GetContent("ErosionHydroTimeDelta"), imageMask.m_erosionHydroTimeDelta, 0.0f, 0.1f);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        imageMask.m_erosionHydroIterations = EditorGUI.IntSlider(rect, m_editorUtils.GetContent("ErosionHydroIterations"), imageMask.m_erosionHydroIterations, 1, 500);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.indentLevel++;
                        imageMask.m_erosionShowAdvancedUI = EditorGUI.Foldout(rect, imageMask.m_erosionShowAdvancedUI, "Advanced");

                        if (imageMask.m_erosionShowAdvancedUI)
                        {
                            //m_ErosionSettings.m_IterationBlendScalar.DrawInspectorGUI();
                            //m_ErosionSettings.m_GravitationalConstant = EditorGUILayout.Slider(ErosionStyles.m_GravitationConstant, m_ErosionSettings.m_GravitationalConstant, 0.0f, -100.0f);
                            rect.y += EditorGUIUtility.singleLineHeight;
                            EditorGUI.indentLevel++;
                            imageMask.m_erosionShowThermalUI = EditorGUI.Foldout(rect, imageMask.m_erosionShowThermalUI, "Thermal Smoothing");
                            if (imageMask.m_erosionShowThermalUI)
                            {
                                //m_ErosionSettings.m_DoThermal = EditorGUILayout.Toggle(ErosionStyles.m_DoThermal, m_ErosionSettings.m_DoThermal);
                                rect.y += EditorGUIUtility.singleLineHeight;
                                imageMask.m_erosionThermalTimeDelta = EditorGUI.Slider(rect, m_editorUtils.GetContent("ErosionThermalTimeDelta"), imageMask.m_erosionThermalTimeDelta, 0, 0.01f);
                                rect.y += EditorGUIUtility.singleLineHeight;
                                imageMask.m_erosionThermalIterations = EditorGUI.IntSlider(rect, m_editorUtils.GetContent("ErosionThermalIterations"), imageMask.m_erosionThermalIterations, 0, 100);
                                rect.y += EditorGUIUtility.singleLineHeight;
                                imageMask.m_erosionThermalReposeAngle = EditorGUI.IntSlider(rect, m_editorUtils.GetContent("ErosionThermalReposeAngle"), imageMask.m_erosionThermalReposeAngle, 0, 90);
                            }
                            rect.y += EditorGUIUtility.singleLineHeight;
                            imageMask.m_erosionShowWaterUI = EditorGUI.Foldout(rect, imageMask.m_erosionShowWaterUI, "Water Transport");
                            if (imageMask.m_erosionShowWaterUI)
                            {
                                //m_ErosionSettings.m_WaterLevelScale = EditorGUILayout.Slider(ErosionStyles.m_WaterLevelScale, m_ErosionSettings.m_WaterLevelScale, 0.0f, 100.0f);
                                rect.y += EditorGUIUtility.singleLineHeight;
                                imageMask.m_erosionPrecipRate = EditorGUI.Slider(rect, m_editorUtils.GetContent("ErosionPrecipRate"), imageMask.m_erosionPrecipRate, 0f, 1f);
                                rect.y += EditorGUIUtility.singleLineHeight;
                                imageMask.m_erosionEvaporationRate = EditorGUI.Slider(rect, m_editorUtils.GetContent("ErosionEvaporationRate"), imageMask.m_erosionEvaporationRate, 0f, 1f);
                                rect.y += EditorGUIUtility.singleLineHeight;
                                imageMask.m_erosionFlowRate = EditorGUI.Slider(rect, m_editorUtils.GetContent("ErosionFlowRate"), imageMask.m_erosionFlowRate, 0f, 1f);
                            }
                            rect.y += EditorGUIUtility.singleLineHeight;
                            imageMask.m_erosionShowSedimentUI = EditorGUI.Foldout(rect, imageMask.m_erosionShowSedimentUI, "Sediment Transport");
                            if (imageMask.m_erosionShowSedimentUI)
                            {
                                //m_ErosionSettings.m_SedimentScale = EditorGUILayout.Slider(ErosionStyles.m_SedimentScale, m_ErosionSettings.m_SedimentScale, 0.0f, 10.0f);
                                rect.y += EditorGUIUtility.singleLineHeight;
                                imageMask.m_erosionSedimentCapacity = EditorGUI.Slider(rect, m_editorUtils.GetContent("ErosionSedimentCapacity"), imageMask.m_erosionSedimentCapacity, 0f, 1f);
                                rect.y += EditorGUIUtility.singleLineHeight;
                                imageMask.m_erosionSedimentDepositRate = EditorGUI.Slider(rect, m_editorUtils.GetContent("ErosionSedimentDepositRate"), imageMask.m_erosionSedimentDepositRate, 0f, 1f);
                                rect.y += EditorGUIUtility.singleLineHeight;
                                imageMask.m_erosionSedimentDissolveRate = EditorGUI.Slider(rect, m_editorUtils.GetContent("ErosionSedimentDissolveRate"), imageMask.m_erosionSedimentDissolveRate, 0f, 1f);
                            }
                            rect.y += EditorGUIUtility.singleLineHeight;
                            imageMask.m_erosionShowRiverBankUI = EditorGUI.Foldout(rect, imageMask.m_erosionShowRiverBankUI, "Riverbank");
                            if (imageMask.m_erosionShowRiverBankUI)
                            {
                                rect.y += EditorGUIUtility.singleLineHeight;
                                imageMask.m_erosionRiverBankDepositRate = EditorGUI.Slider(rect, m_editorUtils.GetContent("ErosionRiverBankDepositRate"), imageMask.m_erosionRiverBankDepositRate, 0f, 10f);
                                rect.y += EditorGUIUtility.singleLineHeight;
                                imageMask.m_erosionRiverBankDissolveRate = EditorGUI.Slider(rect, m_editorUtils.GetContent("ErosionRiverBankDissolveRate"), imageMask.m_erosionRiverBankDissolveRate, 0f, 10f);
                                rect.y += EditorGUIUtility.singleLineHeight;
                                imageMask.m_erosionRiverBedDepositRate = EditorGUI.Slider(rect, m_editorUtils.GetContent("ErosionRiverBedDepositRate"), imageMask.m_erosionRiverBedDepositRate, 0f, 10f);
                                rect.y += EditorGUIUtility.singleLineHeight;
                                imageMask.m_erosionRiverBedDissolveRate = EditorGUI.Slider(rect, m_editorUtils.GetContent("ErosionRiverBedDissolveRate"), imageMask.m_erosionRiverBedDissolveRate, 0f, 10f);
                            }
                        }
#endif
                        break;
                    case ImageMaskOperation.Smooth:
                        rect.height = EditorGUIUtility.singleLineHeight;
                        imageMask.m_smoothVerticality = EditorGUI.Slider(rect, m_editorUtils.GetContent("MaskSmoothVerticality"), imageMask.m_smoothVerticality, -1, 1f);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        imageMask.m_smoothBlurRadius = EditorGUI.Slider(rect, m_editorUtils.GetContent("MaskSmoothBlurRadius"), imageMask.m_smoothBlurRadius, 0, 30f);
                        break;
                    case ImageMaskOperation.TerrainTexture:
                        rect.height = EditorGUIUtility.singleLineHeight;
                       
                        if (ImageMask.m_allTextureSpawnRules.Count() > 0)
                        {
                            int selectedGUIDIndex =-99;
                            //if there was never a texture selected, initialize with the first available GUID
                            if (String.IsNullOrEmpty(imageMask.m_textureMaskSpawnRuleGUID) && ImageMask.m_allTextureSpawnRules.Count()>0)
                            {
                                imageMask.m_textureMaskSpawnRuleGUID = ImageMask.m_allTextureSpawnRules[0].GUID;
                            }

                            SpawnRule selectedRule = ImageMask.m_allTextureSpawnRules.FirstOrDefault(x => x.GUID == imageMask.m_textureMaskSpawnRuleGUID);
                            if (selectedRule != null)
                            {
                                selectedGUIDIndex = Array.IndexOf(ImageMask.m_allTextureSpawnRules, selectedRule);
                            }
                            if (selectedGUIDIndex == -99)
                            {
                                rect.height = EditorGUIUtility.singleLineHeight;
                                EditorGUI.HelpBox(rect, m_editorUtils.GetTextValue("MaskTextureGUIDError"), MessageType.Warning);
                                rect.y += EditorGUIUtility.singleLineHeight;
                                rect.height = EditorGUIUtility.singleLineHeight;
                                if (GUI.Button(rect, m_editorUtils.GetContent("MaskTextureResetButton")))
                                {
                                    if(ImageMask.m_allTextureSpawnRules.Count() > 0)
                                    {
                                        imageMask.m_textureMaskSpawnRuleGUID = ImageMask.m_allTextureSpawnRules[0].GUID;
                                    }
                                    else
                                    {
                                        imageMask.m_textureMaskSpawnRuleGUID = "";
                                    }
                                    imageMask.m_hasErrors = false;
                                }
                            }
                            else
                            {
                                selectedGUIDIndex = EditorGUI.IntPopup(rect, m_editorUtils.GetContent("MaskTextureLayer").text, selectedGUIDIndex, ImageMask.m_allTextureSpawnRuleNames, ImageMask.m_allTextureSpawnRuleIndices);
                                if (selectedGUIDIndex >= 0 && selectedGUIDIndex < ImageMask.m_allTextureSpawnRules.Length)
                                {
                                    imageMask.m_textureMaskSpawnRuleGUID = ImageMask.m_allTextureSpawnRules[selectedGUIDIndex].GUID;
                                }
                            }


                        }
                        else
                        {
                            EditorGUI.HelpBox(rect, "No texture spawn rules yet! Add at least one texture spawn rule to a spawner to use texture masks!", MessageType.Warning);
                            rect.y += EditorGUIUtility.singleLineHeight;
                        }
                        break;
                    case ImageMaskOperation.PROConcaveConvex:
#if GAIA_2023_PRO
                        rect.height = EditorGUIUtility.singleLineHeight;
                        imageMask.m_concavity = EditorGUI.Slider(rect, m_editorUtils.GetContent("MaskConcaveConvexConcavity"), imageMask.m_concavity, -1f, 1f);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        imageMask.m_concavityFeatureSize = EditorGUI.Slider(rect, m_editorUtils.GetContent("MaskConcaveConvexFeatureSize"), imageMask.m_concavityFeatureSize, 1f, 50f);
#endif
                        break;
                    case ImageMaskOperation.GrowAndShrink:
                        imageMask.m_growAndShrinkValue = EditorGUI.Slider(rect, m_editorUtils.GetContent("MaskGrowAndShrinkValue"), imageMask.m_growAndShrinkValue, -50f, 50f);
                        break;
                    case ImageMaskOperation.GlobalSpawnerMaskStack:
                        //This mask has no controls since it will only re-route the output of the global spawner mask stack into a spawn rule mask stack.
                        //The only location where this mask has a function in is in the spawn rules - on all other locations we need to display a warning message
                        if (imageMask.m_imageMaskLocation != ImageMaskLocation.SpawnRule)
                        {
                            rect.height = EditorGUIUtility.singleLineHeight * 4;
                            EditorGUI.HelpBox(rect, "This mask will have no effect here. This mask type will route the output of the global spawn mask stack into a spawn rule so it can further be refined in there. It does not serve any purpose if it is not used within the mask stack of a spawn rule.", MessageType.Warning);
                            rect.y += EditorGUIUtility.singleLineHeight * 4;
                            rect.height = EditorGUIUtility.singleLineHeight;
                        }
                        break;
                    case ImageMaskOperation.PROCurvature:
#if GAIA_2023_PRO
                        rect.y += EditorGUIUtility.singleLineHeight;
                        imageMask.m_curvatureBlurAmount = EditorGUI.Slider(rect, m_editorUtils.GetContent("CurvatureAmount"), imageMask.m_curvatureBlurAmount, 0.0005f, 0.1f);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        imageMask.m_curvatureWorldUnits = EditorGUI.Slider(rect, m_editorUtils.GetContent("CurvatureConcavity"), imageMask.m_curvatureWorldUnits, -1, 1);
                        rect.y += EditorGUIUtility.singleLineHeight;
                        imageMask.m_curvatureIntensity = EditorGUI.Slider(rect, m_editorUtils.GetContent("CurvatureIntensity"), imageMask.m_curvatureIntensity, 0.01f, 1f);
#endif
                        break;
                    default:
                        break;
                }
            }

            EditorGUI.indentLevel = oldIndent;
            return MaskListButtonCommand.None;
        }

        private static void DrawOffsetFields(ImageMask imageMask, Rect rect, EditorUtils editorUtils, Terrain currentTerrain)
        {
            //if (imageMask.m_operation == ImageMaskOperation.ImageMask)
            //{
                rect.y += EditorGUIUtility.singleLineHeight;
                Rect flagsRect = new Rect(rect);
                EditorGUI.LabelField(flagsRect, editorUtils.GetContent("MaskSpace"));
                flagsRect.x += EditorGUIUtility.labelWidth;
                flagsRect.width = (rect.width - EditorGUIUtility.labelWidth) * 0.48f;
                EditorGUIUtility.labelWidth = 50;
                imageMask.m_imageMaskSpace = (ImageMaskSpace)EditorGUI.EnumPopup(flagsRect, imageMask.m_imageMaskSpace);
                flagsRect.x += flagsRect.width + 10;
                imageMask.m_tiling = EditorGUI.Toggle(flagsRect, editorUtils.GetContent("MaskTiling"), imageMask.m_tiling);
                EditorGUIUtility.labelWidth = 0;
            //}

            rect.y += EditorGUIUtility.singleLineHeight;
            Rect offsetRect = new Rect(rect);
            EditorGUI.LabelField(offsetRect, editorUtils.GetContent("MaskOffset"));
            offsetRect.x += EditorGUIUtility.labelWidth;
            offsetRect.width = (rect.width - EditorGUIUtility.labelWidth) * 0.48f;
            EditorGUIUtility.labelWidth = 10;
            imageMask.m_xOffSet = EditorGUI.FloatField(offsetRect, editorUtils.GetContent("MaskXOffset"), imageMask.m_xOffSet);
            offsetRect.x += offsetRect.width + 10;
            imageMask.m_zOffSet = EditorGUI.FloatField(offsetRect, editorUtils.GetContent("MaskZOffset"), imageMask.m_zOffSet);
            EditorGUIUtility.labelWidth = 0;
            //if (imageMask.m_operation == ImageMaskOperation.ImageMask)
            //{
                rect.y += EditorGUIUtility.singleLineHeight;
                Rect scaleRect = new Rect(rect);
                EditorGUI.LabelField(scaleRect, editorUtils.GetContent("MaskScale"));
                scaleRect.x += EditorGUIUtility.labelWidth;
                scaleRect.width = (rect.width - EditorGUIUtility.labelWidth) * 0.48f;
                EditorGUIUtility.labelWidth = 10;
                imageMask.m_xScale = EditorGUI.FloatField(scaleRect, editorUtils.GetContent("MaskXScale"), imageMask.m_xScale);
                scaleRect.x += offsetRect.width + 10;
                imageMask.m_zScale = EditorGUI.FloatField(scaleRect, editorUtils.GetContent("MaskZScale"), imageMask.m_zScale);
                EditorGUIUtility.labelWidth = 0;
                rect.y += EditorGUIUtility.singleLineHeight;
                Rect rotateRect = new Rect(rect);
                EditorGUI.LabelField(rotateRect, editorUtils.GetContent("MaskRotation"));
                rotateRect.x += EditorGUIUtility.labelWidth;
                rotateRect.width = (rect.width - EditorGUIUtility.labelWidth);
                EditorGUIUtility.labelWidth = 50;
                imageMask.m_rotationDegrees = EditorGUI.Slider(rotateRect, imageMask.m_rotationDegrees, -180f, 180f);
                EditorGUIUtility.labelWidth = 0;
            //}
        }

        public static void OpenStampBrowser(ImageMask imageMask=null)
        {
            var window = EditorWindow.GetWindow<GaiaStampSelectorEditorWindow>(false);
            if (imageMask != null)
            {
                window.minSize = new Vector2(270, 410);
            }
            else
            { 
                window.minSize = new Vector2(270, 500);
            }
            window.position = new Rect(new Vector2(Screen.currentResolution.width / 2f - window.minSize.x / 2f, Screen.currentResolution.height / 2f - window.minSize.y / 2f), window.minSize);
            window.Init(imageMask);
            window.Show();
        }

        public static GUIContent PropertyCount(string key, Array array, EditorUtils editorUtils)
        {
            GUIContent content = editorUtils.GetContent(key);
            content.text += " [" + array.Length + "]";
            return content;
        }


    }
}