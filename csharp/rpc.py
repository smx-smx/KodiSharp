"""
JSON handling
"""

import json
import sys

from utils import Utils
from lock import g_closed

class RPCError:
    SUCCESS = 0
    RESULT_NOT_SERIALIZABLE = 1


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
    def json_error(exitcode):
        """
        Generates an integer exit status message
        """
        return json.dumps({
            "exit_code": exitcode
        })

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
            print(message)
        except Exception as exc:
            RPC.on_exception(exc)
            return RPC.json_error(1)

        msg_type = message.get('type')
        if not msg_type:
            print("Invalid Message Type")
            return RPC.json_error(1)

        if msg_type == 'exit':
            print("We got exit, bye")
            g_closed.set()
            return RPC.json_error(0)
        elif msg_type == 'eval':
            exec_code = message.get('exec_code')
            if exec_code:
                try:
                    g_state.eval(exec_code)
                except Exception as exc:
                    RPC.on_exception(exc)
                    return RPC.json_error(1)
        elif msg_type == 'del_var':
            var_name = message.get('var_name')
            if var_name:
                g_state.del_var(var_name)

        result = g_state.get_result()
        typeName = result.__class__.__name__
        print("=> TypeName is " + typeName)

        jDict = {
            "type": typeName,
            "value": result,
            "exit_code": RPCError.SUCCESS
        }

        try:
            jsonData = json.dumps(jDict)
        except TypeError:
            jDict['value'] = None
            jDict['exit_code'] = RPCError.RESULT_NOT_SERIALIZABLE
            jsonData = json.dumps(jDict)

        return jsonData
