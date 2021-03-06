/*

   Copyright 2016 Esri

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

   See the License for the specific language governing permissions and
   limitations under the License.

*/
using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.ADF.CATIDs;
using System.Runtime.InteropServices;

namespace PanZoom
{
	[ClassInterface(ClassInterfaceType.None)]
	[Guid("2C214724-BFA2-4e8c-BC5D-775C67FA6F51")]

	public class ZoomIn : ICommand, ITool
	{
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Unregister(regKey);

        }

        #endregion
        #endregion
		[DllImport("gdi32.dll")]
		static extern bool DeleteObject(IntPtr hObject);

		private System.Drawing.Bitmap m_bitmap;
		private IntPtr m_hBitmap;
		private IHookHelper m_pHookHelper;
		private INewEnvelopeFeedback m_feedBack;
		private IPoint m_point;
		private Boolean m_isMouseDown;
		private System.Windows.Forms.Cursor m_zoomInCur;
		private System.Windows.Forms.Cursor m_moveZoomInCur;

		public ZoomIn()
		{
			//Load resources
			string[] res = GetType().Assembly.GetManifestResourceNames();
			if(res.GetLength(0) > 0)
			{
				m_bitmap = new System.Drawing.Bitmap(GetType().Assembly.GetManifestResourceStream(GetType(), "ZoomIn.bmp"));
				if(m_bitmap != null)
				{
					m_bitmap.MakeTransparent(m_bitmap.GetPixel(1,1));
					m_hBitmap = m_bitmap.GetHbitmap();
				}
			}
			m_pHookHelper = new HookHelperClass ();
		}
	
		~ZoomIn()
		{
			if(m_hBitmap.ToInt32() != 0)
				DeleteObject(m_hBitmap);
		}

		#region ICommand Members

		public void OnClick()
		{
		}

		public string Message
		{
			get
			{
				return "zooms the Display In By Rectangle or Single Click";
			}
		}

		public int Bitmap
		{
			get
			{
				return m_hBitmap.ToInt32();
			}
		}

		public void OnCreate(object hook)
		{
			m_pHookHelper.Hook = hook;
			m_zoomInCur = new System.Windows.Forms.Cursor(GetType().Assembly.GetManifestResourceStream(GetType(), "ZoomIn.cur"));
			m_moveZoomInCur = new System.Windows.Forms.Cursor(GetType().Assembly.GetManifestResourceStream(GetType(), "MoveZoomIn.cur"));
		}

		public string Caption
		{
			get
			{
				return "Zoom In";
			}
		}

		public string Tooltip
		{
			get
			{
				return "Zoom In";
			}
		}

		public int HelpContextID
		{
			get
			{
				// TODO:  Add ZoomIn.HelpContextID getter implementation
				return 0;
			}
		}

		public string Name
		{
			get
			{
				return "Sample_Pan/Zoom_Zoom In";
			}
		}

		public bool Checked
		{
			get
			{
				return false;
			}
		}

		public bool Enabled
		{
			get
			{
				if(m_pHookHelper.FocusMap == null) return false;

				return true;
			}
		}

		public string HelpFile
		{
			get
			{
				// TODO:  Add ZoomIn.HelpFile getter implementation
				return null;
			}
		}

		public string Category
		{
			get
			{
				return "Sample_Pan/Zoom";
			}
		}

		#endregion
	
		#region ITool Members

		public void OnMouseDown(int button, int shift, int x, int y)
		{
			if(m_pHookHelper.ActiveView == null) return;

			//If the active view is a page layout
			if(m_pHookHelper.ActiveView is IPageLayout)
			{
				//Create a point in map coordinates
				IPoint pPoint = (IPoint) m_pHookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);

				//Get the map if the point is within a data frame
				IMap pMap = m_pHookHelper.ActiveView.HitTestMap(pPoint);

				if(pMap == null) return;

				//Set the map to be the page layout's focus map
				if(pMap != m_pHookHelper.FocusMap)
				{
					m_pHookHelper.ActiveView.FocusMap = pMap;
					m_pHookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
				}
			}
			//Create a point in map coordinates
			IActiveView pActiveView = (IActiveView) m_pHookHelper.FocusMap;
			m_point = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);

			m_isMouseDown = true;

		}

		public void OnMouseMove(int button, int shift, int x, int y)
		{
			if(!m_isMouseDown) return;

			//Get the focus map
			IActiveView pActiveView = (IActiveView) m_pHookHelper.FocusMap;

			//Start an envelope feedback
			if(m_feedBack == null)
			{
				m_feedBack = new NewEnvelopeFeedbackClass();
				m_feedBack.Display = pActiveView.ScreenDisplay;
				m_feedBack.Start(m_point);
			}

			//Move the envelope feedback
			m_feedBack.MoveTo(pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y));
		}

		public void OnMouseUp(int button, int shift, int x, int y)
		{
			if(!m_isMouseDown) return;

			//Get the focus map
			IActiveView pActiveView = (IActiveView) m_pHookHelper.FocusMap;

			//If an envelope has not been tracked
			IEnvelope pEnvelope;

			if(m_feedBack == null)
			{
				//Zoom in from mouse click
				pEnvelope = pActiveView.Extent;
				pEnvelope.Expand(0.5, 0.5, true);
				pEnvelope.CenterAt(m_point);
			}
			else
			{
				//Stop the envelope feedback
				pEnvelope = m_feedBack.Stop();

				//Exit if the envelope height or width is 0
				if(pEnvelope.Width ==0 || pEnvelope.Height == 0)
				{
					m_feedBack = null;
					m_isMouseDown = false;
				}
			}
			//Set the new extent
			pActiveView.Extent = pEnvelope;
			//Refresh the active view
			pActiveView.Refresh();
			m_feedBack = null;
			m_isMouseDown = false;
		}

		public void OnKeyDown(int keyCode, int shift)
		{
			if(m_isMouseDown)
			{
				if(keyCode == 27)
				{
					m_isMouseDown = false;
					m_feedBack = null;
					m_pHookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
				}
			}
		}

		public void OnKeyUp(int keyCode, int shift)
		{
			// TODO:  Add ZoomIn.OnKeyUp implementation
		}

		public int Cursor
		{
			get
			{
				if(m_isMouseDown)
					return m_moveZoomInCur.Handle.ToInt32();
				else
					return m_zoomInCur.Handle.ToInt32();
			}
		}
		
		public bool OnContextMenu(int x, int y)
		{
			// TODO:  Add ZoomIn.OnContextMenu implementation
			return false;
		}

		public bool Deactivate()
		{
			return true;
		}

		public void Refresh(int hdc)
		{
			// TODO:  Add ZoomIn.Refresh implementation
		}

		public void OnDblClick()
		{
			// TODO:  Add ZoomIn.OnDblClick implementation
		}

		#endregion
	}
}