using AstroLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AstroLib.Model
{
	public class DummyFocusAnalysis : IFocusAnalysis
	{
		public double Score(byte[] imgData)
		{
			return 0;
		}
	}
}
