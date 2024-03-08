# Procedural Generation of Terrain

- Given 2 points on a line a and b
	- Compute elevation y at the midpoint between a and b
	- $y = 1/2 * (a_y + b_y) + r$
		- Where $r = s*r_g*|b_x-a_x|$,
			- r_g is a Gaussian random number with a mean of 0 and variance of 1
			- s is a surface roughness factor. Just choose s to be a value that looks good
				- Can change s for each recursive layer
- Repeat the process for the two lines created by the midpoint splitting

- Need a safe zone (landing zone) for lander
	- Should be 15% in from the edges minimum

**Notes:** 
- Don't let y go below 0
- Don't apply midpoint thingy to safe zones
	- Can do this by starting with multiple line segments
		- Mark one of the lines as the landing zone


- Look at input demos for remapping
	- Keyboard.GetState can help get the key pressed
	- GetKeys can also help