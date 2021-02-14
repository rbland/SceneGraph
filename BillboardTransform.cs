/// <summary>
/// A billboarding transform node that tracks an arbitrary transformable node position.
/// The primary use of this class is for creating 3D sprites.
/// </summary>
public class BillboardTransform : TransformNode
{
	/// <summary>
	/// the default up direction to use when
	/// calculating the billboard facing direction
	/// </summary>
	public static Vector3 DEFAULT_UP = Vector3.UnitY;
	/// <summary>
	/// the up direction to use if forward axis == DEFAULT_UP
	/// </summary>
	public static Vector3 ALTERNATE_UP = Vector3.UnitX;

	/*
	* CLASS PROPERTIES
	*/
	// the target the billboard will track
	protected SceneItem mTrackTarget;

	// the position of the tracker the last
	// time the billboard matrix was updated
	private Vector3 lastTrackerPosition;
	// the position of this transform node
	// the last time the billboard matrix was updated
	private Vector3 lastBillboardPosition;

	// a rotation matrix that can rotate
	// the billboard around its forward axis
	protected Matrix axisRotationMat;
	// the amoutn of rotation to apply around the forward axis
	protected float axisRotation;
	// the constraining up vector that orients the billboard
	protected Vector3 upVector;

	/*
	* CONSTRUCTOR
	*/
	/// <summary>
	/// Basic constructor for a new billboard transform.
	/// </summary>
	/// <param name="newTrackTarget">The new target the billboard
	/// transform should track. Typically this is the camera.</param>
	public BillboardTransform(TransformNode newTrackTarget)
		: base(EulerRotateOrder.XYZ)
	{
		mTrackTarget = newTrackTarget;
		init();
	}

	private void init()
	{
		upVector = BillboardTransform.DEFAULT_UP;

		axisRotationMat = Matrix.Identity;
		lastTrackerPosition = Vector3.Zero;
		lastBillboardPosition = Vector3.Zero;
		axisRotation = 0;
	}

	/*
	 * UPDATE ROTATION MATRIX
	 */
	/// <summary>
	/// updates the rotation matrix to
	/// apply the current billboarding transform
	/// </summary>
	protected override void updateRotation()
	{
		if (mTrackTarget == null)
		{
			rotateMat = Matrix.Identity;
			return;
		}
		lastTrackerPosition = mTrackTarget.WorldPosition;
		lastBillboardPosition = this.WorldPosition;

		//determine billboard matrix
		Matrix lookAt = Matrix.Identity;
		lookAt.Forward = Vector3.Normalize(lastTrackerPosition - lastBillboardPosition);

		Vector3 up = upVector;
		if (up == lookAt.Forward)
			up = BillboardTransform.ALTERNATE_UP;

		lookAt.Right = Vector3.Normalize(Vector3.Cross(lookAt.Forward, up));
		lookAt.Up = Vector3.Cross(lookAt.Forward, lookAt.Right);

		//add axis rotation
		rotateMat = axisRotationMat * lookAt;
		base.updateTranformation();
	}


	/*
	 * ENCAPSULATORS
	 */
	/// <summary>
	/// The target which the billboard will track. Typically
	/// this will be the camera object though any transform
	/// node can be tracked.
	/// </summary>
	public SceneItem TrackTarget
	{
		get { return mTrackTarget; }
		set
		{
			mTrackTarget = value;
			lastTrackerPosition = Vector3.Zero;
			UpdateBillboard();
		}
	}
	/// <summary>
	/// the up direction to use in orienting the billboard
	/// </summary>
	public Vector3 UpVector
	{
		get { return upVector; }
		set { upVector = value; }
	}

	/// <summary>
	/// the rotation of the billboard around
	/// its forward axis measured in radians
	/// </summary>
	public virtual float AxisRotation
	{
		get { return axisRotation; }
		set
		{
			axisRotation = value;
			axisRotationMat = Matrix.CreateRotationZ(axisRotation);
			updateRotation();
		}
	}

	/*
	 * UPDATE
	 */
	/// <summary>
	/// updates the billboard transform if its position or
	/// its tracking target's position has changed
	/// </summary>
	public void UpdateBillboard()
	{
		if (mTrackTarget != null)
		{
			if (mTrackTarget.WorldPosition != lastTrackerPosition ||
				this.WorldPosition != lastBillboardPosition)
			{
				updateRotation();
			}
		}
	}

	public override void Update(Matrix viewMatrix, Matrix projMatrix)
	{
		base.Update(viewMatrix, projMatrix);
		UpdateBillboard();
	}

	/*
	 * ROTATIONS OVERRIDE
	 */
	/// <summary>
	/// invalid for a billboard transformation
	/// </summary>
	public override void Rotate(Vector3 deg)
	{
		throw new Exception("Cannot set the rotation of a billboard transform");
	}

	/// <summary>
	/// invalid for a billboard transformation
	/// </summary>
	public override void RotateX(float deg)
	{
		throw new Exception("Cannot set the rotation of a billboard transform");
	}

	/// <summary>
	/// invalid for a billboard transformation
	/// </summary>
	public override void RotateY(float deg)
	{
		throw new Exception("Cannot set the rotation of a billboard transform");
	}

	/// <summary>
	/// invalid for a billboard transformation
	/// </summary>
	public override void RotateZ(float deg)
	{
		throw new Exception("Cannot set the rotation of a billboard transform");
	}

	/// <summary>
	/// invalid for a billboard transformation
	/// </summary>
	public override Vector3 Rotation
	{
		get { return Vector3.Zero; }
		set { throw new Exception("Cannot set the rotation of a billboard transform"); }
	}
	/// <summary>
	/// invalid for a billboard transformation
	/// </summary>
	public override float RotationX
	{
		get { return 0; }
		set { throw new Exception("Cannot set the rotation of a billboard transform"); }
	}
	/// <summary>
	/// invalid for a billboard transformation
	/// </summary>
	public override float RotationY
	{
		get { return 0; }
		set { throw new Exception("Cannot set the rotation of a billboard transform"); }
	}
	/// <summary>
	/// invalid for a billboard transformation
	/// </summary>
	public override float RotationZ
	{
		get { return 0; }
		set { throw new Exception("Cannot set the rotation of a billboard transform"); }
	}
	/// <summary>
	/// invalid for a billboard transformation
	/// </summary>
	public override void RotateAxis(Vector3 axis, float deg)
	{
		throw new Exception("Cannot set the rotation of a billboard transform");
	}
}