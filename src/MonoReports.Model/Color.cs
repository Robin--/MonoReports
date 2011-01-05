// 
// Color.cs
//  
// Author:
//       Tomasz Kubacki <tomasz.kubacki(at)gmail.com>
// 
// Copyright (c) 2010 Tomasz Kubacki
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
	public struct Color : ICloneable
	{
	
		public  Color (double r,double g,double b):this(r, g, b,1)
		{
				
		}

		public  Color (double r,double g, double b, double a)
		{
			this.r = r;
			this.b = b;
			this.g = g;
			this.a = a;			 
		}

		double r;

		public double R {
			get { return r; }
			set { r = value; }
		}

		double g;

		public double G {
			get { return g; }
			set { g = value; }
		}

		double b;

		public double B {
			get { return b; }
			set { b = value; }
		}

		double a;

		public double A {
			get { return a; }
			set { a = value; }
		}

		public object Clone ()
		{
			return new Color (r,g,b,a);		
		}
		
		#region static colors
		
		public static Color AliceBlue {
			get { return new Color (0.94,0.97,1,1); }
		}

		public static Color AntiqueWhite {
			get { return new Color (0.98,0.92,0.84,1); }
		}

		public static Color Aqua {
			get { return new Color (0,1,1,1); }
		}

		public static Color Aquamarine {
			get { return new Color (0.5,1,0.83,1); }
		}

		public static Color Azure {
			get { return new Color (0.94,1,1,1); }
		}

		public static Color Beige {
			get { return new Color (0.96,0.96,0.86,1); }
		}

		public static Color Bisque {
			get { return new Color (1,0.89,0.77,1); }
		}

		public static Color Black {
			get { return new Color (0,0,0,1); }
		}

		public static Color BlanchedAlmond {
			get { return new Color (1,0.92,0.8,1); }
		}

		public static Color Blue {
			get { return new Color (0,0,1,1); }
		}

		public static Color BlueViolet {
			get { return new Color (0.54,0.17,0.89,1); }
		}

		public static Color Brown {
			get { return new Color (0.65,0.16,0.16,1); }
		}

		public static Color BurlyWood {
			get { return new Color (0.87,0.72,0.53,1); }
		}

		public static Color CadetBlue {
			get { return new Color (0.37,0.62,0.63,1); }
		}

		public static Color Chartreuse {
			get { return new Color (0.5,1,0,1); }
		}

		public static Color Chocolate {
			get { return new Color (0.82,0.41,0.12,1); }
		}

		public static Color Coral {
			get { return new Color (1,0.5,0.31,1); }
		}

		public static Color CornflowerBlue {
			get { return new Color (0.39,0.58,0.93,1); }
		}

		public static Color Cornsilk {
			get { return new Color (1,0.97,0.86,1); }
		}

		public static Color Crimson {
			get { return new Color (0.86,0.08,0.24,1); }
		}

		public static Color Cyan {
			get { return new Color (0,1,1,1); }
		}

		public static Color DarkBlue {
			get { return new Color (0,0,0.55,1); }
		}

		public static Color DarkCyan {
			get { return new Color (0,0.55,0.55,1); }
		}

		public static Color DarkGoldenrod {
			get { return new Color (0.72,0.53,0.04,1); }
		}

		public static Color DarkGray {
			get { return new Color (0.66,0.66,0.66,1); }
		}

		public static Color DarkGreen {
			get { return new Color (0,0.39,0,1); }
		}

		public static Color DarkKhaki {
			get { return new Color (0.74,0.72,0.42,1); }
		}

		public static Color DarkMagenta {
			get { return new Color (0.55,0,0.55,1); }
		}

		public static Color DarkOliveGreen {
			get { return new Color (0.33,0.42,0.18,1); }
		}

		public static Color DarkOrange {
			get { return new Color (1,0.55,0,1); }
		}

		public static Color DarkOrchid {
			get { return new Color (0.6,0.2,0.8,1); }
		}

		public static Color DarkRed {
			get { return new Color (0.55,0,0,1); }
		}

		public static Color DarkSalmon {
			get { return new Color (0.91,0.59,0.48,1); }
		}

		public static Color DarkSeaGreen {
			get { return new Color (0.56,0.74,0.55,1); }
		}

		public static Color DarkSlateBlue {
			get { return new Color (0.28,0.24,0.55,1); }
		}

		public static Color DarkSlateGray {
			get { return new Color (0.18,0.31,0.31,1); }
		}

		public static Color DarkTurquoise {
			get { return new Color (0,0.81,0.82,1); }
		}

		public static Color DarkViolet {
			get { return new Color (0.58,0,0.83,1); }
		}

		public static Color DeepPink {
			get { return new Color (1,0.08,0.58,1); }
		}

		public static Color DeepSkyBlue {
			get { return new Color (0,0.75,1,1); }
		}

		public static Color DimGray {
			get { return new Color (0.41,0.41,0.41,1); }
		}

		public static Color DodgerBlue {
			get { return new Color (0.12,0.56,1,1); }
		}

		public static Color Firebrick {
			get { return new Color (0.7,0.13,0.13,1); }
		}

		public static Color FloralWhite {
			get { return new Color (1,0.98,0.94,1); }
		}

		public static Color ForestGreen {
			get { return new Color (0.13,0.55,0.13,1); }
		}

		public static Color Fuchsia {
			get { return new Color (1,0,1,1); }
		}

		public static Color Gainsboro {
			get { return new Color (0.86,0.86,0.86,1); }
		}

		public static Color GhostWhite {
			get { return new Color (0.97,0.97,1,1); }
		}

		public static Color Gold {
			get { return new Color (1,0.84,0,1); }
		}

		public static Color Goldenrod {
			get { return new Color (0.85,0.65,0.13,1); }
		}

		public static Color Gray {
			get { return new Color (0.5,0.5,0.5,1); }
		}

		public static Color Green {
			get { return new Color (0,0.5,0,1); }
		}

		public static Color GreenYellow {
			get { return new Color (0.68,1,0.18,1); }
		}

		public static Color Honeydew {
			get { return new Color (0.94,1,0.94,1); }
		}

		public static Color HotPink {
			get { return new Color (1,0.41,0.71,1); }
		}

		public static Color IndianRed {
			get { return new Color (0.8,0.36,0.36,1); }
		}

		public static Color Indigo {
			get { return new Color (0.29,0,0.51,1); }
		}

		public static Color Ivory {
			get { return new Color (1,1,0.94,1); }
		}

		public static Color Khaki {
			get { return new Color (0.94,0.9,0.55,1); }
		}

		public static Color Lavender {
			get { return new Color (0.9,0.9,0.98,1); }
		}

		public static Color LavenderBlush {
			get { return new Color (1,0.94,0.96,1); }
		}

		public static Color LawnGreen {
			get { return new Color (0.49,0.99,0,1); }
		}

		public static Color LemonChiffon {
			get { return new Color (1,0.98,0.8,1); }
		}

		public static Color LightBlue {
			get { return new Color (0.68,0.85,0.9,1); }
		}

		public static Color LightCoral {
			get { return new Color (0.94,0.5,0.5,1); }
		}

		public static Color LightCyan {
			get { return new Color (0.88,1,1,1); }
		}

		public static Color LightGoldenrodYellow {
			get { return new Color (0.98,0.98,0.82,1); }
		}

		public static Color LightGray {
			get { return new Color (0.83,0.83,0.83,1); }
		}

		public static Color LightGreen {
			get { return new Color (0.56,0.93,0.56,1); }
		}

		public static Color LightPink {
			get { return new Color (1,0.71,0.76,1); }
		}

		public static Color LightSalmon {
			get { return new Color (1,0.63,0.48,1); }
		}

		public static Color LightSeaGreen {
			get { return new Color (0.13,0.7,0.67,1); }
		}

		public static Color LightSkyBlue {
			get { return new Color (0.53,0.81,0.98,1); }
		}

		public static Color LightSlateGray {
			get { return new Color (0.47,0.53,0.6,1); }
		}

		public static Color LightSteelBlue {
			get { return new Color (0.69,0.77,0.87,1); }
		}

		public static Color LightYellow {
			get { return new Color (1,1,0.88,1); }
		}

		public static Color Lime {
			get { return new Color (0,1,0,1); }
		}

		public static Color LimeGreen {
			get { return new Color (0.2,0.8,0.2,1); }
		}

		public static Color Linen {
			get { return new Color (0.98,0.94,0.9,1); }
		}

		public static Color Magenta {
			get { return new Color (1,0,1,1); }
		}

		public static Color Maroon {
			get { return new Color (0.5,0,0,1); }
		}

		public static Color MediumAquamarine {
			get { return new Color (0.4,0.8,0.67,1); }
		}

		public static Color MediumBlue {
			get { return new Color (0,0,0.8,1); }
		}

		public static Color MediumOrchid {
			get { return new Color (0.73,0.33,0.83,1); }
		}

		public static Color MediumPurple {
			get { return new Color (0.58,0.44,0.86,1); }
		}

		public static Color MediumSeaGreen {
			get { return new Color (0.24,0.7,0.44,1); }
		}

		public static Color MediumSlateBlue {
			get { return new Color (0.48,0.41,0.93,1); }
		}

		public static Color MediumSpringGreen {
			get { return new Color (0,0.98,0.6,1); }
		}

		public static Color MediumTurquoise {
			get { return new Color (0.28,0.82,0.8,1); }
		}

		public static Color MediumVioletRed {
			get { return new Color (0.78,0.08,0.52,1); }
		}

		public static Color MidnightBlue {
			get { return new Color (0.1,0.1,0.44,1); }
		}

		public static Color MintCream {
			get { return new Color (0.96,1,0.98,1); }
		}

		public static Color MistyRose {
			get { return new Color (1,0.89,0.88,1); }
		}

		public static Color Moccasin {
			get { return new Color (1,0.89,0.71,1); }
		}

		public static Color NavajoWhite {
			get { return new Color (1,0.87,0.68,1); }
		}

		public static Color Navy {
			get { return new Color (0,0,0.5,1); }
		}

		public static Color OldLace {
			get { return new Color (0.99,0.96,0.9,1); }
		}

		public static Color Olive {
			get { return new Color (0.5,0.5,0,1); }
		}

		public static Color OliveDrab {
			get { return new Color (0.42,0.56,0.14,1); }
		}

		public static Color Orange {
			get { return new Color (1,0.65,0,1); }
		}

		public static Color OrangeRed {
			get { return new Color (1,0.27,0,1); }
		}

		public static Color Orchid {
			get { return new Color (0.85,0.44,0.84,1); }
		}

		public static Color PaleGoldenrod {
			get { return new Color (0.93,0.91,0.67,1); }
		}

		public static Color PaleGreen {
			get { return new Color (0.6,0.98,0.6,1); }
		}

		public static Color PaleTurquoise {
			get { return new Color (0.69,0.93,0.93,1); }
		}

		public static Color PaleVioletRed {
			get { return new Color (0.86,0.44,0.58,1); }
		}

		public static Color PapayaWhip {
			get { return new Color (1,0.94,0.84,1); }
		}

		public static Color PeachPuff {
			get { return new Color (1,0.85,0.73,1); }
		}

		public static Color Peru {
			get { return new Color (0.8,0.52,0.25,1); }
		}

		public static Color Pink {
			get { return new Color (1,0.75,0.8,1); }
		}

		public static Color Plum {
			get { return new Color (0.87,0.63,0.87,1); }
		}

		public static Color PowderBlue {
			get { return new Color (0.69,0.88,0.9,1); }
		}

		public static Color Purple {
			get { return new Color (0.5,0,0.5,1); }
		}

		public static Color Red {
			get { return new Color (1,0,0,1); }
		}

		public static Color RosyBrown {
			get { return new Color (0.74,0.56,0.56,1); }
		}

		public static Color RoyalBlue {
			get { return new Color (0.25,0.41,0.88,1); }
		}

		public static Color SaddleBrown {
			get { return new Color (0.55,0.27,0.07,1); }
		}

		public static Color Salmon {
			get { return new Color (0.98,0.5,0.45,1); }
		}

		public static Color SandyBrown {
			get { return new Color (0.96,0.64,0.38,1); }
		}

		public static Color SeaGreen {
			get { return new Color (0.18,0.55,0.34,1); }
		}

		public static Color SeaShell {
			get { return new Color (1,0.96,0.93,1); }
		}

		public static Color Sienna {
			get { return new Color (0.63,0.32,0.18,1); }
		}

		public static Color Silver {
			get { return new Color (0.75,0.75,0.75,1); }
		}

		public static Color SkyBlue {
			get { return new Color (0.53,0.81,0.92,1); }
		}

		public static Color SlateBlue {
			get { return new Color (0.42,0.35,0.8,1); }
		}

		public static Color SlateGray {
			get { return new Color (0.44,0.5,0.56,1); }
		}

		public static Color Snow {
			get { return new Color (1,0.98,0.98,1); }
		}

		public static Color SpringGreen {
			get { return new Color (0,1,0.5,1); }
		}

		public static Color SteelBlue {
			get { return new Color (0.27,0.51,0.71,1); }
		}

		public static Color Tan {
			get { return new Color (0.82,0.71,0.55,1); }
		}

		public static Color Teal {
			get { return new Color (0,0.5,0.5,1); }
		}

		public static Color Thistle {
			get { return new Color (0.85,0.75,0.85,1); }
		}

		public static Color Tomato {
			get { return new Color (1,0.39,0.28,1); }
		}

		public static Color Transparent {
			get { return new Color (0,0,0,0); }
		}

		public static Color Turquoise {
			get { return new Color (0.25,0.88,0.82,1); }
		}

		public static Color Violet {
			get { return new Color (0.93,0.51,0.93,1); }
		}

		public static Color Wheat {
			get { return new Color (0.96,0.87,0.7,1); }
		}

		public static Color White {
			get { return new Color (1,1,1,1); }
		}

		public static Color WhiteSmoke {
			get { return new Color (0.96,0.96,0.96,1); }
		}

		public static Color Yellow {
			get { return new Color (1,1,0,1); }
		}

		public static Color YellowGreen {
			get { return new Color (0.6,0.8,0.2,1); }
		}
		
		#endregion
	}
}

