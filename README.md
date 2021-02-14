# Overview
A C# library to create a hierarchical scene graph for XNA. This code is only sample work. It has been simplified and stripped of any pipeline-specific content.

# Classes
## SceneItem
A generic scene node that serves as the basic building block for creating a 3D scene graph. Any scene item can be transformed by parenting it to a `TransformNode`.

## TransformNode
The transform node uses the XNA matrix struct to create cumulative similarity transforms. This class provides  encapsulators to manipulate translation, rotation, and scale.

## BillboardTransform
A billboarding transform node that tracks another position. The class can be used to to create 3D sprites.