class AssemblyFunc(object):
    def __init__(self, namespace_name, method_name):
        self.namespace = namespace_name
        self.method = method_name

    def __str__(self):
        return "%s::%s" % (
            self.namespace,
            self.method
        )


class Assembly(object):
    def __init__(self, assembly_path, assembly_entry):
        self.path = assembly_path
        self.entry = assembly_entry
