﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using OpenTK.Graphics;

namespace Hero
{
    class Shader : System.IDisposable
    {
        int Handle;

        public Shader(string vertPath, string fragPath)
        {
            int VertexShader;
            int FragmentShader;

            string VertexShaderSource = LoadSource(vertPath);

            VertexShader = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(VertexShader, VertexShaderSource);

            GL.CompileShader(VertexShader);

            string infoLogVert = GL.GetShaderInfoLog(VertexShader);
            if (infoLogVert != System.String.Empty)
                System.Console.WriteLine(infoLogVert);

            string FragmentShaderSource = LoadSource(fragPath);
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            GL.CompileShader(FragmentShader);

            string infoLogFrag = GL.GetShaderInfoLog(VertexShader);
            if (infoLogFrag != System.String.Empty)
                System.Console.WriteLine(infoLogFrag);

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            string infoLogLink = GL.GetShaderInfoLog(VertexShader);
            if (infoLogLink != System.String.Empty)
                System.Console.WriteLine(infoLogLink);

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }


        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }
        
        public void SetUniform(string Name, float x)
        {
            //TODO: Don't lookup uniform each time.
            int positionLocation = GL.GetUniformLocation(Handle, Name);

            GL.Uniform1(positionLocation, x);
        }

        public void SetUniform(string Name, Color4 x)
        {
            //TODO: Don't lookup uniform each time.
            int positionLocation = GL.GetUniformLocation(Handle, Name);
            
            GL.Uniform4(positionLocation, x);
        }

        public void SetUniform(string Name, Vector4 x)
        {
            //TODO: Don't lookup uniform each time.
            int positionLocation = GL.GetUniformLocation(Handle, Name);

            GL.Uniform4(positionLocation, x);
        }

        private string LoadSource(string path)
        {
            string readContents;

            using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8))
            {
                readContents = streamReader.ReadToEnd();
            }

            return readContents;
        }


        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }

        ~Shader()
        {
            GL.DeleteProgram(Handle);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}