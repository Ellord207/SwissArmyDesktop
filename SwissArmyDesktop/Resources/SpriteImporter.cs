using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SwissArmyDesktop
    {
    static public class SpriteImporter
        {
        // TODO : Add functions to import the sprites using an XML file.

        static private Type type = typeof(SpriteImporter);
        static private FieldInfo[] field = type.GetFields(BindingFlags.Public | BindingFlags.Static);

        static public int FieldCount { get { return field.Length; } }

        static public SPRITE getByNumber(int i)
            {
            if (i < 0 || i >= FieldCount)
                return null;
            return (SPRITE)field[i].GetValue(null);
            }

        //                        \\NAOU24186\Users\Nathan.Elrod.BENTLEY\Documents\Shared\Walker\Resources\
        //const string basefile = @"\\NAOU24186\Users\Nathan.Elrod.BENTLEY\Documents\Shared\Walker\Resources\";
        const string basefile = @"..\..\Resources\Images\";
        readonly public static SPRITE Link = new SPRITE(@"Link (Normal) (Left)2.gif", 140, 2);
        readonly public static SPRITE Crono = new SPRITE(@"Crono - Walk (Left).gif", 145 / 4, 8);
        readonly public static SPRITE CronoRun = new SPRITE(@"Crono - Run (Left).gif", 130 / 3, 6);
        readonly public static SPRITE Ness = new SPRITE(@"Ness - Walk (Left).gif", 140 / 4, 8);
        readonly public static SPRITE NessBike = new SPRITE(@"Ness (Bike) (Left).gif", 130 / 3, 6);
        readonly public static SPRITE RedMage = new SPRITE(@"RedMage-Walk.gif", 140 / 2, 4);
        readonly public static SPRITE Octorok = new SPRITE(@"Octorok - Red (Left).gif", 10, 15);
        readonly public static SPRITE SuperShroom = new SPRITE(@"Super Mushroom.gif", 20, 12);
        readonly public static SPRITE Goomba = new SPRITE(@"Goomba.gif", 50, 6);

        public class SPRITE
            {
            public uint probability { get; set; }
            public string filename { get; private set; }
            public int tickSpeed { get; set; }
            public int jumpDiv { get; set; }
            public Bitmap bitmap { get; private set; }

            public SPRITE(string file, int speed, int div)
                {
                filename = basefile + file;
                bitmap = (Bitmap)Image.FromFile(filename);
                if (System.IO.File.Exists(file))
                    probability = 0;
                else
                    probability = 100;
                tickSpeed = speed;
                jumpDiv = div;
                }

            public SPRITE(Bitmap image, int speed, int div)
                {
                filename = "";
                bitmap = (Bitmap)image.Clone();
                if (bitmap == null)
                    probability = 0;
                else
                    probability = 100;
                tickSpeed = speed;
                jumpDiv = div;
                }
            }
        }


    }
