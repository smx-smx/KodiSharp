"""
C# module loader
"""

import ctypes
import threading
import os
import sys
from .module import Module
from .rpc import RPC
from .state import State
from .lock import g_closed

from ctypes import *

# Pointer to a pyhton function that will be invoked to handle the message
on_message_delegate = CFUNCTYPE(c_char_p, c_char_p)
# Pointer to a python function that will be invoked on exit
on_exit_delegate = CFUNCTYPE(None)

post_event_delegate = CFUNCTYPE(None, c_char_p)

common_imports = {
    "runMethod": ([c_size_t, c_char_p, c_char_p, c_int, POINTER(c_char_p)], c_int),
    "clrInit": ([c_char_p, c_char_p, c_bool], c_size_t),
    "clrDeInit": ([c_size_t], None)
}


class Bridge(object):
    def __init__(self, library_path, interop_path, assembly_path, enable_debug=False):
        print(library_path)
        print(assembly_path)
        self.module = Module(library_path)
        self.assembly_path = assembly_path

        state = State(self)
        self.rpc = RPC(state)

        # Prepare delegates
        self.on_message = on_message_delegate(RPC.on_message)
        self.on_exit = on_exit_delegate(Bridge.on_exit)
        self.post_event_func = post_event_delegate()

        self._imp_common()

        pluginDir = os.path.dirname(
            os.path.abspath(sys.modules['__main__'].__file__)
        )

        # Initialize AppDomain and call initialize from the plugin
        self.plugin_handle = self.module.clrInit(
            interop_path.encode('ascii'),
            pluginDir.encode('ascii'),
            enable_debug
        )

    def post_event(self, jsonString):
        self.post_event_func(jsonString.encode('utf-8'))

    def run(self):
        g_closed.clear()

        argv = (c_char_p * 4)(
            self.assembly_path.encode('utf-8'),
            hex(ctypes.cast(self.on_message, c_void_p).value or 0).encode('utf-8'),
            hex(ctypes.cast(self.on_exit, c_void_p).value or 0).encode('utf-8'),
            hex(ctypes.cast(ctypes.addressof(self.post_event_func), c_void_p).value or 0).encode('utf-8')
        )

        print("GOING TO CALL C# ENTRY")
        res = self.module.runMethod(
            self.plugin_handle,
            "Smx.KodiInterop.KodiBridgeABI".encode('utf-8'),
            "Entry".encode('utf-8'),
            len(argv), argv
        )
        print("RETURNED FROM C#, status: " + str(res))
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
