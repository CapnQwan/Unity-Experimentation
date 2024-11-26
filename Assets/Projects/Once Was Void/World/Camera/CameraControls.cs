using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{

	Transform swivel, stick;

	float zoom = 1f;

	public float stickMinZoom, stickMaxZoom;

	public float swivelMinZoom, swivelMaxZoom;

	public float moveSpeedMinZoom, moveSpeedMaxZoom;

	public float rotationSpeed;

	float RotationAngle, xRotationAngle, zRotationAngle;

	int ActiveFace = 0;

	int mode = 1;

	void Awake()
	{
		swivel = transform.GetChild(0);
		stick = swivel.GetChild(0);
	}

	void Update()
	{
		float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
		if (zoomDelta != 0f)
		{
			AdjustZoom(zoomDelta);
		}

		float rotationDelta = Input.GetAxis("Rotation");
		if (rotationDelta != 0f)
		{
			AdjustRotation(rotationDelta);
		}

		float xDelta = Input.GetAxis("Horizontal");
		float zDelta = Input.GetAxis("Vertical");
		if (xDelta != 0f || zDelta != 0f)
		{
			if (mode == 0)
			{
				AdjustPivotRotation(xDelta, zDelta);
			}
			else if (mode == 1)
			{
				AdjustPosition(xDelta, zDelta);
			}
		}
	}

	void AdjustZoom(float delta)
	{
		zoom = Mathf.Clamp01(zoom - delta);

		float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
		stick.localPosition = new Vector3(0f, 0f, distance);
	}

	void AdjustRotation(float delta)
	{
		RotationAngle -= delta * rotationSpeed * Time.deltaTime;

		transform.localRotation = Quaternion.Euler(0f, RotationAngle, 0f);
	}

	void AdjustPivotRotation(float xDelta, float zDelta)
	{
		xRotationAngle -= xDelta * rotationSpeed * Time.deltaTime;
		zRotationAngle += zDelta * rotationSpeed * Time.deltaTime;
		if (xRotationAngle < 0f)
		{
			xRotationAngle += 360f;
		}
		else if (xRotationAngle >= 360f)
		{
			xRotationAngle -= 360f;
		}
		if (zRotationAngle < 0f)
		{
			zRotationAngle += 360f;
		}
		else if (zRotationAngle >= 360f)
		{
			zRotationAngle -= 360f;
		}
		transform.localRotation = Quaternion.Euler(zRotationAngle, xRotationAngle, 0f);
	}

	bool UpdateFace()
	{
		float nZ = zRotationAngle;
		float nX = xRotationAngle;
		bool flip = false;
		//Debug.Log(nZ + " | " + nX);

		if (nZ >= 225 && nZ < 315)
		{
			if (ActiveFace != 2)
			{
				ActiveFace = 2;
				return true;
			}
			return false;
		}
		else if (nZ >= 45 && nZ < 135)
		{

			ActiveFace = 3;
			return true;
		}
		else if (nZ >= 135 && nZ < 225)
		{
			nX = 360 - nX;
			flip = true;
		}

		if (nX >= 45 && nX < 135)
		{
			ActiveFace = 5;
			return true;
		}
		else if (nX >= 135 && nX < 225)
		{
			if (flip)
			{
				ActiveFace = 0;
			}
			else
			{
				ActiveFace = 4;
			}
			return true;
		}
		else if (nX >= 225 && nX < 315)
		{
			ActiveFace = 1;
			return true;
		}
		else
		{
			if (flip)
			{
				ActiveFace = 4;
			}
			else
			{
				ActiveFace = 0;
			}
			return true;
		}
	}

	void AdjustPosition(float xDelta, float zDelta)
	{
		Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
		float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
		float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime * 3;

		Vector3 position = transform.localPosition;
		position += direction * distance;
		transform.localPosition = position;
	}
}