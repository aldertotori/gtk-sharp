// Canvas.cs - port of standard GNOME canvas demo 
//
// Author: Rachel Hestilow <hestilow@ximian.com> 
//
// (c) 2002 Rachel Hestilow

namespace GtkSamples {

	using Gnome;
	using Gtk;
	using Gdk;
	using GtkSharp;
	using System;

	public class CanvasExample {
		private int width = 400, height = 400;
		private double remember_x = 0.0, remember_y = 0.0;

		private Canvas canvas;
		private Random random = new Random ();

		public CanvasExample () {
			Gtk.Window win = new Gtk.Window ("Canvas example");
			win.DeleteEvent += new DeleteEventHandler (Window_Delete);

			VBox vbox = new VBox (false, 0);
			win.Add (vbox);

			vbox.PackStart (new Label ("Drag - move object.\n" +
						   "Double click - change color\n" +
						   "Right click - delete object"),
					false, false, 0);
			
			canvas = new Canvas ();
			canvas.SetSizeRequest (width, height);
			canvas.SetScrollRegion (0.0, 0.0, (double) width, (double) height);
			vbox.PackStart (canvas, false, false, 0);

			HBox hbox = new HBox (false, 0);
			vbox.PackStart (hbox, false, false, 0);

			Button add_button = new Button ("Add an object");
			add_button.Clicked += new EventHandler (AddObject);
			hbox.PackStart (add_button, false, false, 0);

			Button quit_button = new Button ("Quit");
			quit_button.Clicked += new EventHandler (Quit);
			hbox.PackStart (quit_button, false, false, 0);

			win.ShowAll ();
		}

		void Swap (ref double a, ref double b) {
			double tmp = a;
			a = b;
			b = tmp;
		}
		
		void AddObject (object obj, EventArgs args)
		{
			double x1 = random.Next (width);
			double y1 = random.Next (height);
			double x2 = random.Next (width);
			double y2 = random.Next (height);

			if (x1 > x2)
				Swap (ref x1, ref x2);
			
			if (y1 > y2)
				Swap (ref y1, ref y2);

			if ((x2 - x1) < 10)
				x2 += 10;

			if ((y2 - y1) < 10)
				y2 += 10;

			CanvasRE item = null;
			if (random.Next (2) > 0)
				item = new CanvasRect (canvas.Root ());
			else
				item = new CanvasEllipse (canvas.Root ());

			item.X1 = x1;
			item.Y1 = y1;
			item.X2 = x2;
			item.Y2 = y2;
			item.FillColor = "white";
			item.OutlineColor = "black";
			item.WidthUnits = 1.0;

			item.CanvasEvent += new GnomeSharp.CanvasEventHandler (Item_Event);
		}
		
		void ChangeItemColor (CanvasRE item)
		{
			string[] colors = new string[] {"red", "yellow", "green", "cyan", "blue", "magenta"};
			item.FillColor = colors[random.Next (colors.Length)];	
		}
		
		void Item_Event (object obj, GnomeSharp.CanvasEventArgs args) {
			EventButton ev = EventButton.New (args.Event.Handle);
			SignalArgs sa = (SignalArgs) args;
			CanvasRE item = (CanvasRE) obj;

			switch (ev.type) {
			case EventType.ButtonPress:
				if (ev.button == 1) {
					remember_x = ev.x;
					remember_y = ev.y;
					sa.RetVal = true;
					return;
				} else if (ev.button == 3) {
					item.Destroy ();
					sa.RetVal = true;
					return;
				}
				break;
			case EventType.TwoButtonPress:
				ChangeItemColor (item);
				sa.RetVal = true;
				return;
			case EventType.MotionNotify:
				Gdk.ModifierType state = (Gdk.ModifierType) ev.state;
				if ((state & Gdk.ModifierType.Button1Mask) != 0) {
					double new_x = ev.x, new_y = ev.y;
					item.Move (new_x - remember_x, new_y - remember_y);
					remember_x = new_x;
					remember_y = new_y;
					sa.RetVal = true;
					return;
				}
				break;
			case EventType.EnterNotify:
				item.WidthUnits = 3.0;
				sa.RetVal = true;
				return;
			case EventType.LeaveNotify:
				item.WidthUnits = 1.0;
				sa.RetVal = true;
				return;
			}

			sa.RetVal = false;
			return;
		}
		
		void Quit (object obj, EventArgs args)
		{
			Application.Quit ();
		}

		void Window_Delete (object obj, DeleteEventArgs args)
		{
			SignalArgs sa = (SignalArgs) args;
			Application.Quit ();
			sa.RetVal = true;
		}

		public static int Main (string[] args)
		{
			Application.Init ();
			
			CanvasExample example = new CanvasExample ();

			Application.Run ();
			return 0;
		}
	}
}

