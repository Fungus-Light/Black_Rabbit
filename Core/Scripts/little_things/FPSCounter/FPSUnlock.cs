using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Black_Rabbit
{
	public class FPSUnlock : MonoBehaviour 
	{
		public int TargetFPS;

		protected virtual void Start()
		{
			Application.targetFrameRate = TargetFPS;
		}		
	}
}
