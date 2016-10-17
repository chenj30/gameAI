For emergence:

Tried to make it so that each bird would have two children to try and make a formation something like this:

               ^ - leader
              ^  ^
            ^ ^  ^ ^
            etc.

1. I tried using raycasting for obstacle avoidance.

2. I didn't really use anything extra to go through the tunnels. I set the ray cast such that if a bird was going in a direction and its ray hit a wall I would have it ignore the emergence and instead focus on trying to avoid the obstacle.
	so in if a ray hit something the emergence weight would be 0
3. Emergence went a bit wild, I think because each bird was determining for itself if it could go to an area instead of the leader trying to decide if the formation is small enough to squeeze through an area.
The emergent group would also sometimes have groups that would go off on their onw because they were all following each other and not following something that was following the red leader