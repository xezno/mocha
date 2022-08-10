﻿using System.ComponentModel;

namespace Mocha;

[Category( "Player" ), Title( "Camera" ), Icon( FontAwesome.Camera )]
public class Camera : BaseEntity
{
	[HideInInspector]
	public SceneCamera SceneCamera { get; set; }

	private float FieldOfView
	{
		get => SceneCamera.FieldOfView;
		set => SceneCamera.FieldOfView = value;
	}

	[HideInInspector]
	public Matrix4x4 ProjMatrix => SceneCamera.ProjMatrix;

	[HideInInspector]
	public Matrix4x4 ViewMatrix => SceneCamera.ViewMatrix;

	public Camera()
	{
		SceneCamera = new( this );
	}

	public override void Update()
	{
		base.Update();

		// Apply fov
		FieldOfView = 50f;

		// Run view/proj matrix calculations
		SceneCamera.CalcViewProjMatrix();
	}
}