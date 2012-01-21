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
	public class Camera
	{
		private Matrix4 projectionMatrix;
		private Matrix4 viewMatrix;
		
		//Cam parameters
		float aspectRatio = 800/600;
		float fov = (float)Math.PI / 4;
		float nearClipPlane = 1.0f;
		float farClipPlane = 100.0f;

		Vector3 upVec = Vector3.UnitY;
		Vector3 camPos = new Vector3(0, 3, 15);
		Vector3 targetPos = Vector3.Zero;

		public Camera(float ratio)
		{
			this.aspectRatio = ratio;
			Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, nearClipPlane, farClipPlane, out projectionMatrix);
			updateViewMatrix();

			Matrix4 p = projectionMatrix;
			p.Transpose();
			Console.WriteLine(p);
			Console.WriteLine();
		
			p = viewMatrix;
			p.Transpose();
			Console.WriteLine(p);

		}

		internal void updateViewMatrix()
		{
			viewMatrix = Matrix4.LookAt(camPos, targetPos, upVec);
		}

		internal Matrix4 getModelViewProjectionMatrix()
		{
			return Matrix4.Mult(viewMatrix,projectionMatrix);
		}

		internal Matrix4 getProjectionMatrix()
		{
			return projectionMatrix;
		}

		internal Matrix4 getViewMatrix()
		{
			return viewMatrix;
		}

		internal void setViewMatrix(Matrix4 mv)
		{
			viewMatrix = mv;
		}

		public void Translate(Vector3 translation)
		{
			Vector3 dir = targetPos - camPos;
			dir.Normalize();

			translation.X *= (float)Math.Sin(dir.Y);

			camPos += translation;
			targetPos += translation;
			updateViewMatrix();
		}

		public void RotateY(Vector3 angle)
		{
			Vector3 dir = targetPos - camPos;
			dir = Vector3.Transform(dir, Matrix4.CreateRotationX(angle.X)*Matrix4.CreateRotationY(angle.Y)*Matrix4.CreateRotationZ(angle.Z));
			targetPos = dir + camPos;
			updateViewMatrix();
		}
	}
}
