Assignment 4 - Leah Magrane, Johnny Chen, Andy Wong



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