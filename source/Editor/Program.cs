﻿global using static Mocha.Engine.Global;
global using Matrix4x4 = System.Numerics.Matrix4x4;
global using Vector4 = System.Numerics.Vector4;
global using EditorUI = Mocha.Glue.EditorUI;

using System.Runtime.InteropServices;

namespace Mocha.Engine;

public class Program
{
	private static Editor editor;

	[UnmanagedCallersOnly]
	public static void HostedMain( IntPtr args )
	{
		SetupFunctionPointers( args );

		editor = new();
	}

	[UnmanagedCallersOnly]
	public static void Render()
	{
		editor.Render();
	}

	private static void FilesystemTest()
	{
		var fs = new FileSystem();
		var text = fs.ReadAllText( "materials/dev/dev_floor.mat" );
		Log.Info( text );
	}

	private static void SetupFunctionPointers( IntPtr args )
	{
		Common.Global.UnmanagedArgs = Marshal.PtrToStructure<UnmanagedArgs>( args );
		Log.NativeLogger = new Glue.CLogger();
	}
}