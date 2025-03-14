# YARG Renderer

Yet Another Ray-Graphics Renderer

## Controls

Arrow Keys - Move Camera

Space - Move Camera Up

Left Shift - Move Camera Down

Q + Arrow Keys - Turn Camera

Q + Space - Reset Camera Angle

Backtick [`] - Change Shader Mode

Tab - Toggle Map View

## About

The YARG Renderer is a ray casting renderer written from scratch in C#, using the Form library for drawing to the screen in a window.

Currently the renderer supports rendering spheres and planes, with their positions hardcoded into Program.InitializeGeometry

The roadmap for this project has polygon rendering as the next priority, and then a code cleanup pass to follow.

## Shaders

The YARG Renderer comes with a few preset shaders
- Y-Normal - Colors geometry by multiplying the Y component of the hit normal by the object's color

  ```(float)Normal.Y * (Vec3)Geometry.Material.Color```
- Normal - Colors geometry using the x, y, and z components of the normal

  ```(Vec3)(Normal.X, Normal.Y, Normal.Z)```
- Flat - Uses the flat color of the object with no shading

  ```(Vec3)Geometry.Material.Color```

- Distance - Multiplies the inverse distance of each ray by the color of the contacted object, with n being the distance at which a pixel becomes black

   ```(1 - ((float)Ray.Distance / n)) * (Vec3)Geometry.Material.Color```