/// <summary>
/// SceneItem is a generic scene graph item that acts as the
/// basic building block for making a scene graph for a 3D world.
/// </summary>
public class SceneItem
{
	/*
	 * CLASS PROPERTIES
	 */
	/// <summary>
	/// visible state of the scene item
	/// </summary>
	protected bool visible;
	/// <summary>
	/// the scene item's parent (or null if none)
	/// </summary>
	protected SceneItem parent;
	/// <summary>
	/// the scene item's children
	/// </summary>
	protected List<SceneItem> children;

	/// <summary>
	/// the transfromation matrix that describes the
	/// translation, rotation and scale of this scene item
	/// </summary>
	private Matrix worldMatrix;
	/// <summary>
	/// indicates if the inverse matrix property is up to date
	/// </summary>
	private bool invNeedsUpdate;
	/// <summary>
	/// the inverse matrix of the world matrix
	/// </summary>
	private Matrix invWorld;

	/*
	 * CONSTRUCTORS
	 */
	/// <summary>
	/// main constructor for a scene item
	/// </summary>
	public SceneItem()
	{
		init();
	}

	private void init()
	{
		visible = true;
		parent = null;
		children = new List<SceneItem>();

		worldMatrix = Matrix.Identity;
		invWorld = Matrix.Identity;

		invNeedsUpdate = false;
	}
	
	/*
	* UNLOADING
	*/
	/// <summary>
	/// unloads this scene item and all of its children
	/// </summary>
	/// <param name="disposeResources"></param>
	public virtual void Unload(bool disposeResources)
	{
		//remove from parent's hierarchy
		if (parent != null)
			parent.RemoveChild(this);

		if (children != null)
		{
			//unload all children...
			SceneItem[] childArray = children.ToArray();
			for (int i = 0; i < childArray.Length; i++)
				childArray[i].Unload(disposeResources);
			children.Clear();
		}
		children = null;
		parent = null;
		visible = false;
	}

	/*
	 * ENCAPSULATORS
	 */
	/// <summary>
	/// gets the current transformed position of this scene
	/// item in world coordinates
	/// </summary>
	public virtual Vector3 WorldPosition
	{
		get { return worldMatrix.Translation; }
	}

	/// <summary>
	/// gets /sets the world matrix
	/// transformation for this SceneItem
	/// </summary>
	public virtual Matrix WorldMatrix
	{
		get { return worldMatrix; }
		set
		{
			worldMatrix = value;
			invNeedsUpdate = true;

			//children inherit world matrix property
			foreach (SceneItem child in children)
				child.WorldMatrix = worldMatrix;
		}
	}

	/// <summary>
	/// the inverse of the world matrix
	/// </summary>
	public virtual Matrix WorldInverse
	{
		get
		{
			if (invNeedsUpdate)
			{
				invWorld = Matrix.Invert(worldMatrix);
				invNeedsUpdate = false;
			}
			return invWorld;
		}
	}

	/// <summary>
	/// indicates if this scene item is the child of a
	/// TransformNode object.
	/// </summary>
	public bool HasParentTransform
	{
		get { return (parent is TransformNode); }
	}

	/// <summary>
	/// gets the direct parent of this
	/// scene item as a TransformNode.
	/// Or null if there is not one.
	/// </summary>
	public virtual TransformNode ParentTransform
	{
		get { return (parent as TransformNode); }
	}

	/*
	 * POSITION AND NORMAL TRANSFORMATIONS
	 */
	/// <summary>
	/// transforms a position from the local space of this SceneItem node to the global space
	/// </summary>
	/// <param name="localPos">a position in local space</param>
	/// <returns>a position in global space</returns>
	public Vector3 LocalToGlobal(Vector3 localPos)
	{
		return Vector3.Transform(localPos, worldMatrix);
	}

	/// <summary>
	/// transforms a position from a global space to the local space of this SceneItem node
	/// </summary>
	/// <param name="globalPos">a position in global space</param>
	/// <returns>a position in local space</returns>
	public Vector3 GlobalToLocal(Vector3 globalPos)
	{
		return Vector3.Transform(globalPos, this.WorldInverse);
	}

	/// <summary>
	/// transforms a normal from the local space of this SceneItem node to the global space
	/// </summary>
	/// <param name="localNormal">a normal in local space</param>
	/// <returns>a normal in global space</returns>
	public Vector3 LocalToGlobalNormal(Vector3 localNormal)
	{
		return Vector3.TransformNormal(localNormal, worldMatrix);
	}

	/// <summary>
	/// transforms a normal from a global space to the local space of this SceneItem node
	/// </summary>
	/// <param name="globalPos">a normal in global space</param>
	/// <returns>a normal in local space</returns>
	public Vector3 GlobalToLocalNormal(Vector3 globalNormal)
	{
		return Vector3.TransformNormal(globalNormal, this.WorldInverse);
	}

	/// <summary>
	/// transforms a position from the local space of on SceneItem node to another
	/// </summary>
	/// <param name="localPos">the local position in the space of the SceneItem node "localSpace"</param>
	/// <param name="localSpace">the SceneItem node that has the "localPos"</param>
	/// <param name="targetSpace">the returned position will be in the local space of this SceneItem node</param>
	/// <returns>the position in the local space of "targetSpace"</returns>
	public static Vector3 LocalToLocal(Vector3 localPos, SceneItem localSpace, SceneItem targetSpace)
	{
		return targetSpace.GlobalToLocal(localSpace.LocalToGlobal(localPos));
	}

	/// <summary>
	/// transforms a position from the local space of one SceneItem node to another
	/// </summary>
	/// <param name="localPos">the local position in the space of the SceneItem node "localSpace"</param>
	/// <param name="currentSpace">the SceneItem node that has the "localPos"</param>
	/// <param name="targetSpace">the returned position will be in the local space of this SceneItem node</param>
	/// <returns>the position in the local space of "targetSpace"</returns>
	public static Vector3 LocalToLocalNormal(Vector3 localDirection, SceneItem currentSpace, SceneItem targetSpace)
	{
		return targetSpace.GlobalToLocalNormal(currentSpace.LocalToGlobalNormal(localDirection));
	}

	/// <summary>
	/// gets the matrix that will transform points
	/// from the current space to the target space
	/// </summary>
	/// <param name="currentSpace">the SceneItem node that is the current local space</param>
	/// <param name="targetSpace">the trnasformable node that is the target local space</param>
	/// <returns>a matrix that will transform points or directions from the current space to the target space</returns>
	public static Matrix GetTransform(SceneItem currentSpace, SceneItem targetSpace)
	{
		return (targetSpace.WorldMatrix * currentSpace.WorldInverse);
	}

	/// <summary>
	/// gets / sets the visiblity of
	/// this item and its children
	/// </summary>
	public bool Visible
	{
		get { return visible; }
		set { visible = value; }
	}


	/*
	 * HIERARCHY CREATION / MANIPULATION
	 */
	/// <summary>
	/// gets or sets the parent of this scene item
	/// </summary>
	public virtual SceneItem Parent
	{
		get { return parent; }
		set
		{
			if (value == null)
				parent = value;
			else
				value.AddChild(this);
		}
	}

	/// <summary>
	/// add achild to this scene item
	/// </summary>
	/// <param name="newChild">the child to add</param>
	public virtual void AddChild(SceneItem newChild)
	{
		if (newChild == this)
			throw new ArgumentException("Attempted to parent a scene item to itself.");
		if (newChild.descendant(this, 1, -1))
			throw new ArgumentException("Cannot make a scene item as one of it's own descendants");
		children.Add(newChild);

		//remove from old parent
		if (newChild.HasParent)
			newChild.parent.RemoveChild(newChild);

		//add to hierarchy
		newChild.parent = this;
		newChild.WorldMatrix = this.WorldMatrix;
	}

	/// <summary>
	/// remove all children from this scene item
	/// </summary>
	public virtual void RemoveAllChildren()
	{
		while (children.Count > 0)
		{
			//remove children from the top down
			RemoveChild(children[children.Count - 1]);
		}
		children.Clear();
	}

	/// <summary>
	/// remove the specified child from this scene item
	/// </summary>
	/// <param name="xChild">the child to be remove</param>
	public virtual void RemoveChild(SceneItem xChild)
	{
		if (children.Contains(xChild))
		{
			xChild.parent = null;
			xChild.WorldMatrix = Matrix.Identity;
			children.Remove(xChild);
		}
	}

	/// <summary>
	/// removes the child at the specified index from this scene item
	/// </summary>
	/// <param name="index">the index of the child to remove</param>
	public virtual void RemoveChildAt(int index)
	{
		if (index >= 0 && index < children.Count)
			this.RemoveChild(children[index]);
	}

	/// <summary>
	/// gets the child at the specified index
	/// </summary>
	/// <param name="index">the index in the children list</param>
	/// <returns>the child, or null if index is invalid</returns>
	public virtual SceneItem GetChildAt(int index)
	{
		if (index >= 0 && index < this.children.Count)
			return children[index];
		return null;
	}

	/// <summary>
	/// returns true if the specified scene item is
	/// an immediate child of this scene item
	/// </summary>
	public virtual bool Contains(SceneItem curChild)
	{
		return children.Contains(curChild);
	}

	/// <summary>
	/// returns true if the specified scene item is a descendant of this scene item
	/// </summary>
	/// <param name="startDepth">setting this value to 1 will cause the check to skip the first
	/// level of the hierarchy, 2 will skip the first two levels and so on</param>
	/// <param name="maxDepth">the maximum depth into the hierarchy past the start depth
	/// to check or -1 to check entire hirearchy</param>
	protected virtual bool descendant(SceneItem item, int startDepth, int maxDepth)
	{
		if (startDepth <= 0 && this == item)
			return true;

		startDepth--;
		if (maxDepth >= 0 && startDepth < -maxDepth)
			return false;

		for (int i = 0; i < children.Count; i++)
		{
			if (children[i].descendant(item, startDepth, maxDepth))
				return true;
		}
		return false;
	}

	/*
	* HIERARCHY ACCESORS
	*/
	/// <summary>
	/// gets the list of children for this node
	/// </summary>
	public List<SceneItem> Children
	{
		get { return this.children; }
	}

	/// <summary>
	/// Gets the number of immediate
	/// children of this scene item
	/// </summary>
	public int Nuchildren
	{
		get { return children.Count; }
	}

	/// <summary>
	/// gets the total number of children,
	/// in the hierachy of this scene item
	/// </summary>
	public int NumTotalChildren
	{
		get
		{
			int totalChildren = 0;
			foreach(SceneItem child in children)
				totalChildren += child.NumTotalChildren;
			return totalChildren;
		}
	}

	/// <summary>
	/// gets true if this scene item has a parent node
	/// </summary>
	public bool HasParent
	{
		get { return (parent != null); }
	}

	/*
	 * SIBLING SORTING
	 */
	/// <summary>
	/// swap the children at the specified indeces
	/// </summary>
	/// <param name="index1">the index of the first child in the swap</param>
	/// <param name="index2">the index of the second child in the swap</param>
	public virtual void SwapChildrenAt(int index1, int index2)
	{
		if (index1 >= 0 && index1 < children.Count && index2 >= 0 && index2 < children.Count)
		{
			SceneItem temp = children[index1];
			children[index1] = children[index2];
			children[index2] = temp;
		}
	}

	/// <summary>
	/// swap the render order of the two specified children
	/// </summary>
	/// <param name="item1">the first child in the swap</param>
	/// <param name="item2">the second child in the swap</param>
	public virtual void SwapChildren(SceneItem item1, SceneItem item2)
	{
		int index1 = children.IndexOf(item1);
		int index2 = children.IndexOf(item2);

		if (index1 >= 0 && index2 >= 0)
		{
			children[index1] = item2;
			children[index2] = item1;
		}
	}

	/// <summary>
	/// sets the specified child to be drawn last in
	/// the render order effectively bringing it to the front 
	/// </summary>
	public virtual void BringToFront(SceneItem curChild)
	{
		if (children.Contains(curChild))
		{
			int curIndex = children.IndexOf(curChild);
			if (curIndex < children.Count - 1)
			{
				children[curIndex] = children[children.Count - 1];
				children[children.Count - 1] = curChild;
			}
		}
	}

	/// <summary>
	/// sets the specified child to be drawn first in
	/// the render order effectively send it to the back
	/// </summary>
	public virtual void SendToBack(SceneItem curChild)
	{
		if (children.Contains(curChild))
		{
			int curIndex = children.IndexOf(curChild);
			if (curIndex > 0)
			{
				children[curIndex] = children[0];
				children[0] = curChild;
			}
		}
	}

	/// <summary>
	/// bring this item to the front
	/// of its parent's display order
	/// </summary>
	public virtual void BringToFront()
	{
		if (this.HasParent)
			parent.BringToFront(this);
	}

	/// <summary>
	/// send this item to the back of
	/// its parent's display order
	/// </summary>
	public virtual void SendToBack()
	{
		if (this.HasParent)
			parent.SendToBack(this);
	}

	/*
	 * UPDATE AND RENDER
	 */
	/// <summary>
	/// updates this node and all its children
	/// </summary>
	public virtual void Update(Matrix viewMatrix, Matrix projMatrix)
	{
		foreach (SceneItem child in children)
			child.Update(viewMatrix, projMatrix);
	}

	/// <summary>
	/// Renders this object and all its children if visibiltiy is on.
	/// </summary>
	public virtual void RenderChildren()
	{
		if (visible)
		{
			//render the object content itself
			Render();

			//render any children of this object
			foreach (SceneItem child in children)
				child.RenderChildren();
		}
	}

	/// <summary>
	/// Renders the content of just this scene item (not its children).
	/// This method is called by the RenderChild method if the object
	/// is visible.
	/// </summary>
	public virtual void Render()
	{
		//overwritten by any class with content to render
	}
}