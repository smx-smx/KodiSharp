"""
JSON handling
"""

from email import utils
from faulthandler import dump_traceback
import json
import sys

from .utils import Utils
from .lock import g_closed

class RPCError:
    SUCCESS = 0
    ERROR = 1
    '''
    eval result is not serializable/representable
    this is not an error. it indicates that the result is a complex data type
    '''
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
    def on_message(data: str):
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
        except Exception as exc:
            RPC.on_exception(exc)
            return RPC.json_error(RPCError.ERROR, exc).encode('utf-8')

        msg_type = message.get('type')
        if not msg_type:
            return RPC.json_error(RPCError.ERROR).encode('utf-8')

        if msg_type == 'exit':
            g_closed.set()
            return RPC.json_error(RPCError.SUCCESS).encode('utf-8')
        elif msg_type == 'eval':
            exec_code = message.get('exec_code')
            if exec_code:
                try:
                    g_state.eval(exec_code)
                except Exception as exc:
                    RPC.on_exception(exc)
                    return RPC.json_error(RPCError.ERROR, exc).encode('utf-8')

        result = g_state.get_result()

        jDict = {
            "value": result,
            "exit_code": RPCError.SUCCESS,
            "error" : ""
        }

        try:
            jsonData = json.dumps(jDict)
        except TypeError as e:
            jDict['value'] = None
            jDict['exit_code'] = RPCError.RESULT_NOT_SERIALIZABLE
            #jDict['exception'] = str(e)
            jsonData = json.dumps(jDict)
        except Exception as e:
            jDict['value'] = None
            jDict['exit_code'] = RPCError.ERROR
            jDict['error'] = repr(e)
            jsonData = json.dumps(jDict)

        return jsonData.encode('utf-8')
