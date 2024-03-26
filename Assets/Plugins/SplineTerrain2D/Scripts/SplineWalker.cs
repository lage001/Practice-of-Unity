using UnityEngine;

namespace Kolibri2d
{
	public class SplineWalker : MonoBehaviour
	{

		public BezierSpline spline;

		public float duration;
		public bool lookForward;
		[Tooltip("Length allows constant speed for the interpolation.\n  note: it is an aproximated value and its precision depends on the amount of subdivisions")]
		public bool useLength;
		//Vector3 startRotation = Vector3.zero;
		Quaternion startRotation;

		public enum SplineWalkerMode
		{
			Once,
			Loop,
			PingPong
		}

		void OnEnable()
		{
			progress = 0.0f;
			if (spline != null)
			{
				spline.RefreshIfMissingData();
			}
			startRotation = Quaternion.Euler(transform.eulerAngles);	
		}

		public SplineWalkerMode mode;

		bool goingForward = true;
		float progress;

		private void Update()
		{
			if (spline != null)
			{
				if (goingForward)
				{
					progress += Time.deltaTime / duration;
					if (progress > 1f)
					{
						switch (mode)
						{
							default:
							case SplineWalkerMode.Once:
								progress = 1f;
								break;
							case SplineWalkerMode.Loop:
								progress -= 1f;
								break;
							case SplineWalkerMode.PingPong:
								progress = 2f - progress;
								goingForward = false;
								break;
						}
					}
				}
				else
				{
					progress -= Time.deltaTime / duration;
					if (progress < 0f)
					{
						progress = -progress;
						goingForward = true;
					}
				}

				if (useLength)
				{
					transform.localPosition = spline.transform.TransformPoint(spline.GetPositionFromLength01(progress));

					if (lookForward)
					{
						var dir = spline.GetDirectionFromLength01(progress);
						float angle = 90 - (Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg);
						transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
					}
					else
					{
						transform.rotation = startRotation;
					}
				}
				else
				{
					transform.localPosition = spline.transform.TransformPoint(spline.GetPositionAtT(progress));

					if (lookForward)
					{
						var dir = spline.GetDirectionAtT(progress);
						float angle = 90 - (Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg);
						transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
					}
					else
					{
						transform.rotation = startRotation;
					}
				}
			}
		}
	}
}
