using System;
using System.Linq;
using TommoJProductions.Debugging;
using UnityEngine;

namespace TommoJProductions.MoControls
{
    public class AssetHolder
    {
        public Texture2D[] textures
        {
            get;
            set;
        }
        public Texture2D blankKey
        {
            get;
            set;
        }
        public Texture2D controllerIcon
        {
            get;
            set;
        }
        public Texture2D mouseIcon
        {
            get;
            set;
        }
        public Texture2D a
        {
            get;
            set;
        }
        public Texture2D b
        {
            get;
            set;
        }
        public Texture2D x
        {
            get;
            set;
        }
        public Texture2D y
        {
            get;
            set;
        }
        public Texture2D start
        {
            get;
            set;
        }
        public Texture2D back
        {
            get;
            set;
        }
        public Texture2D lb
        {
            get;
            set;
        }
        public Texture2D rb
        {
            get;
            set;
        }
        public Texture2D ls
        {
            get;
            set;
        }
        public Texture2D rs
        {
            get;
            set;
        }
        public Texture2D lt
        {
            get;
            set;
        }
        public Texture2D rt
        {
            get;
            set;
        }
        public Texture2D dp
        {
            get;
            set;
        }
        public Texture2D dpup
        {
            get;
            set;
        }
        public Texture2D dpdown
        {
            get;
            set;
        }
        public Texture2D dpleft
        {
            get;
            set;
        }
        public Texture2D dpright
        {
            get;
            set;
        }
        public Texture2D lsup
        {
            get;
            set;
        }
        public Texture2D lsdown
        {
            get;
            set;
        }
        public Texture2D lsleft
        {
            get;
            set;
        }
        public Texture2D lsright
        {
            get;
            set;
        }
        public Texture2D rsup
        {
            get;
            set;
        }
        public Texture2D rsdown
        {
            get;
            set;
        }
        public Texture2D rsleft
        {
            get;
            set;
        }
        public Texture2D rsright
        {
            get;
            set;
        }
        public Texture2D rsPress
        {
            get;
            set;
        }
        public Texture2D lsPress
        {
            get;
            set;
        }

        public AssetHolder(UnityEngine.Object[] inAssets)
        {
            // Written, 15.10.2018

            Texture2D[] textures = new Texture2D[inAssets.Length];
            for (int i = 0; i < inAssets.Length; i++)
            {
                if (inAssets[i] is Texture2D)
                {
                    textures[i] = inAssets[i] as Texture2D;
                    if (textures[i] is null)
                        MoControlsMod.print("<color=red>Unsuccessfully</color> loaded texture, texure is null" + textures[i].name, DebugTypeEnum.full);
                    else
                        MoControlsMod.print("<color=green>successfully</color> loaded texture, " + textures[i].name, DebugTypeEnum.full);
                }
                else
                {
                    MoControlsMod.print("<color=red>Error loading texture</color>, <color=grey><b><i>" + (inAssets[i]?.name ?? "[Asset name null]") + ".</i></b></color>", DebugTypeEnum.full);
                }
            }
            this.textures = textures;

            try
            {
                // setting textures.
                a = this.textures.First(_texture => _texture.name == "xc_a");
                b = this.textures.First(_texture => _texture.name == "xc_b");
                x = this.textures.First(_texture => _texture.name == "xc_x");
                y = this.textures.First(_texture => _texture.name == "xc_y");
                back = this.textures.First(_texture => _texture.name == "xc_back");
                start = this.textures.First(_texture => _texture.name == "xc_start");
                lb = this.textures.First(_texture => _texture.name == "xc_lb");
                rb = this.textures.First(_texture => _texture.name == "xc_rb");
                lt = this.textures.First(_texture => _texture.name == "xc_lt");
                rt = this.textures.First(_texture => _texture.name == "xc_rt");
                dp = this.textures.First(_texture => _texture.name == "xc_dp");
                dpup = this.textures.First(_texture => _texture.name == "xc_dpu");
                dpdown = this.textures.First(_texture => _texture.name == "xc_dpd");
                dpleft = this.textures.First(_texture => _texture.name == "xc_dpl");
                dpright = this.textures.First(_texture => _texture.name == "xc_dpr");
                ls = this.textures.First(_texture => _texture.name == "xc_ls");
                lsup = this.textures.First(_texture => _texture.name == "xc_lsu");
                lsdown = this.textures.First(_texture => _texture.name == "xc_lsd");
                lsleft = this.textures.First(_texture => _texture.name == "xc_lsl");
                lsright = this.textures.First(_texture => _texture.name == "xc_lsr");
                rs = this.textures.First(_texture => _texture.name == "xc_rs");
                rsup = this.textures.First(_texture => _texture.name == "xc_rsu");
                rsdown = this.textures.First(_texture => _texture.name == "xc_rsd");
                rsleft = this.textures.First(_texture => _texture.name == "xc_rsl");
                rsright = this.textures.First(_texture => _texture.name == "xc_rsr");
                controllerIcon = this.textures.First(_texture => _texture.name == "xc_icon");
                mouseIcon = this.textures.First(_texture => _texture.name == "mouse_icon");
                blankKey = this.textures.First(_texture => _texture.name == "blank_key");
                lsPress = this.textures.First(_texture => _texture.name == "xc_ls_press");
                rsPress = this.textures.First(_texture => _texture.name == "xc_rs_press");
            }
            catch (NullReferenceException ex)
            {
                MoControlsMod.print("<color=red>Error occured when setting asset textures...</color>.\r\n<b>Message:</b>" + ex.Message + "\r\nStacktrace:\r\n" + ex.StackTrace, DebugTypeEnum.full);
                throw;
            }

            if (a == null || b == null || x == null || y == null || lb == null || rb == null || back == null || start == null || ls == null ||
                rs == null || lt == null || rt == null || dp == null || dpdown == null || dpup == null || dpleft == null || dpright == null ||
                lsup == null || lsdown == null || lsleft == null || lsright == null || rsdown == null || rsup == null || rsleft == null || rsright == null ||
                controllerIcon == null || mouseIcon == null || blankKey == null)
            {
                MoControlsMod.print("<color=red>Error</color> - could not find one or more textures by name. possible asset name change.", DebugTypeEnum.none);
            }
        }
    }
}
