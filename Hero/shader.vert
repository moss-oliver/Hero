#version 330

// In
uniform float aDepth;
uniform vec4 aLocation;

// Out
layout(location = 0) in vec2 aPosition;

// Code
void main(void)
{
	float xloc = (aPosition.x * aLocation.z) + aLocation.x;
	float yloc = (aPosition.y * aLocation.w) + aLocation.y;

    gl_Position = vec4((xloc * 2) - 1.0, (yloc * 2) - 1.0, aDepth, 1.0);
}
