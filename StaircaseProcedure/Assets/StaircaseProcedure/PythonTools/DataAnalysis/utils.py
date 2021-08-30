import tkinter as tk
from tkinter import *


def create_label(frame, text="", type="normal", method="", row=0, column=0, color="black"):

    if(type == "label"):
        label = Label(frame, text=text ,fg=color, bg=  "#F4F4F4", font='Helvetica 13 bold')
        if (method=="pack"): label.pack(anchor='w')
        else: label.grid(row=row, column=column, padx='5', pady='5', sticky='ew')

    elif(type == "normal"):
        label = Label(frame, text=text ,fg=color, font='Helvetica 13', anchor='w',justify='left')
        if (method=="pack"): label.pack(anchor='w')
        else: label.grid(row=row, column=column, padx='2', pady='2', sticky='ew')

    elif(type == "bold"):
        label = Label(frame, text=text ,fg=color, font='Helvetica 13 bold',anchor='w', justify='left')
        if (method=="pack"): label.pack(anchor='w', fill="both")
        else: label.grid(row=row, column=column, padx='2', pady='2', sticky='ew')

    elif(type == "entry"):
        my_text = StringVar()
        my_text.set(text)
        entry = tk.Entry(frame,state="readonly",fg=color, bd=0, highlightthickness=0, textvariable = my_text)
        if (method=="pack"): entry.pack(anchor='w', fill="both" ,pady='0')
        else: entry.grid(row=row, column=column, padx='2', pady='2', sticky='w')
        

    elif(type=="heading"):
        label = Label(frame, pady='5',text=text,fg=color, bg="#F4F4F4", font='Helvetica 14 bold',anchor='w')
        if (method=="pack"): label.pack(fill='x',pady=(15,5))
        else: label.grid(row=row, column=column, padx='5', pady='5', sticky='ew')

    else:
        pass



def format_number(number, precision=4):
    return "{:.{}f}".format( number, precision )