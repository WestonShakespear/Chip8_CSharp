using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Program
{
    public class Game : GameWindow
    {

        uint[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        const int SCREEN_X = 64;
        const int SCREEN_Y = 32;

        const float PIXEL_PADDING = 0.25f;

        const float FULL_PIXEL_WIDTH = (2.0f / SCREEN_X) - ((2*((2.0f / SCREEN_X)*PIXEL_PADDING)/SCREEN_X));
        const float FULL_PIXEL_HEIGHT = (2.0f / SCREEN_Y) - ((2*((2.0f / SCREEN_Y)*PIXEL_PADDING)/SCREEN_Y));

        const float PADDING_WIDTH = (2*(FULL_PIXEL_WIDTH*PIXEL_PADDING));
        const float PADDING_HEIGHT = (2*(FULL_PIXEL_HEIGHT*PIXEL_PADDING));

        const float PIXEL_WIDTH = FULL_PIXEL_WIDTH - PADDING_WIDTH;
        const float PIXEL_HEIGHT = FULL_PIXEL_HEIGHT - PADDING_HEIGHT;
        
        const float BACKR = 0.0f;
        const float BACKG = 0.169f;
        const float BACKB = 0.218f;

        const int fps = 1000;
        const int key_freq = 60;
        const int cycle_freq = 60;

        int SCREEN_TEST = 0;
        byte SCREEN_VAL = 0;

        int run = 0;



        // int VertexDataBufferObject;
        // int ElementBufferObject;
        // int VertexArrayObject;
        

        // int VertexDataBufferObject2;
        // int VertexArrayObject2;

        Shader shader;
        // Shader shader2;

        Chip chip;

        System.Diagnostics.Stopwatch watch;
        int current_frame = 0;
        int frame_counter = 0;




        int[] VertexBuffers;
        int[] ElementBuffers;
        int[] VertexArrays;

        public Game(int width, int height, string title, Chip input_chip) : base(
            GameWindowSettings.Default,
            new NativeWindowSettings() { Size = (width, height), Title = title }
            )
        {
            shader = new Shader("shader.vert", "shader.frag");

            watch = System.Diagnostics.Stopwatch.StartNew();

            this.UpdateFrequency = fps;

            VertexBuffers = new int[SCREEN_X * SCREEN_Y];
            ElementBuffers = new int[SCREEN_X * SCREEN_Y];
            VertexArrays = new int[SCREEN_X * SCREEN_Y];


            chip = input_chip;
            if (SCREEN_TEST == 1)
            {
                chip.TestSCREEN(SCREEN_VAL);
            }
            
            

            

            // shader2 = new Shader("shader.vert", "shader2.frag");
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            watch.Stop();
            // Console.WriteLine(1000 / watch.ElapsedMilliseconds);
            watch.Reset();
            watch.Start();

            
            if (KeyboardState.IsKeyPressed(Keys.Up))
            {
                // Console.Write("up..");
                chip.Cycle();
            }

            if (KeyboardState.IsKeyPressed(Keys.Right))
            {
                // Console.Write("up..");
                if (run == 0)
                {
                    run = 1;
                }
                else
                {
                    run = 0;
                }
            }

            // Run cycle
            if (run == 1)
            {
                chip.Cycle();
            }
            

            if (current_frame % (fps / key_freq) == 0)
            {
                
                if (KeyboardState.IsKeyDown(Keys.Down))
                {
                    chip.Cycle();
                }
            }

            if (current_frame % (fps / cycle_freq) == 0)
            {
                if (SCREEN_TEST ==  1)
                {
                    if (SCREEN_VAL == 0)
                    {
                        SCREEN_VAL = 1;
                    }
                    else
                    {
                        SCREEN_VAL = 0;
                    }
                    chip.TestSCREEN(SCREEN_VAL);
                }
                else
                {
                    
                }
            }

            // Console.WriteLine(++current_frame);
            current_frame++;
            if (current_frame == fps)
            {
                // Console.WriteLine("cycle");
                current_frame = 0;
                frame_counter++;
            }


           
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Set the background color
            

            GL.ClearColor(BACKR, BACKG, BACKB, 1.0f);

            float baseX = -1.0f + PADDING_WIDTH;
            float baseY = 1.0f - PADDING_HEIGHT;

            // Load the shader
            

            for (int y = 0; y < SCREEN_Y; y++)
            {
                for (int x = 0; x < SCREEN_X; x++)
                {
                    float c_x = baseX + (x * FULL_PIXEL_WIDTH);
                    float c_y = baseY - (y * FULL_PIXEL_HEIGHT);
                    int index = (y * SCREEN_X) + x;

                    float[] vertices = MakeSquare(c_x, c_y, PIXEL_WIDTH, PIXEL_HEIGHT);
            
                    VertexArrays[index] = GL.GenVertexArray();
                    GL.BindVertexArray(VertexArrays[index]);

                    // Create and bind buffer for vertex data
                    VertexBuffers[index] = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffers[index]);

                    // Now that it's bound, load with data
                    GL.BufferData(
                        BufferTarget.ArrayBuffer,
                        vertices.Length * sizeof(float),
                        vertices,
                        BufferUsageHint.StaticDraw);

                    

                    

                    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                    GL.EnableVertexAttribArray(0);

                    ElementBuffers[index] = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBuffers[index]);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
                }
            }

            

        }

        

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            //vec4(0.992f, 0.964f, 0.89f, 1.0f)

            


            
            shader.Use();
            int vertexColorLocation = GL.GetUniformLocation(shader.Handle, "ourColor");
            

            for (int y = 0; y < SCREEN_Y; y++)
            {
                for (int x = 0; x < SCREEN_X; x++)
                {
                    if (Chip.SCREEN_MEM[(y*SCREEN_X)+x] == 0)
                    {
                        GL.Uniform4(vertexColorLocation, BACKR, BACKG, BACKB, 0.0f);
                    }
                    else
                    {
                        GL.Uniform4(vertexColorLocation, 0.992f, 0.964f, 0.89f, 1.0f);
                    }
                    


                    GL.BindVertexArray(VertexArrays[(y*SCREEN_X)+x]);
                    GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
                }
            }
            // GL.DrawArrays(PrimitiveType.Triangles, 0, 3);



            // Last
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            shader.Dispose();
            // shader2.Dispose();
        }



        public float[] MakeSquare(float origin_x, float origin_y, float x_len, float y_len)
        {
            float[] vert =
            {
                origin_x + x_len, origin_y, 0.0f,  // top right
                origin_x + x_len, origin_y - y_len, 0.0f,  // bottom right
                origin_x, origin_y - y_len, 0.0f,  // bottom left
                origin_x, origin_y, 0.0f   // top left
            };

            return vert;
        }
    }

    
}