﻿using Mocha.Renderer.UI;

namespace Mocha.Engine.Editor;

internal class Selectable : Button
{
	public Selectable( string text, Action? onClick = null ) : base( text, onClick )
	{
	}

	bool mouseWasDown = false;

	internal override void Render()
	{
		Vector4 colorA = new( 0, 0, 0, 0.1f );
		Vector4 colorB = new( 0, 0, 0, 0.2f );

		Vector4 border = ITheme.Current.Border;
		label.Color = ITheme.Current.TextColor;

		if ( InputFlags.HasFlag( PanelInputFlags.MouseOver ) )
		{
			if ( InputFlags.HasFlag( PanelInputFlags.MouseDown ) )
			{
				Graphics.DrawRect( Bounds.Shrink( new( 1f, 2f ) ),
					colorB * 1.25f,
					colorA * 1.25f,
					colorB * 1.25f,
					colorA * 1.25f
				);

				mouseWasDown = true;
			}
			else
			{
				Graphics.DrawRect( Bounds.Shrink( new( 1f, 2f ) ), Colors.Blue );
				Graphics.DrawRect( Bounds.Shrink( new( 1f, 2f ) ),
					colorA * 0.5f,
					colorB * 0.5f,
					colorA * 0.5f,
					colorB * 0.5f
				);

				if ( mouseWasDown )
				{
					OnClick?.Invoke();
				}

				mouseWasDown = false;

				label.Color = new System.Numerics.Vector4( 0, 0, 0, 1 );
			}
		}

		UpdateLabel();
	}
}