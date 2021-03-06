﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;

namespace RepoZ.App.Win
{
	public static class AcrylicHelper
	{
		public static void EnableBlur(Visual visual)
		{
			var hwnd = (HwndSource)PresentationSource.FromVisual(visual);
			EnableBlur(hwnd.Handle);
		}

		public static void EnableBlur(IntPtr hwnd)
		{
			WindowsCompositionHelper.EnableBlur(hwnd);
		}
	}
}
