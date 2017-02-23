﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

namespace SB_VerticalToolMenu
{
    class VerticalToolBar : IClickableMenu
    {
        public List<ClickableComponent> buttons = new List<ClickableComponent>();
        private const int NUM_BUTTONS = 5;
        private string hoverTitle = "";
        private float transparency = 1f;
        public Rectangle toolbarTextSource = new Rectangle(0, 256, 60, 60);
        private new int yPositionOnScreen;
        public int numToolsinToolbar = 0;
        private Item hoverItem;

        public VerticalToolBar(int x, int y)
            : base(x, y, 
                  (Game1.tileSize * 3 / 2), 
                  ( (Game1.tileSize * NUM_BUTTONS) + (Game1.tileSize / 2) ), false)
        {
            for(int index = 0; index < NUM_BUTTONS; ++index)
            {
                this.buttons.Add(
                    new ClickableComponent(
                        new Rectangle(
                            xPositionOnScreen + 16,
                            yPositionOnScreen + (index * Game1.tileSize),
                            Game1.tileSize, 
                            Game1.tileSize),
                        string.Concat((object)index)));
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.player.usingTool)
                return;
            foreach (ClickableComponent button in this.buttons)
            {
                if (button.containsPoint(x, y))
                {
                    Game1.player.CurrentToolIndex = Convert.ToInt32(button.name);
                    if (Game1.player.ActiveObject != null)
                    {
                        Game1.player.showCarrying();
                        Game1.playSound("pickUpItem");
                        break;
                    }
                    Game1.player.showNotCarrying();
                    Game1.playSound("stoneStep");
                    break;
                }
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            this.hoverItem = (Item)null;
            foreach (ClickableComponent button in this.buttons)
            {
                if (button.containsPoint(x, y))
                {
                    int int32 = Convert.ToInt32(button.name);
                    if (int32 < Game1.player.items.Count && Game1.player.items[int32] != null)
                    {
                        button.scale = Math.Min(button.scale + 0.05f, 1.1f);
                        this.hoverTitle = Game1.player.items[int32].Name;
                        this.hoverItem = Game1.player.items[int32];
                    }
                }
                else
                    button.scale = Math.Max(button.scale - 0.025f, 1f);
            }
        }

        public void shifted(bool right)
        {
            if (right)
            {
                for (int index = 0; index < this.buttons.Count; ++index)
                    this.buttons[index].scale = (float)(1.0 + (double)index * 0.0299999993294477);
            }
            else
            {
                for (int index = this.buttons.Count - 1; index >= 0; --index)
                    this.buttons[index].scale = (float)(1.0 + (double)(11 - index) * 0.0299999993294477);
            }
        }

        public override void update(GameTime time)
        {
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            for (int index = 0; index < NUM_BUTTONS; ++index)
                buttons[index].bounds = new Rectangle(
                            xPositionOnScreen + 16,
                            yPositionOnScreen + (index * Game1.tileSize),
                            Game1.tileSize,
                            Game1.tileSize);
        }

        public override bool isWithinBounds(int x, int y)
        {
            return new Rectangle(
                this.buttons.First<ClickableComponent>().bounds.X, 
                this.buttons.First<ClickableComponent>().bounds.Y,
                Game1.tileSize,
                this.buttons.Last<ClickableComponent>().bounds.Y - this.buttons.First<ClickableComponent>().bounds.Y + Game1.tileSize).Contains(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            //Checks if the player is on any other menu before drawing the tooltip
            if (Game1.activeClickableMenu != null)
                return;
            //Checks and draws the buttons
            int positionOnScreen1 = this.yPositionOnScreen;
            if (Game1.options.pinToolbarToggle)
            {
                this.yPositionOnScreen = Game1.viewport.Height - getInitialHeight();
                this.transparency = Math.Min(1f, this.transparency + 0.075f);
                if ((double)Game1.GlobalToLocal(Game1.viewport, new Vector2((float)Game1.player.GetBoundingBox().Center.X, (float)Game1.player.GetBoundingBox().Center.Y)).Y > (double)(Game1.viewport.Height - Game1.tileSize * 3))
                    this.transparency = Math.Max(0.33f, this.transparency - 0.15f);
            }
            else
                this.yPositionOnScreen = (double)Game1.GlobalToLocal(Game1.viewport, new Vector2((float)Game1.player.GetBoundingBox().Center.X, (float)Game1.player.GetBoundingBox().Center.Y)).Y > (double)(Game1.viewport.Height / 2 + Game1.tileSize) ? Game1.tileSize / 8  : Game1.viewport.Height - getInitialHeight() - Game1.tileSize / 8;
            int positionOnScreen2 = this.yPositionOnScreen;
            if (positionOnScreen1 != positionOnScreen2)
            {
                for (int index = 0; index < NUM_BUTTONS; ++index)
                    this.buttons[index].bounds.Y = this.yPositionOnScreen + (index * Game1.tileSize);
            }
            //Draws the backgound texture. 
            IClickableMenu.drawTextureBox(
                b, 
                Game1.menuTexture, 
                this.toolbarTextSource, 
                xPositionOnScreen, 
                yPositionOnScreen,
                Game1.tileSize * 3 / 2,
                ((Game1.tileSize * NUM_BUTTONS) + (Game1.tileSize / 2)), 
                Color.White * this.transparency, 1f, false);
            int toolBarIndex = 0;
            for (int itemIndex = 0; itemIndex < Game1.player.items.Count; itemIndex++)
            {
                if(Game1.player.Items[itemIndex] != null && 
                    ( Game1.player.Items[itemIndex] is Axe || Game1.player.Items[itemIndex] is Hoe
                    || Game1.player.Items[itemIndex] is Pickaxe|| (Game1.player.items[itemIndex] is MeleeWeapon && (Game1.player.items[itemIndex] as MeleeWeapon).Name.Equals("Scythe"))
                    || Game1.player.Items[itemIndex] is FishingRod))
                {
                    this.buttons[toolBarIndex].scale = Math.Max(1f, this.buttons[toolBarIndex].scale - 0.025f);
                    Vector2 location = new Vector2(
                        xPositionOnScreen + 16,
                        (float)(yPositionOnScreen + (toolBarIndex * Game1.tileSize + 16)));
                    b.Draw(Game1.menuTexture, location, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, Game1.player.CurrentToolIndex == itemIndex ? 56 : 10, -1, -1)), Color.White * transparency);
                    // Need to customize it for toolset //string text = index == 9 ? "0" : (index == 10 ? "-" : (index == 11 ? "=" : string.Concat((object)(index + 1))));
                    //b.DrawString(Game1.tinyFont, text, position + new Vector2(4f, -8f), Color.DimGray * this.transparency);
                    Game1.player.items[itemIndex].drawInMenu(b, location, Game1.player.CurrentToolIndex == itemIndex ? 0.9f : this.buttons.ElementAt<ClickableComponent>(toolBarIndex).scale * 0.8f, this.transparency, 0.88f);
                    buttons[toolBarIndex].name = string.Concat((object)itemIndex);
                    toolBarIndex++;
                    if (toolBarIndex >= NUM_BUTTONS) break;
                }
            }
            if (toolBarIndex != numToolsinToolbar)
                numToolsinToolbar = toolBarIndex;
            //If an item is hovered, shows its tooltip.
            if (this.hoverItem == null)
                return;
            IClickableMenu.drawToolTip(b, this.hoverItem.getDescription(), this.hoverItem.Name, this.hoverItem, false, -1, 0, -1, -1, (CraftingRecipe)null, -1);
            this.hoverItem = (Item)null;
        }

        public static int getInitialWidth()
        {
            return (Game1.tileSize * 3 / 2) ;
        }

        public static int getInitialHeight()
        {
            return ((Game1.tileSize * NUM_BUTTONS) + (Game1.tileSize / 2));
        }
    }
}
