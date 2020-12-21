using Microsoft.Xna.Framework;

namespace Shiftnet.Dialog
{
    public abstract class DialogAction
    {
        public bool Completed { get; private set; }
        
        public DialogPlayer DialogPlayer { get; set; }

        public void Update(GameTime gameTime)
        {
            if (!Completed)
            {
                Completed = OnUpdate(gameTime);
            }
        }

        protected abstract bool OnUpdate(GameTime gameTime);
    }
}