# IInput
Toolset for recording and playing back user-input to Unity, great for integration testing. There are three main components:

**ScreenInputService** should be your new source of Input information.  It replicates all of Unity's default behaviour in terms of calling your OnMouseEnter/OnMouseExit/etc methods on your monoBehaviors, as well as your OnPointerEnter/OnPointerExit/etc methods on your UI components.  However, it can be driven by either live or mock input.

**ScreenInputMocker** takes in a serialized object of mock-input (represented as keyframes of input), and drives the ScreenInputService.

**ScreenInputRecorder** records your live input and returns data that can be used for the ScreenInputMocker to replay that input.


# Integration Steps
1. Any of your monobehaviours that make use of OnMouseEnter, OnMouseExit, etc., need to make those methods public, and implement the cooresponding interface (IMouseEnterHandler, IMouseExitHandler, etc)
2. Any direct calls you make to UnityEngine.Input (ex: Input.GetMousePosition) should be replaced with calls to the IInput interface.
