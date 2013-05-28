using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;

namespace Ester.Model.UserControls
{
	public class ViewportAxesRangeRestriction : ViewportRestriction
	{
		public DisplayRange XRange { get; set; }
		public DisplayRange YRange { get; set; }

		public override DataRect Apply(DataRect previousDataRect, DataRect proposedDataRect, Viewport2D viewport)
		{
			if (!proposedDataRect.IsEmpty)
			{
				if (XRange != null)
				{
					proposedDataRect.XMin = Math.Max(XRange.Start, proposedDataRect.XMin);
					proposedDataRect.Width = XRange.End - XRange.Start;
				}

				if (YRange != null)
				{
					proposedDataRect.YMin = YRange.Start;
					proposedDataRect.Height = YRange.End - YRange.Start;
				}
				return proposedDataRect;
			}
			return previousDataRect;
		}
	}
}
