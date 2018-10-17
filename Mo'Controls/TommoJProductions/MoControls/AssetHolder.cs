using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TommoJProductions.MoControls.Debugging;
using UnityEngine;

namespace TommoJProductions.MoControls
{
    public class AssetHolder
    {
        public bool result
        {
            get;
            private set;
        }
        public Texture2D[] textures
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

        public AssetHolder(UnityEngine.Object[] assets)
        {
            // Written, 15.10.2018

            if (assets.Count() == 25)
            {
                Texture2D[] textures = new Texture2D[assets.Length];
                for (int i = 0; i < assets.Length; i++)
                {
                    if (assets[i] is Texture2D)
                    {
                        textures[i] = assets[i] as Texture2D;
                        if (MoControlsMod.debugTypeEquals(DebugTypeEnum.full))
                            if (textures[i] is null)
                                MoControlsMod.print("<color=red>Unsuccessfully</color> loaded texture, texure is null" + textures[i].name);
                            else
                                MoControlsMod.print("<color=green>successfully</color> loaded texture, " + textures[i].name);
                    }
                    else
                    {
                        if (MoControlsMod.debugTypeEquals(DebugTypeEnum.full))
                            MoControlsMod.print("<color=red>Error loading texture</color>, <color=grey><b><i>" + (assets[i]?.name ?? "[Asset name null]") + ".</i></b></color>");
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

                    if (a is null || b is null || x is null || y is null || lb is null ||
                        rb is null || back is null || start is null || ls is null ||
                        rs is null || lt is null || rt is null || dp is null ||
                        dpdown is null || dpup is null || dpleft is null || dpright is null ||
                        lsup is null || lsdown is null || lsleft is null || lsright is null ||
                        rsdown is null || rsup is null || rsleft is null || rsright is null)
                    {
                        throw new NullReferenceException("One or more assets are null..");
                    }
                }
                catch (NullReferenceException ex)
                {
                    this.result = false;
                    if (MoControlsMod.debugTypeEquals(DebugTypeEnum.full))
                        MoControlsMod.print("<color=red>Error occured when setting asset textures...</color>.\r\n<b>Message:</b>" + ex.Message + "\r\nStacktrace:\r\n" + ex.StackTrace);
                    throw;
                }
            }
            else
            {
                this.result = false;
                if (MoControlsMod.debugTypeEquals(DebugTypeEnum.full))
                    MoControlsMod.print("<color=red>Error not all assets could be found..</color>.");
            }
            this.result = true;
        }
    }
}
