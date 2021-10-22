using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is part of the BulletPro package for Unity.
// But it's only used in the example scene and I recommend writing a better one that fits your needs.
// Author : Simon Albou <albou.simon@gmail.com>

namespace BulletPro.DemoScripts
{
	public class BPDemo_TargetFramerateSetter : MonoBehaviour
	{
		public int targetFramerate = 60;

		void Awake ()
		{
			Application.targetFrameRate = targetFramerate;	
		}
	}
}
