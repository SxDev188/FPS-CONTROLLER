# FPS Controller
A simple, physics-based, fps controller built for Unity 6.3 LTS. 

> [!NOTE]
> This Repo is under construction. I reserve the right to change things as time goes on.

## Features:
- Physics-based movement using Rigidbody
- Coyote time jump system
- Air Control Multiplier
- Camera-aligned movement
- Configurable movement forces
- Layer-based ground detection

## Technical Highlights:
- Uses ```ForceMode.VelocityChange``` for tight FPS responsiveness
- Separates input polling from physics calculations
- Implements buffered jumping logic
- Uses raycast-based ground detection with layer masking

## Controls:

|Action|Input|
|---|---|
|Move|WASD|
|Jump|Space|
|Look|Mouse|

## Future Improvements: 
- [ ] Wall running
- [-] Sprinting System
- [ ] Gravity Boots
- [ ] Jump buffering
- [ ] Crouch & Slide mechanics
- [ ] Head bob
- [ ] Surface-based movement
- [ ] General QoL improvements & fixes (e.g slope handling or slope limiting)

## Demo: 
> [!NOTE] 
Coming soon!

## Design Decisions: 
> [!NOTE] 
More coming soon!

## License: 
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
