"""
C# module loader
"""

import threading
import os
import sys
from module import Module
from rpc import RPC
from state import State
from lock import g_closed

from ctypes import *

# Pointer to a pyhton function that will be invoked to handle the message
on_message_delegate = CFUNCTYPE(c_char_p, c_char_p)
# Pointer to a python function that will be invoked on exit
on_exit_delegate = CFUNCTYPE(None)

common_imports = {
    ## Invokes the entry point
    "PluginMain": ([c_size_t], c_int),

    ## Send the event message in JSON format
    "PostEvent" : ([c_size_t, c_char_p], c_bool),
}

proxy_imports = {
    #"createInstance": ([c_char_p, c_char_p], c_int),
    "clrInit": ([c_char_p, c_char_p, on_message_delegate, on_exit_delegate, c_bool], c_size_t),
    #"setMainMethodName": ([c_char_p], None),
    "clrDeInit": ([c_size_t], None)
}

class Bridge(object):
    def __init__(self, library_path, assembly_path, enable_debug=False):
        self.is_windows = "win" in sys.platform

        print(library_path)
        self.module = Module(library_path)

        state = State(self)
        self.rpc = RPC(state)

        # Prepare delegates
        self.on_message = on_message_delegate(RPC.on_message)
        self.on_exit = on_exit_delegate(Bridge.on_exit)

        self._imp_common()
        self._imp_proxy()

        pluginDir = os.path.dirname(
            os.path.abspath(
                sys.modules['__main__'].__file__
            )
        )

        # Initialize AppDomain and call initialize from the plugin
        self.plugin_handle = self.module.clrInit(
            assembly_path, pluginDir,
            self.on_message,
            self.on_exit,
            enable_debug
        )

    def post_event(self, jsonString):
        self.module.PostEvent(self.plugin_handle, jsonString)

    def run(self):
        g_closed.clear()

        print("GOING TO CALL C# ENTRY")
        self.module.PluginMain(self.plugin_handle)

        print("RETURNED FROM C#")
        # wait for C# to unblock us
        g_closed.wait()

    '''
    C# Calls this method when it's done and handles control back to Python
    '''
    @staticmethod
    def on_exit():
        print("=> OnExit called")
        g_closed.set()
        pass

    def _imp_common(self):
        return self.module.import_methods(common_imports)

    def _imp_proxy(self):
        return self.module.import_methods(proxy_imports)
