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
    ## Prepares the plugin to be ran
    "Initialize": ([on_message_delegate, on_exit_delegate, c_bool], c_bool),

    ## Invokes the entry point
    "PluginMain": ([], c_int),

    ## Send the event message in JSON format
    "PostEvent" : ([c_char_p], c_bool),

    ## Signals Python to stop listening for messages
    "StopRPC": ([], c_bool),
}

monohost_imports = {
    "createInstance": ([c_char_p, c_char_p], c_int),
    "clrInit": ([c_char_p, on_message_delegate, on_exit_delegate], c_int),
    "setMainMethodName": ([c_char_p], None),
    "clrDeInit": ([], None)
}

class Bridge(object):
    def __init__(self, library_path, assembly, enable_debug=False):
        self.is_windows = "win" in sys.platform

        self.module = Module(library_path)

        #print("Init State: %s" % extra_vars)
        #state = State(**extra_vars)
        state = State(self)

        self.rpc = RPC(state)

        # Prepare delegates
        self.on_message = on_message_delegate(RPC.on_message)
        self.on_exit = on_exit_delegate(Bridge._on_exit)

        self._imp_common()
        # Initialize MonoHost on Unix
        if not self.is_windows:
            self._imp_monohost()

            self.module.setMainMethodName(str(assembly.entry))

            self.module.clrInit(
                assembly.path,
                self.on_message,
                self.on_exit
            )

        # Call initialize from the plugin
        self.module.Initialize(
            self.on_message,
            self.on_exit,
            enable_debug
        )

    def run(self):
        g_closed.clear()

        self.module.PluginMain()

        # wait synchonously for C# to unblock us
        g_closed.wait()

    def _initialize(self):
        if not self.is_windows:
            self.module.clrInit()

    @staticmethod
    def _reaper():
        g_closed.set()

    @staticmethod
    def _on_exit():
        print("=> OnExit called")
        # Start a thread so C# can return from the exit call
        # while we unblock the RPC state
        rt = threading.Thread(target=Bridge._reaper)
        rt.start()
        pass

    def _imp_common(self):
        return self.module.import_methods(common_imports)

    def _imp_monohost(self):
        return self.module.import_methods(monohost_imports)
