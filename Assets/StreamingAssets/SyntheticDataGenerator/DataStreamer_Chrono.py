import socket
import struct
import time

fPath = 'SamplePath_Chrono.csv'
fConn = open(fPath, 'r')
fHead = fConn.readline()
vPath = [list(map(float, cLine.split(','))) for cLine in fConn]
fConn.close()

ipAddr = 'localhost'
portNo = 6412

dataRows = len(vPath)

s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, 0)
s.connect((ipAddr, portNo))

i = 0
    
while True:

    i = i + 1

    # A format string that reads 5f converts the floats to single precision
    # A format string that reads 5d converts the floats to double precision

    dataToSend = vPath[i-1]
    fmtStr = '{:d}f'.format(len(dataToSend))
    bufData = struct.pack(fmtStr, *dataToSend)
    s.send(bufData)
    
    #print(fmtStr)

    time.sleep(0.0166666)
    
    if i == dataRows: i = 0

s.close()