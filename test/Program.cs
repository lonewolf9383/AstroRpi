using AstroLib.Model;
using System;
using System.IO;

namespace test
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			StackedImage image = null;

			string[] paths = Directory.GetFiles(".", "*.jpg");
			foreach(string p in paths)
			{
				if (image == null)
					image = new StackedImage(File.ReadAllBytes(p));
				else
					image.AddImage(File.ReadAllBytes(p));
			}

			File.WriteAllBytes("stacked.png", image.GetStackedImage());
		}
	}
}
