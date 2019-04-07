import xbmc
import json

class xbmc_monitor(xbmc.Monitor):

	def __init__(self):
		super(xbmc_monitor, self).__init__()

	def set_bridge(self, bridge):
		self.bridge = bridge

	def post(self, eventName, *args, **kwargs):
		jDict = {
			"sender": "Monitor",
			"source": eventName,
			"args": args,
			"kwargs": kwargs
		}
		self.bridge.post_event(json.dumps(jDict))

	def onAbortRequested(self, *args, **kwargs):
		self.post("onAbortRequested", *args, **kwargs)

	def onCleanStarted(self, *args, **kwargs):
		self.post("onCleanStarted", *args, **kwargs)
	
	def onCleanFinished(self, *args, **kwargs):
		self.post("onCleanFinished", *args, **kwargs)
	
	def onDPMSActivated(self, *args, **kwargs):
		self.post("onDPMSActivated", *args, **kwargs)
	
	def onDPMSDeactivated(self, *args, **kwargs):
		self.post("onDPMSDeactivated", *args, **kwargs)
	
	def onDatabaseScanStarted(self, *args, **kwargs):
		self.post("onDatabaseScanStarted", *args, **kwargs)
	
	def onDatabaseUpdated(self, *args, **kwargs):
		self.post("onDatabaseUpdated", *args, **kwargs)
	
	def onNotification(self, *args, **kwargs):
		self.post("onNotification", *args, **kwargs)
	
	def onScanStarted(self, *args, **kwargs):
		self.post("onScanStarted", *args, **kwargs)
	
	def onScanFinished(self, *args, **kwargs):
		self.post("onScanFinished", *args, **kwargs)
	
	def onScreensaverActivated(self, *args, **kwargs):
		self.post("onScreensaverActivated", *args, **kwargs)
	
	def onScreensaverDeactivated(self, *args, **kwargs):
		self.post("onScreensaverDeactivated", *args, **kwargs)
	
	def onSettingsChanged(self, *args, **kwargs):
		self.post("onSettingsChanged", *args, **kwargs)
