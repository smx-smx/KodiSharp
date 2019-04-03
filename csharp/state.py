"""
This file contains methods, modules and variables that are used by C#
"""

import xbmc
import xbmcgui
import xbmcplugin
import xbmcaddon
import xbmcvfs
import sys

import threading

from xbmc_monitor import xbmc_monitor
from xbmc_player import xbmc_player

class MonitorThread(threading.Thread):
    def __init__(self, monitor):
        self.monitor = monitor
    
    def run(self):
        threading.Event().wait()

class State(object):
    LastResult = None
    bridge = None
    Variables = {}

    def set_vars(self, *args, **kwargs):
        print(kwargs)
        for key, value in kwargs.items():
            #print("Setting %s" % key)
            self.__dict__[key] = value

    def __init__(self, bridge):
        self.bridge = bridge
    
    def get_result(self):
        return self.LastResult

    def new_monitor(self):
        monitor = xbmc_monitor()
        monitor.set_bridge(self.bridge)
        return monitor

    def new_player(self):
        player = xbmc_player()
        player.set_bridge(self.bridge)
        return player

    def eval(self, code):
        eval(compile(code, '<string>', 'exec'), globals(), locals())
        #eval(compile(code, '<string>', 'exec'), self.AllVars, locals())
