using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AstroLib.Interfaces
{
	public interface IFocusAnalysis
	{
		double Score(byte[] imgData);
	}
}
