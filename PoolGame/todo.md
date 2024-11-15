# Todo list
## Ordered by importance:
1. follow (roughly) the draw.io class diagram: 'Class Diagram v0.1.draw.io' in NEA folder
	1. make sure Sprite accounts for the window changing size
1. update README.md
1. finish CollideObject
	1. make circles 'bounce' off of other circles and rectangles
		1. use: https://www.real-world-physics-problems.com/physics-of-billiards.html
	1. define a 'line' collision for borders
		1. (optional) make it work with various gradients
1. redo CueBall and TableRim to work with CollideObject
1. make Ball and put CueBall under it
1. implement player-controlled movement for CueBall (basic WASD/cursor, just for testing)
1. implement ObjectBall and make it respond to hits by moving
1. implement:
	1. aiming
	1. charging shot
	1. shooting
1. friction / air resistance
1. menus
	1. start screen
	1. (add later) game select
	1. game
	1. settings

## Ball collision physics:

1. nearly elastic (simplify to be elastic), i.e. KE is conserved before and after collision
1. 

## Misc.:
1. use Normalize(Vector2) to make friction consistent
1. The MonoGame Framework provides the BoundingBox, BoundingFrustum, BoundingSphere, Plane, and Ray classes for representing simplified versions of geometry for the purpose of efficient collision and hit testing.
	1. These classes have methods for checking for intersection and containment with each other.