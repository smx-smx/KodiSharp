"""
Represent a native module
"""

from ctypes import *

class Module(object):
    def __init__(self, lib_path):
        # Handle to the loaded library
        self._imports = {}
        print("Loading %s" % lib_path)
        
        self._library = CDLL(lib_path)
        print("Library %s" % self._library)

    def import_methods(self, methods):
        if not isinstance(methods, dict):
            raise Exception("Pass a method dict, describing prototypes")
        for name, signature in methods.items():
            if name in self.__dict__:
                raise Exception("Cannot import %s (conflicting property name)" % name)
            args, ret = signature
            # build property for function
            constructor = self._library.__getattr__(name)
            constructor.argtypes = args
            constructor.restype = ret
            self._imports[name] = constructor

    @property
    def handle(self):
        """
        Return library handle
        """
        return self._library

    def __getattr__(self, name):
        """
        Gets only called if attribute is not found in object
        """
        if name in self._imports:
            return self._imports[name]
