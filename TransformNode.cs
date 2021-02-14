/// <summary>
/// Euler rotation axis order.
/// </summary>
public enum EulerRotateOrder
{
	XYZ,
	XZY,
	YXZ,
	YZX,
	ZXY,
	ZYX
}

/// <summary>
/// The transform node uses the XNA matrix struct to create
/// cumulative similarity transforms. This class provides a
/// variety of encapsulators to manipulate these transforms.
/// </summary>
public class TransformNode : SceneItem
{
	/// <summary>
	/// the ratio that converts radians to degrees
	/// </summary>
	public const float RADIANS_TO_DEGREES = 57.2957795f;
	/// <summary>
	/// the ratio that converts degrees to radians
	/// </summary>
	public const float DEGREES_TO_RADIANS = 0.0174532925f;

	/// <summary>
	/// the transformation matrix added in the
	/// hierarchy by this TransformNode
	/// </summary>
	protected Matrix mTransformation;
	/// <summary>
	/// the transformation added by the
	/// parent of this node
	/// </summary>
	protected Matrix mParentMatrix;

	//current translation
	protected EulerRotateOrder rotateOrder; //euler rotaion order
	protected Vector3 curRotation; //current rotation
	protected Vector3 curScale; //current scale

	//individual transformations
	protected Matrix translateMat; //translation matrix
	protected Matrix rotateMat; //rotation matrix
	protected Matrix scaleMat; //scale matrix

	/// <summary>
	/// constructor to initialze the transform node with
	/// the given name and rotation order
	/// </summary>
	/// <param name="newOrder">the euler rotation order</param>
	public TransformNode(EulerRotateOrder newOrder)
		: base()
	{
		rotateOrder = newOrder;

		init();
	}

	/// <summary>
	/// constructor to initialze the transform node with the
	/// given name and the specified transformation
	/// </summary>
	/// <param name="newTranslate">new translate offset</param>
	/// <param name="newRotate">new rotation (X,Y,Z indicate offset from 0 measured in radians)</param>
	/// <param name="newScale">new scale factor (no scale is (1,1,1)</param>
	/// <param name="newOrder">new euler rotation order</param>
	public TransformNode(Vector3 newTranslate, Vector3 newRotate,
		Vector3 newScale, EulerRotateOrder newOrder)
		: base()
	{
		rotateOrder = newOrder;
		curRotation = newRotate;
		curScale = newScale;

		translateMat = Matrix.CreateTranslation(newTranslate);
		scaleMat = Matrix.CreateScale(curScale);

		mTransformation = Matrix.Identity; //local transform

		updateRotation();
	}

	/// <summary>
	/// sets this transformation matrix to be the identity matrix
	/// </summary>
	private void init()
	{
		curRotation = Vector3.Zero;
		curScale = Vector3.One;

		//initialize to having no transformation
		mTransformation = Matrix.Identity;
		translateMat = Matrix.Identity;
		rotateMat = Matrix.Identity;
		scaleMat = Matrix.Identity;
	}

	/*
	 * UPDATE TRANSFORMATIONS
	 */
	/// <summary>
	/// update rotation using the Euler transforms
	/// </summary>
	protected virtual void updateRotation()
	{
		switch (rotateOrder)
		{
			case EulerRotateOrder.XYZ:
				rotateMat = Matrix.CreateRotationX(curRotation.X) * Matrix.CreateRotationY(curRotation.Y) * Matrix.CreateRotationZ(curRotation.Z);
				break;
			case EulerRotateOrder.XZY:
				rotateMat = Matrix.CreateRotationX(curRotation.X) * Matrix.CreateRotationZ(curRotation.Z) * Matrix.CreateRotationY(curRotation.Y);
				break;
			case EulerRotateOrder.YXZ:
				rotateMat = Matrix.CreateRotationY(curRotation.Y) * Matrix.CreateRotationX(curRotation.X) * Matrix.CreateRotationZ(curRotation.Z);
				break;
			case EulerRotateOrder.YZX:
				rotateMat = Matrix.CreateRotationY(curRotation.Y) * Matrix.CreateRotationZ(curRotation.Z) * Matrix.CreateRotationX(curRotation.X);
				break;
			case EulerRotateOrder.ZXY:
				rotateMat = Matrix.CreateRotationZ(curRotation.Z) * Matrix.CreateRotationX(curRotation.X) * Matrix.CreateRotationY(curRotation.Y);
				break;
			case EulerRotateOrder.ZYX:
				rotateMat = Matrix.CreateRotationZ(curRotation.Z) * Matrix.CreateRotationY(curRotation.Y) * Matrix.CreateRotationX(curRotation.X);
				break;
		}
		updateTranformation();
	}

	/// <summary>
	/// update the local transform matrix by combining the scale, rotation and translation matrices
	/// </summary>
	protected void updateTranformation()
	{
		//remove identity matrix from transformation product
		mTransformation = scaleMat * rotateMat * translateMat;
		updateCumulative();
	}

	/// <summary>
	/// gets the cumulative transformation matrix
	/// for this transform node
	/// </summary>
	public Matrix Transformation
	{
		get { return mTransformation; }
	}

	/// <summary>
	/// sets the world matrix of all child nodes to be
	/// Transformation * ParentMatrix
	/// </summary>
	private void updateCumulative()
	{
		base.WorldMatrix = mTransformation * mParentMatrix;
	}

	/// <summary>
	/// gets / sets the world transformation matrix
	/// </summary>
	/// <remarks>set the world matrix is really only setting
	/// the ParentMatrix property of the TransformNode. Children
	/// of this node will receive the cumulative world matrix of
	/// Transformation * ParentMatrix</remarks>
	public override Matrix WorldMatrix
	{
		get { return base.WorldMatrix; }
		set
		{
			mParentMatrix = value;
			updateCumulative();
		}
	}

	/*
	 * RESET TRANSFORMATIONS
	 */
	/// <summary>
	/// reset to no translation
	/// </summary>
	public void resetTranslate()
	{
		translateMat = Matrix.Identity;
		updateTranformation();
	}

	/// <summary>
	/// reset to no rotation
	/// </summary>
	public void resetRotate()
	{
		curRotation = Vector3.Zero;
		rotateMat = Matrix.Identity;
		updateTranformation();
	}

	/// <summary>
	/// reset to no scaling
	/// </summary>
	public void resetScale()
	{
		curScale = Vector3.One;
		scaleMat = Matrix.Identity;
		updateTranformation();
	}

	/*
	 * TRANSLATION
	 */
	/// <summary>
	/// offset translation by specified vector
	/// </summary>
	/// <param name="translate">amout to translate the transformation</param>
	public void Translate(Vector3 translate)
	{
		translateMat = Matrix.CreateTranslation(translateMat.Translation + translate);
		updateTranformation();
	}

	/// <summary>
	/// alternate name for position
	/// </summary>
	public Vector3 Translation
	{
		get { return this.Position; }
		set { this.Position = value; }
	}

	/// <summary>
	/// get / sets the position from
	/// the origin of this node
	/// </summary>
	/// <param name="translate"></param>
	public Vector3 Position
	{
		get { return translateMat.Translation; }
		set
		{
			translateMat = Matrix.CreateTranslation(value);
			updateTranformation();
		}
	}

	/// <summary>
	/// gets or sets the X translation of
	/// this transform node
	/// </summary>
	public float X
	{
		get { return translateMat.Translation.X; }
		set
		{
			translateMat = Matrix.CreateTranslation(new Vector3(value,
				translateMat.Translation.Y, translateMat.Translation.Z));

			updateTranformation();
		}
	}
	/// <summary>
	/// gets or sets the Y translation of this transform node
	/// </summary>
	public float Y
	{
		get { return translateMat.Translation.Y; }
		set
		{
			translateMat = Matrix.CreateTranslation(new Vector3(translateMat.Translation.X,
									value, translateMat.Translation.Z));

			updateTranformation();
		}
	}

	/// <summary>
	/// gets or sets the Z translation of this transform node
	/// </summary>
	public float Z
	{
		get { return translateMat.Translation.Z; }
		set
		{
			translateMat = Matrix.CreateTranslation(new Vector3(translateMat.Translation.X,
				translateMat.Translation.Y, value));

			updateTranformation();
		}
	}

	/*
	 * ROTATIONS
	 */
	/// <summary>
	/// offset rotation of node around X,Y, and Z axis
	/// using a standard Euler transformation
	/// </summary>
	/// <param name="rad"></param>
	public virtual void Rotate(Vector3 rad)
	{
		curRotation = curRotation + rad;
		updateRotation();
	}

	/// <summary>
	/// rotates around the X axis by the
	/// specified number of radians
	/// </summary>
	/// <param name="rad"></param>
	public virtual void RotateX(float rad)
	{
		curRotation.X += rad;
		updateRotation();
	}

	/// <summary>
	/// rotates around the Y axis by the
	/// specified number of radians
	/// </summary>
	/// <param name="rad"></param>
	public virtual void RotateY(float rad)
	{
		curRotation.Y += rad;
		updateRotation();
	}

	/// <summary>
	/// rotates around the Z axis by the
	/// specified number of radians
	/// </summary>
	/// <param name="rad"></param>
	public virtual void RotateZ(float rad)
	{
		curRotation.Z += rad;
		updateRotation();
	}

	/// <summary>
	/// get / sets the amount of rotation for the
	/// transform as Euler rotations in radians
	/// </summary>
	public virtual Vector3 Rotation
	{
		get { return curRotation; }
		set
		{
			curRotation = value;
			updateRotation();
		}
	}

	/// <summary>
	/// get / sets the amount of rotation for the
	/// transform as Euler rotations in degrees
	/// </summary>
	public virtual Vector3 RotationDegrees
	{
		get
		{
			Vector3 degRotation;
			degRotation.X = TransformNode.RADIANS_TO_DEGREES * curRotation.X;
			degRotation.Y = TransformNode.RADIANS_TO_DEGREES * curRotation.Y;
			degRotation.Z = TransformNode.RADIANS_TO_DEGREES * curRotation.Z;
			return degRotation;
		}
		set
		{
			curRotation.X = TransformNode.DEGREES_TO_RADIANS * value.X;
			curRotation.Y = TransformNode.DEGREES_TO_RADIANS * value.Y;
			curRotation.Z = TransformNode.DEGREES_TO_RADIANS * value.Z;

			updateRotation();
		}
	}

	/// <summary>
	/// get / sets the current rotation for the
	/// node around the X axis in radians
	/// </summary>
	public virtual float RotationX
	{
		get { return curRotation.X; }
		set
		{
			curRotation.X = value;
			updateRotation();
		}
	}
	
	/// <summary>
	/// get / sets the current rotation for the
	/// node around the Y axis in radians
	/// </summary>
	public virtual float RotationY
	{
		get { return curRotation.Y; }
		set
		{
			curRotation.Y = value;
			updateRotation();
		}
	}
	
	/// <summary>
	/// get / sets the current rotation for the 
	/// node around the Z axis in radians
	/// </summary>
	public virtual float RotationZ
	{
		get { return curRotation.Z; }
		set
		{
			curRotation.Z = value;
			updateRotation();
		}
	}
	
	/// <summary>
	/// rotates around an arbitrary axis, 
	/// WARNING: using this method will invalidate the
	/// use of other rotation methods
	/// </summary>
	/// <param name="axis">the axis to rotate around</param>
	/// <param name="deg">the angle in radians to rotate</param>
	public virtual void RotateAxis(Vector3 axis, float deg)
	{
		rotateMat = Matrix.Identity * Matrix.CreateFromAxisAngle(axis, deg) * rotateMat;
		updateTranformation();
	}

	/*
	 * SCALING
	 */
	/// <summary>
	/// modify scale factor by specified amount
	/// </summary>
	/// <param name="scale"></param>
	public void Scale(Vector3 scale)
	{
		curScale.X = scale.X * curScale.X;
		curScale.Y = scale.Y * curScale.Y;
		curScale.Z = scale.Z * curScale.Z;
		scaleMat = Matrix.CreateScale(curScale);
		updateTranformation();
	}

	/// <summary>
	/// get / sets the scale vector for the node
	/// </summary>
	/// <param name="translate"></param>
	public Vector3 CurrentScale
	{
		get { return curScale; }
		set
		{
			curScale = value;
			scaleMat = Matrix.Identity * Matrix.CreateScale(curScale);
			updateTranformation();
		}
	}

	/*
	 * ENCAPSULATORS
	 */
	/// <summary>
	/// gets / sets the euler rotation order
	/// </summary>
	public EulerRotateOrder RotateOrder
	{
		get { return rotateOrder; }
		set
		{
			rotateOrder = value;
			updateRotation();
		}
	}

	/// <summary>
	/// the translation part of the transformation matrix
	/// </summary>
	public Matrix TranslationMatrix
	{
		get { return translateMat; }
	}
	
	/// <summary>
	/// the scale part of the transformation matrix
	/// </summary>
	public Matrix ScaleMatrix
	{
		get { return scaleMat; }
	}
	
	/// <summary>
	/// the rotation part of the transformation matrix
	/// </summary>
	public Matrix RotationMatrix
	{
		get { return rotateMat; }
	}
}