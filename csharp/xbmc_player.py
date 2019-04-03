import xbmc
import json

#class xbmc_player(xbmc.Player):
class xbmc_player(xbmc.Player):
	
	def make_player(self):
		player = xbmc.Player()
		for attr in dir(player):
			if not attr is "__class__":
				setattr(self, attr, getattr(player, attr))

		player.onPlayBackEnded = self.onPlayBackEnded
		player.onPlayBackPaused = self.onPlayBackPaused
		player.onPlayBackResumed = self.onPlayBackResumed
		player.onPlayBackSeek = self.onPlayBackSeek
		player.onPlayBackSeekChapter = self.onPlayBackSeekChapter
		player.onPlayBackSpeedChanged = self.onPlayBackSpeedChanged
		player.onPlayBackStarted = self.onPlayBackStarted
		player.onQueueNextItem = self.onQueueNextItem
		self.player = player

	def __init__(self):
		super(xbmc_player, self).__init__()

	# NOTE: this *HAS* to be separate from __init__, because apparently
	# xbmc.Player will try to call __init__ and fail to do so (it expects it to have no arguments)
	def set_bridge(self, bridge):
		self.bridge = bridge

	def post(self, eventName, *args, **kwargs):
		jDict = {
			"sender": "Player",
			"source": eventName,
			"args": args,
			"kwargs": kwargs
		}
		self.bridge.module.PostEvent(self.bridge.plugin_handle, json.dumps(jDict))

	def onPlayBackEnded(self, *args, **kwargs):
		self.post("onPlayBackEnded", *args, **kwargs)
	
	def onPlayBackPaused(self, *args, **kwargs):
		self.post("onPlayBackPaused", *args, **kwargs)
	
	def onPlayBackResumed(self, *args, **kwargs):
		self.post("onPlayBackResumed", *args, **kwargs)
	
	def onPlayBackSeek(self, *args, **kwargs):
		self.post("onPlayBackSeek", *args, **kwargs)
	
	def onPlayBackSeekChapter(self, *args, **kwargs):
		self.post("onPlayBackSeekChapter", *args, **kwargs)
	
	def onPlayBackSpeedChanged(self, *args, **kwargs):
		self.post("onPlayBackSpeedChanged", *args, **kwargs)
	
	def onPlayBackStarted(self, *args, **kwargs):
		self.post("onPlayBackStarted", *args, **kwargs)
	
	def onQueueNextItem(self, *args, **kwargs):
		self.post("onQueueNextItem", *args, **kwargs)