using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using System.Linq;
using HeroCommon;
using System.Collections.Concurrent;

namespace Hero
{
    class DisplayWindow : GameWindow
    {
        float[] vertices = {
             0f,  0f,    //Bottom-left vertex
             1f,  0f,    //Bottom-right vertex
             1f,  1f, //Top-right vertex
             0f,  1f, //Top-left vertex
        };
        
        int VertexBufferObject;
        int VertexArrayObject;
        Shader shader;

        Song currentSong;
        public Song CurrentSong
        {
            get => currentSong;
            set
            {
                currentSong = value;
                moveRate = currentSong.Bpm / 60.0 * currentSong.TicksPerQuarterNote;
                noteIterator = currentSong.Notes.SelectMany((x) => x).GetEnumerator();
                noteIterator.MoveNext();
            }
        }

        public ConcurrentDictionary<PianoKey, byte> PressedKeys;


        IEnumerator<SongEntry> noteIterator;

        List<SongEntry> playingEntries = new List<SongEntry>();

        bool TimeAdvancing = true;

        bool playing = true;

        //Settings.
        bool autoplay = true;
        bool pauseForLeftHand = true; //Only pause for left hand.

        double currentTime = -10; //Start 5 seconds before song.
        double moveRate = 0;

        public DisplayWindow(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) { }
        
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.ClearDepth(0);

            VertexBufferObject = GL.GenBuffer();


            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            shader = new Shader("shader.vert", "shader.frag");
            
            
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            GL.EnableVertexAttribArray(0);


            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Greater);
            GL.DepthRange(-0.01f, 1.0f);
            
            base.OnLoad(e);
        }

        private PianoPlayer soundPlayer;
        public PianoPlayer SoundPlayer
        {
            get => soundPlayer;
            set => soundPlayer = value;
        }

        private void RenderSquare(float depth, Color4 color, float x, float y, float w, float h )
        {
            if (x > 1.0 || x + w < 0.0 ||
                y > 1.0 || y + h < 0.0)
            {
                return;
            }

            //TODO: Batch rendering.
            shader.SetUniform("aDepth", depth);
            shader.SetUniform("aLocation", new Vector4(x, y, w, h));
            shader.SetUniform("exportColor", color);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
        }

        private float getXOfNote(PianoKey key)
        {
            //TODO: Calculate once.
            int octaveCount = 8;
            float octaveWidth = 1.0f / octaveCount;

            int noteCount = 7;
            float noteWidth = octaveWidth / noteCount;

            float x = octaveWidth * key.Octave;

            bool isSharp = false;
            int keyId = 0;

            //TODO: make this neater.
            switch (key.Note)
            {
                case Note.A:
                    keyId = 0;
                    break;
                case Note.As:
                    keyId = 0;
                    isSharp = true;
                    break;
                case Note.B:
                    keyId = 1;
                    break;
                case Note.C:
                    keyId = 2;
                    break;
                case Note.Cs:
                    keyId = 2;
                    isSharp = true;
                    break;
                case Note.D:
                    keyId = 3;
                    break;
                case Note.Ds:
                    keyId = 3;
                    isSharp = true;
                    break;
                case Note.E:
                    keyId = 4;
                    break;
                case Note.F:
                    keyId = 5;
                    break;
                case Note.Fs:
                    keyId = 5;
                    isSharp = true;
                    break;
                case Note.G:
                    keyId = 6;
                    break;
                case Note.Gs:
                    keyId = 6;
                    isSharp = true;
                    break;
            }

            x += noteWidth * keyId;
            if (isSharp)
            {
                x += noteWidth / 2;
            }

            return x;
        }

        private float GetPlayOffset(long songOffset, float timeOffset)
        {
            float r = (float)(moveRate);

            float yOffset = (float)(((double)songOffset) / r);

            float location = yOffset - timeOffset;

            return location;
        }


        private Tuple<float, float> GetNoteLocation(SongEntry item, float offset)
        {
            float r = (float)(moveRate);

            float yOffset = (float)(((double)item.EventStart) / r);
            float height = (float)(((double)item.EventDuration) / r);

            float location = yOffset - offset;

            return new Tuple<float, float>(location, location + height);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            var keys = PressedKeys.Select((x) => x.Key).ToHashSet();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            shader.Use();
            GL.BindVertexArray(VertexArrayObject);


            //TODO: Calculate once.
            int octaveCount = 8;
            float octaveWidth = 1.0f / octaveCount;

            int noteCount = 7;
            float noteWidth = octaveWidth / noteCount;
            
            foreach (var item in CurrentSong.Notes.SelectMany((x) => x))
            {
                const float border = 0.0040f;

                int oCompare = item.Key.Octave + (item.Key.KeyIndex <= 1 ? 1 : 0);

                var newKey = new PianoKey(oCompare, item.Key.Note);

                float xOffset = getXOfNote(newKey);

                var noteLocation = GetNoteLocation(item, (float)currentTime);

                var height = noteLocation.Item2 - noteLocation.Item1;

                RenderSquare(0.25f, Color4.ForestGreen,
                        xOffset + border, ((noteLocation.Item1) / 8 ) + 0.15f, noteWidth - (border * 2), height / 8);
            }

            RenderSquare(0.45f, Color4.DarkGray,
                        0, 0, 1.0f, 0.15f);

            for (int o = 0; o < octaveCount; o++)
            {
                float octaveOffset = octaveWidth * o;
                for (int n = 0; n < noteCount; n++)
                {
                    float noteOffset = noteWidth * n;

                    Color4 drawcol;
                    int oCompare = o + (n <= 1 ? -1 : 0);
                    if (keys.FirstOrDefault((x) => x.IsSharp == false && x.Octave == oCompare && x.KeyIndex == n) != null)
                    {
                        drawcol = Color4.LightBlue;
                    }
                    else if (playingEntries.FirstOrDefault((x) => x.Key.IsSharp == false && x.Key.Octave == oCompare && x.Key.KeyIndex == n) != null)
                    {
                        drawcol = Color4.Red;
                    }
                    else
                    {
                        drawcol = Color4.LightGray;
                    }

                    const float border = 0.0020f;
                    RenderSquare(0.5f, drawcol,
                        octaveOffset + noteOffset + (border/2), 0.0f, noteWidth - border, 0.15f);

                    if (n == 0 || (n > 1 && n < 4) || n > 4)
                    {
                        if (n != noteCount - 1 || oCompare != octaveCount - 1)
                        {
                            if (keys.FirstOrDefault((x) => x.IsSharp == true && x.Octave == oCompare && x.KeyIndex == n) != null)
                            {
                                drawcol = Color4.DarkBlue;
                            }
                            else if (playingEntries.FirstOrDefault((x) => x.Key.IsSharp == true && x.Key.Octave == oCompare && x.Key.KeyIndex == n) != null)
                            {
                                drawcol = Color4.Maroon;
                            }
                            else
                            {
                                drawcol = Color4.Black;
                            }

                            RenderSquare(0.55f, drawcol,
                                octaveOffset + noteOffset + (border / 2) + (noteWidth / 2), 0.05f, noteWidth - border, 0.10f);
                        }
                    }
                }

            }
            
            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }
        
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var keys = PressedKeys.Select((x) => x.Key).ToHashSet();
            
            if (TimeAdvancing)
            {
                currentTime += e.Time;
            }

            while (playing && noteIterator.Current != null && GetPlayOffset(noteIterator.Current.EventStart, (float)currentTime) < 0)
            {
                if (!autoplay)
                {
                    bool isInLeftHandRange = noteIterator.Current.Key.Octave > 4 || (noteIterator.Current.Key.Octave == 4 && noteIterator.Current.Key.Note >= Note.E);

                    if (isInLeftHandRange || !pauseForLeftHand)
                    {
                        if (keys.Contains(noteIterator.Current.Key))
                        {
                            TimeAdvancing = true;
                        }
                        else
                        {
                            TimeAdvancing = false;
                            return;
                        }
                    }
                }

                playingEntries.Add(noteIterator.Current);
                soundPlayer.PlaySound(noteIterator.Current.Key);

                playing = noteIterator.MoveNext();
            }

            playingEntries = playingEntries.Where((x) => GetNoteLocation(x, (float)currentTime).Item2 > 0).ToList();
            
            base.OnUpdateFrame(e);
        }


        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);

            shader.Dispose();
            base.OnUnload(e);
        }
    }
}