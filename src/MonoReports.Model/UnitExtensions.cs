// 
// UnitExtensions.cs
//  
// Author:
//       Tomasz Kubacki <tomasz (dot) kubacki (at) gmail (dot ) com>
// 
// Copyright (c) 2011 Tomasz Kubacki
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;

namespace MonoReports.Model
{
	public static class UnitExtensions
	{		
	
		public static double cm (this double val)
		{			
			return 	96 / 2.54 * val;
		}
		
		public static double mm (this double val)
		{			
			return 	96 / 25.4 * val;
		}
		
		public static double inch (this double val)
		{			
			return 	96 * val;
		}
		
		
		public static double pt (this double val)
		{			
			return 	96 / 72 * val;
		}
		
		public static double cm (this int val)
		{			
			return 	96 / 2.54 * (double) val;
		}
		
		public static double mm (this int val)
		{			
			return 	96 / 25.4 * (double) val;
		}
		
		public static double inch (this int val)
		{			
			return 	96 * (double) val;
		}
		
		
		public static double pt (this int val)
		{			
			return 	96 / 72 * (double) val;
		}		
		
		//yea it's stuipid but may be usable to make code cleaner
		public static double px (this double val)
		{			
			return 	val;
		}
				
		public static double px (this int val)
		{			
			return (double)	val;
		}
		
		
	 
		
		public static double Tomm (this double  val)
		{			
			return 	 25.4 * val / 96;
		}
		
		public static double Tocm (this double  val)
		{			
			return 	 2.54 * val / 96;
		}
		
		
		public static double Toinch (this double  val)
		{			
			return 	  val / 96;
		}
		
		public static double Topt (this double  val)
		{			
			return 	 25.4 / 72 * val / 96;
		}
	}
}

