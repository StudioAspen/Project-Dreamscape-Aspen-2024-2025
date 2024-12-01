How to use:
- See the FogPrefab folder for implementation examples
1. Place a volume in the scene.
2. Inspect the volume.
3. Add a profile under the volume tab.
4. Add override, select the Fog Effect override.
5. Play around with the parameters! Check the prefabs for an example to work off of.


Explaining each parameter:
- Primary Fog Color: The color that is rendered near the camera.
- Secondary Fog Color: The color that is rendered far from the camera.
- Gradient Strength: Affects the strength of the gradient transition between the two fog colors.
- Fog Density: Affects the strength/density of the fog effect.
- Fog Scattering: Affects the softness of the noise texture of the fog. Behaves like fog density when no noise is set.

- Sky Box Fog Color: The color that is rendered at the skybox (a depth value of 1).
- Sky Box Fog Density: Affects the stength/density of the skybox fog color.

- Noise Texture: An optional noise texture to simulate fog movement.

- Rotate Fog Noise: Rotates the noise texture. Values are between 0 to 2PI.
- Fog Noise Scale: Scales the size of the noise texture.
- Fog Noise Velocity: Affects the velocity of the noise texture's movement across the camera.

- Rotate Sky Box Noise: Rotates the seperate noise in the skybox.
- Sky Box Noise Scale: Scales the size of the skybox's noise texture.
- Sky Box Noise Velocity: Affects the velocity of the skybox noise texture's movement across the camera.
- Sky Box Noise Transparency: Affects the sky box noise's opacity.


Tips:
- If you want to keep it simple, you can set the noise parameters to 0, and remove the texture. The noise is not required.
- Keeping the colors similar to each other makes the fog more realistic!
- If you want to go for a more stylized look, try adding stronger and brighter colors!


Trouble-shooting:
"The post effect fog does not occur inside the volume, how can I fix this?"
- Check if your URP renderer has the "FogEffectFeature" added as a renderer feature.
    - For the Aspen2025 project, the feature should already be added.

"The post effect fog is overwriting particle effects, how can I fix this?"
- Add a second no post camera.
    - For the Aspen2025 project, I added a NoPostCamera prefab in the FogPostEffect folder.
    - Add the NoPostCamera as a child under the main camera.
    - Include any objects/particles in the NoPostEffect layer. Alternatively, create your own layer and add it to the no post camera's culling mask.
- A tutorial can be found here: 
https://www.youtube.com/watch?v=LAopDQDCwak&pp=ygUUbm8gcG9zdCBjYW1lcmEgdW5pdHk%3D