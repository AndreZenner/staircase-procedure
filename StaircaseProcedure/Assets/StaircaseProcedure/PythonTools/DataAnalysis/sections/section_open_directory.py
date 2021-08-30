import tkinter as tk
from tkinter import *
import os
import csv
import pandas as pd
from tkinter import filedialog, messagebox

from utils import create_label

class OpenDirectorySection(tk.Frame):

    def addApp(self):
    
        rootdir = filedialog.askdirectory()
        head, experimentname = os.path.split(rootdir)

        # path
        create_label(self, row=0, column= 1, text=rootdir, type="label") 

        df = pd.DataFrame()

        # foreach participant
        sorted_participants = sorted(os.scandir(rootdir), key=lambda x: (x.is_dir(), x.name))
        for p in sorted_participants:
            if p.is_dir():
                #foreach condition file (sorted)
                sorted_condition_files = sorted(os.scandir(p), key=lambda x: (x.is_dir(), x.name))
                for f in sorted_condition_files:
                    if f.name.endswith('.csv'):
                        with open(f, 'r') as file:
                            reader = csv.reader(file, delimiter=";")
                            for index, row_csv in enumerate(reader):
                                if row_csv:
                                    if row_csv[0] == "name":
                                        #print(row_csv)
                                        condition_name = row_csv[2]                                                           
                                        number_participant = row_csv[3]
                                    elif row_csv[0] == "threshold":
                                        threshold = float(row_csv[1])
                                        df.at[number_participant, condition_name] = threshold
        # Check for errors
        if df.empty:
            print('DataFrame is empty!')
            messagebox.showinfo(title="Error Message", message="Files could not be found! Please select the folder which contains all the P_[experimentName]_[number] folders.")
            return
        elif df.isnull().values.any():
            print('Missing Values!')
            messagebox.showinfo(title="Error Message", message="Missing Values. There might be erros in the following calculations!\n")
            df_nans = df[df.isna().any(axis=1)]
            create_label(self.root.frame, text=df_nans.to_string(), type="label",method="pack") 
            
        else:
            self.number_open_experiments += 1

        label = Label(self.root.frame, pady='5',text=experimentname, bg="#F4F4F4", font='Helvetica 18 bold',anchor='w')
        if(self.number_open_experiments > 1): label.pack(fill='x', pady=(75,10))
        else: label.pack(fill='x', pady=(10,10))

        self.root.df = df
        self.root.createMainSections()
            

    def __init__(self, root):

        self.number_open_experiments = 0

        self.root = root

        tk.Frame.__init__(self, root.frame, bg = "white")

        openFile = tk.Button(self, text="Open Experiment Directory", padx=10, pady =5, fg="black",  bg = "#F4F4F4", command= self.addApp )
        openFile.grid(row=0, column=0, padx='5', pady='5', sticky='ew')



   