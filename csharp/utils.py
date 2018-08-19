import traceback


class Utils(object):
    @staticmethod
    def exception_hook(exc_type, exc_value, exc_traceback):
        print("--- Caught Exception ---")
        lines = traceback.format_exception(exc_type, exc_value, exc_traceback)
        print(''.join('!! ' + line for line in lines))
        print("------------------------")
