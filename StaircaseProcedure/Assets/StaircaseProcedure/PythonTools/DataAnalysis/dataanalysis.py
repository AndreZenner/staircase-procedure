#!/usr/bin/env python3
# -*- coding: utf-8 -*-
# dataanalysis.py

"""This module provides the Data Analysis application."""


import tkinter as tk
from tkinter import messagebox

# import sections
import sections

        
class DataAnalysis():
    def __init__(self, root):

        # Configure root
        self.root = root;
        root.title("Analyze Staircase Procedure Results") # Makes the title that will appear in the top
        root.config(background = "#ffffff", padx='10',pady='10')

        # Fullscreen
        w, h = root.winfo_screenwidth(), root.winfo_screenheight()
        root.geometry("%dx%d+0+0" % (w, h))
        root.protocol("WM_DELETE_WINDOW", self.on_closing)

        # Configure Canvas
        self.canvas = tk.Canvas(root, background="white",highlightthickness=0)
        self.frame = tk.Frame(self.canvas, background="white")
        vsb = tk.Scrollbar(root, orient="vertical", command=self.canvas.yview)
        self.canvas.configure(yscrollcommand=vsb.set)
        vsb.pack(side="right", fill="y")
        self.canvas.pack(side="left", fill="both", expand=True)
        self.canvas.create_window((4,4), window= self.frame, anchor="nw")
        self.canvas.bind_all("<MouseWheel>", self.on_mousewheel)

        self.frame.bind("<Configure>", self.scrollcanvas)

        self.boxplot = ''
        self.df = ''

        # Include Section Open Directory
        self.opendirectory = sections.OpenDirectorySection(self);
        self.opendirectory.pack(pady='5',fill="x")
        

    def on_closing(self, *args):
        if messagebox.askokcancel("Quit", "Do you want to quit?"):
            if(self.boxplot): self.boxplot.close_plot()
            self.root.destroy()
            
    def on_mousewheel(self, event):
        scroll = -1 if event.delta > 0 else 1
        self.canvas.yview_scroll(scroll, "units")

    def scrollcanvas(self,event):
        self.canvas.configure(scrollregion=self.canvas.bbox("all"))

    # called by Button click
    def createMainSections(self):
        self.statisticalvalues = sections.StatisticalValueSection(self);
        self.statisticalvalues.pack(pady='5',fill="x")

        self.boxplot = sections.BoxplotSection(self);
        self.boxplot.pack(pady='5',fill="x")

        self.tests = sections.StatisticalTestSection(self);
        self.tests.pack(pady='5',fill="x")


def run():
    root = tk.Tk()
    DataAnalysis(root)
    root.mainloop()

if __name__ == "__main__":
    run()