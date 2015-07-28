using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wax
{
    class Program
    {
        static void Main(string[] args)
        {
            string imageFile = args[0];
            string desiredSize = args[1];
            Wax pp = new Wax(imageFile, desiredSize);
            //Wax pp = new Wax("escher.jpg", "100x131");
            //PushPin pp = new PushPin("StarryNight.jpg", "100x131");
            pp.calculateResultImage();
            pp.writeResultImage();
        }
    }
}
