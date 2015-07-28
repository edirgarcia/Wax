using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Configuration;
using System.IO;

namespace Wax
{
    class Wax
    {
        private Bitmap original;
        private Bitmap result;
        private int width;
        private int height;
        private List<Color> palette;

        public Wax(string imagePath, string widthXHeight) 
        {
            original = new Bitmap(imagePath);
            result = new Bitmap(original.Width, original.Height,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            string[] stringSize = widthXHeight.Split(new char[1]{'x'} );

            try
            {
                width = Int32.Parse(stringSize[0]);
                height = Int32.Parse(stringSize[1]);
                string paletteFileName = ConfigurationManager.AppSettings["PaletteFile"];
                loadPalette(paletteFileName);
            }
            catch(IOException ioe)
            {
                Console.Out.WriteLine("Problem with pallete file");
                printHelp();
                Console.Out.WriteLine(ioe.Message);
            }
            catch(Exception e)
            {
               Console.Out.WriteLine("Incorrect syntax");
               printHelp();
               Console.Out.WriteLine(e.Message);
            }
        }

        private void loadPalette(string fileName)
        {
            string[] paletteText = File.ReadAllLines(fileName);
            palette = new List<Color>();

            foreach (string colorString in paletteText)
            {
                palette.Add(ColorTranslator.FromHtml(colorString));
            }
        }

        public void calculateResultImage()
        {
            int pinHeigth = (int) Math.Ceiling((double)original.Height / height);
            int pinWidth =  (int) Math.Ceiling((double)original.Width / width);

            for(int i = 0; i < original.Width - pinWidth; i+= pinWidth)
            {
                for(int j = 0; j < original.Height - pinHeigth; j+= pinHeigth)
                {
                    Color averageColor = getAverageColorAt(i,j,pinWidth,pinHeigth);
                    Color closestColor = getClosestColor(averageColor);
                    setColorAt(i, j, pinWidth, pinHeigth, closestColor);
                }
            }
        }

        public void writeResultImage()
        {
            result.Save("result.jpg");
        }

       private Color getAverageColorAt(int x, int y, int windowWidth, int windowHeight)
        {
           int windowSize = windowWidth * windowHeight;

           int componentR = 0;
           int componentG = 0;
           int componentB = 0;

           for(int i = x; i < x + windowWidth ; i++)
           {
               for (int j = y; j < y + windowHeight; j++) 
               {
                   Color current = original.GetPixel(i, j);
                   componentR += current.R;
                   componentG += current.G;
                   componentB += current.B;
               }
           }

           componentR = componentR / windowSize;
           componentG = componentG / windowSize;
           componentB = componentB / windowSize;

           return Color.FromArgb(componentR, componentG, componentB);
        }

        //sets the whole window at x,y to that color
       private void setColorAt(int x, int y, int windowWidth, int windowHeight, Color col)
       {
           for (int i = x; i < windowWidth + x; i++)
           {
               for (int j = y; j < windowHeight + y; j++)
               {
                   result.SetPixel(i, j, col);
               }
           }
       }

        //gets the closest color from the palette.
       private Color getClosestColor(Color c) 
       {
           double minDistance = Double.MaxValue;
           Color retColor = new Color();

           foreach(Color paletteColor in palette)
           {
                //Euclidean distance, color component as coordinate.
                double currDistance = Math.Sqrt(Math.Pow(c.R - paletteColor.R, 2) + Math.Pow(c.R - paletteColor.R, 2) + Math.Pow(c.R - paletteColor.R, 2));
                
                if(currDistance<minDistance)
                {
                    minDistance = currDistance;
                    retColor = paletteColor;
                }
           }

           return retColor;
       }

        private void printHelp()
        {
            Console.Out.WriteLine("The syntax is:");
            Console.Out.WriteLine("Wax.exe <image> <desiredSize>");
            Console.Out.WriteLine("image: is the path to the image file. e.g. escher.jpg");
            Console.Out.WriteLine("desiredSize: is the amount in pixels of the desired output (make sure to aprox. keep the original aspect ratio) e.g. 100x131 ");
        }
        
    }
}
