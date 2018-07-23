using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trawick.Common.Extensions
{
	public static class IntegerExtensions
	{
		#region Static Methods

		public static byte ToByte(this int value)
		{
			if (value < 0)
			{
				value = 0;
			}
			else if (value > 255)
			{
				value = 255;
			}

			return (byte)value;
		}

		#endregion
	}
}
