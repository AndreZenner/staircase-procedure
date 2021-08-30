import tkinter as tk
import matplotlib
matplotlib.use('TkAgg')
import numpy as np
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
import matplotlib.pyplot as plt
from tkinter import *
from tkinter import filedialog

from matplotlib import pyplot as plt

from utils import create_label


# grey,blue, orange, violet, green, red
colors = ['#b3b3b3','#6596db', '#db9465', '#da83f5','#80da00', '#db544b']
colors_border = ['#737373', '#154fb7', '#8c4811', '#a711d4', '#3f8200','#85322b']
colors_border_d = ['#737373','#737373', '#154fb7','#154fb7', '#8c4811','#8c4811', '#a711d4', '#a711d4', '#3f8200','#3f8200','#85322b','#85322b']


class BoxplotSection(tk.Frame):

    def __init__(self, root):

        self.root = root;

        tk.Frame.__init__(self, root.frame, bg = "white")

       # data = list(root.cond_dict.values())
        #xlabels = list(root.cond_dict.keys())

            
        # create figure with custom size (figsize=(x,y))
        fig, axes = plt.subplots(figsize=(5, 3))
        reversed_df = root.df.iloc[:, ::-1]

        self.bplot = reversed_df.boxplot(ax=axes,vert=False, patch_artist=True, widths = 0.6,return_type='dict')

        # set layout
        axes.grid(color='#e6e6e6', linestyle='-', linewidth=1)
        axes.set_axisbelow(True)
        axes.set(frame_on=False)
        axes.set_title("Mean Detection Thresholds per Condition",fontsize=12,pad=15)
        axes.set_ylabel("Condition",fontsize=10,fontweight='bold',labelpad=10)
        axes.set_xlabel("Threshold",fontsize=10,fontweight='bold',labelpad=15)
        
        # set colors
        linewidth = 1
        self.set_colors('boxes', colors_border,linewidth)  
        self.set_colors('boxes', colors,linewidth, "facecolor")
        self.set_colors('whiskers', colors_border_d,linewidth)
        self.set_colors('caps',colors_border_d,linewidth)       
        self.set_colors('medians',colors_border, linewidth)
        self.set_colors('fliers',colors_border,linewidth,"markeredgecolor")
        self.set_colors('fliers',colors_border,linewidth,"markerfacecolor")  

        plt.tight_layout()

        canvas = FigureCanvasTkAgg(fig, master=self)
        canvas.get_tk_widget().pack(side='left', anchor='w')
        canvas.draw()

        savePlot = tk.Button(self, text="Save Plot", padx=10, pady =5, fg="black",  bg = "#F4F4F4", command= self.save_plot )
        savePlot.pack(side='left', anchor='w')

    
    def save_plot(self):
        files = [('svg', '*.svg')]
        file = filedialog.asksaveasfilename(filetypes = files, defaultextension = files)
        plt.savefig(file, format="svg")


    def close_plot(self):
        plt.close('all')

    def set_colors(self,component, color_list,linewidth, type_attr=""):
        for component, c in zip(self.bplot[component], color_list):
            component.set(linewidth=linewidth)
            if type_attr == "markeredgecolor":
                component.set(markeredgecolor = c)
            elif type_attr == "markerfacecolor":
                component.set(markerfacecolor = c)
            elif type_attr == "facecolor":
                component.set(facecolor = c)
            else:
                component.set(color = c)

