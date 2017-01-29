using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanqueSons : MonoBehaviour 
{
	public static FMOD.Studio.EventInstance Ambiance; 

	public static FMOD.Studio.EventInstance Walk; 
	public static FMOD.Studio.EventInstance Catch; 
	public static FMOD.Studio.EventInstance Throw; 
	public static FMOD.Studio.EventInstance Jump; 
	public static FMOD.Studio.EventInstance Land; 
	public static FMOD.Studio.EventInstance Signal; 

	public static FMOD.Studio.EventInstance Freeze; 
	public static FMOD.Studio.EventInstance Impact; 


	void Start () 
	{
		Ambiance = FMODUnity.RuntimeManager.CreateInstance ("event:/Ambiance/Ambiance");

		Walk = FMODUnity.RuntimeManager.CreateInstance ("event:/Avatar/Walk");
		Catch = FMODUnity.RuntimeManager.CreateInstance ("event:/Avatar/Catch");
		Throw = FMODUnity.RuntimeManager.CreateInstance ("event:/Avatar/Throw");
		Jump = FMODUnity.RuntimeManager.CreateInstance ("event:/Avatar/Jump");
		Land = FMODUnity.RuntimeManager.CreateInstance ("event:/Avatar/Land");
		Signal = FMODUnity.RuntimeManager.CreateInstance ("event:/Avatar/Signal");

		Freeze = FMODUnity.RuntimeManager.CreateInstance ("event:/Blocs/Freeze");
		Impact = FMODUnity.RuntimeManager.CreateInstance ("event:/Blocs/Impact");


		Ambiance.start ();

	}
}
