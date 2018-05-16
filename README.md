# ConceptVR

![Settings Window](https://raw.github.com/jtstein/ConceptualDesignVR/master/CVRlogo.jpg)

Conceptual  Design  VR  is  a  Unity  application  that  integrates  rapid-prototyping  within  a  shared  virtual-environment.  This  allows  the  users  to  creatively  collaborate  through  the  design  process  of  three  dimensional  modeling.  Leap  Motion  technology  enables  a  controller  free  experience  by  tracking  the  user’s  hands  in  real  time,  and  translating  their  movements  and  gestures  inside  the  virtual  reality  space.  By  networking  the  users  into  a  three  dimensional  environment,  we  have  removed  the  constraints  of  the  traditional  two  dimensional  modeling  applications.  This  allows  the  users  to  precisely  modify  their  designs. 

# Conceptual Design VR  Documentation

## Gesture-Defined Design Tools

Conceptual Design VR enables the user to create models through the integration of gesture-defined design tools. There are a variety of different tools that integrate these gestures to foster an intuitive design process.The integrated tools are: Point, Link, Curve, Move, Select, Extrude, Select, Destroy, Rotate, Scale, Polygon, Rectangle, Doodle, Lights, Teleport, and Materials.


## Point Tool

The point tool allows the user to instantiate a desired vertex (or point), the building block of a 3d polygon, by tapping anywhere within the VR space.

## Link Tool

The link tool allows the user to take any selected DCG objects and link them to all other selected objects with a simple pinch. Linking points will create an edge. Linking edges will create a face and linking faces will create a solid. Additionally this tool inherits the ability to select objects with a tap.

## Curve Tool

The curve tool allows the user to create curved, bezier geometry. The user can pinch to generate a bezier object, and tap during generation to create anchor points for curves. Once the desired curve is complete, the use can pinch again to release it into space. 

## Move Tool

The move tool gives the user the ability to move all selected DCG objects in the VR space by pinching and moving their hand around; ending the pinch will stop moving the objects. An additional functionality of the move tool lets the user copy all selected objects by pinching with their left hand. This tool inherits the ability to select objects with a tap.

## Select Tool

The select tool allows the user to select any DCG object to perform other actions. By pinching, a blue sphere will appear above the fingertips and will select and geometry it collides with. This is useful for bulk-selecting. The selected geometry will attach a new material to denote that it is selected. A geometry can also be selected by tapping on it. The user can choose if they desire to select a point, edge, or face by pressing the corresponding button on the HUD. Multiple DCG objects may be selected at once. Once the desired geometry is selected, the user can switch to many different tools that interface with selected geometry such as Extrude, Move, Rotate, Scale, Clip, and Link.

## Clip Tool

The clip tool allows the user to subtract two DCG objects from each other. With one selected DCG object overlapping another pre-existing DCG object, the user can pinch with their left hand to perform the clip action. 

## Extrude Tool

The extrude tool will take any selected DCG objects and with a pinch will extrude each object into the next object of the hierarchy . By swiping the user can undo the very last extrusion performed. Similar to the move tool, the user can tap to select any DCG object while the extrude tool is active.

## Destroy Tool

The destroy tool allows the user to delete any DCG objects that have been created. By pinching, a spherical eraser will spawn on the user’s fingertips. They can then collide the eraser with any object to destroy it.

## Rotate Tool

The rotate tool allows the user to rotate any selected DCG object by pinching and dragging. The user can select the geometry by tapping on it and then pinch and drag in the desired rotation direction. 

## Scale Tool

The scale tool allows the user to scale any selected DCG object. The user can select the object by tapping on it, and then scale it larger by pinching and dragging away from the object, or scaling it smaller by pinching and dragging towards the object.

## Polygon Tool

The polygon tool allows the user to outline a polygon by drawing it with the pinch gesture. Once the user stops pinching, a polygonal face will be generated through the DCG in accordance with the users drawing.

# #Rectangle Tool

The rectangle tool allows the user to pinch and drag to create a perfect DCG rectangle. The user can pinch to instantiate the origin vertex of the rectangle, and drag to extend the rectangle into their desired dimensions.

## Doodle Tool

The doodle tool allows the user to draw in the 3D environment by pinching. They can update the color of each doodle within the hands-up-display. The tool also contains an eraser in the hands up display that function similarly to that of the destroy tool, but instead with doodles.

## Light Tool

The light tool allows the user to tap to generate lights anywhere in the 3D environment. After selecting a generated light, the user can update the intensity and heat of each light within the hands-up-display.

## Teleport Tool

The teleport tool allows the user to teleport to a different location. Once the user has selected the teleport tool, they can point at a location and fire.

## Material Tool

The material tool allows the user to apply different materials and textures to pre-existing DCG faces. Once a material is selected, they can tap on any face to apply the material.

# Hands Up Display

The Hands Up Display (HUD) is the main controller for swapping between tools, altering settings, and performing file I/O. By default, the HUD is attached to the users left-hand. Non-deterministic infrared overlapping has interfered with our initial design of the Hands-Up-Display. The HUD was originally planned for many buttons to be intractable above the palm, however due to the unavoidable overlapping that would occur, it was re-designed to extend off the left-side of the users left hand. The HUD can be anchored to “float” in space if the user wishes to free their left hand. As the user interacts with the HUD, the contents will update based on the current settings they are altering. There are many different “views” the user can interact with on the HUD. These views are defined as HUD-Frames. By selecting specific frame-buttons, the current HUD-Frame displayed will update.

## Main Frame

The main frame contains all of the general frame-buttons, including a file button, settings button, edit button, tools button, back button, and the anchor button. There is also an analog/digital clock that floats above the palm. The main frame is always active as it allows the user to switch between the main settings. As the user interacts with a frame button, new buttons will appear based on the settings the user wishes to configure. The Anchor button allows the user to detach the HUD from the left hand. The HUD will float in space wherever it was detached until anchor is toggled back on.

## File Frame

The file frame will appear next to the file button when selected. The file frame allows the user to interact with file I/O, and networking. The buttons in the file-frame include Import, Export, Save, and Connect. Import allows the user to import .obj meshes that are placed in the Resources/Imports/ directory. These meshes are passed through the DCG before instantiating in the VR environment. Export allows the user to export all DCG models into the pre-defined exports folder. The models are exported as .objs, where can be loaded into various other programs and can be used with 3D printers. The save button allows the user to save the current scene. This enables the users to load all models they were previously editing into the environment without the need of importing and exporting. The connect button allows a user to connect to another users environment to collaborate while designing prototypes. A number pad will appear, and will require the user to enter the internet-protocol address of the other user hosting their environment session. Once connected, the two users can collaboratively interact with their designs.

## Settings Frame

The settings frame will appear next to the settings button when selected. The settings frame allows the user to update general player settings. The tutorial mode button will replace the clock above the palm with a video player. If tutorial mode is toggled, a short clip will loop in the video player to describe how to use any selected tool. The teleport button enables the teleport tool. The change clock button swaps the clock between digital and analog on the main frame. The player-scale slider allows the user to update their own scale. This lets them grow and shrink to create variable sized geometry. Finally, there is a restart button that will destroy all DCG objects to give the user a fresh environment to work in. The user will be prompted twice to confirm this action.

## Edit Frame

When the user interacts with the edit button, the edit frame will appear. This frame allows the user to swap between the majority of the geometry-creating tools. As they interact with the tool buttons, their current tool is replaced with the desired tool. The buttons on this frame are Point, Link, Extrude, Curve, Polygon, Move, Select, Destroy, Rotate, Scale, and Clip. By interacting with any of these buttons, the user will enable the respective tool.

## Tools Frame

The tools frame spawns next to the tools button when selected. This frame includes miscellaneous non-geometry tools the user can utilize during their design process. The doodle button spawns the doodle subframe. The lights button enables the light tool and the rectangle button enables the rectangle tool. The notes button spawns a virtual keyboard. This allows the user to type notes and annotations, and then place them within the 3D environment. The materials button spawns the materials sub-frame. This allows the user to select different materials and textures to apply to generated geometry. The select button enables the select tool. When tapping on a light, the light subframe will spawn. Multiple lights can be selected and altered at once.

## Doodle Frame

The doodle frame is a sub-frame that extends from the tools frame and fosters all buttons unique to creating, editing, and destroying doodles.The doodle button enables the doodle tool. The colors button spawns a color-pallet on the screen that allows the user to select the color they wish to doodle with. The destroy button enables the destroy tool to remove undesired doodles.

## Light Frame

The light frame is another sub-frame that extends from the tools frame and fosters all buttons unique to editing, and destroying generated lights. This frame includes an intensity slider that allows the user to increase/decrease the intensity of a selected light. It also includes a heat slider that allows the user to edit the “heat” of a selected light. The color range goes from a “cool” blue to a “hot” red. There is also a destroy button that will destroy the selected light. 

## Materials Frame

The materials frame is the final sub-frame extended from the tools frame that includes three different material options. The user can choose between colors, wood, and metals. Once the user selects a desired material type, a color pallet will spawn including pre-generated textures that user can select to apply to pre-existing geometry.


