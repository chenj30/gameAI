Johnny Chen - chenj30
Game AI - Homework 2 - Multi-Agent Movement

I was able to implement both cone check and collision prediction, but was not able to keep the group separated unless I applied flocking behaviour. 

How I avoided a group of agents was through looping through the other agents and finding the cloest target to the agent. From their I did the calculations for cone chekc nad collision prediction and applied the flee steering behaviour to the agent in respect to the closest target. I put the weight of the evade bahaviour as half its strength and path following as full because with full strength on the evade behaviour the agents move much too far away from their target that the movement looked really weird.

The difference in the cone check and collision prediction behaviour is that cone check does the evade very early and the agents start to flee at a much early time than with collision prediction. Collision prediction also looks a lot cleaner because the agents avoid very smoothly by side stepping away fro mtheir target.