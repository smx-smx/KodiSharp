"""
JSON handling
"""

import json
import sys

from utils import Utils
from lock import g_closed

class RPCError:
    SUCCESS = 0
    ERROR = 1
    RESULT_NOT_SERIALIZABLE = 2


# local module singleton
g_state = None

class RPC(object):
    def __init__(self, state):
        sys.excepthook = Utils.exception_hook

        global g_state
        g_state = state

    @staticmethod
    def on_exception(e):
        """
        Catch and log exceptions
        """
        e_type, e_value, e_traceback = sys.exc_info()
        Utils.exception_hook(e_type, e_value, e_traceback)

    @staticmethod
    def json_error(exitcode, exc=None):
        """
        Generates an integer exit status message
        """

        jDict = {
            "exit_code": exitcode
        }

        if exc is not None:
            jDict['error'] = repr(exc)

        return json.dumps(jDict)

    @staticmethod
    def on_message(data):
        """
        Parses eval messages from C#

        This function is registered using ctypes and called directly by C#.

        Args:
            data (bytes): JSON string received from C#, as bytes.

        Returns:
            data_type: Bla bla
        """
        if isinstance(data, bytes):
            data = data.decode('utf-8')
        
        try:
            message = json.loads(data)
            ## cannot use print if invoked from a callback
            #print(message)
        except Exception as exc:
            RPC.on_exception(exc)
            return RPC.json_error(RPCError.ERROR, exc)

        msg_type = message.get('type')
        if not msg_type:
            return RPC.json_error(RPCError.ERROR)

        if msg_type == 'exit':
            g_closed.set()
            return RPC.json_error(RPCError.SUCCESS)
        elif msg_type == 'eval':
            exec_code = message.get('exec_code')
            if exec_code:
                try:
                    g_state.eval(exec_code)
                except Exception as exc:
                    RPC.on_exception(exc)
                    return RPC.json_error(RPCError.ERROR, exc)
        elif msg_type == 'del_var':
            var_name = message.get('var_name')
            if var_name:
                g_state.del_var(var_name)

        result = g_state.get_result()
        typeName = result.__class__.__name__

        jDict = {
            "type": typeName,
            "value": result,
            "exit_code": RPCError.SUCCESS,
            "error" : ""
        }

        try:
            jsonData = json.dumps(jDict)
        except TypeError as e:
            jDict['value'] = None
            jDict['exit_code'] = RPCError.RESULT_NOT_SERIALIZABLE
            jDict['error'] = repr(e)
        except Exception as e:
            jDict['value'] = None
            jDict['exit_code'] = RPCError.ERROR
            jDict['error'] = repr(e)
        finally:
            jsonData = json.dumps(jDict)

        return jsonData
