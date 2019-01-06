#version 330

// In
uniform vec4 exportColor;

// Out
out vec4 outputColor;

// Code
void main()
{
	outputColor = exportColor;
}
