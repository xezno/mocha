﻿using System.Runtime.InteropServices;
using VConsoleLib;

namespace Mocha;

public class Main
{
	private static World world;
	private static VConsoleServer vconsoleServer;

	private static void SetupFunctionPointers( IntPtr args )
	{
		Global.UnmanagedArgs = Marshal.PtrToStructure<UnmanagedArgs>( args );
	}

	[UnmanagedCallersOnly]
	public static void Run( IntPtr args )
	{
		SetupFunctionPointers( args );
		Log.Info( "Managed init" );

		vconsoleServer = new();
		Log.OnLog += ( s ) => vconsoleServer.Log( s );

		vconsoleServer.OnCommand += ( s ) =>
		{
			Log.Info( $"Command: {s}" );
		};

		ImGuiNative.igSetCurrentContext( Glue.Editor.GetContextPointer() );
		Log.Trace( $"Imgui context is {ImGuiNative.igGetCurrentContext()}" );

		// Get parent process path
		var parentProcess = System.Diagnostics.Process.GetCurrentProcess();
		var parentModule = parentProcess.MainModule;
		var parentPath = parentModule?.FileName ?? "None";
		Log.Info( $"Parent process: {parentPath}" );

		world = new World();
	}

	[UnmanagedCallersOnly]
	public static void Render()
	{
		Input.Update();
		Time.UpdateFrom( Glue.Entities.GetDeltaTime() );
		Screen.UpdateFrom( Glue.Editor.GetWindowSize() );

		if ( Time.Delta > 0.1f )
			return;

		const float threshold = 30f;

		if ( Time.FPS < threshold )
		{
			Log.Warning( $"!!! Deltatime is lower than {threshold}fps: {Time.Delta}ms ({Time.FPS}fps) !!!" );
		}

		world.Update();
	}

	[UnmanagedCallersOnly]
	public static void DrawEditor()
	{
		Editor.Editor.Draw();
	}

	public delegate void FireEventDelegate( IntPtr ptrEventName );

	[UnmanagedCallersOnly]
	public static void FireEvent( IntPtr args, int sizeBytes )
	{
		var eventName = Marshal.PtrToStringUTF8( args );

		if ( eventName == null )
			return;

		Event.Run( eventName );
	}
}