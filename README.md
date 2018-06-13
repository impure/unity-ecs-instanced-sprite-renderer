# Simple ECS Implementation

This is a simplified version of paullj's [Unity ECS implementation](https://github.com/paullj/unity-ecs-instanced-sprite-renderer). It was designed as a proof of concept for use in my current project.

The purpose of this implementation is instantiation speed. The current build can instantiate 3000 objects in under 16 milliseconds (1 frame at 60 FPS). This is a significant improvement compared to paullj's implementation (although I forgot exactly how fast it was). What's even better is this does not appear to require the use of Unity API's meaning it theoretically can be done in a secondary thread.

- Made in Unity 2018.1.4f1

# Stats

- Instantiation (GameObject) >= 40ms (68ms, 79ms, 49ms, 48ms, 48ms, 42ms, 42ms, 52ms, 47ms, 43ms, 41ms, 45ms, 42ms)
- Instantiation (ECS) >= 8ms (9ms, 9ms, 9ms, 8ms, 8ms, 9ms) (Note: Initializing lists outside of stopwatch section does not appear to help at all. Also starting lists at size 1023 doesn’t help at all either.)
- FPS (GameObject) ~80FPS
- FPS (ECS) ~ 110FPS