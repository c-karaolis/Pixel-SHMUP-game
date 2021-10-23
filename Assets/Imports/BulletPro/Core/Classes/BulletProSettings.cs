using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is part of the BulletPro package for Unity.
// Author : Simon Albou <albou.simon@gmail.com>

namespace BulletPro
{
	// There's one instance of this class in Resources, it fills the same role as Unity's TagManager.asset, but for BulletPro.
	[System.Serializable]
	public class BulletProSettings : ScriptableObject
	{
		public string this[int idx]
		{
			get	{ return collisionTags[idx]; }
			set { collisionTags[idx] = value; }
		}

		static BulletProSettings _instance;
		public static BulletProSettings instance
		{
			get
			{
				if (_instance != null) return _instance;
				else
				{
					BulletProSettings bps = Resources.Load("BulletProSettings") as BulletProSettings;
					_instance = bps;
					return _instance;
				}
			}
		}

		public CollisionTagLabels collisionTags;

		[Tooltip("If specified, all the Emitter Profiles you create will look like this one.")]
		public EmitterProfile defaultEmitterProfile;
		public Color defaultBulletGizmoColor = Color.yellow;
		public Color defaultEmitterGizmoColor = Color.cyan;
		public Color defaultReceiverGizmoColor = Color.magenta;

		[Tooltip("A reference to the Compute Shader in charge of collisions.")]
		public ComputeShader collisionHandler;
		public ComputeShadersEnabling computeShaders;
		[Tooltip("How many collisions can occur at once in the same single frame?")]
		public uint maxAmountOfCollisionsPerFrame = 32;
		[Tooltip("How many bullets can be processed at once in the same single frame?")]
		public int maxAmountOfBullets = 2000;
		[Tooltip("How many receivers can be processed at once in the same single frame?")]
		public int maxAmountOfReceivers = 200;

		public static int buildNumber = 14;
	}

	[System.Serializable]
	public enum ComputeShadersEnabling { EnabledWhenPossible, AlwaysOn, AlwaysOff }
}