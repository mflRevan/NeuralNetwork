using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyCarSurface : MonoBehaviour
{
	public TinyCarSurfaceParameters parameters;
}

[System.Serializable]
public class TinyCarSurfaceParameters
{
	[Tooltip("Name of this surface. Used for debugging.")]
	public string name = "";
	[Tooltip("Multiplies the amount of steering to apply while touching this object.")]
	public float steeringMultiplier = 1f;
	[Tooltip("Multiplies the amount of acceleration to apply while touching this object.")]
	public float accelerationMultiplier = 1f;
	[Tooltip("Multiplies the maximum speed of the vehicle touching this object.")]
	public float speedMultiplier = 1f;
	[Tooltip("Multiplies how fast the car stops when releasing the gas.")]
	public float forwardFrictionMultiplier = 1f;
	[Tooltip("Multiplies the grip the car should have on the road when turning.")]
	public float lateralFrictionMultiplier = 1f;
	[Tooltip("Multiplies the amount of friction applied when colliding with a wall.")]
	public float sideFrictionMultiplier = 1f;

	public TinyCarSurfaceParameters(float defaultValue = 1f)
	{
		steeringMultiplier = defaultValue;
		accelerationMultiplier = defaultValue;
		speedMultiplier = defaultValue;
		forwardFrictionMultiplier = defaultValue;
		lateralFrictionMultiplier = defaultValue;
		sideFrictionMultiplier = defaultValue;
	}

	public static TinyCarSurfaceParameters operator /(TinyCarSurfaceParameters a, float b)
	{
		if (b == 0)
		{
			throw new System.DivideByZeroException();
		}
		TinyCarSurfaceParameters c = new TinyCarSurfaceParameters(0);
		c.name = a.name;
		c.steeringMultiplier = a.steeringMultiplier / b;
		c.accelerationMultiplier = a.accelerationMultiplier / b;
		c.speedMultiplier = a.speedMultiplier / b;
		c.forwardFrictionMultiplier = a.forwardFrictionMultiplier / b;
		c.lateralFrictionMultiplier = a.lateralFrictionMultiplier / b;
		c.sideFrictionMultiplier = a.sideFrictionMultiplier / b;
		return c;
	}

	public static TinyCarSurfaceParameters operator *(TinyCarSurfaceParameters a, float b)
	{
		TinyCarSurfaceParameters c = new TinyCarSurfaceParameters(0);
		c.name = a.name;
		c.steeringMultiplier = a.steeringMultiplier * b;
		c.accelerationMultiplier = a.accelerationMultiplier * b;
		c.speedMultiplier = a.speedMultiplier * b;
		c.forwardFrictionMultiplier = a.forwardFrictionMultiplier * b;
		c.lateralFrictionMultiplier = a.lateralFrictionMultiplier * b;
		c.sideFrictionMultiplier = a.sideFrictionMultiplier * b;
		return c;
	}

	public static TinyCarSurfaceParameters operator +(TinyCarSurfaceParameters a, TinyCarSurfaceParameters b)
	{
		TinyCarSurfaceParameters c = new TinyCarSurfaceParameters(0);
		c.name = combineNames(a.name, b.name);
		c.steeringMultiplier = a.steeringMultiplier + b.steeringMultiplier;
		c.accelerationMultiplier = a.accelerationMultiplier + b.accelerationMultiplier;
		c.speedMultiplier = a.speedMultiplier + b.speedMultiplier;
		c.forwardFrictionMultiplier = a.forwardFrictionMultiplier + b.forwardFrictionMultiplier;
		c.lateralFrictionMultiplier = a.lateralFrictionMultiplier + b.lateralFrictionMultiplier;
		c.sideFrictionMultiplier = a.sideFrictionMultiplier + b.sideFrictionMultiplier;
		return c;
	}

	public static TinyCarSurfaceParameters operator +(TinyCarSurfaceParameters a, float b)
	{
		TinyCarSurfaceParameters c = new TinyCarSurfaceParameters(0);
		c.name = combineNames(a.name, b.ToString());
		c.steeringMultiplier = a.steeringMultiplier + b;
		c.accelerationMultiplier = a.accelerationMultiplier + b;
		c.speedMultiplier = a.speedMultiplier + b;
		c.forwardFrictionMultiplier = a.forwardFrictionMultiplier + b;
		c.lateralFrictionMultiplier = a.lateralFrictionMultiplier + b;
		c.sideFrictionMultiplier = a.sideFrictionMultiplier + b;
		return c;
	}

	public static string combineNames(string na, string nb)
	{
		if (na.Length == 0) return nb;
		if (nb.Length == 0) return na;
		return na + ", " + nb;
	}

	public string getName()
	{
		if (name.Length > 0) return name;
		return "(default)";
	}

	public TinyCarSurfaceParameters clone()
	{
		TinyCarSurfaceParameters c = new TinyCarSurfaceParameters(0);
		c.name = name;
		c.steeringMultiplier = steeringMultiplier;
		c.accelerationMultiplier = accelerationMultiplier;
		c.speedMultiplier = speedMultiplier;
		c.forwardFrictionMultiplier = forwardFrictionMultiplier;
		c.lateralFrictionMultiplier = lateralFrictionMultiplier;
		c.sideFrictionMultiplier = sideFrictionMultiplier;
		return c;
	}
}