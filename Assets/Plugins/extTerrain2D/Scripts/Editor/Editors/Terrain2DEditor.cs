﻿/* Copyright (c) 2021 ExT (V.Sigalkin) */

using UnityEngine;

using UnityEditor;
using UnityEngine.UIElements;

using System.Collections.Generic;

namespace extTerrain2D.Editor.Editors
{
	[CustomEditor(typeof(Terrain2D))]
	[CanEditMultipleObjects]
	public class Terrain2DEditor : UnityEditor.Editor
	{
		#region Static Private Vars

		private static readonly GUIContent _widthContent = new GUIContent("Width:");

		private static readonly GUIContent _heightContent = new GUIContent("Height:");

		private static readonly GUIContent _lineWifthContent = new GUIContent("Line Width:");

		private static readonly GUIContent _qualityContent = new GUIContent("Quality:");

		private static readonly GUIContent _groundUVOffsetContent = new GUIContent("UV Offset:");

		private static readonly GUIContent _groundUVScaleContent = new GUIContent("UV Scale:");

		private static readonly GUIContent _positionContent = new GUIContent("Local Position:");

		private static readonly GUIContent _handlesTypeContent = new GUIContent("Handles Type:");

		private static readonly GUIContent _colliderTypeContent = new GUIContent("Collider Type:");
		
		#endregion

		#region Private Vars

		private Terrain2D _terrain;

		private MeshRenderer _meshRenderer;

		private int _selectedIndex = -1;

		private bool _shiftKey;

		private Dictionary<int, int> _indexIds = new Dictionary<int, int>();

		private SerializedProperty _keyPointsProperty;

		private SerializedProperty _widthProperty;

		private SerializedProperty _heightProperty;

		private SerializedProperty _lineWidthProperty;

		private SerializedProperty _qualityProperty;

		private SerializedProperty _colliderTypeProperty;
		
		private SerializedProperty _groundUVOffsetProperty;

		private SerializedProperty _groundUVScaleProperty;

		#endregion

		#region Unity Methods

		protected void OnEnable()
		{
			_terrain = target as Terrain2D;
			_meshRenderer = _terrain.GetComponent<MeshRenderer>();

			_keyPointsProperty = serializedObject.FindProperty("_keyPoints");
			_widthProperty = serializedObject.FindProperty("_width");
			_heightProperty = serializedObject.FindProperty("_height");
			_lineWidthProperty = serializedObject.FindProperty("_lineWidth");
			_qualityProperty = serializedObject.FindProperty("_quality");
			_groundUVOffsetProperty = serializedObject.FindProperty("_groundUVOffset");
			_groundUVScaleProperty = serializedObject.FindProperty("_groundUVScale");
			_colliderTypeProperty = serializedObject.FindProperty("_colliderType");
		}

		public override void OnInspectorGUI()
		{
			var defaultColor = GUI.color;

			serializedObject.Update();

			// LOGO
			GUILayout.Space(10);
			EditorUtils.DrawLogo();
			GUILayout.Space(5);

			EditorGUI.BeginChangeCheck();

			// SETTINGS BLOCK
			EditorGUILayout.LabelField("Terrain 2D", EditorStyles.boldLabel);
			GUILayout.BeginVertical("box");

			// TERRAIN SETTINGS BOX
			EditorGUILayout.LabelField("Terrain Settings:", EditorStyles.boldLabel);
			GUILayout.BeginVertical("box");

			// WIDTH
			EditorGUILayout.PropertyField(_widthProperty, _widthContent);

			// HEIGHT 
			EditorGUILayout.PropertyField(_heightProperty, _heightContent);

			// LINE WIDTH
			EditorGUILayout.PropertyField(_lineWidthProperty, _lineWifthContent);

			// QUALITY
			EditorGUILayout.PropertyField(_qualityProperty, _qualityContent);

			// Collider Type
			EditorGUILayout.PropertyField(_colliderTypeProperty, _colliderTypeContent);
			

			// TERRAIN SETTINGS BOX END
			EditorGUILayout.EndVertical();

			// UV SETTINGS BOX
			EditorGUILayout.LabelField("Terrain UV Settings:", EditorStyles.boldLabel);
			GUILayout.BeginVertical("box");

			// WIDTH
			EditorGUILayout.PropertyField(_groundUVOffsetProperty, _groundUVOffsetContent);

			// HEIGHT 
			EditorGUILayout.PropertyField(_groundUVScaleProperty, _groundUVScaleContent);

			// UV SETTINGS BOX END
			EditorGUILayout.EndVertical();

			if (targets.Length == 1)
			{
				EditorGUILayout.LabelField("KeyPoints:", EditorStyles.boldLabel);
				GUILayout.BeginVertical("box");

				GUILayout.BeginHorizontal("box");
				var buttonSettings = new GUILayoutOption[] { GUILayout.Width(25f), GUILayout.Height(25f) };
				var labelSettings = new GUILayoutOption[] { GUILayout.Height(23f) };

				var prevButton = GUILayout.Button("<", buttonSettings);

				GUILayout.BeginHorizontal("box", labelSettings);
				if (_selectedIndex >= 0)
				{
					GUILayout.Label(string.Format("{0}/{1}", _selectedIndex + 1, _terrain.GetKeyPointsCount()),
						CustomEditorStyles.CenterLabel);
				}
				else
				{
					GUILayout.Label("Select KeyPoint in SceneView.", CustomEditorStyles.CenterLabel);
				}
				GUILayout.EndHorizontal();

				var nextButton = GUILayout.Button(">", buttonSettings);
				GUILayout.EndVertical();

				if (prevButton || nextButton) SwitchId(prevButton, nextButton);

				// KEYPOINT SETTINGS BOX
				EditorGUILayout.LabelField("Settings:", EditorStyles.boldLabel);
				GUILayout.BeginVertical("box");

				if (_selectedIndex < 0)
				{
					var height = EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing;

					GUILayout.Space(height / 2f);
					EditorGUILayout.LabelField("- none -", CustomEditorStyles.CenterLabel);
					GUILayout.Space(height / 2f);
				}
				else
				{
					DrawKeyPointSettings(_selectedIndex);
				}

				// KEYPOINT SETTINGS BOX END
				EditorGUILayout.EndVertical();

				GUILayout.EndVertical();
			}

			// SETTINGS BLOCK END
			EditorGUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		protected void OnSceneGUI()
		{
			_shiftKey = Event.current.modifiers == EventModifiers.Shift;

			var pointsCount = _terrain.GetKeyPointsCount();

			for (var index = 0; index < pointsCount; index++)
			{
				var keypoint = _terrain.GetKeyPoint(index);
				var terrainPosition = _terrain.transform.position;
				var terrainScale = _terrain.transform.localScale;

				var keypointPosition = _terrain.KeyPointToWorld(keypoint.Position);
				var rightHandlePosition = _terrain.KeyPointToWorld(keypoint.Position + keypoint.RightHandlePosition);
				var leftHandlePosition = _terrain.KeyPointToWorld(keypoint.Position + keypoint.LeftHandlePosition);

				Handles.color = Color.yellow;

				if (index < pointsCount - 1)
				{
					Handles.DrawLine(keypointPosition, rightHandlePosition);
				}
				if (index > 0)
				{
					Handles.DrawLine(keypointPosition, leftHandlePosition);
				}

				DrawKeyPointMain(index, keypoint, keypointPosition, pointsCount);

				if (index < pointsCount - 1)
				{
					DrawKeyPointRightHandle(index, keypoint, rightHandlePosition);
				}

				if (index > 0)
				{
					DrawKeyPointLeftHandle(index, keypoint, leftHandlePosition);
				}

				if (Event.current.type == EventType.Used)
				{
					_terrain.Rebuild();

					var compound = _terrain.GetComponentInParent<CompoundTerrain2D>();
					if (compound != null) compound.Rebuild(_terrain);
				}
			}

			if (_indexIds.ContainsKey(GUIUtility.hotControl))
			{
				if (_indexIds[GUIUtility.hotControl] != _selectedIndex)
				{
					_selectedIndex = _indexIds[GUIUtility.hotControl];
					Repaint();
				}
			}

			if (Event.current.type == EventType.MouseDown && Event.current.button == (int)MouseButton.LeftMouse && Event.current.clickCount == 2)
			{
				if (InMesh())
				{
					var worldPosition = GetMousePosition();
					var index = GetNearestKeyPointIndex(worldPosition);
					if (index >= 0)
					{
						CreateKeyPoint(index, worldPosition);

						Undo.RecordObject(target, "Add KeyPoint: " + index);

						_terrain.Rebuild();

						Event.current.Use();
					}
				}
			}
			if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Backspace || Event.current.keyCode == KeyCode.Delete))
			{

				if (_selectedIndex > 0 &&
					_selectedIndex < pointsCount - 1)
				{
					_keyPointsProperty.DeleteArrayElementAtIndex(_selectedIndex);
					serializedObject.ApplyModifiedProperties();

					_terrain.Rebuild();

					_selectedIndex = 0;

					Undo.RecordObject(target, "Remove KeyPoint: " + _selectedIndex);
					Event.current.Use();

					_indexIds.Clear();
				}
			}
		}

		#endregion

		#region Private Methods

		private void SwitchId(bool prevButton, bool nextButton)
		{
			var id = _selectedIndex;
			if (nextButton)
			{
				id++;
			}

			if (prevButton)
			{
				id--;
			}

			if (_selectedIndex != id)
			{
				var keyPointsCount = _terrain.GetKeyPointsCount();

				if (id < 0) id = keyPointsCount - 1;
				if (id >= keyPointsCount) id = 0;

				_selectedIndex = id;
			}
		}

		private void DrawKeyPointSettings(int index)
		{
			EditorGUILayout.LabelField("Selected ID: " + _selectedIndex , EditorStyles.boldLabel);

			var keypointProperty = _keyPointsProperty.GetArrayElementAtIndex(index);
			var positionProperty = keypointProperty.FindPropertyRelative("_position");
			var handlesTypeProperty = keypointProperty.FindPropertyRelative("_handlesType");
			

			// POSITION
			EditorGUILayout.PropertyField(positionProperty, _positionContent);

			// HANDLES 
			EditorGUILayout.PropertyField(handlesTypeProperty, _handlesTypeContent);
			
			var buttonSettings = new GUILayoutOption[] { GUILayout.Width(200f), GUILayout.Height(25f) };
			
			var removeKeyPointButton = GUILayout.Button("RemoveKeyPoint", buttonSettings);
			
			if(removeKeyPointButton) _terrain.RemoveKeyPoint(index);
		}

		private void DrawKeyPointMain(int index, KeyPoint keypoint, Vector3 keypointPosition, int pointsCount)
		{
			EditorGUI.BeginChangeCheck();

			Handles.color = _selectedIndex == index ? Color.yellow : Color.white;
			var size = HandleUtility.GetHandleSize(keypointPosition) * 0.2f;
			var position = (Vector2)Handles.FreeMoveHandle(keypointPosition, Quaternion.identity, size, Vector2.zero, (cID, p, r, s, e) =>
			{
				if (!_indexIds.ContainsKey(cID))
					_indexIds.Add(cID, index);

				EditorUtils.KeyPointHandleCap(cID, p, r, s, e);
			});

			if (EditorGUI.EndChangeCheck())
			{
				if (index == 0 || index >= pointsCount - 1)
					position.x = keypointPosition.x;

				var localPosition = _terrain.WorldToKeyPoint(position);
				if (_shiftKey) localPosition = SnapVector(localPosition);

				keypoint.Position = localPosition;

				_selectedIndex = index;

				EditorUtility.SetDirty(target);
				Undo.RegisterCompleteObjectUndo(target, "Move KeyPoint");
			}
		}

		private void DrawKeyPointRightHandle(int index, KeyPoint keypoint, Vector3 rightHandlePosition)
		{
			EditorGUI.BeginChangeCheck();

			Handles.color = Color.white;
			var size = HandleUtility.GetHandleSize(rightHandlePosition) * 0.15f;
			var position = (Vector2)Handles.FreeMoveHandle(rightHandlePosition, Quaternion.identity, size, Vector2.zero, (cID, p, r, s, e) =>
			{
				if (!_indexIds.ContainsKey(cID))
					_indexIds.Add(cID, index);

				EditorUtils.KeyPointHandleHandleCap(cID, p, r, s, e);
			});

			if (EditorGUI.EndChangeCheck())
			{
				position = _terrain.WorldToKeyPoint(position);
				position = position - keypoint.Position;

				if (_shiftKey) position = SnapVector(position);

				keypoint.SetRightHandle(position);

				EditorUtility.SetDirty(target);
				Undo.RegisterCompleteObjectUndo(target, "Move Right KeyPoint Handle: " + index);
			}
		}

		private void DrawKeyPointLeftHandle(int index, KeyPoint keypoint, Vector3 leftHandlePosition)
		{
			EditorGUI.BeginChangeCheck();

			Handles.color = Color.white;
			var size = HandleUtility.GetHandleSize(leftHandlePosition) * 0.15f;
			var position = (Vector2)Handles.FreeMoveHandle(leftHandlePosition, Quaternion.identity, size, Vector2.zero, (cID, p, r, s, e) =>
			{
				if (!_indexIds.ContainsKey(cID))
					_indexIds.Add(cID, index);

				EditorUtils.KeyPointHandleHandleCap(cID, p, r, s, e);
			});

			if (EditorGUI.EndChangeCheck())
			{
				position = _terrain.WorldToKeyPoint(position);
				position = position - keypoint.Position;

				if (_shiftKey) position = SnapVector(position);

				keypoint.SetLeftHandle(position);

				EditorUtility.SetDirty(target);
				Undo.RegisterCompleteObjectUndo(target, "Move Left KeyPoint Hadle: " + index);
			}
		}

		private Vector3 GetMousePosition()
		{
			var sceneView = SceneView.currentDrawingSceneView;
			return HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
		}

		private bool InMesh()
		{
			var mousePosition = GetMousePosition();
			mousePosition.z = _meshRenderer.transform.position.z;

			return _meshRenderer.bounds.Contains(mousePosition);
		}

		private int GetNearestKeyPointIndex(Vector2 worldPosition)
		{
			var keypointsCount = _terrain.GetKeyPointsCount();
			var distance = float.MaxValue;
			var index = -1;

			for (var i = 0; i < keypointsCount; i++)
			{
				var keypoint = _terrain.GetKeyPoint(i);
				var keypointPosition = _terrain.KeyPointToWorld(keypoint.Position);

				if (Vector2.Distance(worldPosition, keypointPosition) < distance)
				{
					distance = Vector2.Distance(worldPosition, keypointPosition);
					index = i;
				}
			}

			return index;
		}

		private void CreateKeyPoint(int index, Vector2 worldPosition)
		{
			var keypoint = _terrain.GetKeyPoint(index);
			var keypointPosition = _terrain.KeyPointToWorld(keypoint.Position);

			var localPosition = _terrain.WorldToKeyPoint(worldPosition);
			localPosition.x = Mathf.Clamp(localPosition.x, 0, 1f);

			var position = _terrain.GetPosition(localPosition.x / 1f);

			if (keypointPosition.x < worldPosition.x) index++;

			_keyPointsProperty.InsertArrayElementAtIndex(index);

			var newKeypointProperty = _keyPointsProperty.GetArrayElementAtIndex(index);

			var positionProperty = newKeypointProperty.FindPropertyRelative("_position");
			positionProperty.vector2Value = position;

			var leftHandleProperty = newKeypointProperty.FindPropertyRelative("_leftHandlePosition");
			leftHandleProperty.vector2Value = new Vector2(-0.1f, 0f);

			var rightHandleProperty = newKeypointProperty.FindPropertyRelative("_rightHandlePosition");
			rightHandleProperty.vector2Value = new Vector2(0.1f, 0f);

			serializedObject.ApplyModifiedProperties();
		}

		private Vector2 SnapVector(Vector2 vector)
		{
			var yD = 10f;
			var xD = 100f;

			vector.x = Mathf.Round(vector.x * xD) / xD;
			vector.y = Mathf.Round(vector.y * yD) / yD;

			return vector;
		}

		#endregion
	}
}