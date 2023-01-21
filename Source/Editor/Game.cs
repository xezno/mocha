﻿namespace Mocha.Editor;

public class Game : IGame
{
	public void FrameUpdate()
	{
		DebugOverlay.Render();
		Editor.Draw();
	}

	public void Shutdown()
	{
	}

	public void Startup()
	{
		ImGuiNative.igSetCurrentContext( Glue.Editor.GetContextPointer() );
	}

	public void Update()
	{
	}
}