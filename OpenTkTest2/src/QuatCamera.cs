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
	public class QuatCamera
	{
		private Matrix4 projectionMatrix;
		private Matrix4 viewMatrix;
		
		//Cam parameters
		float aspectRatio = 800/600;
		float fov = (float)Math.PI / 4;
		float nearClipPlane = 1.0f;
		float farClipPlane = 10000.0f;

		Vector3 upVec;
		Vector3 camPos;
		Quaternion rotation;

		public QuatCamera(float ratio)
		{
			upVec = new Vector3(0,1,0);
			rotation = Quaternion.FromAxisAngle(upVec,0.0f);
			camPos = new Vector3(20, 10, 50);

			this.aspectRatio = ratio;
			Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, nearClipPlane, farClipPlane, out projectionMatrix);
			updateViewMatrix();
		}

		internal void updateViewMatrix()
		{
			Vector3 dir = Vector3.Transform(-Vector3.UnitZ, rotation);
			dir = Vector3.Normalize(dir); //needed?
			viewMatrix = Matrix4.LookAt(camPos, camPos+dir, upVec);
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
			Vector3.Transform(ref translation, ref rotation, out translation);
			camPos += translation;
			updateViewMatrix();
		}

		public void RotateAxis(Vector3 axis,float angle)
		{
			Vector3.Transform(ref axis, ref rotation, out axis);
			rotation = Quaternion.FromAxisAngle(axis, angle)*rotation;
			updateViewMatrix();
		}
	}
}
