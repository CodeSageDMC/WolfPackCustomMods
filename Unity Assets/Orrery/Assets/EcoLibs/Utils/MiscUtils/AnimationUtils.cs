// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class AnimMap
{
	public string anim;
	public float speed;
	public bool destroyOnStop;
}

public class AnimationUtils : TrackableBehavior
{
	public bool playRadomAnim;

	public List<AnimMap> animMap; 

	private Animation animationComponent;

	void Start () 
	{
		//initialize:
		animationComponent = gameObject.GetComponent<Animation> ();

		//set animation stuff:
		if (animationComponent != null)
		{
			for (int i = 0; i < animMap.Count; i ++)
			{
				if (animMap[i].speed != 0)
					SetAnimStateSpeed(animationComponent, i);
			}

			if (playRadomAnim)
			{
				PlayRandomAnim ();
			}
		}
	}

	void Update()
	{
		if (animationComponent != null)
		{
			DestroyOnAnimationStop(animationComponent);
		}
	}

	public void SetAnimStateSpeed(Animation animComp, int speedValue)
	{
		animComp[animMap[speedValue].anim].speed = animMap[speedValue].speed;
	}

	public void DestroyOnAnimationStop(Animation animComp)
	{
		int counter = 0;

		for (int i = 0; i < animMap.Count; i ++)
		{
			if ( !animComp.IsPlaying(animMap[i].anim) )
			{
				counter++;
			}
		}

		if (counter == animMap.Count)
		{
			Destroy(this.gameObject);
		}
	}

	public void PlayRandomAnim()
	{
		if (animMap.Count < 2)
			return;

		gameObject.GetComponent<Animation>().Play(animMap[Random.Range(0, animMap.Count)].anim);
	}
}
