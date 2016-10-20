Assignment 4 - Leah Magrane, Johnny Chen, Andy Wong


emergence(andy):
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

- blackbird is controlled by mouse not keyboard

TWO LEVEL FORMATION (leah): 
Everything should mostly be there, you can use the arrow keys to move the black bird

SlotAssignment.cs takes care of organizing the slots around the anchor. It organizes the birds into a circle formation
TwoLevelFormation.cs takes care of most of the heavy lifting. 
Each bird is defined as a "character" which holds all the information for each bird, including it's offset from the anchor, and 
some other necessary variables.
In each update the birds check their distance to obstacles in the direction they are moving, if they are not in distance of obstacles
they will continue to follow the anchor. Once they detect they are going to collide they will switch their state to evade and move around
the obstacle with reference to the anchor point. 
PathFollow.cs: this is simply the path definition and following mechanism for the anchor point
blackbird.cs: just controls the blackbird