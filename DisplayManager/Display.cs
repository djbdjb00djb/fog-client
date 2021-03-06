﻿
using System;
using System.Runtime.InteropServices;

namespace FOG {
	/// <summary>
	/// Contains functionality to resize display
	/// </summary>
	public class Display {

		private User_32.DEVMODE1 devMode;
		private const String LOG_NAME = "DisplayManager:Display";
		private Boolean settingsPopulated;
		
		public Display() {
			
			this.devMode = new User_32.DEVMODE1();
			this.devMode.dmDeviceName = new String(new char[32]);
			this.devMode.dmFormName = new String(new char[32]);
			this.devMode.dmSize = (short)Marshal.SizeOf(this.devMode);
			this.settingsPopulated = updateSettings();

		}
		
		public Boolean updateSettings() {
			if(User_32.EnumDisplaySettings(null, User_32.ENUM_CURRENT_SETTINGS, ref this.devMode) !=0) {
				return true;
			}			
			LogHandler.Log(LOG_NAME, "Unable to load display settings");
			return false;
		}
		
		public Boolean settingsLoaded() {
			return this.settingsPopulated;
		}
		
		public User_32.DEVMODE1 getSettings() {
			return this.devMode;
		}
		
		public void changeResolution(String device, int width, int height, int refresh) {
			if(settingsLoaded()) {
				this.devMode.dmPelsWidth = width;
				this.devMode.dmPelsHeight = height;
				this.devMode.dmDisplayFrequency = refresh;
				this.devMode.dmDeviceName = device;
				
				//Test changing the resolution first
				LogHandler.Log(LOG_NAME, "Testing resolution to ensure it is compatible");
				int changeStatus = User_32.ChangeDisplaySettings(ref this.devMode, User_32.CDS_TEST);
				
				if(changeStatus.Equals(User_32.DISP_CHANGE_FAILED)) {
					LogHandler.Log(LOG_NAME, "Failed");
				} else {
					LogHandler.Log(LOG_NAME, "Changing resolution");
					changeStatus = User_32.ChangeDisplaySettings(ref this.devMode, User_32.CDS_UPDATEREGISTRY);
					
					if(changeStatus.Equals(User_32.DISP_CHANGE_SUCCESSFUL)) {
						LogHandler.Log(LOG_NAME, "Success");
					} else if(changeStatus.Equals(User_32.DISP_CHANGE_RESTART)) {
						LogHandler.Log(LOG_NAME, "Success, requires reboot");
					} else if(changeStatus.Equals(User_32.DISP_CHANGE_FAILED)) {
						LogHandler.Log(LOG_NAME, "Failed");
					}
				}
				
			} else {
				
			}
		}
	}
}
