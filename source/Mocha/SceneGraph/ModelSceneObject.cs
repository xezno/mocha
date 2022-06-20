﻿namespace Mocha.Renderer;

public class ModelSceneObject : SceneObject
{
    public List<Model> models;

    public Matrix4x4 ModelMatrix
    {
        get
        {
            var matrix = Matrix4x4.CreateScale( Entity.Transform.Scale );
            matrix *= Matrix4x4.CreateTranslation( Entity.Transform.Position );
            matrix *= Matrix4x4.CreateFromQuaternion( Entity.Transform.Rotation.GetSystemQuaternion() );
            return matrix;
        }
    }

    public ModelSceneObject( IEntity entity ) : base( entity )
    {
    }

    public void SetModels( List<Model> models )
    {
        this.models = models;
    }

    public override void Render( CommandList commandList )
    {
        var currentCamera = SceneWorld.Current.Camera;

        var uniformBuffer = new GenericModelUniformBuffer
        {
            g_mModel = ModelMatrix,
            g_mView = currentCamera.ViewMatrix,
            g_mProj = currentCamera.ProjMatrix,
            g_flTime = Time.Now,

            g_vSunLightDir = SceneWorld.Current.Sun.Transform.Rotation.Forward,
            g_vSunLightColor = SceneWorld.Current.Sun.Color.ToVector4(),
            g_flSunLightIntensity = SceneWorld.Current.Sun.Intensity,
            g_vCameraPos = currentCamera.Transform.Position,

            _padding1 = 0,
            _padding2 = 0
        };

        models.ForEach( x => x.Draw( uniformBuffer, commandList ) );
    }
}