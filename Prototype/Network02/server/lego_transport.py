from thrift import Thrift
from thrift.transport import TSocket
from thrift.transport import TTransport
from thrift.protocol import TBinaryProtocol
        
def read_binary(obj, data):
    transport = TTransport.TMemoryBuffer(data)
    protocol = TBinaryProtocol.TBinaryProtocol(transport)
    obj.read(protocol)
    
def write_binary(obj):
    transport = TTransport.TMemoryBuffer()
    protocol = TBinaryProtocol.TBinaryProtocol(transport)
    obj.write(protocol)
    return transport.getvalue()
    
def read_custom(raw, classes):
    transport = TTransport.TMemoryBuffer(raw)
    protocol = TBinaryProtocol.TBinaryProtocol(transport)
    
    # Read the message count
    count = protocol.readI32()
    
    # Read the types
    keys = [protocol.readByte() for c in range(count)]
    
    # Instantiate the objects
    values = [classes[k]() for k in keys]
    
    # Fill the objects
    for v in values:
        v.read(protocol)

    return zip(keys, values)
        
    
def write_custom(tuples):
    keys, values = zip(*tuples)
    transport = TTransport.TMemoryBuffer()
    protocol = TBinaryProtocol.TBinaryProtocol(transport)
    
    # Write the message count
    protocol.writeI32(len(keys))
    
    # Write the type of each message
    for k in keys:
        protocol.writeByte(k)
        
    #Write the content of each message
    for v in values:
        v.write(protocol)
        
    return transport.getvalue()
    
    
        
    