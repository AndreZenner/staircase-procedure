import socket 
import threading
import sys
import os

# import script
import plotter

ipAddress = str(sys.argv[1])
port = int(sys.argv[2])

def recv_msg():
    while True:
        msg_length = s.recv(64).decode(FORMAT)
        if msg_length:
            msg_length = int(msg_length)
            msg = s.recv(msg_length).decode(FORMAT)
            arg = msg.split("::")[0]
            if arg == DISCONNECT_MESSAGE:
                print(f"[{s.getsockname()}] disconnecting..")
                break
            else:
                print(f"[{s.getsockname()}] [MESSAGE] {msg}")
                plotter.append_msg(msg)
               
    s.close()



if __name__ == '__main__':

    FORMAT = 'utf-8'
    DISCONNECT_MESSAGE = "!DISCONNECT"
    
    # create plot
    plotter.create_plot()
   
    # create an INET, STREAMing socket
    ADDR = (ipAddress, port)
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    print("trying to connect")
    s.connect(ADDR)
    print(f"[{s.getsockname()}] connected")

    thread = threading.Thread(target=recv_msg)
    thread.start()

    # start plot animation
    plotter.start_animate()