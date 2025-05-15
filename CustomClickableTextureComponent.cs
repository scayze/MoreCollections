using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace CollectionsMod
{
    internal class CustomClickableTextureComponent : ClickableTextureComponent
    {
        Rectangle dyedSourceRect;
        public CustomClickableTextureComponent(string name, Rectangle bounds, string label, string hoverText, Texture2D texture, Rectangle sourceRect, Rectangle dyedSourceRect, float scale, bool drawShadow = false)
            : base(name, bounds, label, hoverText, texture, sourceRect, scale, drawShadow)
        {
            this.dyedSourceRect = dyedSourceRect;
        }

        public override void draw(SpriteBatch b, Color c, float layerDepth, int frameOffset = 0, int xOffset = 0, int yOffset = 0)
        {
            base.draw(b, c, layerDepth, frameOffset, xOffset, yOffset);

            if (texture != null)
            {
                Utility.drawWithShadow(b, texture, new Vector2((bounds.X + xOffset) + (dyedSourceRect.Width / 2) * baseScale, (bounds.Y + yOffset) + (dyedSourceRect.Height / 2) * baseScale), dyedSourceRect, c, 0f, new Vector2(dyedSourceRect.Width / 2, dyedSourceRect.Height / 2), scale, flipped: false, layerDepth, -1, -1, 0);
            }
        }
    }
}
