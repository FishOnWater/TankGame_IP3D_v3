using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankGame_IP3D
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ClsBattlefield terreno;
        TankClass tanque;
        Camera camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            terreno = new ClsBattlefield(GraphicsDevice, Content);
            camera = new Camera(GraphicsDevice);
            tanque = new TankClass(GraphicsDevice, Content, terreno);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            tanque.UpdateTankStuff(Keyboard.GetState(), terreno);
            camera.UpdateCameraPositionTankFollow(tanque, Keyboard.GetState());
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            terreno.Draw(GraphicsDevice, camera.view);
            tanque.Draw(camera.view, terreno);
            base.Draw(gameTime);
        }
    }
}