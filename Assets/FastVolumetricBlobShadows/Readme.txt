 _____         _    __     __    _                      _        _      
|  ___|_ _ ___| |_  \ \   / /__ | |_   _ _ __ ___   ___| |_ _ __(_) ___ 
| |_ / _` / __| __|  \ \ / / _ \| | | | | '_ ` _ \ / _ \ __| '__| |/ __|
|  _| (_| \__ \ |_    \ V / (_) | | |_| | | | | | |  __/ |_| |  | | (__ 
|_|  \__,_|___/\__|    \_/ \___/|_|\__,_|_| |_| |_|\___|\__|_|  |_|\___|
 ____  _       _       ____  _               _                          
| __ )| | ___ | |__   / ___|| |__   __ _  __| | _____      _____        
|  _ \| |/ _ \| '_ \  \___ \| '_ \ / _` |/ _` |/ _ \ \ /\ / / __|       
| |_) | | (_) | |_) |  ___) | | | | (_| | (_| | (_) \ V  V /\__ \       
|____/|_|\___/|_.__/  |____/|_| |_|\__,_|\__,_|\___/ \_/\_/ |___/                                                                                                                                                
  Paul Gerla, (c) 2020                               


Description

  Fast Volumetric Blob Shadows is a simple shader-based solution intended for making
  fast-to-render shadows. The cost to render is roughly similar to an unlit particle
  with depth blending enabled. The cube or sphere shaped shadow volumes can be moved,
  rotated, and scaled as needed. It works in forward and deferred render pipelines,
  and versions for the HDRP and URP are included as .UnityPackages.
  
  One of the primary downsides to this approach is that if the camera gets too close
  to the geometry, it will clip in and the shadow will disappear. For this reason I
  would not recommend this solution for large shadow volumes or ones that the camera
  will get especially close to.
  
  The builtin version was made with Amplify Shader Editor and can be modified with 
  that package, or with a text editor. The URP has both an ASE version and a Unity 
  Shader Graph version, while the HDRP only has a Unity Shader Graph version.
  

Quickstart Guide
  
  1) Create a new material asset.
  2) Set the shader of that material to be BlobShadow.
  3) Place a cube or a sphere mesh into the scene. A low-poly sphere mesh without uvs
     or normals is included in the package, but an ordinary cube mesh works just as
     well in the majority of cases.
  3) Disable light probes, reflection probes, shadow casting, and shadow receiving on
     the mesh renderer.
  4) Apply the material to the mesh.


Troubleshooting
  
  If the shadow volume is not visible in the scene, make sure that the mesh is
  intersecting geometry in the scene that writes to the depth buffer (generally opaque
  or alpha-tested). Also check to be sure that both Power and Intensity on the material
  are greater than 0.
  
  If the shadow volume is visible through another object and should not be, check to
  ensure that the object you can see the shadow through is writing to the depth buffer.

  If the shadow volume looks like it's clipping incorrectly, make sure your cube geo
  is 1 unit square -- or your sphere geo is 1 unit in diameter -- at a scale of one.
  Also check to ensure that the near clipping plane of the camera is not penetrating
  the geometry of the shadow volume.


Material Properties Guide

  Color - A simple color value for the shadow effect, can be an HDR color for other
    effects, such as a glow.
  
  Intensity - The is a multiplier on the strength of the effect, can be turned up past
    1 for very strong shadows.
  
  Power - Turning this value up will expand the shadow out toward the edges, sharpening
    the edges and filling in the center.
  
  Allow Shape Blending - Disabling this boolean will force the shader to only allow a
    spherical shape, improving render performance by a very small amount.
  
  Cube to Sphere Blend - This slider will blend the shadow from a cube (0) shape to a
    sphere (1) shape. I haven't found much use for a full cube setting, but values in
    the mid range seem to work very well at time. 
	
	
Feedback

  Email - Pawige@gmail.com
  Twitter - https://twitter.com/Pawige

  Please feel free to contact me with questions, issues, requests, a project you used
  this for, or just to say hi!