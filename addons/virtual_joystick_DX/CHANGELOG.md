# Changelog

**v1.0**

## Complete Architectural Refactoring & Modularization

---

- **Monolithic Core Separation:** Split the large core script into highly specialized, decoupled helper classes to improve code maintainability, clean execution, and readability.
    - `vjdx_core_script.gd` (Control Core / Orchestrator): Handles property exports for the Inspector, scene lifecycle, and general input routing.
    - `vjdx_renderer.gd` (VJDXRenderer): A stateless static rendering helper class that takes over all 2D custom canvas drawing.
    - `vjdx_region.gd` (VJDXRegion): Manages viewport boundaries, global-to-local calculations, and dynamic active region clamping.
    - `vjdx_haptics.gd` (VJDXHaptics): Safely wraps mobile platform-specific vibration APIs (`Input.vibrate_handheld`).
    - `vjdx_hardware_detector.gd` (VJDXHardwareDetector): Isolated polling logic for keyboards and external gamepad connections to toggle dynamic visibility.
    - `vjdx_joystick_handler.gd` (VJDXJoystickHandler): Fully owns the Joystick's movement-mode logic — deadzone mapping, proportional speed, activation checks (DYNAMIC/FOLLOWING), clampzone release, and reposition-target computation — on par with the D-Pad handler's ownership model.
    - `vjdx_dpad_handler.gd` (VJDXDpadHandler): Processes 8-way octant mapping, state transitions, and manages custom/preset SVG texture states.
- **Rendering Performance Optimization:** Separated the canvas redrawing routine (`_draw()`) from physics and touch state updates. This prevents redundant drawing calculation loops.
- **API and UI Preservation:** Maintained backward compatibility. The unified Inspector layout and API interface remain unchanged, allowing developers to switch between modes seamlessly on a single control node instance.

## Centered Pivot Offset

---

- `pivot_offset` is now automatically kept at the center of the node (`size / 2.0`), both on `_ready()` and on every resize (`NOTIFICATION_RESIZED`). Applies to both Joystick and D-Pad styles alike, since it only depends on `size`.
- Ensures any future `scale` or `rotation` applied to the control (e.g. a press "punch" tween) pivots from the visual center instead of the top-left corner.
- Hidden from the Inspector (`_validate_property`), since it's now fully code-managed and any manual edit would be overwritten on the next resize.

**v0.4**

## Haptic Feedback

---

- Haptic feedback added.
- Independent values for Joystick and D-Pad.
  Each controller mode (Joystick, D-Pad) has its own set of haptic feedback settings.
- Configurable duration and intensity of haptic feedback.

Read the README.md file to learn more.
