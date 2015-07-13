using System;
using Gtk;
using System.Collections.Generic;
using System.Collections;

namespace SurfaceItems
{
	public class SurfaceList:IList, IEnumerable<DisplayTypeSurfaceItem>
	{
		private Table _table;
		private List<Tuple<DisplayTypeSurfaceItem, VBox>> _labelBoxes;
		private int _count;
		private string _label; 
		private int _maxRows;
		private global::Gtk.Table.TableChild _tbChild;

		public event EventHandler<LabelEventArgs> LabelChanged;

		public SurfaceList (int maxRows, string label)
		{
			_labelBoxes = new List<Tuple<DisplayTypeSurfaceItem, VBox>> ();
			_maxRows = maxRows;
			_label = label;
			_count = 0;

			_table = new Table ((uint)(maxRows + 1), (uint)2, false);
			_table.RowSpacing = ((uint)(6));
			_table.ColumnSpacing = ((uint)(6));


			CreateBottomLabel (label);
		}

		private void CreateBottomLabel(string label) {
			HBox hb = new global::Gtk.HBox ();
			hb.Name = "hbox_bottomLabel";
			hb.Spacing = 6;

			// Container child hbox8.Gtk.Box+BoxChild
			Fixed f1 = new global::Gtk.Fixed ();
			f1.Name = "fixed_left";
			f1.HasWindow = false;
			hb.Add (f1);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(hb [f1]));
			w1.Position = 0;
			// Container child hbox8.Gtk.Box+BoxChild
			Label l = new global::Gtk.Label ();
			l.Name = "bottomLabel";
			l.LabelProp = label;
			hb.Add (l);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(hb [l]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox8.Gtk.Box+BoxChild
			Fixed f2 = new global::Gtk.Fixed ();
			f2.Name = "fixed_right";
			f2.HasWindow = false;
			hb.Add (f2);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(hb [f2]));
			w3.Position = 2;
			this._table.Add (hb);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this._table [hb]));

			_tbChild = w4;
			w4.TopAttach = ((uint)(0));
			w4.BottomAttach = ((uint)(_maxRows + 1));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));

			this.LabelChanged += delegate(object sender, LabelEventArgs e) {
				l.LabelProp = e.Label;
			};
		}

		public void Add(DisplayTypeSurfaceItem item) {
			EventBox ev = item;

			_tbChild.TopAttach = ((uint)(_labelBoxes.Count + 1));

			_table.Add (item);
			global::Gtk.Table.TableChild w = ((global::Gtk.Table.TableChild)(this._table [item]));
			w.TopAttach = ((uint)(_labelBoxes.Count));
			w.BottomAttach = ((uint)(_labelBoxes.Count + 1));
			w.LeftAttach = ((uint)(1));
			w.RightAttach = ((uint)(2));

			AddLabelBox (item);
			_table.ShowAll ();
		}

		private void AddLabelBox(DisplayTypeSurfaceItem item) {
			VBox vb = new global::Gtk.VBox ();
			vb.Name = "vbox_row" + (_labelBoxes.Count + 1);
			vb.Spacing = 6;
			ToggleButton btn1, btn2;
			VSeparator vs1, vs2;
			Label l;

			btn1 = new global::Gtk.ToggleButton ();
			btn1.Sensitive = false;
			btn1.CanFocus = false;
			btn1.Name = "tgbt1_row" + (_labelBoxes.Count + 1);
			btn1.Active = true;
			vs1 = new global::Gtk.VSeparator ();
			vs1.Name = "sep1_row" + (_labelBoxes.Count + 1);
			btn1.Add (vs1);
			btn1.Label = null;
			vb.Add (btn1);
			global::Gtk.Box.BoxChild w = ((global::Gtk.Box.BoxChild)(vb [btn1]));
			w.Position = 0;
			l = new global::Gtk.Label ();
			l.Name = "label_row" + (_labelBoxes.Count + 1);
			l.LabelProp = item.Label;
			l.UseMarkup = true;
			l.Angle = 90;
			vb.Add (l);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(vb [l]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			btn2 = new global::Gtk.ToggleButton ();
			btn2.CanFocus = false;
			btn2.Name = "tgbt2_row" + (_labelBoxes.Count + 1);
			vs2 = new global::Gtk.VSeparator ();
			vs2.Name = "sep2_row" + (_labelBoxes.Count + 1);
			btn2.Add (vs2);
			btn2.Label = null;
			vb.Add (btn2);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(vb [btn2]));
			w3.Position = 2;
			this._table.Add (vb);
			global::Gtk.Table.TableChild wt = ((global::Gtk.Table.TableChild)(this._table [vb]));

			wt.TopAttach = ((uint)(_labelBoxes.Count));
			wt.BottomAttach = ((uint)(_labelBoxes.Count + 1));
			wt.LeftAttach = ((uint)(0));
			wt.RightAttach = ((uint)(1));

			wt.XOptions = ((global::Gtk.AttachOptions)(4));

			item.LabelChanged += delegate(object sender, LabelEventArgs e) {
				l.Text = e.Label;
			};

			_labelBoxes.Add (new Tuple<DisplayTypeSurfaceItem, VBox>(item, vb));
			btn1.Clicked += delegate(object sender, EventArgs e) {
				if(btn1.Active) {
					btn1.Sensitive = false;
					btn2.Sensitive = true;
					//btn1.Active = true;
					btn2.Active = false;
					item.DisplayMode = DisplayMode.Snapshot;
				}
			};
			btn2.Clicked += delegate(object sender, EventArgs e) {
				if(btn2.Active) {
					btn2.Sensitive = false;
					btn1.Sensitive = true;
					//btn2.Active = true;
					btn1.Active = false;
					item.DisplayMode = DisplayMode.History;
				}

			};
			item.DisplayModeChanged += delegate(object sender, DisplayModeEventArgs e) {
				switch (e.DisplayMode) {
				case DisplayMode.Snapshot:
					btn1.Active = true;
					break;
				case DisplayMode.History:
					btn2.Active = true;
					break;
				default:
				break;
				}
			};
		}

		public static implicit operator Table(SurfaceList lst) {
			return lst._table;
		}

		public string Label {
			get {
				return this._label;
			}
			set {
				if(_label != value) {
					_label = value;

					if(LabelChanged != null) {
						LabelChanged (this, new LabelEventArgs (value));
					}
				}
			}
		}

		#region IList implementation

		public int Add (object value)
		{
			throw new NotImplementedException ();
		}

		public void Clear ()
		{
			throw new NotImplementedException ();
		}

		public bool Contains (object value)
		{
			throw new NotImplementedException ();
		}

		public int IndexOf (object value)
		{
			throw new NotImplementedException ();
		}

		public void Insert (int index, object value)
		{
			throw new NotImplementedException ();
		}

		public void Remove (object value)
		{
			throw new NotImplementedException ();
		}

		public void RemoveAt (int index)
		{
			throw new NotImplementedException ();
		}

		public bool IsFixedSize {
			get {
				throw new NotImplementedException ();
			}
		}

		public bool IsReadOnly {
			get {
				throw new NotImplementedException ();
			}
		}

		public object this [int index] {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		#endregion

		#region ICollection implementation

		public void CopyTo (Array array, int index)
		{
			throw new NotImplementedException ();
		}

		public int Count {
			get {
				throw new NotImplementedException ();
			}
		}

		public bool IsSynchronized {
			get {
				throw new NotImplementedException ();
			}
		}

		public object SyncRoot {
			get {
				throw new NotImplementedException ();
			}
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator GetEnumerator ()
		{
			return SurfaceEnum ().GetEnumerator ();
		}

		private System.Collections.Generic.IEnumerable<DisplayTypeSurfaceItem> SurfaceEnum() {
			foreach (var item in _labelBoxes) {
				yield return item.Item1;
			}
		}

		IEnumerator<DisplayTypeSurfaceItem> IEnumerable<DisplayTypeSurfaceItem>.GetEnumerator ()
		{
			foreach (var item in _labelBoxes) {
				yield return item.Item1;
			}
		}

		#endregion
	}
}

