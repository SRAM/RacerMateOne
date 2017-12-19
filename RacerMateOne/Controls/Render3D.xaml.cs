using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.ComponentModel;
using System.Windows.Media.Effects;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace RacerMateOne.Controls  {
	public enum CameraGameMode: int  {
		Normal,
		Lead,
		Max
	};



	/// <summary>
	/// Interaction logic for Renderer.xaml
	/// </summary>
	public partial class Render3D : UserControl  {
		//=================================================================
		private static bool? _isInDesignMode;
		public static bool IsInDesignMode
		{
			get
			{
				if (!_isInDesignMode.HasValue)
				{
#if SILVERLIGHT
					_isInDesignMode = DesignerProperties.IsInDesignTool;
#else
					var prop = DesignerProperties.IsInDesignModeProperty;
					_isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
#endif
				}
				return _isInDesignMode.Value;
			}
		}

		public static Visual FindAncestor(Visual child, Type typeAncestor)
		{
			DependencyObject parent = VisualTreeHelper.GetParent(child);
			while (parent != null && !typeAncestor.IsInstanceOfType(parent))
				parent = VisualTreeHelper.GetParent(parent);
			return (parent as Visual);
		}

		//=================================================================

		bool m_bActive;
		bool m_bInit;

		uint m_width;
		uint m_height;

		// Set by the resize stuff.  Will be used on the next resize. (When activeated)
		uint m_new_width;
		uint m_new_height; 



		public Render3D()
		{
			//InitializeComponent();
			//Children = new ObservableCollection<Button>();

        Loaded += new RoutedEventHandler(Render3D_Loaded);
			SizeChanged += new SizeChangedEventHandler(Render3D_SizeChanged);
			IsVisibleChanged += new DependencyPropertyChangedEventHandler(Render3D_IsVisibleChanged);
		}
		~Render3D()
		{
		}
		public Grid ControlGrid;
		bool m_bLoadedOnce = false;
		private void Render3D_Loaded(object sender, RoutedEventArgs e)
		{
			if (!m_bLoadedOnce)
			{
				grid = (Grid)UIChildFinder.FindChild(this, "render3d_x_grid", typeof(Grid));
				ControlGrid = grid;
				img = (Image)grid.Children[0];
				d3dimg = (D3DImage)img.Source;
                // We hook into this event to handle when a D3D device is lost 
                //Log.WriteLine("The properties of d3dimg: width = " + d3dimg.Width + " height = " + d3dimg.Height); 
  
                d3dimg.IsFrontBufferAvailableChanged += IsFrontBufferAvailableChanged;
                debugGrid = (Grid)grid.Children[2];
				m_bLoadedOnce = true;
			}
			if (IsInDesignMode)
			{
				// Create some elements just for design mode then exit.
				Label label = new Label();
				label.Content = "Render3D Layer";

				DropShadowEffect ef = new DropShadowEffect();
				ef.Color = Colors.Black;
				ef.Direction = 315;
				ef.Opacity = 0.45;
				label.Effect = ef;
				label.FontSize = 30;
				label.FontFamily = new FontFamily("Arial");
				label.Opacity = 0.4;
				grid.Children.Add(label);
				return;
			}
		}
        
        /// <summary>
        /// This should only fire when a D3D device is lost
        /// </summary>
        private void IsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
             if (!d3dimg.IsFrontBufferAvailable)
                           return;

            //d3dimg.Height = m_new_height;
            //d3dimg.Width = m_new_width;
            /* Flag that we have a new surface, even
             * though we really don't */
           // m_newSurfaceAvailable = true;

            /* Force feed the D3DImage the Surface pointer */
          // SetBackBufferInternal(pSurface);
        }
       

        // For safty purposes this actually will allow things to run.
		public void Init()
		{
			if (!m_bInit)
			{
				m_bInit = true;
				ms_Active = this;
				Activate();
			}
		}


		private void Render3D_Unloaded(object sender, RoutedEventArgs e)
		{
			if (!IsInDesignMode)
				CompositionTarget.Rendering -= new EventHandler(CompositionTarget_Rendering);
			if (ms_Active == this)
				ms_Active = null;
		}

		void DispatchActivate()
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(_activate), null);
		}
		object _activate(Object args)
		{
			Activate();
			return null;
		}

		void Activate()
		{
			if (m_bActive || !m_bInit || img==null)
				return;

			m_bActive = true;
			CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
			img.Visibility = Visibility.Visible;
		}

		bool m_RedrawAll;

		TimeSpan m_LastRenderingTime = new TimeSpan();
		void CompositionTarget_Rendering(object sender, EventArgs e)
		{
           RenderingEventArgs args = (RenderingEventArgs)e;
			if (m_LastRenderingTime == args.RenderingTime) // Occurse the same as last frame... don't redo this
				return;
			m_LastRenderingTime = args.RenderingTime;

			// Do we need to resize
            //Debug.WriteLine("New w,h= " + m_new_width + "," + m_new_height + " old " + m_width + "," + m_height); 
			if (m_new_width !=  m_width || m_new_height != m_height)
			{
                // ECT - Just some limititation to fix
                // Make sure to use what is returned by 3D for now to prevent crash
                // This is only a request. The function returns what it actually gets
                
                // ECT - Added a try catch and removed throw Exception here
                try
                {
                    HRESULT.Check(DLL.SetSize(ref m_new_width, ref m_new_height));
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.ToString());
                    //Debug.WriteLine(ex);
                }

                m_width = m_new_width;
                m_height = m_new_height;

				ChangedAll();
			}


			RedoViews();
            //if (ms_CurCourse == null || !IsVisible || m_ActiveList.Count <= 0)
            if (!IsVisible || m_ActiveList.Count <= 0)
			{
				m_bActive = false;
				CompositionTarget.Rendering -= new EventHandler(CompositionTarget_Rendering);
				img.Visibility = Visibility.Hidden;
				return;
			}

			// It's possible for Rendering to call back twice in the same frame 
			// so only render when we haven't already rendered in this frame.
			if (d3dimg.IsFrontBufferAvailable)
			{
				UpdateRiders();
				IntPtr pSurface = IntPtr.Zero;
				HRESULT.Check(DLL.GetBackBufferNoRef(out pSurface));
				if (pSurface != IntPtr.Zero)
				{
					d3dimg.Lock();
					// Repeatedly calling SetBackBuffer with the same IntPtr is 
					// a no-op. There is no performance penalty.
                    //Debug.WriteLine("calling setbackbuffer");
					
                    // PAS: there is a mystery bug that shows randomly due to d3dimg.SetBackBuffer device is invalid.
                    // looking in on D3dRenderer in WPFMediaToolkit, they wrap this in a try catch. I'm going to simply exit since teh setbackbuffer is called
                    // apparently at frame-rate (?). Will write to the log output whenever it happens, but just exit and hope on next pass the device has been re-found
                    // and can be drawn

                    try
                    {
                        d3dimg.SetBackBuffer(D3DResourceType.IDirect3DSurface9, pSurface);



                        HRESULT.Check(DLL.Render());
                        if (m_RedrawAll)
                        {
                            m_RedrawAll = false;

                            //d3dimg.AddDirtyRect(new Int32Rect((int)0, (int)0, (int)ActualWidth, (int)ActualHeight));

                            // ECT - This is added to prevent the crash
                            // We can't ask for a dirty rect larger than actual surface
                            // 3D does not currently switch to a different adapter and 
                            //   it can only create a surface as large as the current adapter 

                            int d3dWidth = (int)ActualWidth > d3dimg.PixelWidth ? d3dimg.PixelWidth : (int)ActualWidth;
                            int d3dHeight = (int)ActualHeight > d3dimg.PixelHeight ? d3dimg.PixelHeight : (int)ActualHeight;
                            d3dimg.AddDirtyRect(new Int32Rect((int)0, (int)0, (int)d3dWidth, (int)d3dHeight));
                        }
                        else
                        {
                            foreach (ViewInfo vinfo in m_ActiveList)
                            {
                                // d3dimg.AddDirtyRect(vinfo.Loc);

                                // ECT - This is added to prevent the crash
                                // We can't ask for a dirty rect larger than actual surface
                                // 3D does not currently switch to a different adapter and 
                                //   it can only create a surface as large as the current adapter 

                                Int32Rect r = vinfo.Loc;
                                if (r.X < d3dimg.PixelWidth && r.Y < d3dimg.PixelHeight)
                                {
                                    if ((r.X + r.Width) > d3dimg.PixelWidth)
                                        r.Width = d3dimg.PixelWidth - r.X;
                                    if ((r.Y + r.Height) > d3dimg.PixelHeight)
                                        r.Height = d3dimg.PixelHeight - r.Y;
                                    d3dimg.AddDirtyRect(r);
                                }
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        Log.WriteLine("SetBackBuffer threw exception : " + err.Message);
                        //return;
                    }
					d3dimg.Unlock();

					// We have to render things at least one frame after removal so that the areas will "black" out again.
					if (m_RemoveActive.Count > 0)
					{
						foreach (ViewInfo vinfo in m_RemoveActive)
						{
							m_ActiveList.Remove(vinfo);
							vinfo.IsVisible = false;
						}
						m_RemoveActive.Clear();
					}
					m_LastRenderingTime = args.RenderingTime;
				}
			}
		}



		void RedoViews()
		{
			ViewInfo vinfo;
			if (m_AddList.Count > 0)
			{
				foreach (Render3DView view in m_AddList)
				{
					if (!m_Map.TryGetValue(view, out vinfo))
					{
						vinfo = new ViewInfo();
						vinfo.Control = view;
						m_Map.Add(view, vinfo);
						m_ChangedList.Add(view);
					}
				}
				m_AddList.Clear();
			}
			if (m_RemoveList.Count > 0)
			{
				foreach (Render3DView view in m_RemoveList)
					m_Map.Remove(view);
				m_RemoveList.Clear();
			}
			if (m_ChangedList.Count > 0)
			{
				m_RedrawAll = true;
				foreach (Render3DView view in m_ChangedList)
				{
					if (m_Map.TryGetValue(view, out vinfo))
					{
						if (view.IsVisible && view.Unit != null)
						{
							if (!vinfo.IsVisible)
							{
								vinfo.IsVisible = true;
                                vinfo.CreateView();
                                m_ActiveList.Add(vinfo);
								// Create the view for this.
							}
							Point ul = view.TransformToAncestor(this).Transform(new Point(0, 0));
                            Point lr = view.TransformToAncestor(this).Transform(new Point(view.ActualWidth, view.ActualHeight));
							Int32Rect r = new Int32Rect((int)ul.X, (int)ul.Y, (int)(lr.X - ul.X), (int)(lr.Y - ul.Y));

							if (view.FixBackground != null)
							{
								Rectangle rc = vinfo.m_FillRect;
								if (rc == null)
								{
									rc = vinfo.m_FillRect = new Rectangle();
									rc.HorizontalAlignment = HorizontalAlignment.Left;
									rc.VerticalAlignment = VerticalAlignment.Top;
									ControlGrid.Children.Insert(0, rc);
								}
								rc.Margin = new Thickness(r.X, r.Y, 0, 0);
								rc.Width = r.Width;
								rc.Height = r.Height;
								rc.Fill = view.FixBackground;
							}
							else
							{
								Rectangle rc = vinfo.m_FillRect;
								if (rc != null)
								{
									ControlGrid.Children.Remove(rc);
									vinfo.m_FillRect = null;
								}
							}


							if (r != vinfo.Loc)
							{
								//change rectangle here.
								//vinfo.Loc = r;
                                vinfo.MoveView(r);
							}
							
						}
						else
						{
							if (vinfo.IsVisible)
							{
								vinfo.IsVisible = false;
								vinfo.DeleteView();
								// todo adjust the view here -- make it display a blank view the next time.
								m_RemoveActive.Add(vinfo); // Kill this next time through
							}
						}
					}
				}
				m_ChangedList.Clear();
			}
		}

		Grid grid;
		Image img;
		D3DImage d3dimg;
		//bool m_bShutdown;

		Grid debugGrid;


		private Mutex m_Mux = new Mutex();

		class ViewInfo
		{
			public Render3DView Control;

			public Int32Rect Loc = new Int32Rect(0,0,1,1);
			public bool IsVisible;
			public int iView = -1;
            public int iRider = -1;
            public int iRiderModel = 1;
            // temporary value, real value is in the 3D
            public int iCamera = 0;

            public int CreateView()
            {
				if (iView != -1)
					DeleteView();
				Unit unit = Control.Unit;
                iRider = (unit == null ? -1 : unit.iRider);
                IsVisible = true; // set to always show, still require a rider to actually be active
                iView = DLL.CreateView(Loc.X, Loc.Y, Loc.Width, Loc.Height, IsVisible, iRider, iCamera);
				return iView;
            }
            public void DeleteView()
            {
				if (iView != -1)
				{
					DLL.DeleteView(iView);
					iView = -1;
                    iRider = -1;
				}
            }
            public void MoveView(Int32Rect r)
            {
				if (r.X < 0)
					r.X = 0;
				if (r.Y < 0)
					r.Y = 0;
                if (Loc != r)
                {
                    Loc = r;
					if (iView != -1)
						DLL.MoveView( iView, Loc.X, Loc.Y, Loc.Width,Loc.Height );
                }
				if (m_FillRect != null)
				{
					m_FillRect.Margin = new Thickness(Loc.X, Loc.Y, 0, 0);
					m_FillRect.Width = Loc.Width;
					m_FillRect.Height = Loc.Height;
				}
            }
            public void ShowView(bool bShow)
            {
                IsVisible = bShow;
                DLL.ShowView(iView, bShow);
            }
            public void SetCamera(int cam)
            {
                iCamera = cam;
                DLL.SetCamera(iView, iCamera);
            }
            public int GetCamera()
            {
                iCamera = (int)DLL.GetCamera(iView);
                return iCamera;
            }
            // Cycle camera modes from follow, first person, rear view, random near, random far, and fixed last random
            public int NextCamera()
            {
                iCamera = (int)DLL.NextCamera(iView);
                return iCamera;
            }
            public void SetRider(int irider)
            {
                iRider = irider;
                DLL.SetRider(iView, iRider);
            }
            public int GetRider()
            {
                iRider = (int)DLL.GetRider(iView);
                return iRider;
            }
            public int CreateRider(int model)
            {
                Unit unit = Control.Unit;
                int riderType = ((unit.Bot != null) ? 2 : 1); // 2==bot, 1==live
                iRiderModel = model;
                iRider = (int)DLL.CreateRider(iRiderModel, iRiderModel, iRiderModel, 1.0f, 1.0f, 2.0f, 210f, 0, riderType);
                return iRider;
            }

			public Rectangle m_FillRect = null;

        }
		Dictionary<Render3DView, ViewInfo> m_Map = new Dictionary<Render3DView, ViewInfo>();
		List<ViewInfo> m_ActiveList = new List<ViewInfo>();
		List<ViewInfo> m_RemoveActive = new List<ViewInfo>();	// One last time through the render.
		
		List<Render3DView> m_AddList = new List<Render3DView>();
		List<Render3DView> m_RemoveList = new List<Render3DView>();
		List<Render3DView> m_ChangedList = new List<Render3DView>();

		public void Add( Render3DView view )
		{
			m_RemoveList.Remove(view);
			m_AddList.Add(view);
			Activate();
		}
		public void Remove(Render3DView view)
		{
			m_AddList.Remove(view);
			m_RemoveList.Add(view);
			Activate();
		}
		public void Changed(Render3DView view)
		{
			m_ChangedList.Add(view);
			Activate();
		}

		public void ChangeUnit(Render3DView view)
		{
			ViewInfo vinfo;
			if (m_Map.TryGetValue(view, out vinfo))
			{
				if (vinfo.iView != -1)
				{
					int num = vinfo.GetCamera();
					vinfo.DeleteView();
					vinfo.CreateView();
					vinfo.SetCamera(num);
				}
			}

		}

		private void Render3D_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			m_new_width = (uint)e.NewSize.Width;
			m_new_height = (uint)e.NewSize.Height;
            Activate();
		}

		private void Render3D_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			ChangedAll();
		}

		private void ChangedAll()
		{
			foreach (KeyValuePair<Render3DView,ViewInfo> n in m_Map)
			{
				m_ChangedList.Add(n.Key);
			}
		}

		//==============================================================================
		static Render3D ms_Active;	// The one that is active... should only be one.
		public static Render3D Active { get { return ms_Active; } }

		static Thread ms_CourseThread;
		static Course ms_Course;
		static Course ms_CurCourse;
		public static Course Course
		{
			get { return ms_Course; }
			set
			{
				if (ms_Course != value)
				{
					if (ms_CourseThread != null)
						ms_CourseThread.Join();		// We will have to wait.
					ms_Course = value;
					if (value == null)
					{
						ms_CurCourse = null; // Will stop the render in its tracks.
					}
					else
					{
                        if (ms_Course.Looped && !ms_Course.Modified)
                        {
                            string strFullPath = RacerMatePaths.CoursesFullPath + @"\Last3D.rmc";
                            string CopyTo = "Copy - " + Unit.Course.FileName + " " + Unit.Course.Description;
                            ms_Course.OriginalCourse.Save_Special(strFullPath, CopyTo);
                        }
                        else
                        {
                            ms_Course.Save_Special(RacerMatePaths.CoursesFullPath + @"\Last3D.rmc", "Modified - " + Unit.Course.FileName + " " + Unit.Course.Description);
                        }
                        ms_CourseThread = new Thread(new ThreadStart(CourseThread));
						ms_CourseThread.Start();	// Start up the new thread ... don't worry it will just wait for a course to be loaded.
					}
				}
			}
		}
		static void CourseThread()
		{
            App.SetDefaultCulture();

            DLL.LoadCourse(RacerMatePaths.CoursesFullPath + @"\Last3D.rmc", RM1_Settings.General.Scenery.Num); // -1 defaults to standard scenery
            //DLL.LoadCourse(ms_Course.FileName, RM1_Settings.General.Scenery.Num); // -1 defaults to standard scenery
			ms_CurCourse = ms_Course;
			if (ms_Active != null)
				ms_Active.DispatchActivate();
			ms_CourseThread = null;
			//DLL.SendSystemCmds("demo 1");
		}
        public static float ShowProgress()
        {
            if (ms_CourseThread != null)
                return (100.0f * DLL.LoadCourseProgress());
            else
                return 100.0f;
        }

		//====================================================================================
		public static void SetupRiders()
		{
			// Start the riders.
			CloseRiders();
			foreach (Unit unit in Unit.RaceUnit)
			{
                int riderType = ((unit.Bot != null) ? 2 : 1); // 2==bot, 1==live
                unit.iRider = DLL.CreateRider(1, 1, 1, 1.0f, 1.0f, 2.0f, 210f, 0, riderType);
				DLL.SetRiderNumber((int)unit.iRider, (uint)unit.Number);
				unit.Statistics.Drafting = false;
				IRiderModel rm = unit.Bot != null ? (IRiderModel)unit.Bot : (IRiderModel)unit.Rider;
				if (rm != null)
				{
					DLL.SetRiderModel(unit.iRider, (uint)rm.Model);
					uint i;
					for (i = 0; i < (uint)RiderMaterials.Max; i++)
					{
						Color c = rm.GetMaterialColor((RiderMaterials)i);
						DLL.SetRiderColor(unit.iRider, i, c.R, c.G, c.B);
					}
				}
			}
		}
		private static CameraGameMode m_LeadCameraMode = CameraGameMode.Normal;
		public static CameraGameMode CameraMode
		{
			get { return m_LeadCameraMode; }
			set
			{
				m_LeadCameraMode = value;
				DLL.SetGameMode((int)m_LeadCameraMode);
			}
		}



		public static void CloseRiders()
		{
			foreach (Unit unit in Unit.Units)
			{
				if (unit.iRider != -1)
				{
					DLL.DeleteRider(unit.iRider);
					unit.iRider = -1;
					unit.Statistics.Drafting = false;
					UpdateRider(unit);
				}
			}
		}

        // Justfor debugging
        public static void ToggleFOV()
        {
            try
            {
                DLL.ToggleFOV();
            }
            catch { }
        }
        public static void ShowInfo()
        {
            try
            {
                DLL.ShowInfo();
            }
            catch { }
        }
        // Cycle camera modes from follow, first person, rear view, random near, random far, and fixed last random
        public static void NextCameraRider(Unit unit)
        {
            RM1.Trainer.LockStats();
            try
            {
                DLL.NextCameraRider(unit.iRider);
            }
			catch (Exception ex) { RM1.MutexException(ex); }
            RM1.Trainer.UnlockStats();
        }
		public static int GetCameraRider(Unit unit)
		{
			int num = 0;
			RM1.Trainer.LockStats();
			try
			{
				num = DLL.GetCameraRider(unit.iRider);
			}
			catch (Exception ex) { RM1.MutexException(ex); }
			RM1.Trainer.UnlockStats();
			return num;
		}
		public static void SetCameraRider(Unit unit, int number)
		{
			RM1.Trainer.LockStats();
			try
			{
				DLL.SetCameraRider(unit.iRider,number);
			}
			catch (Exception ex) { RM1.MutexException(ex); }
			RM1.Trainer.UnlockStats();
		}

        public static void UpdateRider(Unit unit)
		{
			RM1.Trainer.LockStats();
			try
			{
				double sub = (DateTime.Now.Ticks - RM1.Trainer.LastTicks) * ConvertConst.HundredNanosecondToSecond;
				UpdateRider(unit, sub);
			}
			catch (Exception ex) { RM1.MutexException(ex); }
			RM1.Trainer.UnlockStats();
		}
		//public static bool Drafting = false;
		protected static void UpdateRider(Unit unit, double sub )
		{
			if (unit.iRider == -1)
				return;
			Statistics s = unit.Statistics;
			DLL.SetRiderDistance(unit.iRider, (float)s.TrackDistanceSub(sub));

			double rpm = s.Cadence3D;
			double sp = (rpm / 60.0) * 24.0;   // Rotations per second * 24 for one rotation per second.

			DLL.SetRiderSpeed(unit.iRider, (float)sp);
			DLL.SetRiderRPM(unit.iRider, (float)sp);

			// TODO: We might need to refine the drafting.
			unit.Statistics.Drafting = DLL.IsRiderDrafting(unit.iRider); //Drafting; //  
		}

		public static void UpdateRiders()
		{
			RM1.Trainer.LockStats();
			try
			{
				double sub = (DateTime.Now.Ticks - RM1.Trainer.LastTicks) * ConvertConst.HundredNanosecondToSecond;
				foreach (Unit unit in Unit.RaceUnit)
				{
					UpdateRider(unit,sub);
				}
			}
			catch (Exception ex) { RM1.MutexException(ex); }
			RM1.Trainer.UnlockStats();
		}


		public static void Start()
		{
			DLL.StartRace();
		}
		public static void Stop()
		{	
		}
		public static void Reset()
		{
			DLL.ResetRace();
		}

		public static void Pause()
		{
			DLL.PauseRace();
		}
		public static void UnPause()
		{
			DLL.RunRace();
		}
		//====================================================================================
		// Scenery
		//====================================================================================
		public class SceneryInfo
		{
			String m_ID = null;
			public String ID 
			{
				get
				{
					if (m_ID == null)
						m_ID = DLL.GetSceneryID( Num );
					return m_ID;
				}
			}


			public int Num { get; protected set; }
			String m_Name = null;
			public String Name
			{

				get
				{
					if (m_Name == null)
						m_Name = DLL.GetSceneryName(Num);
					return m_Name;
				}					
			}
			public override String ToString()
			{
				return Name;
			}

			BitmapImage m_Thumbnail;
			public BitmapImage Thumbnail
			{
				get
				{
					if (m_Thumbnail == null)
					{
						m_Thumbnail = new BitmapImage();
						m_Thumbnail.BeginInit();
						m_Thumbnail.UriSource = new Uri(DLL.GetSceneryThumbnailFilename(Num));
						m_Thumbnail.EndInit();
					}
					return m_Thumbnail;
				}
			}
			protected SceneryInfo( int num )
			{
				Num = num;
			}

			protected static ObservableCollection<SceneryInfo> ms_Scenery;
			public static ObservableCollection<SceneryInfo> SceneryList
			{
				get
				{
					if (ms_Scenery == null)
					{
						ms_Scenery = new ObservableCollection<SceneryInfo>();
						int cnt = DLL.GetSceneryCount();
						int i;
						for (i = 0; i < cnt; i++)
						{
							ms_Scenery.Add(new SceneryInfo(i));
						}							
					}
					return ms_Scenery;
				}
			}
			public static SceneryInfo Find(String id)
			{
				foreach (SceneryInfo si in SceneryList)
				{
					if (String.Compare(id, si.ID, true) == 0)
						return si;
				}
				return null;
			}
		}

		static public bool m_ShowDemoRider = false;
		static public bool ShowDemoRider
		{
			get { return m_ShowDemoRider; }
			set
			{
				m_ShowDemoRider = value;
				DLL.ShowDemoRider(value);
			}
		}

		//====================================================================================
		// DLL
		//====================================================================================

        static public class DLL
		{
            private struct PointAPI
            {
                public int X;
                public int Y;
            } 

            [DllImport("user32.dll")]
            private static extern int GetCursorPos(out PointAPI lpPoint);

            [DllImport("user32.dll")]
            private static extern Int32 SetCursorPos(Int32 x, Int32 y);

            // Stops the screen saver by moving the cursor. 
            public static void StopScreenSaver()
            {
                PointAPI p = new PointAPI();
                GetCursorPos(out p);
                SetCursorPos(new Random().Next(100), new Random().Next(100));
                SetCursorPos(p.X, p.Y);
            }

            [DllImport("RM1_Ext.dll")]
            public static extern bool IsRegisteredValid();

            [DllImport("RM1_Ext.dll")]
            private static extern int SetBasePath(IntPtr basepath);
            public static void InitBasePath()
            {
                String basepath = Directory.GetCurrentDirectory(); //System.IO.Path.GetDirectoryName(Assembly.GetAssembly(typeof(App)).CodeBase); 
                IntPtr fname = Marshal.StringToHGlobalAnsi(basepath);
                SetBasePath(fname);
                Marshal.FreeHGlobal(fname);
            }

            [DllImport("RM1_Ext.dll")]
			private static extern int LoadCourse(IntPtr coursename, int iscenery);
            public static void LoadCourse(String coursename, int iscenery)
			{
				IntPtr fname = Marshal.StringToHGlobalAnsi(coursename);
                LoadCourse(fname, iscenery);
                Marshal.FreeHGlobal(fname);
			}

            [DllImport("RM1_Ext.dll")]
            private static extern int LoadCourseProgress(out IntPtr progress);
            public static float LoadCourseProgress()
            {
                IntPtr result = IntPtr.Zero;
                LoadCourseProgress(out result);
                float ret = (int)result;
                return (ret / 100.0f);
            }

            [DllImport("RM1_Ext.dll")]
			public static extern int GetBackBufferNoRef(out IntPtr pSurface);

			[DllImport("RM1_Ext.dll")]
			public static extern int SetSize(ref uint width, ref uint height);

			[DllImport("RM1_Ext.dll")]
			public static extern int Render();

            [DllImport("RM1_Ext.dll")]
            public static extern int Render2();

            [DllImport("RM1_Ext.dll")]
			static extern int SendSystemCmds(IntPtr cmdstring, out IntPtr pResult);
			public static IntPtr SendSystemCmds(String cmdstr)
			{
				IntPtr result = IntPtr.Zero;
				IntPtr cmd = Marshal.StringToHGlobalAnsi(cmdstr);
				SendSystemCmds(cmd, out result);
                Marshal.FreeHGlobal(cmd);
                return result;
			}

			[DllImport("RM1_Ext.dll")]
			static extern int SendRiderCmds(int rider, IntPtr cmdstring, out IntPtr pResult);
			public static IntPtr SendRiderCmds(int rider, string cmdstr)
			{
				IntPtr result = IntPtr.Zero;
				IntPtr cmd = Marshal.StringToHGlobalAnsi(cmdstr);
				SendRiderCmds(rider, cmd, out result);
                Marshal.FreeHGlobal(cmd);
				return result;
			}

			public static void SetReal(int irider, bool bReal)
			{
				int val;
				if (bReal)
					val = 1;
				else
					val = 0;
				SendRiderCmds(irider, "breal " + val);
			}

			// Reset 3D race
			public static void ResetRace()
			{
				SendSystemCmds("reset");
			}
			// Pause 3D race
			public static void PauseRace()
			{
				SendSystemCmds("pause");
			}
			// Start 3D race
			public static void StartRace()
			{
				SendSystemCmds("start");
			}
			// Run 3D race
			public static void RunRace()
			{
				SendSystemCmds("run");
			}


			public static int CreateView(int x, int y, int width, int height, bool show, int iRider, int iCamera)
			{
				return (int)SendSystemCmds(string.Format("_CreateView {0},{1},{2},{3},{4},{5},{6}", x, y, width, height, show ? 1 : 0, iRider, iCamera));
			}
			public static int MoveView(int iview, int x, int y, int width, int height)
			{
				return (int)SendSystemCmds(string.Format("_MoveView {0},{1},{2},{3},{4}", iview, x, y, width, height));
			}
			public static int DeleteView(int iview)
			{
				return (int)SendSystemCmds(string.Format("_DeleteView {0}", iview));
			}
			public static int ShowView(int iview, bool bShow)
			{
                return (int)SendSystemCmds(string.Format("_ShowView {0},{1}", iview, bShow ? 1 : 0));
			}
			public static int GetCamera(int iview)
			{
                return (int)SendSystemCmds(string.Format("_GetCameraView {0}", iview));
			}
			public static int SetCamera(int iview, int iCamera)
			{
                return (int)SendSystemCmds(string.Format("_SetCameraView {0},{1}", iview, iCamera));
			}
            public static int GetRider(int iview)
            {
                return (int)SendSystemCmds(string.Format("_GetRiderView {0}", iview));
            }
            public static int SetRider(int iview, int iRider)
            {
                return (int)SendSystemCmds(string.Format("_SetRiderView {0},{1}", iview, iRider));
            }
            // Cycle camera modes from follow, random, and fixed last random
            public static int NextCamera(int iview)
            {
                return (int)SendSystemCmds(string.Format("_NextCameraView {0}", iview));
            }
            // Cycle camera modes from follow, random, and fixed last random
            public static int NextCameraRider(int irider)
            {
                return (int)SendSystemCmds(string.Format("_NextCameraRider {0}", irider));
            }
			public static int GetCameraRider(int irider)
			{
				return (int)SendSystemCmds(string.Format("_GetCameraRider {0}", irider));
			}
			public static int SetCameraRider(int irider, int icamera)
			{
				return (int)SendSystemCmds(string.Format("_SetCameraRider {0} {1}", irider,icamera));
			}
			public static int CreateRider(int ridermodel, int bikemodel, int tiremodel, float bike_frame_size, float tire_size, float rider_height, float rider_weight, uint color, int ridemode)
			{
				int rider = (int)SendSystemCmds(string.Format("_CreateRider {0},{1},{2},{3},{4},{5},{6},{7},{8} ",
                    ridemode, ridermodel, bikemodel, tiremodel, bike_frame_size, tire_size, rider_height, rider_weight, color));
				SetReal(rider, true); 
				return rider;
			}

            public static int SetRideMode(int iRider, int iRideMode)
            {
                return (int)SendSystemCmds(string.Format("_SetRideMode {0},{1}", iRider, iRideMode));
            }

			public static int DeleteRider(int iRider)
			{
				return (int)SendSystemCmds(string.Format("_DeleteRider {0}", iRider));
			}

            public static int ShowDemoRider(bool show)
            {
                return (int)SendSystemCmds(string.Format("_ShowDemoRider {0}", show ? 1 : 0));
            }

            // change this rider's model 
            /*
            enum RIDER_MODEL
            {
                MODEL_BOX = 0,
                MODEL_MALE,
                MODEL_FEMALE,
                MODEL_CHROME,
                MODEL_GOLD,
                MODEL_X,
                MODEL_MAX
            };
            */
            public static int SetRiderModel(int iRider, uint iModelIdx)
            {
                return (int)SendSystemCmds(string.Format("_SetRiderModel {0},{1}", iRider, iModelIdx));
            }
            // change this rider's colors like bike, tires, skin, hair, helmet, shirt, pants, and shoes
            /*
            enum RIDER_COLORS {
                RIDER_BIKE=0,
                RIDER_TIRES,
                RIDER_SKIN,
                RIDER_HAIR,
                RIDER_HELMET,
                RIDER_JERSEY_TOP,
                RIDER_JERSEY_BOTTOM,
                RIDER_SHOES,
                RIDER_COLORS_MAX
            };             
             */
            public static int SetRiderColor(int iRider, uint iColorIdx, uint r, uint g, uint b)
            {
                return (int)SendSystemCmds(string.Format("_SetRiderColor {0},{1},{2},{3},{4}", iRider, iColorIdx, r, g, b));
            }
            public static int SetRiderReal(int iRider, bool bReal)
            {
                return (int)SendSystemCmds(string.Format("_SetRiderReal {0},{1}", iRider, bReal ? 1 : 0));
            }
            public static int SetRiderSpeed(int iRider, float speed)
			{
				return (int)SendSystemCmds(string.Format("_SetRiderSpeed {0},{1}", iRider, speed));
			}
			public static int SetRiderRPM(int iRider, float rpm)
			{
				return (int)SendSystemCmds(string.Format("_SetRiderRPM {0},{1}", iRider, rpm));
			}
			public static int SetRiderDistance(int iRider, float distance)
			{
				return (int)SendSystemCmds(string.Format("_SetRiderDistance {0},{1}", iRider, distance));
			}
			public static bool IsRiderDrafting(int iRider)
			{
				int ret = (int)SendSystemCmds(string.Format("_IsRiderDrafting {0}", iRider));
				return (ret != 0);
			}
            // sets rider's number bubble over its head.
            public static int SetRiderNumber(int iRider, uint iRiderNumber)
            {
                return (int)SendSystemCmds(string.Format("_SetRiderNumber {0},{1}", iRider, iRiderNumber));
            }

            /*
            enum GAME_MODE
            {
                GAME_NORMAL, // what we have now
                GAME_LEAD,   // this selects the rider in front
                GAME_MAX
            };
             */
            public static int SetGameMode(int iGameMode)
            {
                return (int)SendSystemCmds(string.Format("_SetGameMode {0}", iGameMode));
            }



            // Below are other functions

            // PreStart 3D race with Countdown
            public static void PreStartRace()
            {
                SendSystemCmds("prestart");
            }

            // modify speed of the rider
            public static void AddRiderSpeed(int irider, float speed)
            {
                SendRiderCmds(irider, "+speed " + speed);
            }
            // modify the rider's distance
            public static void AddRiderDistance(int irider, float dist)
            {
                SendRiderCmds(irider, "+dist " + dist);
            }
            // change this rider's model 
            public static void UseRider(int irider, int idx)
            {
                SendRiderCmds(irider, "model " + idx);
            }
            // get riders current model
            public static int GetRiderModel(int irider)
            {
                return (int)SendRiderCmds(irider, "=model");
            }
            // get current rider's distance in integer
            public static int GetRiderIntDist(int irider)
            {
                return (int)SendRiderCmds(irider, "=dist");
            }
            // get current rider's state - like finish, race 
            public static int GetRiderState(int irider)
            {
                return (int)SendRiderCmds(irider, "=state");
            }


            // for Scenery
            public static int GetSceneryCount()
            {
                return (int)SendSystemCmds("_GetSceneryCount");
            }
            public static string GetSceneryName(int iscenery)
            {
                IntPtr result = IntPtr.Zero;
                IntPtr cmd = Marshal.StringToHGlobalAnsi(string.Format("_GetSceneryName {0}", iscenery));
                DLL.SendSystemCmds(cmd, out result);
                Marshal.FreeHGlobal(cmd);
                string name = Marshal.PtrToStringAnsi(result);
                return name;
/*
                string name = Marshal.PtrToStringAnsi(result).ToLower();
                string desc = "Standard";
				if (name.EndsWith("\r"))
					name.Remove(name.Length - 1, 1);
                if (name.EndsWith("cc"))
                    desc = "CycloCross";
                else
                if (name.EndsWith("rl"))
                    desc = "No Road Line";
                return string.Format(@"{0}, {1}",name,desc);
 */ 
            }
			public static string GetSceneryID(int iscenery)
			{
				IntPtr result = IntPtr.Zero;
				IntPtr cmd = Marshal.StringToHGlobalAnsi(string.Format("_GetSceneryID {0}", iscenery));
				DLL.SendSystemCmds(cmd, out result);
				Marshal.FreeHGlobal(cmd);
				string name = Marshal.PtrToStringAnsi(result);
				return name;
			}


            public static string GetSceneryThumbnailFilename(int iscenery)
            {
                IntPtr result = IntPtr.Zero;
                IntPtr cmd = Marshal.StringToHGlobalAnsi(string.Format("_GetSceneryThumbnailFilename {0}", iscenery));
                DLL.SendSystemCmds(cmd, out result);
                Marshal.FreeHGlobal(cmd);
                string name = Marshal.PtrToStringAnsi(result);
                return name;
            }


            // for testing the 3D Render speed

            // toggle the info bar to show or hide
            public static void ToggleFOV()
            {
                SendSystemCmds("toggleFOV");
            }
            public static void ShowInfo()
            {
                SendSystemCmds("info");
            }
            // toggle the whole scene to show or hide
            public static void ShowScene()
            {
                SendSystemCmds("showscene");
            }
            // toggle the trees to show or hide
            public static void ShowTrees()
            {
                SendSystemCmds("showtrees");
            }
            // toggle the course to show or hide
            public static void ShowCourse()
            {
                SendSystemCmds("showcourse");
            }
            // toggle the riders to show or hide
            public static void ShowRiders()
            {
                SendSystemCmds("showriders");
            }
            // toggle the skydome to show or hide
            public static void ShowDome()
            {
                SendSystemCmds("showdome");
            }
            // get the number of active riders
            public static int GetNumRiders()
            {
                return (int)SendSystemCmds("=riders");
            }
            // get the number of active views
            public static int GetNumViews()
            {
                return (int)SendSystemCmds("=views");
            }
            // get current race state
            public static int GetRaceState()
            {
                return (int)SendSystemCmds("=state");
            }
            // get current FPS
            public static int GetFPS()
            {
                return (int)SendSystemCmds("=fps");
            }

		}


	}

}
