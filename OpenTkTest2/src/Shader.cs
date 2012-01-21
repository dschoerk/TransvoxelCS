using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

namespace OpenTkTest2
{
    public class Shader
    {
        string vertexShaderSource = @"
#version 130

precision highp float;

uniform mat4 world_matrix;
uniform mat4 projection_matrix;
uniform mat4 view_matrix;

in vec3 in_position;
in vec3 in_normal;
out vec3 normal;

void main(void)
{
  mat4 modelview_mat = view_matrix * world_matrix;
  mat4 mvp_mat = projection_matrix * modelview_mat;
  
  gl_Position = mvp_mat * vec4(in_position, 1);
  normal = normalize(in_normal);//vec3(modelview_mat * vec4(in_normal.xyz,0));
  
}";

        string fragmentShaderSource = @"
#version 130

precision highp float;

in vec3 normal;
out vec4 out_frag_color;

void main(void)
{
   vec3 absNormal = abs(normal);
   vec3 blend_weights = absNormal;
   blend_weights = blend_weights - 0.2679f;
   blend_weights = max(blend_weights, 0);

   blend_weights /= (blend_weights.x + blend_weights.y + blend_weights.z);
    
   vec4 col1 = vec4(0.5,0.2,0.0,1.0); //x
   vec4 col2 = vec4(0.0,1.0,0.0,1.0); //y
   if(normal.y < 0)
   {
      col2 = col1;
   }
   vec4 col3 = vec4(0.5,0.2,0.0,1.0); //z

   vec4 blended_color = vec4(col1.xyzw * blend_weights.xxxx + col2.xyzw * blend_weights.yyyy + col3.xyzw * blend_weights.zzzz);
   
   vec4 light = -normalize(vec4(-1,-1,-1,0));
   float ldn = max(0, dot(light, vec4(normal,1)));
   float ambient = 0.2f;



   out_frag_color = blended_color * (ambient + ldn);; //vec4(normal.x,normal.y,normal.z,1.0);////
}";

       public int vertexShaderHandle,
            fragmentShaderHandle,
            shaderProgramHandle;

        public void CreateShader()
        {
            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShaderHandle, vertexShaderSource);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderSource);

            GL.CompileShader(vertexShaderHandle);
            GL.CompileShader(fragmentShaderHandle);

            Debug.WriteLine(GL.GetShaderInfoLog(vertexShaderHandle));
            Debug.WriteLine(GL.GetShaderInfoLog(fragmentShaderHandle));


            // Create program
            shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);

            GL.BindAttribLocation(shaderProgramHandle, 0, "in_position");
            GL.BindAttribLocation(shaderProgramHandle, 1, "in_normal");

            GL.LinkProgram(shaderProgramHandle);
          
            Debug.WriteLine(GL.GetProgramInfoLog(shaderProgramHandle));
        }

        public void Use()
        {
            GL.UseProgram(shaderProgramHandle);
            activeProgram = shaderProgramHandle;
        }

        private static int activeProgram=0;
        public void UnUse()
        {
            activeProgram = 0;
            GL.UseProgram(0);
        }

        public bool isActive()
        {
            return (shaderProgramHandle == activeProgram);
        }

        private int getUniformLocation(string name)
        {
          //  int location = uniformDictCache.ContainsKey(name) ? uniformDictCache[name] : -1;
          //  if (location == -1)
          //  {
          int      location = GL.GetUniformLocation(shaderProgramHandle, name);
          //      uniformDictCache.Add(name, location);
         //   }
            
         //   Debug.WriteIf(location == -1, "Uniform: " + name + " wurde nicht gefunden");
            return location;
        }

        public void setUniformMatrix4(string uniform,ref Matrix4 matrix)
        {
            if(!isActive())
                GL.UseProgram(shaderProgramHandle);
            
            int location = getUniformLocation(uniform);
            Debug.WriteIf(location==-1,"uniform "+uniform+" existiert nicht");
            GL.UniformMatrix4(location, false, ref matrix);

            if (!isActive())
                GL.UseProgram(activeProgram);
        }

        public Shader()
        {
            
        }
    }
}
